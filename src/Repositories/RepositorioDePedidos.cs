using LojaExemplo.Modelos;

namespace LojaExemplo.Repositorios
{
    public interface IRepositorioDePedidos
    {
        Task<Pedido> AdicionarAsync(Pedido pedido);
        Task<Pedido?> ObterPorIdAsync(int id);
        Task<List<Pedido>> ObterPorClienteAsync(string clienteEmail);
        Task<List<Pedido>> ObterTodosAsync();
        Task<bool> AtualizarAsync(Pedido pedido);
    }

    public class RepositorioDePedidos : IRepositorioDePedidos
    {
        private readonly List<Pedido> _pedidos;
        private int _proximoId = 1;

        public RepositorioDePedidos()
        {
            _pedidos = new List<Pedido>();
        }

        public async Task<Pedido> AdicionarAsync(Pedido pedido)
        {
            await Task.Delay(10); // Simula operação assíncrona
            pedido.Id = _proximoId++;
            _pedidos.Add(pedido);
            return pedido;
        }

        public async Task<Pedido?> ObterPorIdAsync(int id)
        {
            await Task.Delay(10);
            return _pedidos.FirstOrDefault(p => p.Id == id);
        }

        public async Task<List<Pedido>> ObterPorClienteAsync(string clienteEmail)
        {
            await Task.Delay(10);
            return _pedidos.Where(p => p.ClienteEmail.Equals(clienteEmail, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<List<Pedido>> ObterTodosAsync()
        {
            await Task.Delay(10);
            return _pedidos.ToList();
        }

        public async Task<bool> AtualizarAsync(Pedido pedido)
        {
            await Task.Delay(10);
            var pedidoExistente = _pedidos.FirstOrDefault(p => p.Id == pedido.Id);
            if (pedidoExistente != null)
            {
                // Atualiza as propriedades
                pedidoExistente.Status = pedido.Status;
                pedidoExistente.ValorTotal = pedido.ValorTotal;
                pedidoExistente.Itens = pedido.Itens;
                return true;
            }
            return false;
        }
    }
}
