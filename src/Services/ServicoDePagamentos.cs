using LojaExemplo.Modelos;

namespace LojaExemplo.Servicos
{
    public class ServicoDePagamentos : IServicoDePagamentos
    {
        private readonly IServicoDePedidos _servicoDePedidos;
        private readonly Dictionary<int, PagamentoInfo> _pagamentos;

        public ServicoDePagamentos(IServicoDePedidos servicoDePedidos)
        {
            _servicoDePedidos = servicoDePedidos;
            _pagamentos = new Dictionary<int, PagamentoInfo>();
        }

        public async Task<bool> ProcessarPagamentoAsync(int pedidoId, string metodoPagamento, decimal valor)
        {
            if (string.IsNullOrWhiteSpace(metodoPagamento))
                throw new ArgumentException("Método de pagamento é obrigatório", nameof(metodoPagamento));

            if (valor <= 0)
                throw new ArgumentException("Valor deve ser maior que zero", nameof(valor));

            var pedido = await _servicoDePedidos.ObterPedidoPorIdAsync(pedidoId);
            if (pedido == null)
                throw new InvalidOperationException("Pedido não encontrado");

            if (pedido.Status != StatusPedido.Confirmado)
                throw new InvalidOperationException("Apenas pedidos confirmados podem ser pagos");

            if (valor != pedido.ValorTotal)
                throw new InvalidOperationException("Valor do pagamento não confere com o valor do pedido");

            var metodosDisponiveis = await ObterMetodosPagamentoDisponiveisAsync();
            if (!metodosDisponiveis.Contains(metodoPagamento))
                throw new InvalidOperationException("Método de pagamento não suportado");

            // Simular processamento do pagamento
            await Task.Delay(100);

            // Simular falha em 10% dos casos para testes
            var random = new Random();
            if (random.Next(1, 11) == 1)
                return false;

            var pagamentoInfo = new PagamentoInfo
            {
                PedidoId = pedidoId,
                MetodoPagamento = metodoPagamento,
                Valor = valor,
                DataPagamento = DateTime.Now,
                Status = StatusPagamento.Aprovado
            };

            _pagamentos[pedidoId] = pagamentoInfo;

            // Atualizar status do pedido
            pedido.Status = StatusPedido.Pago;
            pedido.DataPagamento = pagamentoInfo.DataPagamento;
            pedido.MetodoPagamento = metodoPagamento;

            return true;
        }

        public async Task<bool> VerificarStatusPagamentoAsync(int pedidoId)
        {
            await Task.Delay(10);
            return _pagamentos.ContainsKey(pedidoId) && 
                   _pagamentos[pedidoId].Status == StatusPagamento.Aprovado;
        }

        public async Task<bool> EstornarPagamentoAsync(int pedidoId)
        {
            await Task.Delay(50);
            
            if (!_pagamentos.ContainsKey(pedidoId))
                return false;

            var pagamento = _pagamentos[pedidoId];
            if (pagamento.Status != StatusPagamento.Aprovado)
                return false;

            pagamento.Status = StatusPagamento.Estornado;
            pagamento.DataEstorno = DateTime.Now;

            var pedido = await _servicoDePedidos.ObterPedidoPorIdAsync(pedidoId);
            if (pedido != null)
            {
                pedido.Status = StatusPedido.Cancelado;
            }

            return true;
        }

        public async Task<List<string>> ObterMetodosPagamentoDisponiveisAsync()
        {
            await Task.Delay(10);
            return new List<string>
            {
                "CartaoCredito",
                "CartaoDebito",
                "Pix",
                "Boleto",
                "TransferenciaBancaria"
            };
        }
    }

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