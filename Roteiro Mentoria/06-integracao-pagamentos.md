# Etapa 6: Testes de Integração - Fluxo de Pagamentos

## 🎯 Objetivo
Testar a integração completa entre pedidos e pagamentos, validando o fluxo desde criação até estorno.

## 📋 Prompt para usar

```text
Crie testes de integração para o ServicoDePagamentos testando o fluxo completo:
1. Criar pedido
2. Confirmar pedido
3. Processar pagamento com diferentes métodos (CartaoCredito, Pix, Boleto)
4. Verificar atualização do status do pedido
5. Estornar pagamento
6. Verificar cancelamento do pedido
Use todos os componentes reais sem mocks.
```

## 💬 O que acontece

A IA irá:
1. **Criar** testes em `tests/Integracao/Services/ServicoDePagamentosIntegracao.Tests.cs`
2. **Integrar** múltiplos serviços reais:
   - ✅ `ServicoDePedidos`
   - ✅ `ServicoDePagamentos`
   - ✅ Todos os repositórios
3. **Testar** múltiplos métodos de pagamento
4. **Validar** transições de status em ambos sistemas (pedido + pagamento)
5. **Lidar** com a falha aleatória de 10% (retry logic)

## 📊 Exemplo de teste gerado

```csharp
[Fact]
public async Task FluxoCompleto_CriarPagarEEstornar_DeveAtualizarStatusCorretamente()
{
    // Arrange
    var (repositorioProdutos, servicoDePedidos, servicoDePagamentos) = CriarServicos();
    
    // Criar produto
    var produto = new Produto 
    { 
        Id = 1, 
        Nome = "Mouse", 
        Preco = 50m, 
        EstoqueDisponivel = 20 
    };
    await repositorioProdutos.AdicionarAsync(produto);
    
    // Criar e confirmar pedido
    var itens = new List<ItemDePedido> 
    { 
        new ItemDePedido { ProdutoId = 1, Quantidade = 1 } 
    };
    var pedido = await servicoDePedidos.CriarPedidoAsync("cliente@teste.com", itens);
    await servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);
    
    // Act 1: Processar pagamento (com retry devido à falha de 10%)
    bool pagamentoProcessado = false;
    int tentativas = 0;
    while (!pagamentoProcessado && tentativas < 20)
    {
        pagamentoProcessado = await servicoDePagamentos.ProcessarPagamentoAsync(
            pedido.Id, "CartaoCredito", pedido.ValorTotal);
        tentativas++;
    }
    
    // Assert 1: Pagamento processado e pedido atualizado
    Assert.True(pagamentoProcessado);
    Assert.Equal(StatusPedido.Pago, pedido.Status);
    
    var statusPagamento = await servicoDePagamentos.VerificarStatusPagamentoAsync(pedido.Id);
    Assert.True(statusPagamento);

    // Act 2: Estornar pagamento
    var estornado = await servicoDePagamentos.EstornarPagamentoAsync(pedido.Id);
    
    // Assert 2: Pagamento estornado e pedido cancelado
    Assert.True(estornado);
    Assert.Equal(StatusPedido.Cancelado, pedido.Status);
    
    var statusAposEstorno = await servicoDePagamentos.VerificarStatusPagamentoAsync(pedido.Id);
    Assert.False(statusAposEstorno); // Não está mais aprovado
}
```

## 🎲 Lidando com Comportamento Não-Determinístico

O `ServicoDePagamentos` tem uma falha aleatória de 10%:

```csharp
// No código de produção
if (random.Next(100) < 10) // 10% de chance de falha
{
    return false;
}
```

**Solução nos testes**: Retry logic

```csharp
bool pagamentoProcessado = false;
int tentativas = 0;
while (!pagamentoProcessado && tentativas < 20)
{
    pagamentoProcessado = await servicoDePagamentos.ProcessarPagamentoAsync(...);
    tentativas++;
}
Assert.True(pagamentoProcessado, "Deveria processar após múltiplas tentativas");
```

## ✅ Resultados esperados

- Total de testes criados: **~15-20 testes de integração**
- Cobertura: **Fluxo completo de pagamentos**
- Métodos testados: **CartaoCredito, Pix, Boleto, etc.**
- Tempo de execução: **< 1s** (devido aos retries)
- Status: **✅ Todos passando de forma confiável**

## 🔄 Cenários testados

1. ✅ Pagamento com CartaoCredito
2. ✅ Pagamento com Pix
3. ✅ Pagamento com Boleto
4. ✅ Validação: pedido não encontrado
5. ✅ Validação: pedido não confirmado
6. ✅ Validação: valor incorreto
7. ✅ Validação: método de pagamento inválido
8. ✅ Estorno de pagamento aprovado
9. ✅ Tentativa de estornar pagamento já estornado
10. ✅ Múltiplos pagamentos para cliente

## 🎓 Conceitos demonstrados

- **Integração multi-camadas**: Pedidos + Pagamentos + Repositórios
- **Retry logic**: Lidar com comportamentos não-determinísticos
- **Validações complexas**: Regras de negócio entre sistemas
- **Estado consistente**: Pagamento e pedido sincronizados
- **Testes robustos**: Funcionam mesmo com aleatoriedade

## 📊 Comparação de métodos de pagamento

| Método | Tempo Processamento | Taxa Sucesso | Suporta Estorno |
|--------|-------------------|--------------|-----------------|
| CartaoCredito | 50ms | 90% (com retries: ~100%) | ✅ Sim |
| Pix | 50ms | 90% (com retries: ~100%) | ✅ Sim |
| Boleto | 50ms | 90% (com retries: ~100%) | ✅ Sim |

## 💡 Mensagem-chave

> "Testes de integração revelam problemas de comunicação entre componentes que testes unitários não podem detectar. O retry logic demonstra como lidar com comportamentos não-determinísticos de forma confiável!"

## ⏱️ Tempo estimado
**6-8 minutos** para explicar integração + retry logic + executar testes
