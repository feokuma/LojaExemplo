# Correção de bug: Estorno não alterava o status do pagamento

## Contexto
- Serviço impactado: `src/Services/ServicoDePagamentos.cs`
- Sintoma: estorno concluía com sucesso, mas o repositório recebia o objeto de pagamento ainda com `StatusPagamento.Aprovado`.
- Causa raiz: antes de chamar `_repositorioDePagamentos.AtualizarAsync`, o serviço não atualizava o status nem a data de estorno.

## Passo a passo da correção
1. **Reproduzir o problema**
   - Criar um teste cobrindo o cenário de estorno para garantir que o status enviado ao repositório seja `Estornado`.
2. **Implementar o ajuste**
   - Em `EstornarPagamentoAsync`, definir `pagamento.Status = StatusPagamento.Estornado;` e `pagamento.DataEstorno = DateTime.Now;` antes de salvar.
3. **Validar com testes automatizados**
   - Adicionar um teste em `tests/LojaExemplo.Tests.Unidade/Services/ServicoDePagamentos.Tests.cs` que verifica se `_repositorioDePagamentos.AtualizarAsync` é chamado com um `PagamentoInfo` cujo `Status` é `Estornado`.
   - Executar `dotnet test tests/LojaExemplo.Tests.Unidade/LojaExemplo.Tests.Unidade.csproj` e confirmar a passagem.

## Código relevante
```csharp
// src/Services/ServicoDePagamentos.cs
pagamento.Status = StatusPagamento.Estornado;
pagamento.DataEstorno = DateTime.Now;
await _repositorioDePagamentos.AtualizarAsync(pagamento);
```

```csharp
// tests/LojaExemplo.Tests.Unidade/Services/ServicoDePagamentos.Tests.cs
_mockRepositorioDePagamentos.Verify(
    r => r.AtualizarAsync(It.Is<PagamentoInfo>(p => p.Status == StatusPagamento.Estornado)),
    Times.Once);
```

## Lições aprendidas
- Sempre validar que o estado do objeto está consistente antes de repassá-lo a um repositório.
- Testes de unidade com `Verify` ajudam a garantir contratos de interação entre serviços e repositórios.

## Dica de prompt
```
Crie um teste de unidade usando xUnit e Moq para o método EstornarPagamentoAsync em ServicoDePagamentos. Garanta que IRepositorioDePagamentos.AtualizarAsync seja verificado com um PagamentoInfo cujo Status seja Estornado e capture o objeto para assertivas adicionais.
```
