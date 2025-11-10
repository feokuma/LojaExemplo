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

        /// <summary>
        /// Cancela o pedido se ele ainda puder ser cancelado
        /// </summary>
        /// <returns>True se o pedido foi cancelado, False se já estava cancelado</returns>
        public bool Cancelar()
        {
            if (Status == StatusPedido.Cancelado)
                return false;

            Status = StatusPedido.Cancelado;
            return true;
        }

        /// <summary>
        /// Verifica se o pedido precisa de reposição de estoque ao ser cancelado
        /// </summary>
        /// <returns>True se deve repor estoque, False caso contrário</returns>
        public bool DeveReporEstoque()
        {
            return Status == StatusPedido.Confirmado || Status == StatusPedido.Pago;
        }

        /// <summary>
        /// Confirma o pedido se ele estiver pendente
        /// </summary>
        /// <returns>True se o pedido foi confirmado, False se não estava pendente</returns>
        public bool Confirmar()
        {
            if (Status != StatusPedido.Pendente)
                return false;

            Status = StatusPedido.Confirmado;
            return true;
        }

        /// <summary>
        /// Verifica se o pedido pode ser cancelado
        /// </summary>
        /// <returns>True se pode ser cancelado, False caso contrário</returns>
        public bool PodeCancelar()
        {
            return Status != StatusPedido.Cancelado;
        }
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