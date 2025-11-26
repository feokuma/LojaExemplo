using Xunit;
using LojaExemplo.Servicos;

namespace LojaExemplo.Tests.Unidade
{
    public class ServicoDeDescontoTests
    {
        private readonly ServicoDeDesconto _servicoDeDesconto;

        public ServicoDeDescontoTests()
        {
            _servicoDeDesconto = new ServicoDeDesconto();
        }

        [Fact]
        public async Task CalcularDescontoProgressivoAsync_ComValoresValidos_DeveCalcularCorretamente()
        {
            // Arrange
            decimal valorTotal = 1000.00m;
            decimal percentualDesconto = 10m; // 10%

            // Act
            var desconto = await _servicoDeDesconto.CalcularDescontoProgressivoAsync(valorTotal, percentualDesconto);

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
            var desconto = await _servicoDeDesconto.CalcularDescontoProgressivoAsync(valorTotal, percentualDesconto);

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
                _servicoDeDesconto.CalcularDescontoProgressivoAsync(valorTotal, percentualDesconto));
        }

        [Fact]
        public async Task CalcularDescontoProgressivoAsync_ComPercentualInvalido_DeveRetornarExcecao()
        {
            // Arrange
            decimal valorTotal = 1000.00m;
            decimal percentualDesconto = 150m; // > 100%

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _servicoDeDesconto.CalcularDescontoProgressivoAsync(valorTotal, percentualDesconto));
        }

        [Fact]
        public async Task CalcularDescontoProgressivoAsync_ComPercentualNegativo_DeveRetornarExcecao()
        {
            // Arrange
            decimal valorTotal = 1000.00m;
            decimal percentualDesconto = -5m;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _servicoDeDesconto.CalcularDescontoProgressivoAsync(valorTotal, percentualDesconto));
        }

        [Fact]
        public async Task AplicarDescontoAsync_ComValoresValidos_DeveAplicarDesconto()
        {
            // Arrange
            decimal valorTotal = 1000.00m;
            decimal desconto = 100.00m;

            // Act
            var valorFinal = await _servicoDeDesconto.AplicarDescontoAsync(valorTotal, desconto);

            // Assert
            Assert.Equal(900.00m, valorFinal);
        }

        [Fact]
        public async Task AplicarDescontoAsync_ComDescontoMaiorQueValor_DeveRetornarZero()
        {
            // Arrange
            decimal valorTotal = 100.00m;
            decimal desconto = 150.00m;

            // Act
            var valorFinal = await _servicoDeDesconto.AplicarDescontoAsync(valorTotal, desconto);

            // Assert
            Assert.Equal(0m, valorFinal);
        }

        [Fact]
        public async Task AplicarDescontoAsync_ComDescontoNegativo_DeveRetornarExcecao()
        {
            // Arrange
            decimal valorTotal = 1000.00m;
            decimal desconto = -50.00m;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _servicoDeDesconto.AplicarDescontoAsync(valorTotal, desconto));
        }

        [Fact]
        public async Task AplicarDescontoAsync_ComValorTotalZero_DeveRetornarExcecao()
        {
            // Arrange
            decimal valorTotal = 0m;
            decimal desconto = 50.00m;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _servicoDeDesconto.AplicarDescontoAsync(valorTotal, desconto));
        }

        [Theory]
        [InlineData(100.00, 10, 9.00)]      // (100-10) * 10/100 = 90 * 0.1 = 9
        [InlineData(200.00, 5, 9.75)]       // (200-5) * 5/100 = 195 * 0.05 = 9.75
        [InlineData(50.00, 20, 6.00)]       // (50-20) * 20/100 = 30 * 0.2 = 6
        public async Task CalcularDescontoProgressivoAsync_ComVariosValores_DeveCalcularCorretamente(
            decimal valorTotal, decimal percentual, decimal descontoEsperado)
        {
            // Act
            var desconto = await _servicoDeDesconto.CalcularDescontoProgressivoAsync(valorTotal, percentual);

            // Assert
            Assert.Equal(descontoEsperado, desconto);
        }
    }
}