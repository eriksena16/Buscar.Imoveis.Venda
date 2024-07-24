using System.Text.RegularExpressions;

namespace Busca_Imoveis_Caixas.Config
{

    public class Imovel
    {
        public string Condominio { get; set; }
        public string Quartos { get; set; }
        public string Comarca { get; set; }
        public string Situacao { get; set; }
        public string Uf { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Preco { get; set; }
        public string Desconto { get; set; }
        public string NumeroImovel { get; set; }
        public string InscricaoImobiliaria { get; set; }
        public string Matricula { get; set; }
        public string Localizacao { get; set; }
        public string ValorAvaliacao { get; set; }
        public string ValorMinimo { get; set; }
        public string Descricao { get; set; }
        public string Endereco { get; set; }
        public string TipoImovel { get; set; }
        public string AreaPrivativa { get; set; }
        public string AreaTerreno { get; set; }
        public string Link { get; set; }
        public string ModalidadeVendaString { get; set; }
        public ModalidadeVenda ModalidadeVenda { get; set; }

        public void TratamentoNumeroImovel()
        {
            string pattern = "[^a-zA-Z0-9]";

            string cleanString = Regex.Replace(NumeroImovel, pattern, "");

            NumeroImovel = cleanString;

            Link = $"https://venda-imoveis.caixa.gov.br/sistema/detalhe-imovel.asp?hdnOrigem=index&hdnimovel={NumeroImovel}";
        }

        public void InserirValores(string texto)
        {
            string[] linhas = texto.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            ValorAvaliacao = linhas[0].Split(':')[1].Trim();
            ValorMinimo = linhas[1].Split(':')[1].Split("(")[0].Trim();
        }
    }

}
