using Buscar.Imoveis.Venda.Messaging;
using Buscar.Imoveis.Venda.Services.Interface;
using CsvHelper;
using CsvHelper.Configuration;
using MassTransit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using RabbitMQ.Client;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace Buscar.Imoveis.Venda.Services
{
    public class ImovelCaixaService : IImovelCaixaService
    {
        private readonly ChromeOptions _options;
        private readonly IPublishEndpoint _publish;
        public IWebDriver driver;
        public ImovelCaixaService(IPublishEndpoint publish)
        {
            _options = new ChromeOptions();
            _publish = publish;
        }
        public async Task ProcessoImoveisCaixa()
        {
            var nomeArquivo = await GetListaImoveis();
            //var nomeArquivo = @"D:\Users\erik\Documents\Caixa\Lista_imoveis_BA.csv";
            if (!string.IsNullOrEmpty(nomeArquivo))
            {
                var imoveis = await ExtrairImoveisCsv(nomeArquivo);
                driver.Quit();
            }
        }

        public async Task<string> GetListaImoveis()
        {
            string pasta = @"D:\Users\erik\Documents\Caixa";

            // Configurações de download
            var downloadPath = Path.Combine(Environment.CurrentDirectory, pasta);
            Directory.CreateDirectory(downloadPath);

            var prefs = new Dictionary<string, object>
            {
                { "download.default_directory", downloadPath },
                { "download.prompt_for_download", false },
                { "download.directory_upgrade", true },
                { "safebrowsing.enabled", true }
            };
            foreach (var pref in prefs)
            {
                _options.AddUserProfilePreference(pref.Key, pref.Value);
            }
            var uf = "BA";
            // Inicia o driver com as opções configuradas
            driver = new ChromeDriver(_options);

            driver.Navigate().GoToUrl("https://venda-imoveis.caixa.gov.br/sistema/download-lista.asp");

            IWebElement dropdownElement = driver.FindElement(By.Id("cmb_estado"));

            SelectElement select = new SelectElement(dropdownElement);

            select.SelectByText(uf);

            // Lógica para clicar no link de download, etc.
            driver.FindElement(By.XPath("//*[@id=\"btn_next1\"]")).Click();

            Thread.Sleep(5000);

            string inputFilePath = @$"{pasta}\Lista_imoveis_{uf}.csv";

            return await Task.FromResult(inputFilePath);

        }

        public async Task<List<ImportImovel>> ExtrairImoveisCsv(string inputFilePath)
        {
            int startLine = 4;
            var imoveis = new List<ImportImovel>();

            using (var reader = new StreamReader(inputFilePath, Encoding.GetEncoding("ISO-8859-1")))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                MissingFieldFound = null,
                Delimiter = ";"
            }))
            {
                for (int i = 0; i < startLine - 1; i++)
                {
                    reader.ReadLine(); // Ignora a linha
                }
                csv.Context.RegisterClassMap<ImovelMap>();

                imoveis = csv.GetRecords<ImportImovel>().ToList();

                csv.Dispose();
                
                if (imoveis.Count > 0)
                {
                    foreach (var imovel in imoveis)
                    {
                        if (!string.IsNullOrEmpty(imovel.NumeroImovel))
                            ObterDescricaoImovel(imovel);

                        if (imovel.Disponivel)
                            imovel.TratarDadosImovel();

                        //await _publish.Publish(imovel);

                    }
                    var imoveisDisponivel = imoveis.Where(c => c.Disponivel).ToList();

                    GerarCsv(@"D:\Users\erik\Documents\Caixa\Lista_imoveis_BA_new.csv", imoveis.Where(c => c.Disponivel));
                }

                File.Delete(inputFilePath);

            }
            return await Task.FromResult(imoveis); ;
        }

        public static void GerarCsv(string caminhoArquivo, IEnumerable<ImportImovel> imoveis)
        {
            var utf8Bom = new UTF8Encoding(true); 

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                Encoding = utf8Bom,
            };

            using var writer = new StreamWriter(caminhoArquivo, false, utf8Bom);
            using var csv = new CsvWriter(writer, config);
            csv.WriteRecords(imoveis);
        }

        private void ObterDescricaoImovel(ImportImovel imovel)
        {
            string originalWindow = driver.CurrentWindowHandle;

            ((IJavaScriptExecutor)driver).ExecuteScript($"window.open('{imovel.Link}');");

            List<string> windowHandles = new List<string>(driver.WindowHandles);

            // Troca para a segunda aba
            driver.SwitchTo().Window(windowHandles[1]);

            //var imovelIndisponivel_1 = GetElementText<string>(By.XPath("//*[@id=\"dadosImovel\"]/div/div/h5/p"));
            var imovelIndisponivel = GetElementText<string>(By.XPath("//*[@id=\"dadosImovel\"]/div/div/h5"));

            bool indisponivel = !string.IsNullOrEmpty(imovelIndisponivel) && imovelIndisponivel.Contains("O imóvel que você procura não está mais disponível para venda");
            if (!indisponivel)
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                string condominio = driver.FindElement(By.CssSelector(".control-item h5")).Text;
                string situacao = GetSituacao();

                IWebElement divPaiDescricao = driver.FindElement(By.XPath("//*[@id=\"dadosImovel\"]/div/div[3]/p[3]"));

                var descricoes = divPaiDescricao.Text;

                var descricaoList = descricoes.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                imovel.InformacoesPagamento(descricaoList);

                imovel.Condominio = condominio;
                imovel.Situacao = situacao;
                imovel.Disponivel = true;


            }

            driver.Close();

            // Voltar para a aba original
            driver.SwitchTo().Window(originalWindow);
            Thread.Sleep(1000);
        }

        public T GetElementText<T>(By by)
        {
            try
            {
                IWebElement element = driver.FindElement(by);
                return (T)Convert.ChangeType(element.Text, typeof(T));
            }
            catch (NoSuchElementException)
            {
                return default;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finding element: {ex.Message}");
                return default;
            }
        }

        private string GetSituacao()
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                // Executar JavaScript para remover comentários do DOM
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript(@"
                var comments = [];
                var walker = document.createTreeWalker(document, NodeFilter.SHOW_COMMENT, null, false);
                var node;
                while (node = walker.nextNode()) {
                    var commentContent = node.nodeValue.trim();
                    if (commentContent.startsWith('span')) {
                        var spanElement = document.createElement('span');
                        spanElement.innerHTML = commentContent.substring(5, commentContent.length - 7); // Remove 'span' tags
                        node.parentNode.insertBefore(spanElement, node.nextSibling);
                    }
                }
            ");

                // Esperar até que a div específica esteja visível
                IWebElement div = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".control-item.control-span-6_12")));

                // Agora você pode interagir com os elementos que estavam comentados
                IWebElement situacaoSpan = div.FindElement(By.XPath(".//span[contains(text(), 'Situação:')]"));
                Console.WriteLine(situacaoSpan.Text);

                return situacaoSpan.Text;
            }
            catch
            {

                return string.Empty;
            }

        }
    }
}
