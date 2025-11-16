using LojaExemplo.Modelos;

namespace LojaExemplo.Repositorios;

public interface IRepositorioDePagamentos
{
    Task<PagamentoInfo?> ObterPorPedidoIdAsync(int pedidoId);
    Task<List<PagamentoInfo>> ObterTodosAsync();
    Task<PagamentoInfo> AdicionarAsync(PagamentoInfo pagamento);
    Task<PagamentoInfo?> AtualizarAsync(PagamentoInfo pagamento);
    Task<bool> ExistePorPedidoIdAsync(int pedidoId);
}
