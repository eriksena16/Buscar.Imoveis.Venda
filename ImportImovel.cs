using System.Text.RegularExpressions;

namespace Buscar.Imoveis.Venda
{

    public class ImportImovel
    {
        public string Condominio { get; set; }
        public string Quantidade { get; set; }
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
        public string AreaTotal { get; set; }
        public string Link { get; set; }
        public string Foto { get; set; }
        public string ModalidadeVenda { get; set; }
        public bool AceitaFgts { get; private set; }
        public bool AceitaConsorcio { get; private set; }
        public bool AceitaFinanciamento { get; private set; }
        public bool AceitaParcelamento { get; private set; }
        public bool AcaoJudicial { get; private set; }

        private static readonly char[] separator = [' '];

        public void TratarDadosImovel()
        {
            string[] linhas = Descricao.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            TipoImovel = linhas[0].Trim();
            AreaTotal = ObterValor(linhas, 1);
            AreaPrivativa = ObterValor(linhas, 2);
            AreaTerreno = ObterValor(linhas, 3);
            Quantidade = linhas.Length > 4 ? linhas[4].Split(separator, StringSplitOptions.RemoveEmptyEntries)[0].Trim() : "N/A";
            Descricao = linhas.Length > 5 ? string.Join(", ", linhas, 5, linhas.Length - 5).Trim() : "N/A";
            NumeroImovel = NumeroImovel.Trim().PadLeft(13, '0');
            Foto = $"https://venda-imoveis.caixa.gov.br/fotos/F{NumeroImovel}21.jpg";
            Cidade = Cidade.Trim();
            Bairro = Bairro.Trim();
            Uf = Uf.Trim();
            ModalidadeVenda = ModalidadeVenda.Trim();
            Situacao = Situacao.Split(':', StringSplitOptions.RemoveEmptyEntries)[1].Trim();

        }
        private static string ObterValor(string[] linhas, int indice)
        {
            if (linhas.Length > indice)
            {
                return linhas[indice].Split(separator, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
            }
            return "N/A";
        }

        private static ModalidadeVenda GetModalidadeVendaByText(string text)
        {
            return text switch
            {
                "Selecione" => Venda.ModalidadeVenda.Selecione,
                "1º Leilão SFI" => Venda.ModalidadeVenda.PrimeiroLeilaoSFI,
                "2º Leilão SFI" => Venda.ModalidadeVenda.SegundoLeilaoSFI,
                "Concorrência Pública" => Venda.ModalidadeVenda.ConcorrenciaPublica,
                "Leilão SFI - Edital Único" => Venda.ModalidadeVenda.LeilaoSFIEditalUnico,
                "Licitação Aberta" => Venda.ModalidadeVenda.LicitacaoAberta,
                "Venda Direta FAR" => Venda.ModalidadeVenda.VendaDiretaFAR,
                "Venda Direta Online" => Venda.ModalidadeVenda.VendaDiretaOnline,
                "Venda Online" => Venda.ModalidadeVenda.VendaOnline,
                _ => throw new ArgumentOutOfRangeException(nameof(text), $"Não existe mapeamento para o texto: {text}")
            };
        }

        public void InformacoesPagamento(string[] descricoes)
        {
            var naoAceitaFinanciamento = " Imóvel NÃO aceita financiamento habitacional.";
            var naoAceitaParcelamento = "Imóvel NÃO aceita parcelamento.";
            var naoAceitaConsorcio = "Imóvel NÃO aceita consórcio.";
            var naoAceitaFgts = "Imóvel NÃO aceita utilização de FGTS.";
            var aceitaFinancimento = "FINANCIAMENTO (consulte condições).";
            var acaoJudicial = "Imóvel com ação judicial";



            if (!descricoes.ToList().Contains(naoAceitaConsorcio))
                AceitaConsorcio = true;

            if (!descricoes.ToList().Contains(naoAceitaFinanciamento) || descricoes.ToList().Contains(aceitaFinancimento))
                AceitaFinanciamento = true;

            if (!descricoes.ToList().Contains(naoAceitaFgts) && AceitaFinanciamento)
                AceitaFgts = true;

            if (!descricoes.ToList().Contains(naoAceitaParcelamento))
                AceitaParcelamento = true;

            if (descricoes.ToList().Contains(acaoJudicial))
                AcaoJudicial = true;

            //if (!descricao.Trim().Contains(naoAceitaParcelamento, StringComparison.CurrentCultureIgnoreCase))
            //    AceitaParcelamento = true;

            //if (descricao.Trim().Contains(acaoJudicial, StringComparison.CurrentCultureIgnoreCase))
            //    AcaoJudicial = true;
            //foreach (string descricao in descricoes)
            //{


            //}


        }


    }

}
