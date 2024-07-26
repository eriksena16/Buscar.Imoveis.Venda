namespace Buscar.Imoveis.Venda.Services.Interface
{
    public interface IImovelCaixaService
    {
        Task<string> GetListaImoveis();
        Task<List<ImportImovel>> ExtrairImoveisCsv(string inputFilePath);
        Task ProcessoImoveisCaixa();
    }
}
