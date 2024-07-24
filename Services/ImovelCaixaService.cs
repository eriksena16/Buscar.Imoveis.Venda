using Busca_Imoveis_Caixas.Config;
using Buscar.Imoveis.Venda.Services.Interface;
using CsvHelper;
using CsvHelper.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;

namespace Buscar.Imoveis.Venda.Services
{
    public class ImovelCaixaService : IImovelCaixaService
    {
        private readonly ChromeOptions _options;

        public ImovelCaixaService()
        {
            _options = new ChromeOptions();
            _options.AddArgument("--headless");
            _options.AddArgument("--no-sandbox");
            _options.AddArgument("--disable-dev-shm-usage");
        }

        public async Task<string> GetListaImoveis()
        {
            string pasta = @"D:\Users\erik\Documents\Caixa";

            // Defina o caminho para o driver do Chrome
            ChromeOptions options = new ChromeOptions();

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
                options.AddUserProfilePreference(pref.Key, pref.Value);
            }
            var uf = "BA";
            // Inicia o driver com as opções configuradas
            using (IWebDriver driver = new ChromeDriver(options))
            {
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
        }

        public async Task<List<Imovel>> ExtrairImoveisCsv(string inputFilePath)
        {
            int startLine = 5;
            var imoveis = new List<Imovel>();

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
                imoveis = csv.GetRecords<Imovel>().ToList();



            }
            return await Task.FromResult(imoveis); ;
        }

        public async Task ProcessoImoveisCaixa()
        {
            var nomeArquivo = await GetListaImoveis();
            if (!string.IsNullOrEmpty(nomeArquivo))
            {
                var imoveis = await ExtrairImoveisCsv(nomeArquivo);
            }
        }
    }
}
