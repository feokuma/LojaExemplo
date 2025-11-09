using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LojaExemplo.Repositorios;

namespace LojaExemplo.Testes.Integracao
{
    /// <summary>
    /// Factory customizada para criar uma aplicação web de teste.
    /// Esta classe permite configurar o ambiente de teste de forma isolada,
    /// substituindo dependências e configurações conforme necessário.
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remover o serviço de produção
                // var descriptor = services.SingleOrDefault(
                //     d => d.ServiceType == typeof(IRepositorioDeProdutos));
                // if (descriptor != null)
                // {
                //     services.Remove(descriptor);
                // }
                
                // Adicionar versão de teste ou mock
                // services.AddScoped<IRepositorioDeProdutos, RepositorioDeProdutosMock>();
            });

            builder.UseEnvironment("Test");
        }

        /// <summary>
        /// Cria um escopo de serviços para obter dependências diretamente do container DI.
        /// Útil quando você precisa testar serviços sem fazer chamadas HTTP.
        /// </summary>
        public IServiceScope CreateScope()
        {
            return Services.CreateScope();
        }

        /// <summary>
        /// Obtém um serviço do container de DI.
        /// </summary>
        public T GetService<T>() where T : notnull
        {
            var scope = CreateScope();
            return scope.ServiceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// Limpa todos os dados dos repositórios para garantir isolamento entre testes.
        /// </summary>
        public void LimparRepositorios()
        {
            using var scope = Services.CreateScope();
            var repositorioPedidos = scope.ServiceProvider.GetRequiredService<IRepositorioDePedidos>();
            var repositorioProdutos = scope.ServiceProvider.GetRequiredService<IRepositorioDeProdutos>();
            
            repositorioPedidos.Limpar();
            repositorioProdutos.ResetarDadosPadrao(); // Reseta para os dados iniciais
        }
    }
}
