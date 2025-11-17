using LojaExemplo.Modelos;
using LojaExemplo.Repositorios;

namespace LojaExemplo.Servicos
{
    public class ServicoDePedidos : IServicoDePedidos
    {
        private readonly IRepositorioDeProdutos _repositorioDeProdutos;
        private readonly IServicoDeDesconto _servicoDeDesconto;
        private readonly IRepositorioDePedidos _repositorioDePedidos;

        public ServicoDePedidos(IRepositorioDeProdutos repositorioDeProdutos, IServicoDeDesconto servicoDeDesconto, IRepositorioDePedidos repositorioDePedidos)
        {
            _repositorioDeProdutos = repositorioDeProdutos;
            _servicoDeDesconto = servicoDeDesconto;
            _repositorioDePedidos = repositorioDePedidos;
        }

        public async Task<Pedido> CriarPedidoAsync(string clienteEmail, List<ItemDePedido> itens)
        {
            if (string.IsNullOrWhiteSpace(clienteEmail))
                throw new ArgumentException("Email do cliente é obrigatório", nameof(clienteEmail));

            if (itens == null || !itens.Any())
                throw new ArgumentException("Pedido deve conter pelo menos um item", nameof(itens));

            // Verificar se todos os produtos existem e têm estoque disponível
            foreach (var item in itens)
            {
                var produto = await _repositorioDeProdutos.ObterPorIdAsync(item.ProdutoId);
                if (produto == null)
                    throw new InvalidOperationException($"Produto com ID {item.ProdutoId} não encontrado");

                if (!await _repositorioDeProdutos.VerificarEstoqueDisponivel(item.ProdutoId, item.Quantidade))
                    throw new InvalidOperationException($"Estoque insuficiente para o produto {produto.Nome}");

                item.Produto = produto;
                item.PrecoUnitario = produto.Preco;
            }

            var valorTotal = await CalcularValorTotalAsync(itens);

            var pedido = new Pedido
            {
                Id = _repositorioDePedidos.ObterProximoId(),
                DataPedido = DateTime.Now,
                ClienteEmail = clienteEmail,
                Status = StatusPedido.Pendente,
                Itens = itens,
                ValorTotal = valorTotal
            };

            await _repositorioDePedidos.AdicionarAsync(pedido);
            return pedido;
        }

        public async Task<Pedido?> ObterPedidoPorIdAsync(int id)
        {
            return await _repositorioDePedidos.ObterPorIdAsync(id);
        }

        public async Task<List<Pedido>> ObterPedidosPorClienteAsync(string clienteEmail)
        {
            return await _repositorioDePedidos.ObterPorClienteAsync(clienteEmail);
        }

        public async Task<bool> ConfirmarPedidoAsync(int pedidoId)
        {
            var pedido = await ObterPedidoPorIdAsync(pedidoId);
            if (pedido == null)
                return false;

            if (!pedido.Confirmar())
                return false;

            // Reduzir estoque dos produtos
            foreach (var item in pedido.Itens)
            {
                if (!await _repositorioDeProdutos.ReduzirEstoqueAsync(item.ProdutoId, item.Quantidade))
                    return false;
            }

            return true;
        }

        public async Task<bool> CancelarPedidoAsync(int pedidoId)
        {
            var pedido = await ObterPedidoPorIdAsync(pedidoId);
            if (pedido == null)
                return false;

            var deveReporEstoque = pedido.DeveReporEstoque();
            
            if (!pedido.Cancelar())
                return false;

            // Se o pedido foi confirmado ou pago, devolver produtos ao estoque
            if (deveReporEstoque)
            {
                foreach (var item in pedido.Itens)
                {
                    await _repositorioDeProdutos.AdicionarEstoqueAsync(item.ProdutoId, item.Quantidade);
                }
            }

            return true;
        }

        public async Task<decimal> CalcularValorTotalAsync(List<ItemDePedido> itens)
        {
            await Task.Delay(10);
            return itens.Sum(item => item.Subtotal);
        }



        public async Task<Pedido> CriarPedidoComDescontoAsync(string clienteEmail, List<ItemDePedido> itens, decimal percentualDesconto)
        {
            if (string.IsNullOrWhiteSpace(clienteEmail))
                throw new ArgumentException("Email do cliente é obrigatório", nameof(clienteEmail));

            if (itens == null || !itens.Any())
                throw new ArgumentException("Pedido deve conter pelo menos um item", nameof(itens));

            // Verificar se todos os produtos existem e têm estoque disponível
            foreach (var item in itens)
            {
                var produto = await _repositorioDeProdutos.ObterPorIdAsync(item.ProdutoId);
                if (produto == null)
                    throw new InvalidOperationException($"Produto com ID {item.ProdutoId} não encontrado");

                if (!await _repositorioDeProdutos.VerificarEstoqueDisponivel(item.ProdutoId, item.Quantidade))
                    throw new InvalidOperationException($"Estoque insuficiente para o produto {produto.Nome}");

                item.Produto = produto;
                item.PrecoUnitario = produto.Preco;
            }

            var valorTotal = await CalcularValorTotalAsync(itens);
            
            var desconto = await _servicoDeDesconto.CalcularDescontoProgressivoAsync(valorTotal, percentualDesconto);
            var valorFinal = await _servicoDeDesconto.AplicarDescontoAsync(valorTotal, desconto);

            var pedido = new Pedido
            {
                Id = _repositorioDePedidos.ObterProximoId(),
                DataPedido = DateTime.Now,
                ClienteEmail = clienteEmail,
                Status = StatusPedido.Pendente,
                Itens = itens,
                ValorTotal = valorFinal
            };

            await _repositorioDePedidos.AdicionarAsync(pedido);
            return pedido;
        }
    }
}