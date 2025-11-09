using System.Net.Http.Json;
using System.Net;
using LojaExemplo.DTOs;
using LojaExemplo.Modelos;

namespace LojaExemplo.Testes.Integracao
{
    /// <summary>
    /// Exemplo de testes de integração usando HttpClient para testar endpoints da API.
    /// Esta classe demonstra como fazer requisições HTTP completas através do
    /// WebApplicationFactory para testar controllers.
    /// Herda de BaseIntegrationTest para garantir isolamento entre testes.
    /// </summary>
    public class PedidosControllerIntegracaoTests : BaseIntegrationTest
    {
        public PedidosControllerIntegracaoTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Exemplo de teste que faz uma chamada POST para criar um pedido via API
        /// </summary>
        [Fact]
        public async Task PostCriarPedido_ComDadosValidos_RetornaCreated()
        {
            // Arrange
            var request = new CriarPedidoRequest
            {
                ClienteEmail = "teste@exemplo.com",
                Itens = new List<ItemDePedido>
                {
                    new ItemDePedido { ProdutoId = 1, Quantidade = 2 }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/pedidos", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            var pedidoCriado = await response.Content.ReadFromJsonAsync<Pedido>();
            Assert.NotNull(pedidoCriado);
            Assert.Equal(request.ClienteEmail, pedidoCriado.ClienteEmail);
            Assert.Equal(StatusPedido.Pendente, pedidoCriado.Status);
        }

        /// <summary>
        /// Exemplo de teste que faz uma chamada GET para obter pedidos de um cliente
        /// </summary>
        [Fact]
        public async Task GetPedidosPorCliente_RetornaListaDePedidos()
        {
            // Arrange
            var clienteEmail = "cliente.get@teste.com";
            
            // Primeiro, criar alguns pedidos
            var request = new CriarPedidoRequest
            {
                ClienteEmail = clienteEmail,
                Itens = new List<ItemDePedido>
                {
                    new ItemDePedido { ProdutoId = 1, Quantidade = 1 }
                }
            };
            await _client.PostAsJsonAsync("/api/pedidos", request);

            // Act
            var response = await _client.GetAsync($"/api/pedidos/cliente/{clienteEmail}");

            // Assert
            response.EnsureSuccessStatusCode();
            
            var pedidos = await response.Content.ReadFromJsonAsync<List<Pedido>>();
            Assert.NotNull(pedidos);
            Assert.NotEmpty(pedidos);
            Assert.All(pedidos, p => Assert.Equal(clienteEmail, p.ClienteEmail));
        }

        /// <summary>
        /// Exemplo de teste que valida erro de validação (BadRequest)
        /// </summary>
        [Fact]
        public async Task PostCriarPedido_ComEmailInvalido_RetornaBadRequest()
        {
            // Arrange
            var request = new CriarPedidoRequest
            {
                ClienteEmail = "", // Email vazio - inválido
                Itens = new List<ItemDePedido>
                {
                    new ItemDePedido { ProdutoId = 1, Quantidade = 1 }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/pedidos", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Exemplo de teste de fluxo completo: criar, confirmar, processar pagamento
        /// </summary>
        [Fact]
        public async Task FluxoCompleto_CriarConfirmarPagar_ViaAPI()
        {
            // Arrange - Criar pedido
            var criarRequest = new CriarPedidoRequest
            {
                ClienteEmail = "cliente.fluxo@teste.com",
                Itens = new List<ItemDePedido>
                {
                    new ItemDePedido { ProdutoId = 1, Quantidade = 1 }
                }
            };

            var criarResponse = await _client.PostAsJsonAsync("/api/pedidos", criarRequest);
            Assert.Equal(HttpStatusCode.Created, criarResponse.StatusCode);
            
            var pedidoCriado = await criarResponse.Content.ReadFromJsonAsync<Pedido>();
            Assert.NotNull(pedidoCriado);

            // Act - Confirmar pedido
            var confirmarResponse = await _client.PostAsync(
                $"/api/pedidos/{pedidoCriado.Id}/confirmar", null);
            Assert.True(confirmarResponse.IsSuccessStatusCode);

            // Act - Processar pagamento
            var pagamentoRequest = new ProcessarPagamentoRequest
            {
                PedidoId = pedidoCriado.Id,
                MetodoPagamento = "CartaoCredito",
                Valor = pedidoCriado.ValorTotal
            };

            var pagamentoResponse = await _client.PostAsJsonAsync(
                $"/api/pedidos/{pedidoCriado.Id}/pagar", pagamentoRequest);

            // Assert
            Assert.True(pagamentoResponse.IsSuccessStatusCode);
        }

        /// <summary>
        /// Exemplo de teste que verifica headers da resposta
        /// </summary>
        [Fact]
        public async Task PostCriarPedido_RetornaLocationHeader()
        {
            // Arrange
            var request = new CriarPedidoRequest
            {
                ClienteEmail = "teste.location@exemplo.com",
                Itens = new List<ItemDePedido>
                {
                    new ItemDePedido { ProdutoId = 2, Quantidade = 1 }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/pedidos", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
            Assert.Contains("/api/Pedidos/", response.Headers.Location.AbsolutePath.ToString());
        }
    }
}
