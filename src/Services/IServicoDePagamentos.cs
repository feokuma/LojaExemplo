namespace LojaExemplo.Servicos
{
    public interface IServicoDePagamentos
    {
        Task<bool> ProcessarPagamentoAsync(int pedidoId, string metodoPagamento, decimal valor);
        Task<bool> VerificarStatusPagamentoAsync(int pedidoId);
        Task<bool> EstornarPagamentoAsync(int pedidoId);
        Task<List<string>> ObterMetodosPagamentoDisponiveisAsync();
    }
}
