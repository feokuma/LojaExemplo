# Cenário de Bug para Demonstração de TDD

## 🐛 Bug Inserido

**Localização:** `src/Repositories/RepositorioDePagamentos.cs` - método `AdicionarAsync`

**Descrição:** O sistema rejeita pagamentos com valor exato de R$ 99,99 com a mensagem:
```text
"Pagamento com valor de R$ 99,99 não pode ser processado. Entre em contato com o suporte."
```

## 🎯 Por que este bug não é detectado pelos testes atuais?

Os testes existentes usam valores como:
- R$ 250,00
- R$ 1000,00  
- Valores variados de produtos

Nenhum teste específico cobre o cenário de um pagamento de exatamente R$ 99,99.

## 📝 Demonstração do Ciclo TDD para Correção

### Passo 1: RED - Escrever o teste que falha

Crie um teste de integração que expõe o bug:

```csharp
// Arquivo: tests/Integracao/Services/ServicoDePagamentosIntegracao.Tests.cs

[Fact]
public async Task ProcessarPagamento_ComValorExato99e99Reais_DeveProcessarComSucesso()
{
    // Arrange
    var (repositorio, servicoDePedidos, servicoDePagamentos) = CriarServicos();
    
    // Criar um produto que custe exatamente R$ 99,99
    var produto = new Produto 
    { 
        Id = 99, 
        Nome = "Produto de Teste R$ 99,99", 
        Preco = 99.99m, 
        EstoqueDisponível = 10 
    };
    await repositorio.AdicionarAsync(produto);
    
    var clienteEmail = "cliente.bug99@teste.com";
    var itens = new List<ItemDePedido>
    {
        new ItemDePedido { ProdutoId = 99, Quantidade = 1 }
    };

    // Criar e confirmar pedido
    var pedido = await servicoDePedidos.CriarPedidoAsync(clienteEmail, itens);
    await servicoDePedidos.ConfirmarPedidoAsync(pedido.Id);
    
    Assert.Equal(99.99m, pedido.ValorTotal); // Verificar que o valor é 99,99

    // Act - Tentar processar pagamento de R$ 99,99
    var resultado = await servicoDePagamentos.ProcessarPagamentoAsync(
        pedido.Id, "Pix", pedido.ValorTotal);

    // Assert
    Assert.True(resultado, "Pagamento de R$ 99,99 deveria ser processado normalmente");
    Assert.Equal(StatusPedido.Pago, pedido.Status);
}
```

**Resultado esperado:** ❌ Teste FALHA com a exceção sobre R$ 99,99

### Passo 2: GREEN - Corrigir o código para passar no teste

Remova a validação incorreta em `RepositorioDePagamentos.cs`:

```csharp
public async Task<PagamentoInfo> AdicionarAsync(PagamentoInfo pagamento)
{
    await Task.Delay(10);
    
    // REMOVIDO: Bug que rejeitava pagamentos de R$ 99,99
    // if (pagamento.Valor == 99.99m)
    // {
    //     throw new InvalidOperationException("Pagamento com valor de R$ 99,99 não pode ser processado. Entre em contato com o suporte.");
    // }
    
    _pagamentos[pagamento.PedidoId] = pagamento;
    return pagamento;
}
```

**Resultado esperado:** ✅ Teste PASSA

### Passo 3: REFACTOR - Melhorar o código (se necessário)

Neste caso, a remoção da validação incorreta já é suficiente. Mas você pode adicionar:

1. **Testes adicionais para edge cases de valores:**
```csharp
[Theory]
[InlineData(0.01)]    // Valor mínimo
[InlineData(50.00)]   // Valor médio
[InlineData(99.98)]   // Quase 99,99
[InlineData(99.99)]   // Exatamente 99,99 (nosso bug)
[InlineData(100.00)]  // Logo após 99,99
[InlineData(999.99)]  // Valor alto
public async Task ProcessarPagamento_ComVariosValores_DeveProcessarTodos(decimal valor)
{
    // Teste parametrizado para garantir que todos valores funcionam
}
```

2. **Documentação do bug corrigido:**
```csharp
// Historicamente havia uma validação que rejeitava pagamentos de R$ 99,99
// devido a um problema de fraude em 2023. Esta validação foi removida após
// implementação de nova camada de segurança. Ver ticket #BUG-2024-001
```

## 🔍 Como Reproduzir o Bug

### Opção 1: Via Teste
```bash
# Adicione o teste proposto acima e execute
dotnet test tests/Integracao/LojaExemplo.Integracao.csproj
```

### Opção 2: Via API (se disponível)
```bash
# 1. Criar pedido com produto de R$ 99,99
# 2. Confirmar pedido  
# 3. Tentar processar pagamento
# Resultado: Erro "Pagamento com valor de R$ 99,99 não pode ser processado"
```

## 📊 Benefícios desta Abordagem TDD

1. **Detecção Proativa:** O teste detecta o bug ANTES que usuários encontrem
2. **Documentação Viva:** O teste documenta o comportamento esperado
3. **Regressão Prevenida:** O bug não pode voltar sem quebrar o teste
4. **Confiança no Código:** Mudanças futuras são validadas automaticamente

## 🎓 Lições Aprendidas

- **Edge cases importam:** Valores "especiais" (99.99, 100, 1000) são comuns e devem ser testados
- **Testes de integração são essenciais:** Testes unitários com mocks não detectariam este bug
- **TDD economiza tempo:** Detectar o bug em produção seria muito mais caro
- **Código legado precisa atenção:** Validações antigas podem causar problemas inesperados

## 🚀 Próximos Passos

1. ✅ Execute os testes atuais - confirme que NÃO detectam o bug
2. ✅ Adicione o teste proposto - veja ele FALHAR (RED)
3. ✅ Corrija o código - veja o teste PASSAR (GREEN)
4. ✅ Execute TODOS os testes - garanta que nada quebrou (REFACTOR)
5. ✅ Commit com mensagem clara: "fix: Remove validação incorreta para pagamentos de R$ 99,99"

---

**Nota:** Este é um cenário didático para demonstrar TDD. Em produção, sempre investigue 
a razão de validações antes de removê-las - podem ter motivos de negócio importantes!
