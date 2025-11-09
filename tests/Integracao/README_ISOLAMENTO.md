# Estratégias para Isolamento de Testes de Integração

Este documento explica como garantir que cada teste de integração seja executado de forma isolada, sem interferência de dados de outros testes.

## Problema

Quando os repositórios são **Singleton**, eles mantêm dados em memória que persistem entre diferentes testes. Isso pode causar:
- Testes que falham dependendo da ordem de execução
- Resultados inconsistentes
- Dificuldade em diagnosticar problemas

## Soluções Implementadas

### Opção 1: Limpeza Automática com Classe Base (Recomendado) ✅

**Implementado em**: `BaseIntegrationTest.cs`

```csharp
public abstract class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    protected BaseIntegrationTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        
        // Limpa os repositórios ANTES de cada teste
        _factory.LimparRepositorios();
    }
}
```

**Como usar**:
```csharp
public class MeusTests : BaseIntegrationTest
{
    public MeusTests(CustomWebApplicationFactory factory) : base(factory) { }
    
    [Fact]
    public async Task MeuTeste()
    {
        // Os repositórios já estão limpos aqui!
    }
}
```

**Vantagens**:
- ✅ Automático - não precisa lembrar de limpar
- ✅ Consistente - todos os testes começam com dados limpos
- ✅ Fácil de usar - só herdar da classe base

### Opção 2: Limpeza Manual

**Como usar**:
```csharp
public class MeusTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    
    [Fact]
    public async Task MeuTeste()
    {
        // Limpar manualmente antes do teste
        _factory.LimparRepositorios();
        
        // Executar o teste
        // ...
    }
}
```

**Vantagens**:
- ✅ Controle total sobre quando limpar
- ⚠️ Precisa lembrar de chamar em cada teste

### Opção 3: Nova Factory por Teste (Não Recomendado)

Usar `ICollectionFixture` ao invés de `IClassFixture` para criar uma nova factory por teste.

**Desvantagens**:
- ❌ Mais lento (cria nova aplicação web para cada teste)
- ❌ Mais complexo de configurar
- ❌ Desnecessário quando temos limpeza de dados

## Métodos Disponíveis nos Repositórios

### IRepositorioDePedidos
```csharp
void Limpar(); // Remove todos os pedidos e reseta o ID para 1
```

### IRepositorioDeProdutos
```csharp
void Limpar(); // Remove todos os produtos
void ResetarDadosPadrao(); // Reseta para os 3 produtos iniciais (Notebook, Mouse, Teclado)
```

## Método Helper na Factory

```csharp
_factory.LimparRepositorios();
```

Este método:
1. Limpa todos os pedidos
2. Reseta os produtos para o estado inicial
3. Garante que o estoque dos produtos volte aos valores padrão

## Exemplo Completo

```csharp
public class PedidosControllerIntegracaoTests : BaseIntegrationTest
{
    public PedidosControllerIntegracaoTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Teste1_CriaPedido()
    {
        // Repositórios limpos automaticamente
        var response = await _client.PostAsJsonAsync("/api/pedidos", request);
        Assert.Equal(1, pedido.Id); // Sempre será ID 1
    }

    [Fact]
    public async Task Teste2_CriaPedido()
    {
        // Repositórios limpos automaticamente
        var response = await _client.PostAsJsonAsync("/api/pedidos", request);
        Assert.Equal(1, pedido.Id); // Também será ID 1 (isolado do Teste1)
    }
}
```

## Quando NÃO Limpar

Em alguns casos, você pode querer testar o comportamento com múltiplas operações:

```csharp
[Fact]
public async Task FluxoCompleto_MantemEstadoEntreChamadas()
{
    // NÃO chamar LimparRepositorios() aqui
    
    // 1. Criar pedido
    var pedido = await CriarPedido();
    
    // 2. Confirmar pedido (usa o pedido criado acima)
    await ConfirmarPedido(pedido.Id);
    
    // 3. Verificar estoque (foi reduzido pela confirmação)
    var produto = await ObterProduto(1);
    Assert.Equal(8, produto.EstoqueDisponivel); // Reduzido de 10 para 8
}
```

Neste caso, a limpeza automática da `BaseIntegrationTest` já garante que o teste começa limpo, mas durante o teste mantemos o estado entre as operações.

## Recomendação Final

✅ **Use `BaseIntegrationTest`** para todos os seus testes de integração
- Garante isolamento automático
- Código mais limpo
- Menos bugs relacionados a ordem de execução
