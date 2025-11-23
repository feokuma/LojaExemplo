# Etapa 4: Refatoração Guiada por Testes

## 🎯 Objetivo
Demonstrar como usar testes existentes como rede de segurança para refatorar código com confiança.

## 📋 Contexto

Vamos refatorar o método `EstaAprovado()` na classe `PagamentoInfo`, seguindo o exemplo documentado no README.md (seção "Exemplo de Refatoração Guiada por Testes").

## 🔄 Passo a Passo

### Passo 1: Verificar testes existentes

Antes de qualquer refatoração, confirme que os testes atuais passam:

```bash
dotnet test tests/Unidade/LojaExemplo.Unidade.csproj
# ✅ Resultado esperado: Todos os testes passando
```

### Passo 2: Identificar duplicação de código

**Problema**: A validação de pagamento aprovado está duplicada em vários lugares:

```csharp
// Em ServicoDePagamentos.cs - linha 54
return pagamento != null && pagamento.Status == StatusPagamento.Aprovado;

// Em ServicoDePagamentos.cs - linha 69
if (pagamento.Status != StatusPagamento.Aprovado)
    return false;
```

### Passo 3: Criar o método na classe de domínio

```csharp
// src/Modelos/PagamentoInfo.cs
public bool EstaAprovado()
{
    return Status == StatusPagamento.Aprovado && Valor > 0;
}
```

### Passo 4: Refatorar usando o novo método

```csharp
// ANTES
public async Task<bool> VerificarStatusPagamentoAsync(int pedidoId)
{
    var pagamento = await _repositorioDePagamentos.ObterPorPedidoIdAsync(pedidoId);
    return pagamento != null && pagamento.Status == StatusPagamento.Aprovado;
}

// DEPOIS
public async Task<bool> VerificarStatusPagamentoAsync(int pedidoId)
{
    var pagamento = await _repositorioDePagamentos.ObterPorPedidoIdAsync(pedidoId);
    return pagamento?.EstaAprovado() ?? false;  // ✨ Mais limpo e expressivo
}
```

### Passo 5: Verificar que testes continuam passando

```bash
dotnet test tests/Unidade/LojaExemplo.Unidade.csproj
# ✅ Resultado esperado: TODOS os testes ainda passam
```

## 📋 Prompt opcional para criar testes do novo método

```text
Crie testes unitários para o método EstaAprovado() da classe PagamentoInfo.
Teste: 1) status aprovado com valor positivo, 2) status aprovado com valor 
zero ou negativo, 3) outros status (Pendente, Rejeitado, Estornado) com valor 
positivo. Use [Theory] e [InlineData] quando apropriado.
```

## 📊 Benefícios da refatoração

| Aspecto | Antes | Depois |
|---------|-------|--------|
| **Duplicação** | Lógica repetida em 2+ lugares | Centralizada em 1 método |
| **Validação** | ❌ Não validava valor > 0 | ✅ Valida automaticamente |
| **Legibilidade** | `status == StatusPagamento.Aprovado` | `EstaAprovado()` (mais claro) |
| **Manutenção** | Mudar em múltiplos lugares | Mudar em 1 lugar |
| **Testabilidade** | Testado indiretamente | Testável diretamente |

## ✅ Resultados esperados

- **Zero testes quebrados**: Todos continuam passando ✅
- **Código mais limpo**: Método expressivo e autoexplicativo
- **Melhor encapsulamento**: Lógica de validação dentro da entidade de domínio
- **Cobertura mantida**: 100% de cobertura preservada

## 🎓 Conceitos demonstrados

- **Refatoração segura**: Mudanças com testes como rede de segurança
- **Encapsulamento**: Mover lógica para a classe apropriada
- **Expressividade**: Código mais legível e autoexplicativo
- **Design incremental**: Melhorar código sem quebrar funcionalidades
- **Confiança**: Testes garantem que nada quebrou

## 💡 Mensagem-chave

> "Testes automatizados não são apenas para encontrar bugs - eles permitem REFATORAR com confiança, melhorando continuamente o design do código!"

## ⏱️ Tempo estimado
**4-6 minutos** para explicar contexto + mostrar refatoração + executar testes
