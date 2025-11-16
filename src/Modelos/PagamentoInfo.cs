namespace LojaExemplo.Modelos
{
    public class PagamentoInfo
    {
        public int PedidoId { get; set; }
        public string MetodoPagamento { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataPagamento { get; set; }
        public DateTime? DataEstorno { get; set; }
        public StatusPagamento Status { get; set; }
    }

    public enum StatusPagamento
    {
        Pendente = 1,
        Aprovado = 2,
        Rejeitado = 3,
        Estornado = 4
    }
}
