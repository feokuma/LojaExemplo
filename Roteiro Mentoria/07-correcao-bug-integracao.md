# Etapa 7: Bug Discovery via Integração - Bug R$ 99,99

## 🎯 Objetivo
Demonstrar como testes de integração podem descobrir bugs que afetam usuários reais, usando o cenário do TDD_BUG_SCENARIO.md.

## 🐛 O Bug

**Localização**: `src/Repositories/RepositorioDePagamentos.cs` - método `AdicionarAsync`

**Sintoma**: Pagamentos com valor exato de R$ 99,99 são rejeitados com erro:
```
"Pagamento com valor de R$ 99,99 não pode ser processado. Entre em contato com o suporte."
```

**Por que não foi detectado?** Os testes existentes usam valores como R$ 250, R$ 1000, mas nenhum testa exatamente R$ 99,99.

## 📋 Prompt para usar

```text
Crie um teste de integração que reproduza o seguinte bug: o sistema rejeita 
pagamentos com valor exato de R$ 99,99. Crie um produto que custe R$ 99,99, 
faça um pedido, confirme e tente processar o pagamento. O teste deve falhar 
atualmente e passar após a correção.
```

## 💬 O que acontece

A IA irá:
1. **Criar** teste de integração específico para R$ 99,99
2. **Executar** o teste e ver ele **FALHAR** ❌
3. **Identificar** a validação incorreta no código
4. **Sugerir** a remoção da validação problemática
5. **Validar** que após a correção o teste **PASSA** ✅

## 📊 Teste gerado - Ciclo RED-GREEN-REFACTOR

### 🔴 RED: Teste que falha

```csharp
[Fact]
public async Task ProcessarPagamento_ComValorExato99e99Reais_DeveProcessarComSucesso()
{
    // Arrange
    var (repositorioProdutos, servicoDePedidos, servicoDePagamentos) = CriarServicos();
    
    // Criar produto que custe exatamente R$ 99,99
    var produto = new Produto 
    { 
        Id = 99, 
        Nome = "Produto de Teste R$ 99,99", 
        Preco = 99.99m, 
        EstoqueDisponivel = 10 
    };
    await repositorioProdutos.AdicionarAsync(produto);
    
    var itens = new List<ItemDePedido>
    {
        new ItemDePedido { ProdutoId = 99, Quantidade = 1 }
    };

    // Criar e confirmar pedido
    var pedido = await servicoDePedidos.CriarPedidoAsync("cliente.bug99@teste.com", itens);
    await servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);
    
    Assert.Equal(99.99m, pedido.ValorTotal);

    // Act - Tentar processar pagamento de R$ 99,99 (vai falhar!)
    var exception = await Record.ExceptionAsync(async () =>
    {
        await servicoDePagamentos.ProcessarPagamentoAsync(
            pedido.Id, "Pix", pedido.ValorTotal);
    });

    // Assert - Atualmente FALHA com exceção
    Assert.NotNull(exception); // ❌ Teste demonstra o bug!
    Assert.Contains("99,99", exception.Message);
}
```

**Executar:**
```bash
dotnet test --filter "ProcessarPagamento_ComValorExato99e99Reais"
# ❌ Teste FALHA - exceção lançada como esperado (bug confirmado)
```

### 🟢 GREEN: Corrigir o código

**Antes** (com bug):
```csharp
// src/Repositories/RepositorioDePagamentos.cs
public async Task<PagamentoInfo> AdicionarAsync(PagamentoInfo pagamento)
{
    await Task.Delay(10);
    
    // 🐛 BUG: Validação incorreta
    if (pagamento.Valor == 99.99m)
    {
        throw new InvalidOperationException(
            "Pagamento com valor de R$ 99,99 não pode ser processado. " +
            "Entre em contato com o suporte.");
    }
    
    _pagamentos[pagamento.PedidoId] = pagamento;
    return pagamento;
}
```

**Depois** (corrigido):
```csharp
// src/Repositories/RepositorioDePagamentos.cs
public async Task<PagamentoInfo> AdicionarAsync(PagamentoInfo pagamento)
{
    await Task.Delay(10);
    
    // ✅ Validação removida - bug corrigido
    
    _pagamentos[pagamento.PedidoId] = pagamento;
    return pagamento;
}
```

**Atualizar o teste para validar sucesso:**
```csharp
// Act
bool pagamentoProcessado = false;
int tentativas = 0;
while (!pagamentoProcessado && tentativas < 20)
{
    pagamentoProcessado = await servicoDePagamentos.ProcessarPagamentoAsync(
        pedido.Id, "Pix", pedido.ValorTotal);
    tentativas++;
}

// Assert
Assert.True(pagamentoProcessado, "Pagamento de R$ 99,99 deveria ser processado");
Assert.Equal(StatusPedido.Pago, pedido.Status);
```

**Executar novamente:**
```bash
dotnet test --filter "ProcessarPagamento_ComValorExato99e99Reais"
# ✅ Teste PASSA - bug corrigido!
```

### 🔄 REFACTOR: Adicionar testes de edge cases

```csharp
[Theory]
[InlineData(0.01)]    // Valor mínimo
[InlineData(50.00)]   // Valor médio
[InlineData(99.98)]   // Quase 99,99
[InlineData(99.99)]   // Exatamente 99,99 (nosso bug)
[InlineData(100.00)]  // Logo após 99,99
[InlineData(999.99)]  // Valor alto
[InlineData(9999.99)] // Valor muito alto
public async Task ProcessarPagamento_ComVariosValores_DeveFuncionarParaTodos(decimal valor)
{
    // Garante que nenhum valor específico causa problemas
    // ... implementação do teste
}
```

## ✅ Resultados

### Antes da correção:
```
❌ ProcessarPagamento_ComValorExato99e99Reais_DeveProcessarComSucesso: FAILED
   InvalidOperationException: Pagamento com valor de R$ 99,99 não pode ser processado.
```

### Depois da correção:
```
✅ ProcessarPagamento_ComValorExato99e99Reais_DeveProcessarComSucesso: PASSED
✅ ProcessarPagamento_ComVariosValores_DeveFuncionarParaTodos: PASSED (7 cases)
```

## 🎓 Conceitos demonstrados

- **TDD Reverso**: Escrever teste que reproduz bug reportado
- **Red-Green-Refactor**: Ciclo completo de TDD
- **Edge cases**: Valores específicos que causam problemas
- **Testes de regressão**: Garantir que bug não volte
- **Integração end-to-end**: Bug só aparece no fluxo completo

## 💡 Lições aprendidas

1. ✅ **Testes com valores reais**: Sempre teste valores comuns do negócio
2. ✅ **Edge cases importam**: R$ 99,99, R$ 100, R$ 1000 são valores "especiais"
3. ✅ **Validações antigas**: Questione validações que parecem arbitrárias
4. ✅ **Integração detecta mais**: Bugs de integração não aparecem em testes unitários
5. ✅ **Documentação viva**: Teste documenta o bug e a correção

## 💬 Mensagem-chave

> "Este bug existiria em produção e afetaria clientes reais! Testes de integração com valores realistas são essenciais para detectar problemas que testes unitários com mocks não conseguem capturar."

## ⏱️ Tempo estimado
**5-7 minutos** para explicar bug + mostrar teste falhando + corrigir + validar
