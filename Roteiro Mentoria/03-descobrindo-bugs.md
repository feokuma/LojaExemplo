# Etapa 3: Bug Discovery - IA Encontrando Problemas

## 🎯 Objetivo
Demonstrar como a IA pode identificar bugs através da criação de testes com dados concretos.

## 💡 Regra correta reportada pelo time de negócios

O time de negócios definiu que o cálculo do desconto progressivo deve seguir a seguinte lógica:

Os cenários abaixo deve ser configurados no teste

```csharp
        [InlineData(100.00, 10, 10.00)]      // 100 * 10/100 = 10
        [InlineData(200.00, 20, 40.00)]      // 200 * 20/100 = 40
        [InlineData(500.00, 5, 25.00)]       // 500 * 5/100 = 25
        public async Task CalcularDescontoProgressivoAsync_ComVariosValores_DeveCalcularCorretamente
```

Ou seja, o valor do desconto deve ser sempre o resultado de `valorTotal * percentualDesconto / 100`. Esses exemplos deixam claro como o cálculo deve funcionar para diferentes valores e percentuais, garantindo que a regra de negócio seja aplicada corretamente em todos os casos.

## 📋 Prompt para usar

```text
Ajuste a regra somente nos testes para o método CalcularDescontoProgressivoAsync para:
- valorTotal: 100, percentual: 10 (esperado: desconto de R$ 10)
- valorTotal: 200, percentual: 20 (esperado: desconto de R$ 40)
- valorTotal: 500, percentual: 5 (esperado: desconto de R$ 25)
Identifique se há problemas na implementação atual.
```

## 💬 O que acontece

A IA irá:
1. **Analisar** o método `CalcularDescontoProgressivoAsync` em `src/Services/ServicoDeDesconto.cs`
2. **Identificar** a fórmula incorreta: `(valorTotal - percentualDesconto) * percentualDesconto / 100`
3. **Criar** testes com valores esperados corretos
4. **Executar** os testes e ver falhas ❌
5. **Reportar** o bug encontrado com explicação clara
6. **Sugerir** a correção: `valorTotal * percentualDesconto / 100`

## 📊 Exemplo de teste gerado

```csharp
[Theory]
[InlineData(100, 10, 10)]    // R$ 100 com 10% = R$ 10 de desconto
[InlineData(200, 20, 40)]    // R$ 200 com 20% = R$ 40 de desconto
[InlineData(500, 5, 25)]     // R$ 500 com 5% = R$ 25 de desconto
[InlineData(1000, 15, 150)]  // R$ 1000 com 15% = R$ 150 de desconto
public async Task CalcularDescontoProgressivoAsync_ComValoresConcretos_DeveCalcularCorretamente(
    decimal valorTotal, 
    decimal percentual, 
    decimal descontoEsperado)
{
    // Arrange
    _mockRepositorioDePedidos
        .Setup(r => r.ObterPedidosPorClienteAsync(It.IsAny<string>()))
        .ReturnsAsync(new List<Pedido>());

    // Act
    var desconto = await _servicoDeDesconto.CalcularDescontoProgressivoAsync(
        "cliente@teste.com", valorTotal, percentual);

    // Assert
    Assert.Equal(descontoEsperado, desconto);
}
```

## 🐛 Bug identificado

### Código atual (INCORRETO):
```csharp
public async Task<decimal> CalcularDescontoProgressivoAsync(
    string clienteEmail, decimal valorTotal, decimal percentualDesconto)
{
    // 🐛 BUG: Os parâmetros estão invertidos!
    var desconto = (valorTotal - percentualDesconto) * percentualDesconto / 100;
    return desconto;
}
```

### Problema:
- Com `valorTotal = 100` e `percentualDesconto = 10`
- Cálculo atual: `(100 - 10) * 10 / 100 = 9` ❌
- Cálculo esperado: `100 * 10 / 100 = 10` ✅

## ✅ Resultados esperados

- Testes criados: **4-6 testes parametrizados**
- Status: **❌ FALHAM** (evidenciam o bug)
- A IA identifica: **"A fórmula está incorreta"**
- Sugere correção: `valorTotal * percentualDesconto / 100`

## 🔧 Demonstração da correção

### 1. Ver os testes falhando
```bash
dotnet test tests/Unidade/Services/ServicoDeDesconto.Tests.cs
# ❌ Failed: 4 testes falharam
```

### 2. Aplicar a correção sugerida pela IA
```csharp
public async Task<decimal> CalcularDescontoProgressivoAsync(
    string clienteEmail, decimal valorTotal, decimal percentualDesconto)
{
    // ✅ CORRIGIDO
    var desconto = valorTotal * percentualDesconto / 100;
    return desconto;
}
```

### 3. Executar novamente
```bash
dotnet test tests/Unidade/Services/ServicoDeDesconto.Tests.cs
# ✅ Passed: 4 testes passaram!
```

## 🎓 Conceitos demonstrados

- **TDD reverso**: Criar testes que expõem bugs existentes
- **Testes parametrizados**: Uso de `[Theory]` e `[InlineData]`
- **Valores concretos**: Testes com números reais facilitam validação
- **Red-Green**: Ciclo de ver teste falhar → corrigir → ver passar
- **IA como revisor de código**: Identifica problemas lógicos

## 💡 Mensagem-chave

> "A IA não apenas gera testes - ela pode ENCONTRAR bugs analisando a lógica do código e comparando com o comportamento esperado!"

## ⏱️ Tempo estimado
**5-7 minutos** para explicar bug + mostrar testes falhando + corrigir + ver passar
