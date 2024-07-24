using Busca_Imoveis_Caixas.Config;
using CsvHelper.Configuration;

namespace Buscar.Imoveis.Venda
{
    public sealed class ImovelMap : ClassMap<Imovel>
    {
        public ImovelMap()
        {
            Map(m => m.NumeroImovel).Name("ColumnA");
            Map(m => m.Uf).Name("ColumnB");
            Map(m => m.Cidade).Name("ColumnC");
            Map(m => m.Bairro).Name("ColumnD");
            Map(m => m.Endereco).Name("ColumnE");
            Map(m => m.Preco).Name("ColumnF");
            Map(m => m.ValorAvaliacao).Name("ColumnG");
            Map(m => m.Desconto).Name("ColumnH");
            Map(m => m.Descricao).Name("ColumnI");
            Map(m => m.ModalidadeVendaString).Name("ColumnJ");
            Map(m => m.Link).Name("ColumnK");
        }
    }
}
