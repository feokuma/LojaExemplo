using LojaExemplo.Modelos;
using LojaExemplo.Repositorios;

namespace LojaExemplo.Servicos
{
    public class ServicoDePagamentos : IServicoDePagamentos
    {
        private readonly IServicoDePedidos _servicoDePedidos;
        private readonly IRepositorioDePagamentos _repositorioDePagamentos;

        public ServicoDePagamentos(IServicoDePedidos servicoDePedidos, IRepositorioDePagamentos repositorioDePagamentos)
        {
            _servicoDePedidos = servicoDePedidos;
            _repositorioDePagamentos = repositorioDePagamentos;
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

            var pagamentoInfo = new PagamentoInfo
            {
                PedidoId = pedidoId,
                MetodoPagamento = metodoPagamento,
                Valor = valor,
                DataPagamento = DateTime.Now,
                Status = StatusPagamento.Aprovado
            };

            await _repositorioDePagamentos.AdicionarAsync(pagamentoInfo);

            // Atualizar status do pedido
            pedido.Status = StatusPedido.Pago;
            pedido.DataPagamento = pagamentoInfo.DataPagamento;
            pedido.MetodoPagamento = metodoPagamento;

            return true;
        }

        public async Task<bool> VerificarStatusPagamentoAsync(int pedidoId)
        {
            var pagamento = await _repositorioDePagamentos.ObterPorPedidoIdAsync(pedidoId);
            return pagamento != null && pagamento.Status == StatusPagamento.Aprovado;
        }

        public async Task<bool> EstornarPagamentoAsync(int pedidoId)
        {
            await Task.Delay(50);
            
            var pagamento = await _repositorioDePagamentos.ObterPorPedidoIdAsync(pedidoId);
            if (pagamento == null)
                return false;

            if (pagamento.Status != StatusPagamento.Aprovado)
                return false;

            pagamento.Status = StatusPagamento.Estornado;
            pagamento.DataEstorno = DateTime.Now;

            await _repositorioDePagamentos.AtualizarAsync(pagamento);

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
}