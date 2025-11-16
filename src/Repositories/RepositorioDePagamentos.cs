using LojaExemplo.Modelos;

namespace LojaExemplo.Repositorios
{
    public class RepositorioDePagamentos : IRepositorioDePagamentos
    {
        private readonly Dictionary<int, PagamentoInfo> _pagamentos;

        public RepositorioDePagamentos()
        {
            _pagamentos = new Dictionary<int, PagamentoInfo>();
        }

        public async Task<PagamentoInfo?> ObterPorPedidoIdAsync(int pedidoId)
        {
            await Task.Delay(10); // Simula operação assíncrona
            return _pagamentos.TryGetValue(pedidoId, out var pagamento) ? pagamento : null;
        }

        public async Task<List<PagamentoInfo>> ObterTodosAsync()
        {
            await Task.Delay(10);
            return _pagamentos.Values.ToList();
        }

        public async Task<PagamentoInfo> AdicionarAsync(PagamentoInfo pagamento)
        {
            await Task.Delay(10);
            _pagamentos[pagamento.PedidoId] = pagamento;
            return pagamento;
        }

        public async Task<PagamentoInfo?> AtualizarAsync(PagamentoInfo pagamento)
        {
            await Task.Delay(10);
            if (_pagamentos.ContainsKey(pagamento.PedidoId))
            {
                _pagamentos[pagamento.PedidoId] = pagamento;
                return pagamento;
            }
            return null;
        }

        public async Task<bool> ExistePorPedidoIdAsync(int pedidoId)
        {
            await Task.Delay(10);
            return _pagamentos.ContainsKey(pedidoId);
        }
    }
}
