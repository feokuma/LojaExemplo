using Xunit;

namespace LojaExemplo.Testes.Integracao
{
    /// <summary>
    /// Classe base para testes de integração que garante isolamento entre testes.
    /// Implementa IAsyncLifetime para limpar os repositórios antes de CADA teste.
    /// O xUnit chama InitializeAsync antes de cada teste e DisposeAsync depois.
    /// </summary>
    public abstract class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        protected readonly HttpClient _client;
        protected readonly CustomWebApplicationFactory _factory;

        protected BaseIntegrationTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        /// <summary>
        /// Chamado pelo xUnit ANTES de cada teste individual.
        /// Garante que os repositórios estão limpos.
        /// </summary>
        public Task InitializeAsync()
        {
            _factory.LimparRepositorios();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Chamado pelo xUnit DEPOIS de cada teste individual.
        /// </summary>
        public Task DisposeAsync()
        {
            // Opcionalmente pode limpar depois também
            return Task.CompletedTask;
        }
    }
}
