using Xunit;
using Moq;
using LojaExemplo.Servicos;
using LojaExemplo.Repositorios;
using LojaExemplo.Modelos;

namespace LojaExemplo.Unidade
{
    public class ServicoDePedidosTests
    {
        private readonly Mock<IRepositorioDeProdutos> _mockRepositorioDeProdutos;
        private readonly Mock<IRepositorioDePedidos> _mockRepositorioDePedidos;
        private readonly ServicoDePedidos _servicoDePedidos;
        private int _proximoId = 1;

        public ServicoDePedidosTests()
        {
            _mockRepositorioDeProdutos = new Mock<IRepositorioDeProdutos>();
            _mockRepositorioDePedidos = new Mock<IRepositorioDePedidos>();
            
            // Configurar o mock para simular o comportamento de adicionar pedido
            _mockRepositorioDePedidos.Setup(r => r.AdicionarAsync(It.IsAny<Pedido>()))
                .ReturnsAsync((Pedido p) => { p.Id = _proximoId++; return p; });
            
            _servicoDePedidos = new ServicoDePedidos(_mockRepositorioDeProdutos.Object, _mockRepositorioDePedidos.Object);
        }

        [Fact]
        public async Task CriarPedidoAsync_ComDadosValidos_DeveCriarPedidoComSucesso()
        {
            // Arrange
            var clienteEmail = "cliente@teste.com";
            var produto = new Produto { Id = 1, Nome = "Notebook", Preco = 2500.00m, EstoqueDisponivel = 10 };
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 2 }
            };

            _mockRepositorioDeProdutos.Setup(r => r.ObterPorIdAsync(1))
                .ReturnsAsync(produto);
            _mockRepositorioDeProdutos.Setup(r => r.VerificarEstoqueDisponivel(1, 2))
                .ReturnsAsync(true);

            // Act
            var pedido = await _servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);

            // Assert
            Assert.NotNull(pedido);
            Assert.Equal(clienteEmail, pedido.ClienteEmail);
            Assert.Equal(StatusPedido.Pendente, pedido.Status);
            Assert.Single(pedido.Itens);
            Assert.Equal(5000.00m, pedido.ValorTotal); // 2 * 2500
        }

        [Fact]
        public async Task CriarPedidoAsync_ComEmailVazio_DeveRetornarExcecao()
        {
            // Arrange
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 1 }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _servicoDePedidos.CriarPedidoAsync("", itens));
        }

        [Fact]
        public async Task CriarPedidoAsync_SemItens_DeveRetornarExcecao()
        {
            // Arrange
            var clienteEmail = "cliente@teste.com";
            var itens = new List<ItemDePedido>();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _servicoDePedidos.CriarPedidoAsync(clienteEmail, itens));
        }

        [Fact]
        public async Task CriarPedidoAsync_ComProdutoInexistente_DeveRetornarExcecao()
        {
            // Arrange
            var clienteEmail = "cliente@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 999, Quantidade = 1 }
            };

            _mockRepositorioDeProdutos.Setup(r => r.ObterPorIdAsync(999))
                .ReturnsAsync((Produto?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _servicoDePedidos.CriarPedidoAsync(clienteEmail, itens));
        }

        [Fact]
        public async Task CriarPedidoAsync_ComEstoqueInsuficiente_DeveRetornarExcecao()
        {
            // Arrange
            var clienteEmail = "cliente@teste.com";
            var produto = new Produto { Id = 1, Nome = "Notebook", Preco = 2500.00m, EstoqueDisponivel = 5 };
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 10 }
            };

            _mockRepositorioDeProdutos.Setup(r => r.ObterPorIdAsync(1))
                .ReturnsAsync(produto);
            _mockRepositorioDeProdutos.Setup(r => r.VerificarEstoqueDisponivel(1, 10))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _servicoDePedidos.CriarPedidoAsync(clienteEmail, itens));
        }

        [Fact]
        public async Task ConfirmarPedidoAsync_ComPedidoPendente_DeveConfirmarComSucesso()
        {
            // Arrange
            var clienteEmail = "cliente@teste.com";
            var produto = new Produto { Id = 1, Nome = "Notebook", Preco = 2500.00m, EstoqueDisponivel = 10 };
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 2 }
            };

            _mockRepositorioDeProdutos.Setup(r => r.ObterPorIdAsync(1))
                .ReturnsAsync(produto);
            _mockRepositorioDeProdutos.Setup(r => r.VerificarEstoqueDisponivel(1, 2))
                .ReturnsAsync(true);
            _mockRepositorioDeProdutos.Setup(r => r.ReduzirEstoqueAsync(1, 2))
                .ReturnsAsync(true);

            var pedido = await _servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            
            // Configurar o mock para retornar o pedido criado
            _mockRepositorioDePedidos.Setup(r => r.ObterPorIdAsync(pedido.Id))
                .ReturnsAsync(pedido);
            _mockRepositorioDePedidos.Setup(r => r.AtualizarAsync(It.IsAny<Pedido>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);

            // Assert
            Assert.True(resultado);
            Assert.Equal(StatusPedido.Confirmado, pedido.Status);
        }

        [Fact]
        public async Task ConfirmarPedidoAsync_ComPedidoInexistente_DeveRetornarFalse()
        {
            // Arrange
            _mockRepositorioDePedidos.Setup(r => r.ObterPorIdAsync(999))
                .ReturnsAsync((Pedido?)null);
            
            // Act
            var resultado = await _servicoDePedidos.ConfirmarPedidoAsync(999);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task CancelarPedidoAsync_ComPedidoExistente_DeveCancelarComSucesso()
        {
            // Arrange
            var clienteEmail = "cliente@teste.com";
            var produto = new Produto { Id = 1, Nome = "Notebook", Preco = 2500.00m, EstoqueDisponivel = 10 };
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 1 }
            };

            _mockRepositorioDeProdutos.Setup(r => r.ObterPorIdAsync(1))
                .ReturnsAsync(produto);
            _mockRepositorioDeProdutos.Setup(r => r.VerificarEstoqueDisponivel(1, 1))
                .ReturnsAsync(true);

            var pedido = await _servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            
            // Configurar o mock para retornar o pedido criado
            _mockRepositorioDePedidos.Setup(r => r.ObterPorIdAsync(pedido.Id))
                .ReturnsAsync(pedido);
            _mockRepositorioDePedidos.Setup(r => r.AtualizarAsync(It.IsAny<Pedido>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _servicoDePedidos.CancelarPedidoAsync(pedido.Id);

            // Assert
            Assert.True(resultado);
            Assert.Equal(StatusPedido.Cancelado, pedido.Status);
        }

        [Fact]
        public async Task CalcularValorTotalAsync_ComMultiplosItens_DeveCalcularCorretamente()
        {
            // Arrange
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 2, PrecoUnitario = 100.00m },
                new ItemDePedido { ProdutoId = 2, Quantidade = 3, PrecoUnitario = 50.00m }
            };

            // Act
            var valorTotal = await _servicoDePedidos.CalcularValorTotalAsync(itens);

            // Assert
            Assert.Equal(350.00m, valorTotal); // (2 * 100) + (3 * 50) = 350
        }

        [Fact]
        public async Task ObterPedidosPorClienteAsync_ComClienteExistente_DeveRetornarPedidos()
        {
            // Arrange
            var clienteEmail = "cliente@teste.com";
            var produto = new Produto { Id = 1, Nome = "Notebook", Preco = 2500.00m, EstoqueDisponivel = 10 };
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 1 }
            };

            _mockRepositorioDeProdutos.Setup(r => r.ObterPorIdAsync(1))
                .ReturnsAsync(produto);
            _mockRepositorioDeProdutos.Setup(r => r.VerificarEstoqueDisponivel(1, 1))
                .ReturnsAsync(true);

            // Criar dois pedidos para o mesmo cliente
            var pedido1 = await _servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            var pedido2 = await _servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            
            // Configurar o mock para retornar os pedidos criados
            var pedidosList = new List<Pedido> { pedido1, pedido2 };
            _mockRepositorioDePedidos.Setup(r => r.ObterPorClienteAsync(clienteEmail))
                .ReturnsAsync(pedidosList);

            // Act
            var pedidos = await _servicoDePedidos.ObterPedidosPorClienteAsync(clienteEmail);

            // Assert
            Assert.Equal(2, pedidos.Count);
            Assert.All(pedidos, p => Assert.Equal(clienteEmail, p.ClienteEmail));
        }

        [Fact]
        public async Task CalcularDescontoProgressivoAsync_ComValoresValidos_DeveCalcularCorretamente()
        {
            // Arrange
            decimal valorTotal = 1000.00m;
            decimal percentualDesconto = 10m; // 10%

            // Act
            var desconto = await _servicoDePedidos.CalcularDescontoProgressivoAsync(valorTotal, percentualDesconto);

            // Assert
            // Fórmula: (1000 - 10) * 10 / 100 = 990 * 0.1 = 99
            Assert.Equal(99.00m, desconto);
        }

        [Fact]
        public async Task CalcularDescontoProgressivoAsync_ComPercentual20_DeveCalcularCorretamente()
        {
            // Arrange
            decimal valorTotal = 500.00m;
            decimal percentualDesconto = 20m; // 20%

            // Act
            var desconto = await _servicoDePedidos.CalcularDescontoProgressivoAsync(valorTotal, percentualDesconto);

            // Assert
            // Fórmula: (500 - 20) * 20 / 100 = 480 * 0.2 = 96
            Assert.Equal(96.00m, desconto);
        }

        [Fact]
        public async Task CalcularDescontoProgressivoAsync_ComValorZero_DeveRetornarExcecao()
        {
            // Arrange
            decimal valorTotal = 0m;
            decimal percentualDesconto = 10m;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _servicoDePedidos.CalcularDescontoProgressivoAsync(valorTotal, percentualDesconto));
        }

        [Fact]
        public async Task CalcularDescontoProgressivoAsync_ComPercentualInvalido_DeveRetornarExcecao()
        {
            // Arrange
            decimal valorTotal = 1000.00m;
            decimal percentualDesconto = 150m; // > 100%

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _servicoDePedidos.CalcularDescontoProgressivoAsync(valorTotal, percentualDesconto));
        }
    }
}