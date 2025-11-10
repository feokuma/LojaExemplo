using Xunit;
using LojaExemplo.Servicos;
using LojaExemplo.Repositorios;
using LojaExemplo.Modelos;

namespace LojaExemplo.Testes.Integracao
{
    public class ServicoDePedidosIntegracaoTests
    {
        private (IRepositorioDeProdutos, IServicoDePedidos) CriarServicos()
        {
            var repositorio = new RepositorioDeProdutos();
            var servicoDeDesconto = new ServicoDeDesconto();
            var servicoPedidos = new ServicoDePedidos(repositorio, servicoDeDesconto);
            return (repositorio, servicoPedidos);
        }

        [Fact]
        public async Task CriarPedido_ComDadosValidos_DeveCriarComSucesso()
        {
            // Arrange
            var (_, servicoDePedidos) = CriarServicos();
            var clienteEmail = "cliente.pedido1@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 1 }, // Notebook
                new ItemDePedido { ProdutoId = 2, Quantidade = 2 }  // Mouse
            };

            // Act
            var pedido = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);

            // Assert
            Assert.NotNull(pedido);
            Assert.Equal(StatusPedido.Pendente, pedido.Status);
            Assert.Equal(2600.00m, pedido.ValorTotal); // 2500 + (2 * 50)
            Assert.Equal(clienteEmail, pedido.ClienteEmail);
            Assert.Equal(2, pedido.Itens.Count);
        }

        [Fact]
        public async Task CriarPedido_ComProdutoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var (_, servicoDePedidos) = CriarServicos();
            var clienteEmail = "cliente.pedido2@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 999, Quantidade = 1 }
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                servicoDePedidos.CriarPedidoAsync(clienteEmail, itens));
        }

        [Fact]
        public async Task CriarPedido_ComEstoqueInsuficiente_DeveLancarExcecao()
        {
            // Arrange
            var (_, servicoDePedidos) = CriarServicos();
            var clienteEmail = "cliente.pedido3@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 1000 } // Muito além do estoque
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                servicoDePedidos.CriarPedidoAsync(clienteEmail, itens));
        }

        [Fact]
        public async Task ConfirmarPedido_ComPedidoPendente_DeveConfirmarEReduzirEstoque()
        {
            // Arrange
            var (repositorioDeProdutos, servicoDePedidos) = CriarServicos();
            var produtoId = 1;
            var quantidade = 2;
            var estoqueInicial = (await repositorioDeProdutos.ObterPorIdAsync(produtoId))!.EstoqueDisponivel;
            
            var clienteEmail = "cliente.pedido4@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = produtoId, Quantidade = quantidade }
            };

            var pedido = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);

            // Act
            var confirmado = await servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);

            // Assert
            Assert.True(confirmado);
            Assert.Equal(StatusPedido.Confirmado, pedido.Status);

            // Verificar redução de estoque
            var produtoAtualizado = await repositorioDeProdutos.ObterPorIdAsync(produtoId);
            Assert.Equal(estoqueInicial - quantidade, produtoAtualizado!.EstoqueDisponivel);
        }

        [Fact]
        public async Task CancelarPedido_ComPedidoConfirmado_DeveCancelarEDevolverEstoque()
        {
            // Arrange
            var (repositorioDeProdutos, servicoDePedidos) = CriarServicos();
            var produtoId = 2;
            var quantidade = 3;
            var estoqueAposConfirmacao = 0m;
            
            var clienteEmail = "cliente.pedido5@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = produtoId, Quantidade = quantidade }
            };

            var pedido = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            await servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);
            
            estoqueAposConfirmacao = (await repositorioDeProdutos.ObterPorIdAsync(produtoId))!.EstoqueDisponivel;

            // Act
            var cancelado = await servicoDePedidos.CancelarPedidoAsync(pedido.Id);

            // Assert
            Assert.True(cancelado);
            Assert.Equal(StatusPedido.Cancelado, pedido.Status);

            // Verificar devolução de estoque
            var produtoAtualizado = await repositorioDeProdutos.ObterPorIdAsync(produtoId);
            Assert.Equal(estoqueAposConfirmacao + quantidade, produtoAtualizado!.EstoqueDisponivel);
        }

        [Fact]
        public async Task ObterPedidosPorCliente_ComMultiplosPedidos_DeveRetornarTodos()
        {
            // Arrange
            var (_, servicoDePedidos) = CriarServicos();
            var clienteEmail = "cliente.multiplos@teste.com";

            // Criar três pedidos
            var itens1 = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 1 }
            };
            var pedido1 = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens1);

            var itens2 = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 2, Quantidade = 2 }
            };
            var pedido2 = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens2);

            var itens3 = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 3, Quantidade = 1 }
            };
            var pedido3 = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens3);

            // Act
            var pedidosCliente = await servicoDePedidos.ObterPedidosPorClienteAsync(clienteEmail);

            // Assert
            Assert.Equal(3, pedidosCliente.Count);
            Assert.Contains(pedidosCliente, p => p.Id == pedido1.Id);
            Assert.Contains(pedidosCliente, p => p.Id == pedido2.Id);
            Assert.Contains(pedidosCliente, p => p.Id == pedido3.Id);
        }

        [Fact]
        public async Task FluxoCompleto_CriarConfirmarCancelar_DeveGerenciarEstoqueCorretamente()
        {
            // Arrange
            var (repositorioDeProdutos, servicoDePedidos) = CriarServicos();
            var produtoId = 1;
            var quantidade = 2;
            var estoqueInicial = (await repositorioDeProdutos.ObterPorIdAsync(produtoId))!.EstoqueDisponivel;
            
            var clienteEmail = "cliente.fluxo@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = produtoId, Quantidade = quantidade }
            };

            // Act - Criar pedido
            var pedido = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
            var estoqueAposCriar = (await repositorioDeProdutos.ObterPorIdAsync(produtoId))!.EstoqueDisponivel;
            
            Assert.Equal(estoqueInicial, estoqueAposCriar); // Estoque ainda não foi reduzido

            // Act - Confirmar pedido
            await servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);
            var estoqueAposConfirmar = (await repositorioDeProdutos.ObterPorIdAsync(produtoId))!.EstoqueDisponivel;
            
            Assert.Equal(estoqueInicial - quantidade, estoqueAposConfirmar); // Estoque foi reduzido

            // Act - Cancelar pedido
            await servicoDePedidos.CancelarPedidoAsync(pedido.Id);
            var estoqueAposCancelar = (await repositorioDeProdutos.ObterPorIdAsync(produtoId))!.EstoqueDisponivel;
            
            // Assert - Estoque foi restaurado
            Assert.Equal(estoqueInicial, estoqueAposCancelar);
        }

        [Fact]
        public async Task CalcularValorTotal_ComMultiplosProdutos_DeveCalcularCorretamente()
        {
            // Arrange
            var (_, servicoDePedidos) = CriarServicos();
            var clienteEmail = "cliente.calculo@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 2 },  // 2 * 2500 = 5000
                new ItemDePedido { ProdutoId = 2, Quantidade = 3 },  // 3 * 50 = 150
                new ItemDePedido { ProdutoId = 3, Quantidade = 1 }   // 1 * 150 = 150
            };

            // Act
            var pedido = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);

            // Assert
            var valorEsperado = (2 * 2500m) + (3 * 50m) + (1 * 150m); // 5300
            Assert.Equal(valorEsperado, pedido.ValorTotal);
        }

        #region Testes de Demonstração - Inversão de Parâmetros

        /// <summary>
        /// DEMONSTRAÇÃO: Este teste de integração FALHA porque os parâmetros
        /// estão invertidos na chamada do método CriarPedidoComDescontoAsync.
        /// Os testes unitários passam porque testam o método isoladamente,
        /// mas a integração revela o erro!
        /// </summary>
        [Fact]
        public async Task CriarPedidoComDesconto_Integracao_DeveAplicarDescontoCorretamente()
        {
            // Arrange
            var (_, servicoDePedidos) = CriarServicos();
            var clienteEmail = "cliente.desconto@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 1, Quantidade = 2 } // 2 * 2500 = 5000
            };
            var percentualDesconto = 10m; // 10% de desconto

            // Act - Criar pedido com desconto através do fluxo completo
            var pedido = await servicoDePedidos.CriarPedidoComDescontoAsync(clienteEmail, itens, percentualDesconto);

            // Assert
            // Valor base: 5000
            // Desconto esperado CORRETO: (5000 - 10) * 10 / 100 = 4990 * 0.1 = 499
            // Valor final esperado: 5000 - 499 = 4501
            //
            // MAS na implementação os parâmetros estão INVERTIDOS:
            // desconto calculado ERRADO: (10 - 5000) * 5000 / 100 = -4990 * 50 = -249500
            // Valor final ABSURDO: 5000 - (-249500) = 254500 ❌ ERRO EVIDENTE!
            Assert.Equal(4501.00m, pedido.ValorTotal);
        }

        /// <summary>
        /// DEMONSTRAÇÃO: Este teste FALHA e evidencia o erro de inversão de parâmetros!
        /// </summary>
        [Fact]
        public async Task CriarPedidoComDesconto_Cenario20Porcento_EvidenciaErroDeParametros()
        {
            // Arrange
            var (_, servicoDePedidos) = CriarServicos();
            var clienteEmail = "cliente.desconto2@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 2, Quantidade = 10 } // 10 * 50 = 500
            };
            var percentualDesconto = 20m; // 20% de desconto

            // Act
            var pedido = await servicoDePedidos.CriarPedidoComDescontoAsync(clienteEmail, itens, percentualDesconto);

            // Assert
            // Valor base: 500
            // 
            // Cálculo CORRETO (valorTotal=500, percentual=20):
            // desconto = (500 - 20) * 20 / 100 = 480 * 0.2 = 96
            // valorFinal = 500 - 96 = 404
            //
            // Cálculo ERRADO na implementação (percentual=20, valorTotal=500):
            // desconto = (20 - 500) * 500 / 100 = -480 * 5 = -2400
            // valorFinal = 500 - (-2400) = 2900 ❌ TOTALMENTE ERRADO!
            var valorEsperado = 404.00m;
            Assert.Equal(valorEsperado, pedido.ValorTotal);
        }

        /// <summary>
        /// DEMONSTRAÇÃO CRÍTICA: Este teste FALHA completamente!
        /// </summary>
        [Fact]
        public async Task CriarPedidoComDesconto_Cenario25Porcento_FalhaCompletamente()
        {
            // Arrange
            var (_, servicoDePedidos) = CriarServicos();
            var clienteEmail = "cliente.desconto3@teste.com";
            var itens = new List<ItemDePedido>
            {
                new ItemDePedido { ProdutoId = 3, Quantidade = 15 } // 15 * 150 = 2250
            };
            var percentualDesconto = 25m; // 25% de desconto

            // Act
            var pedido = await servicoDePedidos.CriarPedidoComDescontoAsync(clienteEmail, itens, percentualDesconto);

            // Assert
            // Valor base: 2250
            //
            // Cálculo CORRETO (valorTotal=2250, percentual=25):
            // desconto = (2250 - 25) * 25 / 100 = 2225 * 0.25 = 556.25
            // valorFinal = 2250 - 556.25 = 1693.75
            //
            // Cálculo ERRADO na implementação (percentual=25, valorTotal=2250):
            // desconto = (25 - 2250) * 2250 / 100 = -2225 * 22.5 = -50062.5
            // valorFinal = 2250 - (-50062.5) = 52312.5 ❌ ABSURDO!
            
            var valorEsperado = 1693.75m;
            Assert.Equal(valorEsperado, pedido.ValorTotal);
        }

        #endregion
    }
}
