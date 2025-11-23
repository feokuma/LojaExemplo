# Roteiro de Apresentação - IA e Testes Automatizados

## 🎯 Objetivo da Apresentação
Demonstrar como usar IA (GitHub Copilot / ChatGPT / Claude) para criar e melhorar testes automatizados, desde testes unitários básicos até testes de integração complexos.

## ⏱️ Duração: 25-35 minutos

---

## 📋 Estrutura da Apresentação

### Introdução (2-3 min)
- Apresentação do projeto LojaExemplo
- Arquitetura básica (Controllers → Services → Repositories → Models)
- Contexto: 88 testes já existentes (unitários + integração)
- Objetivo: Mostrar como IA acelera criação e melhoria de testes

---

## 🧪 PARTE 1: Testes Unitários (12-15 min)

### **Etapa 1**: Validação Básica (3-5 min)
📄 **Arquivo**: `01-validacao-basica.md`

**O que mostrar:**
- Testes simples sem dependências externas
- Classe `Pedido` com lógica de negócio pura
- Padrão AAA (Arrange-Act-Assert)

**Prompt**:
```text
Crie testes unitários completos para a classe Pedido em src/Modelos/Pedido.cs. 
Inclua testes para os métodos Confirmar(), Cancelar(), PodeCancelar() e 
DeveReporEstoque(), cobrindo todos os cenários possíveis de transição de status.
```

**Resultado esperado**: ~10 testes, < 50ms execução, 100% cobertura da classe

---

### **Etapa 2**: Trabalhando com Mocks (4-6 min)
📄 **Arquivo**: `02-testes-com-mocks.md`

**O que mostrar:**
- IA configura mocks automaticamente usando Moq
- Testes de `ServicoDePagamentos.ProcessarPagamentoAsync`
- Verificação de interações com `.Verify()`

**Prompt**:
```text
Crie testes unitários usando Moq para o método ProcessarPagamentoAsync do 
ServicoDePagamentos. Mocke as dependências IServicoDePedidos e 
IRepositorioDePagamentos. Teste todos os cenários de exceção e o fluxo de sucesso.
```

**Resultado esperado**: ~12 testes com mocks configurados corretamente

---

### **Etapa 3**: Bug Discovery (4-6 min) ⭐
📄 **Arquivo**: `03-descobrindo-bugs.md`

**O que mostrar:**
- IA identifica bug através de testes com dados concretos
- Bug no cálculo de desconto progressivo
- Ciclo Red → Green (teste falha → correção → teste passa)

**Prompt**:
```text
Crie testes com dados concretos para o método CalcularDescontoProgressivoAsync:
- valorTotal: 100, percentual: 10 (esperado: desconto de R$ 10)
- valorTotal: 200, percentual: 20 (esperado: desconto de R$ 40)
- valorTotal: 500, percentual: 5 (esperado: desconto de R$ 25)
Identifique se há problemas na implementação atual.
```

**Demonstração ao vivo:**
1. IA cria testes → ❌ FALHAM
2. IA identifica: "Fórmula incorreta: deveria ser `valorTotal * percentual / 100`"
3. Aplicar correção sugerida
4. Executar novamente → ✅ PASSAM

**Mensagem-chave**: IA não apenas gera testes, mas ENCONTRA bugs!

---

### **Etapa 4**: Refatoração Segura (3-5 min)
📄 **Arquivo**: `04-refatoracao-segura.md`

**O que mostrar:**
- Usar testes como rede de segurança
- Refatorar método `EstaAprovado()` na classe `PagamentoInfo`
- Testes continuam passando após refatoração

**Passos:**
1. Executar testes existentes → ✅ Todos passam
2. Refatorar código (centralizar lógica duplicada)
3. Executar novamente → ✅ Todos ainda passam
4. Zero regressões!

**Mensagem-chave**: Testes permitem refatorar com confiança

---

## 🔗 PARTE 2: Testes de Integração (10-13 min)

### **Etapa 5**: Fluxo de Pedidos (4-5 min)
📄 **Arquivo**: `05-integracao-pedidos.md`

**O que mostrar:**
- Diferença unitário vs integração
- Componentes reais (sem mocks)
- Validação de efeitos colaterais (estoque)

**Prompt**:
```text
Crie testes de integração para o ServicoDePedidos em tests/Integracao/Services/. 
Use repositórios reais (não mocke nada). Teste o fluxo completo:
1. Criar pedido com produtos reais
2. Verificar redução de estoque
3. Confirmar pedido
4. Cancelar pedido
5. Verificar reposição de estoque
```

**Comparação**:
| Aspecto | Unitário | Integração |
|---------|----------|------------|
| Dependências | Mocks | Reais |
| Velocidade | < 50ms | < 500ms |
| Efeitos colaterais | ❌ | ✅ |

---

### **Etapa 6**: Fluxo de Pagamentos (4-5 min)
📄 **Arquivo**: `06-integracao-pagamentos.md`

**O que mostrar:**
- Integração multi-camadas (Pedidos + Pagamentos)
- Retry logic para lidar com falhas aleatórias de 10%
- Múltiplos métodos de pagamento

**Prompt**:
```text
Crie testes de integração para o ServicoDePagamentos testando o fluxo completo:
1. Criar pedido, 2. Confirmar pedido, 3. Processar pagamento com diferentes 
métodos (CartaoCredito, Pix, Boleto), 4. Verificar atualização do status do 
pedido, 5. Estornar pagamento, 6. Verificar cancelamento do pedido.
Use todos os componentes reais sem mocks.
```

**Destaque**: Mostrar retry logic para comportamento não-determinístico

---

### **Etapa 7**: Bug Discovery via Integração (3-5 min) ⭐⭐
📄 **Arquivo**: `07-correcao-bug-integracao.md`

**O que mostrar:**
- Bug real que afetaria produção
- Pagamentos de R$ 99,99 são rejeitados
- TDD: Red → Green → Refactor

**Demonstração ao vivo:**
1. IA cria teste para R$ 99,99 → ❌ FALHA com exceção
2. IA identifica validação incorreta no `RepositorioDePagamentos`
3. Corrigir (remover validação)
4. Teste passa → ✅
5. Adicionar testes de edge cases (R$ 99,98, R$ 100,00, etc.)

**Mensagem-chave**: Testes de integração com valores realistas detectam bugs que nunca apareceriam em testes unitários com mocks!

---

## 🎬 Conclusão (2-3 min)

### Recapitulação
✅ **Testes Unitários**: IA gera testes rápidos e isolados  
✅ **Bug Discovery**: IA identifica problemas na lógica  
✅ **Refatoração Segura**: Testes como rede de segurança  
✅ **Testes de Integração**: Validam sistema completo  
✅ **Bugs Reais**: R$ 99,99 - detectado e corrigido  

### Benefícios Demonstrados
1. **Velocidade**: IA gera testes em segundos vs horas manualmente
2. **Qualidade**: Cobertura completa de cenários
3. **Confiança**: Detectar bugs antes da produção
4. **Manutenibilidade**: Código bem testado = código confiável

### Próximos Passos
- Experimentar com seus próprios projetos
- Iterar com IA (refinar prompts)
- Combinar testes unitários + integração
- Usar IA para revisar testes existentes

---

## 📊 Estatísticas Finais

| Métrica | Antes | Depois |
|---------|-------|--------|
| Testes | 88 | ~110+ |
| Bugs encontrados | 0 | 2 |
| Cobertura | Alta | Completa |
| Tempo para criar testes | Horas | Minutos |

---

## 💡 Dicas para a Apresentação

### ✅ O que fazer:
- **Executar testes ao vivo** (mostra que funciona de verdade)
- **Mostrar testes falhando e passando** (ciclo Red-Green)
- **Usar exemplos concretos** (R$ 99,99, R$ 100, R$ 10)
- **Destacar mensagens-chave** após cada etapa
- **Interagir com a audiência** (perguntar se alguém já viu bugs similares)

### ❌ O que evitar:
- Não falar demais sobre sintaxe de código
- Não gastar tempo configurando ambiente (já deve estar pronto)
- Não ficar preso em detalhes técnicos
- Não assumir que todo mundo conhece TDD (explicar brevemente)

---

## 🛠️ Preparação Antes da Apresentação

### Checklist:
- [ ] Ambiente configurado (.NET 8 instalado)
- [ ] Projeto LojaExemplo clonado
- [ ] Testes atuais executando (`dotnet test`)
- [ ] Arquivos markdown 01-07 revisados
- [ ] Exemplos de prompts testados
- [ ] Terminal limpo e pronto
- [ ] IDE (VS Code) configurado
- [ ] Copilot/ChatGPT/Claude disponível

### Comandos úteis:
```bash
# Executar todos os testes
dotnet test

# Executar apenas unitários
dotnet test tests/Unidade/LojaExemplo.Unidade.csproj

# Executar apenas integração
dotnet test tests/Integracao/LojaExemplo.Integracao.csproj

# Executar teste específico
dotnet test --filter "ProcessarPagamento_ComValorExato99e99Reais"

# Ver cobertura
dotnet test --collect:"XPlat Code Coverage"
```

---

## 🎯 Mensagem Final

> "IA não substitui desenvolvedores - ela os empodera! Com IA, você pode criar mais testes, de melhor qualidade, em menos tempo. O resultado? Software mais confiável e menos bugs em produção."

**Call to action**: "Experimente hoje mesmo! Comece com um teste simples e veja como a IA pode ajudar seu time."

---

## 📱 Contato

**Fernando Okuma**  
GitHub: [https://github.com/feokuma](https://github.com/feokuma)  
Email: [seu email se desejar]

---

**Boa apresentação! 🚀**
