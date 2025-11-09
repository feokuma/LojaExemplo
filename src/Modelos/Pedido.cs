namespace LojaExemplo.Modelos
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime DataPedido { get; set; }
        public string ClienteEmail { get; set; } = string.Empty;
        public StatusPedido Status { get; set; }
        public List<ItemDePedido> Itens { get; set; } = new List<ItemDePedido>();
        public decimal ValorTotal { get; set; }
        public DateTime? DataPagamento { get; set; }
        public string? MetodoPagamento { get; set; }
    }

    public enum StatusPedido
    {
        Pendente = 1,
        Confirmado = 2,
        Pago = 3,
        Enviado = 4,
        Entregue = 5,
        Cancelado = 6
    }
}