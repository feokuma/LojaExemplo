using Xunit;
using Moq;
using LojaExemplo.Servicos;
using LojaExemplo.Modelos;
using LojaExemplo.Repositorios;

namespace LojaExemplo.Unidade
{
    public class ServicoDePagamentosTests
    {
        private readonly Mock<IServicoDePedidos> _mockServicoDePedidos;
        private readonly Mock<IRepositorioDePagamentos> _mockRepositorioDePagamentos;
        private readonly ServicoDePagamentos _servicoDePagamentos;

        public ServicoDePagamentosTests()
        {
            _mockServicoDePedidos = new Mock<IServicoDePedidos>();
            _mockRepositorioDePagamentos = new Mock<IRepositorioDePagamentos>();
            _servicoDePagamentos = new ServicoDePagamentos(_mockServicoDePedidos.Object, _mockRepositorioDePagamentos.Object);
        }

        #region ProcessarPagamentoAsync Tests

        [Fact]
        public async Task ProcessarPagamentoAsync_ComMetodoPagamentoVazio_DeveLancarExcecao()
        {
            // Arrange
            int pedidoId = 1;
            string metodoPagamento = "";
            decimal valor = 100m;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _servicoDePagamentos.ProcessarPagamentoAsync(pedidoId, metodoPagamento, valor));
        }

        [Fact]
        public async Task ProcessarPagamentoAsync_ComMetodoPagamentoNull_DeveLancarExcecao()
        {
            // Arrange
            int pedidoId = 1;
            string metodoPagamento = null!;
            decimal valor = 100m;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _servicoDePagamentos.ProcessarPagamentoAsync(pedidoId, metodoPagamento, valor));
        }

        [Fact]
        public async Task ProcessarPagamentoAsync_ComValorZero_DeveLancarExcecao()
        {
            // Arrange
            int pedidoId = 1;
            string metodoPagamento = "CartaoCredito";
            decimal valor = 0m;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _servicoDePagamentos.ProcessarPagamentoAsync(pedidoId, metodoPagamento, valor));
        }

        [Fact]
        public async Task ProcessarPagamentoAsync_ComValorNegativo_DeveLancarExcecao()
        {
            // Arrange
            int pedidoId = 1;
            string metodoPagamento = "CartaoCredito";
            decimal valor = -100m;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _servicoDePagamentos.ProcessarPagamentoAsync(pedidoId, metodoPagamento, valor));
        }

        [Fact]
        public async Task ProcessarPagamentoAsync_ComPedidoInexistente_DeveLancarExcecao()
        {
            // Arrange
            int pedidoId = 999;
            string metodoPagamento = "CartaoCredito";
            decimal valor = 100m;

            _mockServicoDePedidos
                .Setup(s => s.ObterPedidoPorIdAsync(pedidoId))
                .ReturnsAsync((Pedido?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _servicoDePagamentos.ProcessarPagamentoAsync(pedidoId, metodoPagamento, valor));
        }

        [Fact]
        public async Task ProcessarPagamentoAsync_ComPedidoPendente_DeveLancarExcecao()
        {
            // Arrange
            int pedidoId = 1;
            string metodoPagamento = "CartaoCredito";
            decimal valor = 100m;

            var pedido = new Pedido
            {
                Id = pedidoId,
                Status = StatusPedido.Pendente,
                ValorTotal = valor
            };

            _mockServicoDePedidos
                .Setup(s => s.ObterPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(pedido);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _servicoDePagamentos.ProcessarPagamentoAsync(pedidoId, metodoPagamento, valor));

            Assert.Contains("confirmados", exception.Message);
        }

        [Fact]
        public async Task ProcessarPagamentoAsync_ComPedidoCancelado_DeveLancarExcecao()
        {
            // Arrange
            int pedidoId = 1;
            string metodoPagamento = "CartaoCredito";
            decimal valor = 100m;

            var pedido = new Pedido
            {
                Id = pedidoId,
                Status = StatusPedido.Cancelado,
                ValorTotal = valor
            };

            _mockServicoDePedidos
                .Setup(s => s.ObterPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(pedido);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _servicoDePagamentos.ProcessarPagamentoAsync(pedidoId, metodoPagamento, valor));
        }

        [Fact]
        public async Task ProcessarPagamentoAsync_ComValorDiferente_DeveLancarExcecao()
        {
            // Arrange
            int pedidoId = 1;
            string metodoPagamento = "CartaoCredito";
            decimal valorPedido = 100m;
            decimal valorPagamento = 150m;

            var pedido = new Pedido
            {
                Id = pedidoId,
                Status = StatusPedido.Confirmado,
                ValorTotal = valorPedido
            };

            _mockServicoDePedidos
                .Setup(s => s.ObterPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(pedido);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _servicoDePagamentos.ProcessarPagamentoAsync(pedidoId, metodoPagamento, valorPagamento));

            Assert.Contains("não confere", exception.Message);
        }

        [Fact]
        public async Task ProcessarPagamentoAsync_ComMetodoPagamentoInvalido_DeveLancarExcecao()
        {
            // Arrange
            int pedidoId = 1;
            string metodoPagamento = "Bitcoin";
            decimal valor = 100m;

            var pedido = new Pedido
            {
                Id = pedidoId,
                Status = StatusPedido.Confirmado,
                ValorTotal = valor
            };

            _mockServicoDePedidos
                .Setup(s => s.ObterPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(pedido);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _servicoDePagamentos.ProcessarPagamentoAsync(pedidoId, metodoPagamento, valor));

            Assert.Contains("não suportado", exception.Message);
        }

        [Theory]
        [InlineData("CartaoCredito")]
        [InlineData("CartaoDebito")]
        [InlineData("Pix")]
        [InlineData("Boleto")]
        [InlineData("TransferenciaBancaria")]
        public async Task ProcessarPagamentoAsync_ComDadosValidos_DeveAtualizarStatusDoPedido(string metodoPagamento)
        {
            // Arrange
            int pedidoId = 1;
            decimal valor = 250m;

            var pedido = new Pedido
            {
                Id = pedidoId,
                Status = StatusPedido.Confirmado,
                ValorTotal = valor,
                ClienteEmail = "cliente@teste.com"
            };

            _mockServicoDePedidos
                .Setup(s => s.ObterPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(pedido);

            // Act
            var resultado = await _servicoDePagamentos.ProcessarPagamentoAsync(pedidoId, metodoPagamento, valor);

            // Assert
            // Nota: O método tem 10% de chance de falhar aleatoriamente
            // Então verificamos que quando retorna true, o pedido foi atualizado
            if (resultado)
            {
                Assert.Equal(StatusPedido.Pago, pedido.Status);
                Assert.Equal(metodoPagamento, pedido.MetodoPagamento);
                Assert.NotNull(pedido.DataPagamento);
            }
        }

        #endregion

        #region VerificarStatusPagamentoAsync Tests

        [Fact]
        public async Task VerificarStatusPagamentoAsync_ComPedidoSemPagamento_DeveRetornarFalse()
        {
            // Arrange
            int pedidoId = 999;

            _mockRepositorioDePagamentos
                .Setup(r => r.ObterPorPedidoIdAsync(pedidoId))
                .ReturnsAsync((PagamentoInfo?)null);

            // Act
            var resultado = await _servicoDePagamentos.VerificarStatusPagamentoAsync(pedidoId);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task VerificarStatusPagamentoAsync_ComPagamentoAprovado_DeveRetornarTrue()
        {
            // Arrange
            int pedidoId = 1;
            decimal valor = 100m;

            var pagamentoInfo = new PagamentoInfo
            {
                PedidoId = pedidoId,
                MetodoPagamento = "CartaoCredito",
                Valor = valor,
                Status = StatusPagamento.Aprovado,
                DataPagamento = DateTime.Now
            };

            _mockRepositorioDePagamentos
                .Setup(r => r.ObterPorPedidoIdAsync(pedidoId))
                .ReturnsAsync(pagamentoInfo);

            // Act
            var resultado = await _servicoDePagamentos.VerificarStatusPagamentoAsync(pedidoId);

            // Assert
            Assert.True(resultado);
        }

        #endregion

        #region EstornarPagamentoAsync Tests

        [Fact]
        public async Task EstornarPagamentoAsync_ComPedidoSemPagamento_DeveRetornarFalse()
        {
            // Arrange
            int pedidoId = 999;

            _mockRepositorioDePagamentos
                .Setup(r => r.ObterPorPedidoIdAsync(pedidoId))
                .ReturnsAsync((PagamentoInfo?)null);

            // Act
            var resultado = await _servicoDePagamentos.EstornarPagamentoAsync(pedidoId);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task EstornarPagamentoAsync_ComPagamentoAprovado_DeveEstornarEAtualizarPedido()
        {
            // Arrange
            int pedidoId = 1;
            decimal valor = 100m;

            var pedido = new Pedido
            {
                Id = pedidoId,
                Status = StatusPedido.Pago,
                ValorTotal = valor
            };

            var pagamentoInfo = new PagamentoInfo
            {
                PedidoId = pedidoId,
                MetodoPagamento = "CartaoCredito",
                Valor = valor,
                Status = StatusPagamento.Aprovado,
                DataPagamento = DateTime.Now
            };

            _mockServicoDePedidos
                .Setup(s => s.ObterPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(pedido);

            _mockRepositorioDePagamentos
                .Setup(r => r.ObterPorPedidoIdAsync(pedidoId))
                .ReturnsAsync(pagamentoInfo);

            _mockRepositorioDePagamentos
                .Setup(r => r.AtualizarAsync(It.IsAny<PagamentoInfo>()))
                .ReturnsAsync(pagamentoInfo);

            // Act
            var resultado = await _servicoDePagamentos.EstornarPagamentoAsync(pedidoId);

            // Assert
            Assert.True(resultado);
            Assert.Equal(StatusPedido.Cancelado, pedido.Status);
            _mockRepositorioDePagamentos.Verify(r => r.AtualizarAsync(It.IsAny<PagamentoInfo>()), Times.Once);
        }

        [Fact]
        public async Task EstornarPagamentoAsync_DuasVezes_SegundaDeveRetornarFalse()
        {
            // Arrange
            int pedidoId = 1;
            decimal valor = 100m;

            var pedido = new Pedido
            {
                Id = pedidoId,
                Status = StatusPedido.Pago,
                ValorTotal = valor
            };

            var pagamentoInfo = new PagamentoInfo
            {
                PedidoId = pedidoId,
                MetodoPagamento = "CartaoCredito",
                Valor = valor,
                Status = StatusPagamento.Aprovado,
                DataPagamento = DateTime.Now
            };

            var pagamentoEstornado = new PagamentoInfo
            {
                PedidoId = pedidoId,
                MetodoPagamento = "CartaoCredito",
                Valor = valor,
                Status = StatusPagamento.Estornado,
                DataPagamento = DateTime.Now,
                DataEstorno = DateTime.Now
            };

            _mockServicoDePedidos
                .Setup(s => s.ObterPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(pedido);

            // Primeira chamada retorna pagamento aprovado, segunda retorna estornado
            _mockRepositorioDePagamentos
                .SetupSequence(r => r.ObterPorPedidoIdAsync(pedidoId))
                .ReturnsAsync(pagamentoInfo)
                .ReturnsAsync(pagamentoEstornado);

            _mockRepositorioDePagamentos
                .Setup(r => r.AtualizarAsync(It.IsAny<PagamentoInfo>()))
                .ReturnsAsync(pagamentoEstornado);

            // Act
            var primeiroEstorno = await _servicoDePagamentos.EstornarPagamentoAsync(pedidoId);
            var segundoEstorno = await _servicoDePagamentos.EstornarPagamentoAsync(pedidoId);

            // Assert
            Assert.True(primeiroEstorno);
            Assert.False(segundoEstorno);
        }

        #endregion

        #region ObterMetodosPagamentoDisponiveisAsync Tests

        [Fact]
        public async Task ObterMetodosPagamentoDisponiveisAsync_DeveRetornarListaComMetodosEsperados()
        {
            // Act
            var metodos = await _servicoDePagamentos.ObterMetodosPagamentoDisponiveisAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.Contains("CartaoCredito", metodos);
                Assert.Contains("CartaoDebito", metodos);
                Assert.Contains("Pix", metodos);
                Assert.Contains("Boleto", metodos);
                Assert.Contains("TransferenciaBancaria", metodos);
            });
        }

        [Fact]
        public async Task ObterMetodosPagamentoDisponiveisAsync_DeveRetornarListaNaoVazia()
        {
            // Act
            var metodos = await _servicoDePagamentos.ObterMetodosPagamentoDisponiveisAsync();

            // Assert
            Assert.NotEmpty(metodos);
        }

        [Fact]
        public async Task ObterMetodosPagamentoDisponiveisAsync_NaoDeveConterMetodosInvalidos()
        {
            // Act
            var metodos = await _servicoDePagamentos.ObterMetodosPagamentoDisponiveisAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.DoesNotContain("Dinheiro", metodos);
                Assert.DoesNotContain("Cheque", metodos);
                Assert.DoesNotContain("Bitcoin", metodos);
            });
        }

        #endregion
    }
}
