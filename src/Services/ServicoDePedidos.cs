using LojaExemplo.Modelos;
using LojaExemplo.Repositorios;

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
        Task<decimal> CalcularDescontoProgressivoAsync(decimal valorTotal, decimal percentualDesconto);
        Task<Pedido> CriarPedidoComDescontoAsync(string clienteEmail, List<ItemDePedido> itens, decimal percentualDesconto);
    }

    public class ServicoDePedidos : IServicoDePedidos
    {
        private readonly IRepositorioDeProdutos _repositorioDeProdutos;
        private readonly List<Pedido> _pedidos;
        private int _proximoId = 1;

        public ServicoDePedidos(IRepositorioDeProdutos repositorioDeProdutos)
        {
            _repositorioDeProdutos = repositorioDeProdutos;
            _pedidos = new List<Pedido>();
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
                Id = _proximoId++,
                DataPedido = DateTime.Now,
                ClienteEmail = clienteEmail,
                Status = StatusPedido.Pendente,
                Itens = itens,
                ValorTotal = valorTotal
            };

            _pedidos.Add(pedido);
            return pedido;
        }

        public async Task<Pedido?> ObterPedidoPorIdAsync(int id)
        {
            await Task.Delay(10);
            return _pedidos.FirstOrDefault(p => p.Id == id);
        }

        public async Task<List<Pedido>> ObterPedidosPorClienteAsync(string clienteEmail)
        {
            await Task.Delay(10);
            return _pedidos.Where(p => p.ClienteEmail.Equals(clienteEmail, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<bool> ConfirmarPedidoAsync(int pedidoId)
        {
            var pedido = await ObterPedidoPorIdAsync(pedidoId);
            if (pedido == null || pedido.Status != StatusPedido.Pendente)
                return false;

            // Reduzir estoque dos produtos
            foreach (var item in pedido.Itens)
            {
                if (!await _repositorioDeProdutos.ReduzirEstoqueAsync(item.ProdutoId, item.Quantidade))
                    return false;
            }

            pedido.Status = StatusPedido.Confirmado;
            return true;
        }

        public async Task<bool> CancelarPedidoAsync(int pedidoId)
        {
            var pedido = await ObterPedidoPorIdAsync(pedidoId);
            if (pedido == null || pedido.Status == StatusPedido.Cancelado)
                return false;

            // Se o pedido foi confirmado, devolver produtos ao estoque
            // if (pedido.Status == StatusPedido.Confirmado || pedido.Status == StatusPedido.Pago)
            // {
            //     foreach (var item in pedido.Itens)
            //     {
            //         await _repositorioDeProdutos.AdicionarEstoqueAsync(item.ProdutoId, item.Quantidade);
            //     }
            // }

            pedido.Status = StatusPedido.Cancelado;
            return true;
        }

        public async Task<decimal> CalcularValorTotalAsync(List<ItemDePedido> itens)
        {
            await Task.Delay(10);
            return itens.Sum(item => item.Subtotal);
        }

        /// <summary>
        /// Calcula desconto progressivo baseado no valor total e percentual.
        /// ATENÇÃO: A ordem dos parâmetros importa pois usa subtração!
        /// Fórmula: desconto = (valorTotal - percentualDesconto) * (percentualDesconto / 100)
        /// </summary>
        /// <param name="valorTotal">Valor total do pedido</param>
        /// <param name="percentualDesconto">Percentual de desconto (ex: 10 para 10%)</param>
        /// <returns>Valor do desconto a ser aplicado</returns>
        public async Task<decimal> CalcularDescontoProgressivoAsync(decimal valorTotal, decimal percentualDesconto)
        {
            await Task.Delay(10);
            
            if (valorTotal <= 0)
                throw new ArgumentException("Valor total deve ser maior que zero", nameof(valorTotal));
            
            if (percentualDesconto < 0 || percentualDesconto > 100)
                throw new ArgumentException("Percentual deve estar entre 0 e 100", nameof(percentualDesconto));

            // Fórmula NÃO COMUTATIVA usando subtração:
            // Ex: valorTotal=1000, percentual=10
            //   Correto: (1000 - 10) * 10 / 100 = 990 * 0.1 = 99
            //   Errado:  (10 - 1000) * 1000 / 100 = -990 * 10 = -9900 (MUITO DIFERENTE!)
            return (valorTotal - percentualDesconto) * percentualDesconto / 100;
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
            
            // ERRO INTENCIONAL: Parâmetros invertidos!
            // Deveria ser: CalcularDescontoProgressivoAsync(valorTotal, percentualDesconto)
            // Mas está: CalcularDescontoProgressivoAsync(percentualDesconto, valorTotal)
            var desconto = await CalcularDescontoProgressivoAsync(percentualDesconto, valorTotal);
            var valorFinal = valorTotal - desconto;

            var pedido = new Pedido
            {
                Id = _proximoId++,
                DataPedido = DateTime.Now,
                ClienteEmail = clienteEmail,
                Status = StatusPedido.Pendente,
                Itens = itens,
                ValorTotal = valorFinal
            };

            _pedidos.Add(pedido);
            return pedido;
        }
    }
}