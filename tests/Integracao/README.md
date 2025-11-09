# Testes de Integração com WebApplicationFactory

Este projeto utiliza o `WebApplicationFactory` do ASP.NET Core para realizar testes de integração, permitindo testar a aplicação em um ambiente que simula uma execução real.

## Configuração Implementada

### 1. Pacote NuGet Adicionado
- **Microsoft.AspNetCore.Mvc.Testing** (versão 8.0.0)

### 2. Modificações no Projeto Principal (`Program.cs`)
Foi adicionada a declaração `public partial class Program { }` ao final do arquivo `Program.cs` para permitir que o `WebApplicationFactory` acesse a classe de inicialização da aplicação.

```csharp
// Make the implicit Program class accessible for testing
public partial class Program { }
```

### 3. CustomWebApplicationFactory
Criada a classe `CustomWebApplicationFactory` que herda de `WebApplicationFactory<Program>`. Esta classe:

- **Configura o ambiente de teste**: Define o ambiente como "Test"
- **Permite substituição de serviços**: Através do método `ConfigureWebHost`, você pode:
  - Substituir banco de dados real por banco in-memory
  - Mockar serviços externos
  - Configurar serviços específicos para testes
- **Fornece métodos auxiliares**:
  - `CreateScope()`: Cria um escopo de serviços
  - `GetService<T>()`: Obtém um serviço do container de DI

### 4. Integração com Testes
As classes de teste foram modificadas para:

```csharp
public class ServicoDePedidosIntegracaoTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly IServiceScope _scope;

    public ServicoDePedidosIntegracaoTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _scope = _factory.CreateScope();
    }

    private (IRepositorioDeProdutos, IServicoDePedidos) CriarServicos()
    {
        var repositorio = _scope.ServiceProvider.GetRequiredService<IRepositorioDeProdutos>();
        var servicoPedidos = _scope.ServiceProvider.GetRequiredService<IServicoDePedidos>();
        return (repositorio, servicoPedidos);
    }
}
```

## Vantagens desta Abordagem

### 1. **Injeção de Dependência Real**
- Os serviços são obtidos do container de DI da aplicação
- Garante que a configuração de DI está correta
- Testa o comportamento real da aplicação

### 2. **Isolamento de Testes**
- Cada classe de teste recebe sua própria instância da factory via `IClassFixture`
- Permite configurações específicas por classe de teste

### 3. **Facilidade de Substituição**
- Fácil substituir implementações reais por mocks ou stubs
- Ideal para mockar serviços externos (APIs, banco de dados, etc.)

### 4. **Testes de API**
- A factory também cria um `HttpClient` que pode ser usado para fazer chamadas HTTP reais aos controllers
- Permite testar endpoints completos (não implementado ainda, mas disponível)

## Exemplo de Uso Avançado

### Substituindo Serviços para Testes

Você pode modificar a `CustomWebApplicationFactory` para substituir serviços:

```csharp
protected override void ConfigureWebHost(IWebHostBuilder builder)
{
    builder.ConfigureServices(services =>
    {
        // Remover serviço de produção
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(IRepositorioDeProdutos));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
        
        // Adicionar mock ou implementação in-memory
        services.AddScoped<IRepositorioDeProdutos, RepositorioDeProdutosInMemory>();
    });

    builder.UseEnvironment("Test");
}
```

### Testando Endpoints HTTP

```csharp
[Fact]
public async Task Get_Pedidos_ReturnsSuccessStatusCode()
{
    // Arrange
    var client = _factory.CreateClient();

    // Act
    var response = await client.GetAsync("/api/pedidos");

    // Assert
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStringAsync();
    Assert.NotNull(content);
}
```

## Executando os Testes

```bash
# Executar todos os testes de integração
dotnet test tests/Integracao/LojaExemplo.Integracao.csproj

# Executar com verbosidade detalhada
dotnet test tests/Integracao/LojaExemplo.Integracao.csproj -v detailed

# Executar um teste específico
dotnet test tests/Integracao/LojaExemplo.Integracao.csproj --filter "FullyQualifiedName~CriarPedido_ComDadosValidos"
```

## Próximos Passos

1. **Implementar banco de dados in-memory** para evitar estado compartilhado entre testes
2. **Adicionar testes de endpoints HTTP** usando `HttpClient`
3. **Configurar ambiente de CI/CD** para executar os testes automaticamente
4. **Implementar fixture de dados** para preparar estados iniciais consistentes
5. **Adicionar testes de autenticação e autorização** se aplicável

## Recursos Adicionais

- [Testing ASP.NET Core Services](https://docs.microsoft.com/aspnet/core/test/integration-tests)
- [WebApplicationFactory](https://docs.microsoft.com/dotnet/api/microsoft.aspnetcore.mvc.testing.webapplicationfactory-1)
- [xUnit Test Patterns](http://xunitpatterns.com/)
