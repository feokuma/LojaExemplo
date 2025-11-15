using Moq;
using LojaExemplo.Servicos;
using LojaExemplo.Repositorios;
using LojaExemplo.Modelos;

namespace LojaExemplo.Unidade
{
    public class ServicoDePedidosTests
    {
        private readonly Mock<IRepositorioDeProdutos> _mockRepositorioDeProdutos;
        private readonly Mock<IServicoDeDesconto> _mockServicoDeDesconto;
        private readonly ServicoDePedidos _servicoDePedidos;

        public ServicoDePedidosTests()
        {
            _mockRepositorioDeProdutos = new Mock<IRepositorioDeProdutos>();
            _mockServicoDeDesconto = new Mock<IServicoDeDesconto>();
            _servicoDePedidos = new ServicoDePedidos(_mockRepositorioDeProdutos.Object, _mockServicoDeDesconto.Object);
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

            // Act
            var resultado = await _servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);

            // Assert
            Assert.True(resultado);
            Assert.Equal(StatusPedido.Confirmado, pedido.Status);
        }

        [Fact]
        public async Task ConfirmarPedidoAsync_ComPedidoInexistente_DeveRetornarFalse()
        {
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
            await _servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            await _servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);

            // Act
            var pedidos = await _servicoDePedidos.ObterPedidosPorClienteAsync(clienteEmail);

            // Assert
            Assert.Equal(2, pedidos.Count);
            Assert.All(pedidos, p => Assert.Equal(clienteEmail, p.ClienteEmail));
        }



        [Fact]
        public async Task CancelarPedidoAsync_ComPedidoConfirmado_DeveAdicionarEstoqueDeVolta()
        {
            // Arrange
            var clienteEmail = "cliente@teste.com";
            var produto = new Produto { Id = 1, Nome = "Notebook", Preco = 2500.00m, EstoqueDisponivel = 10 };
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 3 },
                new ItemDePedido { ProdutoId = 2, Quantidade = 2 }
            };

            _mockRepositorioDeProdutos.Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync(produto);
            _mockRepositorioDeProdutos.Setup(r => r.VerificarEstoqueDisponivel(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            _mockRepositorioDeProdutos.Setup(r => r.ReduzirEstoqueAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            // Criar e confirmar o pedido
            var pedido = await _servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            await _servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);

            // Act
            var resultado = await _servicoDePedidos.CancelarPedidoAsync(pedido.Id);

            // Assert
            Assert.True(resultado);
            Assert.Equal(StatusPedido.Cancelado, pedido.Status);
            
            // Verificar se AdicionarEstoqueAsync foi chamado para cada item do pedido
            _mockRepositorioDeProdutos.Verify(r => r.AdicionarEstoqueAsync(1, 3), Times.Once);
            _mockRepositorioDeProdutos.Verify(r => r.AdicionarEstoqueAsync(2, 2), Times.Once);
        }

        [Fact]
        public async Task CancelarPedidoAsync_ComPedidoPago_DeveAdicionarEstoqueDeVolta()
        {
            // Arrange
            var clienteEmail = "cliente@teste.com";
            var produto = new Produto { Id = 1, Nome = "Notebook", Preco = 2500.00m, EstoqueDisponivel = 10 };
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 1 }
            };

            _mockRepositorioDeProdutos.Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync(produto);
            _mockRepositorioDeProdutos.Setup(r => r.VerificarEstoqueDisponivel(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            _mockRepositorioDeProdutos.Setup(r => r.ReduzirEstoqueAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            // Criar pedido e simular status Pago
            var pedido = await _servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            pedido.Status = StatusPedido.Pago; // Simular pedido pago

            // Act
            var resultado = await _servicoDePedidos.CancelarPedidoAsync(pedido.Id);

            // Assert
            Assert.True(resultado);
            Assert.Equal(StatusPedido.Cancelado, pedido.Status);
            
            // Verificar se AdicionarEstoqueAsync foi chamado
            _mockRepositorioDeProdutos.Verify(r => r.AdicionarEstoqueAsync(1, 1), Times.Once);
        }

        [Fact]
        public async Task CancelarPedidoAsync_ComPedidoPendente_NaoDeveAdicionarEstoque()
        {
            // Arrange
            var clienteEmail = "cliente@teste.com";
            var produto = new Produto { Id = 1, Nome = "Notebook", Preco = 2500.00m, EstoqueDisponivel = 10 };
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 2 }
            };

            _mockRepositorioDeProdutos.Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync(produto);
            _mockRepositorioDeProdutos.Setup(r => r.VerificarEstoqueDisponivel(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            // Criar pedido (fica com status Pendente)
            var pedido = await _servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);

            // Act
            var resultado = await _servicoDePedidos.CancelarPedidoAsync(pedido.Id);

            // Assert
            Assert.True(resultado);
            Assert.Equal(StatusPedido.Cancelado, pedido.Status);
            
            // Verificar que AdicionarEstoqueAsync NÃO foi chamado para pedidos pendentes
            _mockRepositorioDeProdutos.Verify(r => r.AdicionarEstoqueAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task CriarPedidoComDescontoAsync_DeveUsarServicoDeDesconto()
        {
            // Arrange
            var clienteEmail = "teste@exemplo.com";
            var percentualDesconto = 10m;
            var valorTotal = 200m;
            var descontoCalculado = 19m; // (200-10) * 10/100 = 19
            var valorFinal = 181m;

            var produto1 = new Produto { Id = 1, Nome = "Produto 1", Preco = 100m };
            var produto2 = new Produto { Id = 2, Nome = "Produto 2", Preco = 100m };

            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 1 },
                new ItemDePedido { ProdutoId = 2, Quantidade = 1 }
            };

            _mockRepositorioDeProdutos.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(produto1);
            _mockRepositorioDeProdutos.Setup(r => r.ObterPorIdAsync(2)).ReturnsAsync(produto2);
            _mockRepositorioDeProdutos.Setup(r => r.VerificarEstoqueDisponivel(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            _mockServicoDeDesconto.Setup(s => s.CalcularDescontoProgressivoAsync(valorTotal, percentualDesconto))
                .ReturnsAsync(descontoCalculado);
            _mockServicoDeDesconto.Setup(s => s.AplicarDescontoAsync(valorTotal, descontoCalculado))
                .ReturnsAsync(valorFinal);

            // Act
            var pedido = await _servicoDePedidos.CriarPedidoComDescontoAsync(clienteEmail, itens, percentualDesconto);

            // Assert
            Assert.NotNull(pedido);
            Assert.Equal(valorFinal, pedido.ValorTotal);
            Assert.Equal(StatusPedido.Pendente, pedido.Status);
            Assert.Equal(clienteEmail, pedido.ClienteEmail);

            // Verify que os métodos do serviço de desconto foram chamados
            _mockServicoDeDesconto.Verify(s => s.CalcularDescontoProgressivoAsync(valorTotal, percentualDesconto), Times.Once);
            _mockServicoDeDesconto.Verify(s => s.AplicarDescontoAsync(valorTotal, descontoCalculado), Times.Once);
        }
    }
}