# Etapa 5: Testes de Integração - Fluxo de Pedidos

## 🎯 Objetivo
Testar o fluxo end-to-end de criação e gerenciamento de pedidos usando componentes reais (sem mocks).

## 📋 Prompt para usar

```text
Crie testes de integração para o ServicoDePedidos em tests/Integracao/Services/. 
Use repositórios reais (não mocke nada). Teste o fluxo completo:
1. Criar pedido com produtos reais
2. Verificar redução de estoque
3. Confirmar pedido
4. Cancelar pedido
5. Verificar reposição de estoque
```

## 💬 O que acontece

A IA irá:
1. **Criar** testes em `tests/Integracao/Services/ServicoDePedidosIntegracao.Tests.cs`
2. **Instanciar** todos os componentes reais (sem mocks):
   - ✅ `RepositorioDeProdutos` real
   - ✅ `RepositorioDePedidos` real
   - ✅ `ServicoDeDesconto` real
   - ✅ `ServicoDePedidos` real
3. **Testar** o fluxo completo com verificações de estado
4. **Validar** efeitos colaterais (estoque reduzido/reposto)

## 📊 Exemplo de teste gerado

```csharp
[Fact]
public async Task FluxoCompleto_CriarConfirmarECancelar_DeveGerenciarEstoqueCorretamente()
{
    // Arrange
    var (repositorioProdutos, servicoDePedidos) = CriarServicos();
    
    // Criar produto com estoque
    var produto = new Produto 
    { 
        Id = 1, 
        Nome = "Notebook", 
        Preco = 2500m, 
        EstoqueDisponivel = 10 
    };
    await repositorioProdutos.AdicionarAsync(produto);
    
    var itens = new List<ItemDePedido>
    {
        new ItemDePedido { ProdutoId = 1, Quantidade = 2 }
    };

    // Act 1: Criar pedido
    var pedido = await servicoDePedidos.CriarPedidoAsync(
        "cliente@teste.com", itens);
    
    // Assert 1: Pedido criado, estoque ainda não reduzido
    Assert.Equal(StatusPedido.Pendente, pedido.Status);
    var produtoAposCrear = await repositorioProdutos.ObterPorIdAsync(1);
    Assert.Equal(10, produtoAposCrear.EstoqueDisponivel); // ✅ Ainda 10

    // Act 2: Confirmar pedido
    var confirmado = await servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);
    
    // Assert 2: Estoque reduzido
    Assert.True(confirmado);
    var produtoAposConfirmar = await repositorioProdutos.ObterPorIdAsync(1);
    Assert.Equal(8, produtoAposConfirmar.EstoqueDisponivel); // ✅ 10 - 2 = 8

    // Act 3: Cancelar pedido
    var cancelado = await servicoDePedidos.CancelarPedidoAsync(pedido.Id);
    
    // Assert 3: Estoque reposto
    Assert.True(cancelado);
    var produtoAposCancelar = await repositorioProdutos.ObterPorIdAsync(1);
    Assert.Equal(10, produtoAposCancelar.EstoqueDisponivel); // ✅ 8 + 2 = 10
}
```

## ✅ Resultados esperados

- Total de testes criados: **~8-12 testes de integração**
- Cobertura: **Fluxo completo de pedidos**
- Tempo de execução: **< 500ms** (mais lentos que unitários, mas ainda rápidos)
- Status: **✅ Todos passando**

## 🔍 Diferenças entre Unitário e Integração

| Aspecto | Teste Unitário | Teste Integração |
|---------|---------------|------------------|
| **Dependências** | Mockadas com Moq | Reais (em memória) |
| **Escopo** | Método isolado | Fluxo completo |
| **Velocidade** | Muito rápido (< 50ms) | Rápido (< 500ms) |
| **Foco** | Lógica isolada | Interação entre componentes |
| **Efeitos colaterais** | Não testados | Validados (estoque, status) |
| **Falsos positivos** | Possível (mocks incorretos) | Improvável (usa código real) |

## 🎓 Conceitos demonstrados

- **Testes de integração**: Componentes reais trabalhando juntos
- **Validação de estado**: Verificar efeitos colaterais no sistema
- **Fluxo end-to-end**: Do início ao fim de uma funcionalidade
- **Isolamento de testes**: Cada teste cria seus próprios dados
- **Testes mais confiáveis**: Sem mocks = mais próximo da produção

## 🔄 Cenários testados

1. ✅ Criar pedido com múltiplos produtos
2. ✅ Validar estoque insuficiente
3. ✅ Produto não encontrado
4. ✅ Confirmar pedido → reduz estoque
5. ✅ Cancelar pedido pendente → não repõe estoque
6. ✅ Cancelar pedido confirmado → repõe estoque
7. ✅ Múltiplos pedidos para mesmo cliente

## 💡 Mensagem-chave

> "Testes de integração validam que os componentes funcionam juntos corretamente. Eles complementam os testes unitários, fornecendo confiança no sistema como um todo!"

## ⏱️ Tempo estimado
**5-7 minutos** para explicar diferença unitário vs integração + executar testes
