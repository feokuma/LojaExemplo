# Etapa 1: Testes Unitários - Validação Básica

## 🎯 Objetivo
Demonstrar testes simples de lógica de negócio sem dependências externas, focando na classe `Pedido`.

## 📋 Prompt para usar

```text
Crie testes unitários completos para a classe Pedido em src/Modelos/Pedido.cs seguindo a mesma estrutura de diretórios do src no tests utilizando xunit. 
Inclua testes para os métodos Confirmar(), Cancelar(), PodeCancelar() e 
DeveReporEstoque(), cobrindo todos os cenários possíveis de transição de status.
```

## 💬 O que acontece

A IA irá:
1. **Analisar** a classe `Pedido` em `src/Modelos/Pedido.cs`
2. **Identificar** todos os métodos públicos que precisam ser testados
3. **Criar** arquivo de testes em `tests/Unidade/Modelos/Pedido.Tests.cs`
4. **Gerar** testes xUnit para cada método, cobrindo:
   - ✅ Cenários de sucesso
   - ✅ Validações de regras de negócio
   - ✅ Transições de status válidas e inválidas
   - ✅ Edge cases

## 📊 Exemplo de teste gerado

```csharp
[Fact]
public void Confirmar_PedidoPendente_DeveAlterarStatusParaConfirmado()
{
    // Arrange
    var pedido = new Pedido 
    { 
        Status = StatusPedido.Pendente 
    };

    // Act
    pedido.Confirmar();

    // Assert
    Assert.Equal(StatusPedido.Confirmado, pedido.Status);
}

[Fact]
public void Confirmar_PedidoJaConfirmado_DeveLancarExcecao()
{
    // Arrange
    var pedido = new Pedido 
    { 
        Status = StatusPedido.Confirmado 
    };

    // Act & Assert
    Assert.Throws<InvalidOperationException>(() => pedido.Confirmar());
}
```

## ✅ Resultados esperados

- Total de testes criados: **~8-12 testes**
- Cobertura: **100% da classe Pedido**
- Tempo de execução: **< 50ms** (muito rápidos)
- Status: **✅ Todos passando**

## 🎓 Conceitos demonstrados

- **Testes unitários puros**: Sem dependências externas
- **Padrão AAA**: Arrange-Act-Assert bem definido
- **Nomenclatura clara**: `Metodo_Cenario_ResultadoEsperado`
- **Teste de exceções**: Uso de `Assert.Throws<T>()`
- **Teste de estado**: Verificação de mudanças no objeto

## ⏱️ Tempo estimado
**3-5 minutos** para explicar + executar + mostrar resultados
