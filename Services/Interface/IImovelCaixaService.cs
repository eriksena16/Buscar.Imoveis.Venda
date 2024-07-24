using Busca_Imoveis_Caixas.Config;

namespace Buscar.Imoveis.Venda.Services.Interface
{
    public interface IImovelCaixaService
    {
        Task<string> GetListaImoveis();
        Task<List<Imovel>> ExtrairImoveisCsv(string inputFilePath);
        Task ProcessoImoveisCaixa();
    }
}
