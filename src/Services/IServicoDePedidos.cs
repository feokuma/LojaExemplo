using LojaExemplo.Modelos;

namespace LojaExemplo.Servicos
{
    public interface IServicoDePedidos
    {
        Task<Pedido> CriarPedidoAsync(string clienteEmail, List<ItemDePedido> itens);
        Task<Pedido?> ObterPedidoPorIdAsync(int id);
        Task<List<Pedido>> ObterPedidosPorClienteAsync(string clienteEmail);
        Task<bool> ConfirmarPedidoAsync(int pedidoId);
        Task<bool> CancelarPedidoAsync(int pedidoId);
        Task<decimal> CalcularValorTotalAsync(List<ItemDePedido> itens);
        Task<Pedido> CriarPedidoComDescontoAsync(string clienteEmail, List<ItemDePedido> itens, decimal percentualDesconto);
    }
}
