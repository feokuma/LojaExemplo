using LojaExemplo.Modelos;

namespace LojaExemplo.Repositorios;

public interface IRepositorioDePedidos
{
    Task<Pedido?> ObterPorIdAsync(int id);
    Task<List<Pedido>> ObterTodosAsync();
    Task<List<Pedido>> ObterPorClienteAsync(string clienteEmail);
    Task<Pedido> AdicionarAsync(Pedido pedido);
    Task<Pedido?> AtualizarAsync(Pedido pedido);
    Task<bool> ExistePorIdAsync(int id);
    int ObterProximoId();
}
