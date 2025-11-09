namespace LojaExemplo.DTOs
{
    /// <summary>
    /// Request para processar pagamento de um pedido
    /// </summary>
    /// <param name="MetodoPagamento">Método de pagamento (CartaoCredito, CartaoDebito, Pix, Boleto, TransferenciaBancaria)</param>
    /// <param name="Valor">Valor do pagamento</param>
    public record ProcessarPagamentoRequest{
        public required string MetodoPagamento { get; init; }
        public decimal Valor { get; init; }
        public int PedidoId { get; init; }
    }
}
