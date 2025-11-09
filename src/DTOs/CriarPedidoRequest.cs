using LojaExemplo.Modelos;

namespace LojaExemplo.DTOs
{
    /// <summary>
    /// Request para criação de um novo pedido
    /// </summary>
    /// <param name="ClienteEmail">Email do cliente que está realizando o pedido</param>
    /// <param name="Itens">Lista de itens do pedido</param>
    public record CriarPedidoRequest(
        string ClienteEmail,
        List<ItemDePedido> Itens
    );
}
