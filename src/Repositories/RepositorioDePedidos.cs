using LojaExemplo.Modelos;

namespace LojaExemplo.Repositorios
{
    public class RepositorioDePedidos : IRepositorioDePedidos
    {
        private readonly List<Pedido> _pedidos;
        private int _proximoId = 1;

        public RepositorioDePedidos()
        {
            _pedidos = new List<Pedido>();
        }

        public async Task<Pedido?> ObterPorIdAsync(int id)
        {
            await Task.Delay(10); // Simula operação assíncrona
            return _pedidos.FirstOrDefault(p => p.Id == id);
        }

        public async Task<List<Pedido>> ObterTodosAsync()
        {
            await Task.Delay(10);
            return _pedidos.ToList();
        }

        public async Task<List<Pedido>> ObterPorClienteAsync(string clienteEmail)
        {
            await Task.Delay(10);
            return _pedidos.Where(p => p.ClienteEmail.Equals(clienteEmail, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<Pedido> AdicionarAsync(Pedido pedido)
        {
            await Task.Delay(10);
            if (pedido.Id == 0)
            {
                pedido.Id = _proximoId++;
            }
            else
            {
                // Atualizar o próximo ID se o pedido já tem um ID maior
                if (pedido.Id >= _proximoId)
                {
                    _proximoId = pedido.Id + 1;
                }
            }
            _pedidos.Add(pedido);
            return pedido;
        }

        public async Task<Pedido?> AtualizarAsync(Pedido pedido)
        {
            await Task.Delay(10);
            var pedidoExistente = _pedidos.FirstOrDefault(p => p.Id == pedido.Id);
            if (pedidoExistente != null)
            {
                var indice = _pedidos.IndexOf(pedidoExistente);
                _pedidos[indice] = pedido;
                return pedido;
            }
            return null;
        }

        public async Task<bool> ExistePorIdAsync(int id)
        {
            await Task.Delay(10);
            return _pedidos.Any(p => p.Id == id);
        }

        public int ObterProximoId()
        {
            return _proximoId;
        }
    }
}
