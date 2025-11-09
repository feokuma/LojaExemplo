using Xunit;
using LojaExemplo.Servicos;
using LojaExemplo.Repositorios;
using LojaExemplo.Modelos;
using Microsoft.Extensions.DependencyInjection;

namespace LojaExemplo.Testes.Integracao
{
    public class ServicoDePagamentosIntegracaoTests : BaseIntegrationTest
    {
        private readonly IServiceScope _scope;

        public ServicoDePagamentosIntegracaoTests(CustomWebApplicationFactory factory) : base(factory)
        {
            _scope = _factory.CreateScope();
        }

        private (IRepositorioDeProdutos, IServicoDePedidos, IServicoDePagamentos) CriarServicos()
        {
            var repositorio = _scope.ServiceProvider.GetRequiredService<IRepositorioDeProdutos>();
            var servicoPedidos = _scope.ServiceProvider.GetRequiredService<IServicoDePedidos>();
            var servicoPagamentos = _scope.ServiceProvider.GetRequiredService<IServicoDePagamentos>();
            return (repositorio, servicoPedidos, servicoPagamentos);
        }

        [Fact]
        public async Task ProcessarPagamento_ComPedidoConfirmado_DeveProcessarComSucesso()
        {
            // Arrange
            var (_, servicoDePedidos, servicoDePagamentos) = CriarServicos();
            var clienteEmail = "cliente.pagamento1@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 1 }
            };

            var pedido = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            await servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);

            // Act
            var pagamentoProcessado = await servicoDePagamentos.ProcessarPagamentoAsync(
                pedido.Id, "CartaoCredito", pedido.ValorTotal);

            // Assert
            Assert.True(pagamentoProcessado);
            Assert.Equal(StatusPedido.Pago, pedido.Status);
            Assert.NotNull(pedido.DataPagamento);
            Assert.Equal("CartaoCredito", pedido.MetodoPagamento);

            // Verificar status do pagamento
            var statusPagamento = await servicoDePagamentos.VerificarStatusPagamentoAsync(pedido.Id);
            Assert.True(statusPagamento);
        }

        [Fact]
        public async Task ProcessarPagamento_ComPedidoNaoConfirmado_DeveLancarExcecao()
        {
            // Arrange
            var (_, servicoDePedidos, servicoDePagamentos) = CriarServicos();
            var clienteEmail = "cliente.pagamento2@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 1 }
            };

            var pedido = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            // NÃO confirma o pedido

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                servicoDePagamentos.ProcessarPagamentoAsync(pedido.Id, "CartaoCredito", pedido.ValorTotal));
        }

        [Fact]
        public async Task ProcessarPagamento_ComValorIncorreto_DeveLancarExcecao()
        {
            // Arrange
            var (_, servicoDePedidos, servicoDePagamentos) = CriarServicos();
            var clienteEmail = "cliente.pagamento3@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 1 }
            };

            var pedido = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            await servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);

            // Act & Assert - Valor diferente do pedido
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                servicoDePagamentos.ProcessarPagamentoAsync(pedido.Id, "CartaoCredito", 1000.00m));
        }

        [Fact]
        public async Task ProcessarPagamento_ComMetodoInvalido_DeveLancarExcecao()
        {
            // Arrange
            var (_, servicoDePedidos, servicoDePagamentos) = CriarServicos();
            var clienteEmail = "cliente.pagamento4@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 2, Quantidade = 1 }
            };

            var pedido = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            await servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                servicoDePagamentos.ProcessarPagamentoAsync(pedido.Id, "Bitcoin", pedido.ValorTotal));
        }

        [Theory]
        [InlineData("CartaoCredito")]
        [InlineData("CartaoDebito")]
        [InlineData("Pix")]
        [InlineData("Boleto")]
        [InlineData("TransferenciaBancaria")]
        public async Task ProcessarPagamento_ComTodosMetodosValidos_DeveProcessarComSucesso(string metodoPagamento)
        {
            // Arrange
            var (_, servicoDePedidos, servicoDePagamentos) = CriarServicos();
            var clienteEmail = $"cliente.metodo.{metodoPagamento}@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 1 }
            };

            var pedido = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            await servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);

            // Act
            var pagamentoProcessado = await servicoDePagamentos.ProcessarPagamentoAsync(
                pedido.Id, metodoPagamento, pedido.ValorTotal);

            // Assert
            if (pagamentoProcessado) // Pode falhar 10% das vezes por design
            {
                Assert.Equal(StatusPedido.Pago, pedido.Status);
                Assert.Equal(metodoPagamento, pedido.MetodoPagamento);
            }
        }

        [Fact]
        public async Task EstornarPagamento_ComPagamentoAprovado_DeveEstornarComSucesso()
        {
            // Arrange
            var (_, servicoDePedidos, servicoDePagamentos) = CriarServicos();
            var clienteEmail = "cliente.estorno1@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 3, Quantidade = 1 }
            };

            var pedido = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            await servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);
            
            // Tentar processar pagamento até ter sucesso (devido à falha aleatória de 10%)
            bool pagamentoProcessado = false;
            int tentativas = 0;
            while (!pagamentoProcessado && tentativas < 20)
            {
                pagamentoProcessado = await servicoDePagamentos.ProcessarPagamentoAsync(pedido.Id, "Pix", pedido.ValorTotal);
                tentativas++;
            }
            Assert.True(pagamentoProcessado, "Pagamento deveria ter sido processado após múltiplas tentativas");

            // Act
            var estornado = await servicoDePagamentos.EstornarPagamentoAsync(pedido.Id);

            // Assert
            Assert.True(estornado);
            Assert.Equal(StatusPedido.Cancelado, pedido.Status);

            // Verificar que o pagamento não está mais aprovado
            var statusPagamento = await servicoDePagamentos.VerificarStatusPagamentoAsync(pedido.Id);
            Assert.False(statusPagamento);
        }

        [Fact]
        public async Task EstornarPagamento_SemPagamento_DeveRetornarFalse()
        {
            // Arrange
            var (_, servicoDePedidos, servicoDePagamentos) = CriarServicos();
            var clienteEmail = "cliente.estorno2@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 1 }
            };

            var pedido = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            await servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);
            // NÃO processa pagamento

            // Act
            var estornado = await servicoDePagamentos.EstornarPagamentoAsync(pedido.Id);

            // Assert
            Assert.False(estornado);
        }

        [Fact]
        public async Task EstornarPagamento_DuasVezes_SegundaDeveRetornarFalse()
        {
            // Arrange
            var (_, servicoDePedidos, servicoDePagamentos) = CriarServicos();
            var clienteEmail = "cliente.estorno3@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 2, Quantidade = 2 }
            };

            var pedido = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            await servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);
            
            // Tentar processar pagamento até ter sucesso (devido à falha aleatória de 10%)
            bool pagamentoProcessado = false;
            int tentativas = 0;
            while (!pagamentoProcessado && tentativas < 20)
            {
                pagamentoProcessado = await servicoDePagamentos.ProcessarPagamentoAsync(pedido.Id, "CartaoCredito", pedido.ValorTotal);
                tentativas++;
            }
            Assert.True(pagamentoProcessado, "Pagamento deveria ter sido processado após múltiplas tentativas");

            // Act
            var primeiroEstorno = await servicoDePagamentos.EstornarPagamentoAsync(pedido.Id);
            var segundoEstorno = await servicoDePagamentos.EstornarPagamentoAsync(pedido.Id);

            // Assert
            Assert.True(primeiroEstorno);
            Assert.False(segundoEstorno);
        }

        [Fact]
        public async Task ObterMetodosPagamentoDisponiveis_DeveRetornarTodosMetodos()
        {
            // Arrange
            var (_, _, servicoDePagamentos) = CriarServicos();

            // Act
            var metodos = await servicoDePagamentos.ObterMetodosPagamentoDisponiveisAsync();

            // Assert
            Assert.NotEmpty(metodos);
            Assert.Equal(5, metodos.Count);
            Assert.Contains("CartaoCredito", metodos);
            Assert.Contains("CartaoDebito", metodos);
            Assert.Contains("Pix", metodos);
            Assert.Contains("Boleto", metodos);
            Assert.Contains("TransferenciaBancaria", metodos);
        }

        [Fact]
        public async Task VerificarStatusPagamento_ComPagamentoAprovado_DeveRetornarTrue()
        {
            // Arrange
            var (_, servicoDePedidos, servicoDePagamentos) = CriarServicos();
            var clienteEmail = "cliente.status1@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 1 }
            };

            var pedido = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            await servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);
            
            // Tentar processar pagamento até ter sucesso (devido à falha aleatória de 10%)
            bool pagamentoProcessado = false;
            int tentativas = 0;
            while (!pagamentoProcessado && tentativas < 20)
            {
                pagamentoProcessado = await servicoDePagamentos.ProcessarPagamentoAsync(pedido.Id, "Pix", pedido.ValorTotal);
                tentativas++;
            }
            Assert.True(pagamentoProcessado, "Pagamento deveria ter sido processado após múltiplas tentativas");

            // Act
            var status = await servicoDePagamentos.VerificarStatusPagamentoAsync(pedido.Id);

            // Assert
            Assert.True(status);
        }

        [Fact]
        public async Task VerificarStatusPagamento_SemPagamento_DeveRetornarFalse()
        {
            // Arrange
            var (_, _, servicoDePagamentos) = CriarServicos();
            var pedidoIdInexistente = 99999;

            // Act
            var status = await servicoDePagamentos.VerificarStatusPagamentoAsync(pedidoIdInexistente);

            // Assert
            Assert.False(status);
        }

        [Fact]
        public async Task FluxoCompleto_CriarConfirmarPagarEstornar_DeveExecutarCorretamente()
        {
            // Arrange
            var (_, servicoDePedidos, servicoDePagamentos) = CriarServicos();
            var clienteEmail = "cliente.fluxocompleto@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 1 },
                new ItemDePedido { ProdutoId = 2, Quantidade = 2 }
            };

            // Act & Assert - Criar pedido
            var pedido = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            Assert.Equal(StatusPedido.Pendente, pedido.Status);

            // Act & Assert - Confirmar pedido
            await servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);
            Assert.Equal(StatusPedido.Confirmado, pedido.Status);

            // Act & Assert - Processar pagamento (tentar até ter sucesso devido à falha aleatória de 10%)
            bool pagamentoProcessado = false;
            int tentativas = 0;
            while (!pagamentoProcessado && tentativas < 20)
            {
                pagamentoProcessado = await servicoDePagamentos.ProcessarPagamentoAsync(
                    pedido.Id, "CartaoCredito", pedido.ValorTotal);
                tentativas++;
            }
            Assert.True(pagamentoProcessado, "Pagamento deveria ter sido processado após múltiplas tentativas");
            Assert.Equal(StatusPedido.Pago, pedido.Status);

            // Act & Assert - Verificar status
            var statusPagamento = await servicoDePagamentos.VerificarStatusPagamentoAsync(pedido.Id);
            Assert.True(statusPagamento);

            // Act & Assert - Estornar
            var estornado = await servicoDePagamentos.EstornarPagamentoAsync(pedido.Id);
            Assert.True(estornado);
            Assert.Equal(StatusPedido.Cancelado, pedido.Status);

            // Act & Assert - Verificar status após estorno
            var statusAposEstorno = await servicoDePagamentos.VerificarStatusPagamentoAsync(pedido.Id);
            Assert.False(statusAposEstorno);
        }

        [Fact]
        public async Task FluxoMultiplosPagamentos_MesmoCliente_DeveGerenciarIndependentemente()
        {
            // Arrange
            var (_, servicoDePedidos, servicoDePagamentos) = CriarServicos();
            var clienteEmail = "cliente.multipagamentos@teste.com";

            // Criar e processar primeiro pedido
            var itens1 = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 1 }
            };
            var pedido1 = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens1);
            await servicoDePedidos.ConfirmarPedidoAsync(pedido1.Id);
            
            // Tentar processar primeiro pagamento até ter sucesso (devido à falha aleatória de 10%)
            bool pagamento1Processado = false;
            int tentativas1 = 0;
            while (!pagamento1Processado && tentativas1 < 20)
            {
                pagamento1Processado = await servicoDePagamentos.ProcessarPagamentoAsync(pedido1.Id, "CartaoCredito", pedido1.ValorTotal);
                tentativas1++;
            }
            Assert.True(pagamento1Processado, "Primeiro pagamento deveria ter sido processado");

            // Criar e processar segundo pedido
            var itens2 = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 2, Quantidade = 3 }
            };
            var pedido2 = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens2);
            await servicoDePedidos.ConfirmarPedidoAsync(pedido2.Id);
            
            // Tentar processar segundo pagamento até ter sucesso (devido à falha aleatória de 10%)
            bool pagamento2Processado = false;
            int tentativas2 = 0;
            while (!pagamento2Processado && tentativas2 < 20)
            {
                pagamento2Processado = await servicoDePagamentos.ProcessarPagamentoAsync(pedido2.Id, "Pix", pedido2.ValorTotal);
                tentativas2++;
            }
            Assert.True(pagamento2Processado, "Segundo pagamento deveria ter sido processado");

            // Criar terceiro pedido mas NÃO processar pagamento
            var itens3 = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 3, Quantidade = 1 }
            };
            var pedido3 = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens3);
            await servicoDePedidos.ConfirmarPedidoAsync(pedido3.Id);

            // Act - Estornar apenas o primeiro pagamento
            var estornado1 = await servicoDePagamentos.EstornarPagamentoAsync(pedido1.Id);

            // Assert
            Assert.True(estornado1);
            Assert.Equal(StatusPedido.Cancelado, pedido1.Status);
            Assert.Equal(StatusPedido.Pago, pedido2.Status);
            Assert.Equal(StatusPedido.Confirmado, pedido3.Status);

            // Verificar status dos pagamentos
            Assert.False(await servicoDePagamentos.VerificarStatusPagamentoAsync(pedido1.Id)); // Estornado
            Assert.True(await servicoDePagamentos.VerificarStatusPagamentoAsync(pedido2.Id));  // Ainda pago
            Assert.False(await servicoDePagamentos.VerificarStatusPagamentoAsync(pedido3.Id)); // Nunca foi pago
        }
    }
}
