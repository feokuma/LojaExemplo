# Etapa 2: Testes Unitários - Trabalhando com Mocks

## 🎯 Objetivo
Demonstrar como a IA configura mocks usando Moq para testar classes com dependências injetadas.

## 📋 Prompt para usar

```text
Crie testes unitários usando Moq para o método ProcessarPagamentoAsync do 
ServicoDePagamentos. Mocke as dependências IServicoDePedidos e 
IRepositorioDePagamentos. Teste todos os cenários de exceção e o fluxo de 
sucesso.
```

## 💬 O que acontece

A IA irá:
1. **Identificar** as dependências do `ServicoDePagamentos`
2. **Criar** mocks para `IServicoDePedidos` e `IRepositorioDePagamentos`
3. **Configurar** os mocks com `.Setup()` e `.Returns()`
4. **Gerar** testes que cobrem:
   - ✅ Validações de entrada (método vazio, valor inválido)
   - ✅ Cenários de erro (pedido não encontrado, não confirmado)
   - ✅ Fluxo de sucesso completo
   - ✅ Verificação de interações com `.Verify()`

## 📊 Exemplo de teste gerado

```csharp
[Fact]
public async Task ProcessarPagamentoAsync_ComPedidoNaoConfirmado_DeveRetornarFalse()
{
    // Arrange
    var pedidoPendente = new Pedido 
    { 
        Id = 1, 
        Status = StatusPedido.Pendente,
        ValorTotal = 100m 
    };
    
    _mockServicoDePedidos
        .Setup(s => s.ObterPedidoPorIdAsync(1))
        .ReturnsAsync(pedidoPendente);

    // Act
    var resultado = await _servicoDePagamentos.ProcessarPagamentoAsync(
        1, "CartaoCredito", 100m);

    // Assert
    Assert.False(resultado);
    
    // Verificar que nenhum pagamento foi adicionado
    _mockRepositorioDePagamentos.Verify(
        r => r.AdicionarAsync(It.IsAny<PagamentoInfo>()), 
        Times.Never);
}
```

## ✅ Resultados esperados

- Total de testes criados: **~10-15 testes**
- Cobertura: **100% do método ProcessarPagamentoAsync**
- Tempo de execução: **< 100ms**
- Status: **✅ Todos passando**

## 🎓 Conceitos demonstrados

- **Mocking de dependências**: Uso do framework Moq
- **Configuração de mocks**: `.Setup()`, `.Returns()`, `.ReturnsAsync()`
- **Verificação de comportamento**: `.Verify()` para garantir interações
- **Isolamento de testes**: Cada teste é independente
- **Testes assíncronos**: Uso de `async/await` nos testes

## 🔍 Variações úteis

### Foco em exceções
```text
No ServicoDePagamentos, crie testes para ProcessarPagamentoAsync validando: 
1) método de pagamento vazio/nulo, 2) valor inválido (zero ou negativo), 
3) pedido não encontrado, 4) pedido não confirmado, 5) valor diferente do 
total, 6) método de pagamento não suportado, 7) processamento bem-sucedido.
```

### Foco em interações
```text
Crie testes que usem Moq.Verify() para garantir que os métodos corretos 
foram chamados nas dependências mockadas, incluindo a ordem das chamadas.
```

## ⏱️ Tempo estimado
**5-7 minutos** para explicar + executar + mostrar como mocks funcionam
