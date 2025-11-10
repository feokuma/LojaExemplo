using Xunit;
using LojaExemplo.Modelos;

namespace LojaExemplo.Unidade
{
    public class PedidoTests
    {
        [Fact]
        public void Cancelar_PedidoPendente_DeveAterarStatusParaCancelado()
        {
            // Arrange
            var pedido = new Pedido
            {
                Id = 1,
                Status = StatusPedido.Pendente,
                ClienteEmail = "teste@exemplo.com"
            };

            // Act
            var resultado = pedido.Cancelar();

            // Assert
            Assert.True(resultado);
            Assert.Equal(StatusPedido.Cancelado, pedido.Status);
        }

        [Fact]
        public void Cancelar_PedidoJaCancelado_NaoDeveCancelar()
        {
            // Arrange
            var pedido = new Pedido
            {
                Id = 1,
                Status = StatusPedido.Cancelado,
                ClienteEmail = "teste@exemplo.com"
            };

            // Act
            var resultado = pedido.Cancelar();

            // Assert
            Assert.False(resultado);
            Assert.Equal(StatusPedido.Cancelado, pedido.Status);
        }

        [Fact]
        public void Confirmar_PedidoPendente_DeveAterarStatusParaConfirmadoERetornarTrue()
        {
            // Arrange
            var pedido = new Pedido
            {
                Id = 1,
                Status = StatusPedido.Pendente,
                ClienteEmail = "teste@exemplo.com"
            };

            // Act
            var resultado = pedido.Confirmar();

            // Assert
            Assert.True(resultado);
            Assert.Equal(StatusPedido.Confirmado, pedido.Status);
        }

        [Fact]
        public void Confirmar_PedidoJaConfirmado_NaoDeveConfirmarRetornandoFalse()
        {
            // Arrange
            var pedido = new Pedido
            {
                Id = 1,
                Status = StatusPedido.Confirmado,
                ClienteEmail = "teste@exemplo.com"
            };

            // Act
            var resultado = pedido.Confirmar();

            // Assert
            Assert.False(resultado);
            Assert.Equal(StatusPedido.Confirmado, pedido.Status);
        }

        [Theory]
        [InlineData(StatusPedido.Confirmado, true)]
        [InlineData(StatusPedido.Pago, true)]
        [InlineData(StatusPedido.Pendente, false)]
        [InlineData(StatusPedido.Cancelado, false)]
        [InlineData(StatusPedido.Enviado, false)]
        [InlineData(StatusPedido.Entregue, false)]
        public void DeveReporEstoque_DeveRetornarCorretamente(StatusPedido status, bool esperado)
        {
            // Arrange
            var pedido = new Pedido
            {
                Id = 1,
                Status = status,
                ClienteEmail = "teste@exemplo.com"
            };

            // Act
            var resultado = pedido.DeveReporEstoque();

            // Assert
            Assert.Equal(esperado, resultado);
        }

        [Theory]
        [InlineData(StatusPedido.Pendente, true)]
        [InlineData(StatusPedido.Confirmado, true)]
        [InlineData(StatusPedido.Pago, true)]
        [InlineData(StatusPedido.Enviado, true)]
        [InlineData(StatusPedido.Entregue, true)]
        [InlineData(StatusPedido.Cancelado, false)]
        public void PodeCancelar_DeveRetornarCorretamente(StatusPedido status, bool esperado)
        {
            // Arrange
            var pedido = new Pedido
            {
                Id = 1,
                Status = status,
                ClienteEmail = "teste@exemplo.com"
            };

            // Act
            var resultado = pedido.PodeCancelar();

            // Assert
            Assert.Equal(esperado, resultado);
        }
    }
}