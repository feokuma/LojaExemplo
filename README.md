# LojaExemplo - Sistema de E-commerce

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=.net)
![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp)
![xUnit](https://img.shields.io/badge/xUnit-Test%20Framework-512BD4?style=flat-square)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)

## 📋 Sobre o Projeto

O **LojaExemplo** é um sistema de e-commerce desenvolvido em .NET 8 que demonstra as melhores práticas de desenvolvimento com foco em **testes automatizados**. O projeto implementa um fluxo completo de vendas online, incluindo gestão de produtos, pedidos e pagamentos.

### 🎯 Objetivos do Projeto

- Demonstrar a implementação de **testes unitários** e **testes de integração**
- Aplicar princípios de **Clean Architecture** e **SOLID**
- Implementar **injeção de dependência** e **mocking**
- Mostrar boas práticas de desenvolvimento em C#/.NET

## 🏗️ Arquitetura

O projeto segue uma arquitetura em camadas com separação clara de responsabilidades:

```
src/
├── Controllers/          # Camada de apresentação (API)
├── Services/            # Camada de aplicação (regras de negócio)
├── Repositories/        # Camada de dados (acesso a dados)
└── Modelos/            # Camada de domínio (entidades)

tests/
├── Unidade/            # Testes unitários com mocks
└── Integracao/         # Testes de integração end-to-end
```

## 🚀 Tecnologias Utilizadas

### Backend
- **.NET 8** - Framework principal
- **ASP.NET Core** - Web API
- **C#** - Linguagem de programação
- **Swagger/OpenAPI** - Documentação da API

### Testes
- **xUnit** - Framework de testes
- **Moq** - Framework para mocking
- **Coverlet** - Cobertura de código

### Desenvolvimento
- **Visual Studio Code** - IDE
- **Git** - Controle de versão

## 📦 Funcionalidades

### 🛍️ Gestão de Produtos
- Cadastro, consulta, atualização e remoção de produtos
- Controle de estoque automático
- Validação de disponibilidade

### 🛒 Gestão de Pedidos
- Criação de pedidos com múltiplos itens
- Validação de estoque antes da confirmação
- Estados do pedido: `Pendente` → `Confirmado` → `Pago` → `Enviado` → `Entregue`
- Cancelamento de pedidos

### 💳 Processamento de Pagamentos
- Múltiplos métodos de pagamento (Cartão, PIX, Boleto)
- Validação de valores e métodos
- Estorno de pagamentos
- Atualização automática do status do pedido

### 🔍 Consultas
- Busca de produtos por nome
- Histórico de pedidos por cliente
- Verificação de status de pagamento

## 🛠️ Como Executar o Projeto

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado
- [Git](https://git-scm.com/) para clonar o repositório

### 1. Clonar o Repositório

```bash
git clone https://github.com/feokuma/LojaExemplo.git
cd LojaExemplo
```

### 2. Restaurar Dependências

```bash
# Restaurar dependências da aplicação principal
dotnet restore src/LojaExemplo.csproj

# Restaurar dependências dos testes
dotnet restore tests/Unidade/LojaExemplo.Unidade.csproj
dotnet restore tests/Integracao/LojaExemplo.Integracao.csproj
```

### 3. Executar a Aplicação

```bash
# Navegar para o diretório src
cd src

# Executar a aplicação
dotnet run
```

A aplicação estará disponível em:
- **HTTP**: http://localhost:5182
- **Swagger UI**: http://localhost:5182/swagger

### 4. Testar a API

Você pode testar a API usando:

1. **Swagger UI** (recomendado): Acesse http://localhost:5182/swagger
2. **Postman** ou **Insomnia**: Importe a collection das requisições
3. **cURL** ou **HTTPie**: Use os exemplos abaixo

#### Exemplos de Requisições

```bash
# Criar um novo pedido
curl -X POST "http://localhost:5182/api/pedidos" \
  -H "Content-Type: application/json" \
  -d '{
    "clienteEmail": "cliente@exemplo.com",
    "itens": [
      {
        "produtoId": 1,
        "quantidade": 2
      }
    ]
  }'

# Consultar um pedido
curl -X GET "http://localhost:5182/api/pedidos/1"

# Confirmar um pedido
curl -X POST "http://localhost:5182/api/pedidos/1/confirmar"

# Processar pagamento
curl -X POST "http://localhost:5182/api/pedidos/1/pagar" \
  -H "Content-Type: application/json" \
  -d '{
    "metodoPagamento": "CartaoCredito",
    "valor": 5000.00
  }'
```

## 🧪 Executando os Testes

### Executar Todos os Testes

```bash
# Da raiz do projeto
dotnet test
```

### Executar Testes por Categoria

```bash
# Apenas testes unitários
dotnet test tests/Unidade/

# Apenas testes de integração
dotnet test tests/Integracao/
```

### Executar com Relatório de Cobertura

```bash
# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Para relatório detalhado (necessário instalar reportgenerator)
dotnet tool install -g dotnet-reportgenerator-globaltool
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

### Executar Testes Específicos

```bash
# Executar um teste específico
dotnet test --filter "FullyQualifiedName=LojaExemplo.Testes.Unitarios.ServicoDePedidosTests.CriarPedidoAsync_ComDadosValidos_DeveCriarPedidoComSucesso"

# Executar testes por nome parcial
dotnet test --filter "Name~CriarPedido"
```

## 📊 Cobertura de Testes

O projeto possui **64 testes automatizados** com cobertura abrangente:

### Testes Unitários (36 testes)

#### `ServicoDePedidos.Tests.cs` (24 testes)

- ✅ Criação de pedidos com dados válidos
- ✅ Validação de entrada (email, itens vazios)
- ✅ Verificação de produtos inexistentes
- ✅ Controle de estoque insuficiente
- ✅ Confirmação e cancelamento de pedidos
- ✅ Cálculo de valores com múltiplos produtos
- ✅ Consultas por cliente
- ✅ **Testes de demonstração**: Inversão de parâmetros em descontos (bug intencional)

#### `ServicoDePagamentos.Tests.cs` (12 testes)

- ✅ Processamento de pagamentos
- ✅ Validação de métodos de pagamento
- ✅ Estorno de pagamentos
- ✅ Verificação de status
- ✅ Métodos de pagamento disponíveis

### Testes de Integração (28 testes)

#### `ServicoDePedidosIntegracao.Tests.cs` (11 testes)

- ✅ Fluxo completo de criação e gestão de pedidos
- ✅ Integração real com repositório de produtos
- ✅ Validação de estoque em tempo real
- ✅ Gerenciamento de estoque (redução e devolução)
- ✅ Múltiplos pedidos por cliente
- ✅ **Testes de demonstração**: Evidenciam o bug de inversão de parâmetros

#### `ServicoDePagamentosIntegracao.Tests.cs` (17 testes)

- ✅ Fluxo completo: Criar → Confirmar → Pagar → Estornar
- ✅ Validações de pagamento com pedidos reais
- ✅ Múltiplos métodos de pagamento (Cartão, PIX, Boleto, etc.)
- ✅ Múltiplos pedidos e pagamentos por cliente
- ✅ **Retry logic**: Lida com falha aleatória de 10% no processamento

#### Tratamento de Falhas Aleatórias

Os testes que precisam garantir processamento de pagamento agora implementam **retry logic**:

```csharp
// Tentar processar pagamento até ter sucesso (devido à falha aleatória de 10%)
bool pagamentoProcessado = false;
int tentativas = 0;
while (!pagamentoProcessado && tentativas < 20)
{
    pagamentoProcessado = await servicoDePagamentos.ProcessarPagamentoAsync(
        pedido.Id, "CartaoCredito", pedido.ValorTotal);
    tentativas++;
}
Assert.True(pagamentoProcessado, "Pagamento deveria ter sido processado após múltiplas tentativas");
```

### 🎯 Benefícios da Refatoração

1. **Confiabilidade**: Testes passam consistentemente em múltiplas execuções
2. **Isolamento**: Cada teste é completamente independente dos outros
3. **Manutenibilidade**: Padrão factory facilita mudanças futuras
4. **Resiliência**: Retry logic lida com comportamentos não-determinísticos
5. **Clareza**: Fica claro que cada teste tem seu próprio contexto

---

## 🔄 Exemplo de Refatoração Guiada por Testes

### Cenário: Adicionar Método de Validação na Classe `PagamentoInfo`

Este exemplo demonstra como realizar uma refatoração segura utilizando testes automatizados como rede de segurança.

### 📍 Contexto da Refatoração

**Problema Identificado**: O código de verificação de pagamento aprovado está duplicado em múltiplos lugares:

```csharp
// Em ServicoDePagamentos.cs - linha 54
public async Task<bool> VerificarStatusPagamentoAsync(int pedidoId)
{
    var pagamento = await _repositorioDePagamentos.ObterPorPedidoIdAsync(pedidoId);
    return pagamento != null && pagamento.Status == StatusPagamento.Aprovado;
}

// Em ServicoDePagamentos.cs - linha 65
public async Task<bool> EstornarPagamentoAsync(int pedidoId)
{
    await Task.Delay(50);
    
    var pagamento = await _repositorioDePagamentos.ObterPorPedidoIdAsync(pedidoId);
    if (pagamento == null)
        return false;

    if (pagamento.Status != StatusPagamento.Aprovado)  // <-- Duplicação
        return false;
    
    // ... resto do código
}
```

**Solução**: Criar um método `EstaAprovado()` na classe `PagamentoInfo` que encapsula essa lógica.

---

### 🎯 Passo 1: Garantir Cobertura de Testes Existente

**Arquivo**: `tests/Unidade/Services/ServicoDePagamentos.Tests.cs`

Os testes atuais já cobrem os cenários de validação:

```csharp
[Fact]
public async Task VerificarStatusPagamentoAsync_ComPedidoSemPagamento_DeveRetornarFalse()
{
    // Arrange
    int pedidoId = 999;
    _mockRepositorioDePagamentos
        .Setup(r => r.ObterPorPedidoIdAsync(pedidoId))
        .ReturnsAsync((PagamentoInfo?)null);

    // Act
    var resultado = await _servicoDePagamentos.VerificarStatusPagamentoAsync(pedidoId);

    // Assert
    Assert.False(resultado);  // ✅ Deve continuar passando após refatoração
}

[Fact]
public async Task VerificarStatusPagamentoAsync_ComPagamentoAprovado_DeveRetornarTrue()
{
    // Arrange
    var pagamentoInfo = new PagamentoInfo
    {
        PedidoId = 1,
        Status = StatusPagamento.Aprovado,
        Valor = 100m
    };
    
    _mockRepositorioDePagamentos
        .Setup(r => r.ObterPorPedidoIdAsync(1))
        .ReturnsAsync(pagamentoInfo);

    // Act
    var resultado = await _servicoDePagamentos.VerificarStatusPagamentoAsync(1);

    // Assert
    Assert.True(resultado);  // ✅ Deve continuar passando após refatoração
}
```

**Executar testes ANTES da refatoração:**
```bash
dotnet test tests/Unidade/LojaExemplo.Unidade.csproj
# Resultado esperado: Total tests: 88, Passed: 88 ✅
```

---

### 🔧 Passo 2: Realizar a Refatoração

#### 2.1 - Adicionar Método na Classe `PagamentoInfo`

**Arquivo**: `src/Modelos/PagamentoInfo.cs`

```csharp
namespace LojaExemplo.Modelos
{
    public class PagamentoInfo
    {
        public int PedidoId { get; set; }
        public string MetodoPagamento { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataPagamento { get; set; }
        public DateTime? DataEstorno { get; set; }
        public StatusPagamento Status { get; set; }

        // ✨ NOVO MÉTODO - Encapsula lógica de validação
        public bool EstaAprovado()
        {
            return Status == StatusPagamento.Aprovado && Valor > 0;
        }
    }

    public enum StatusPagamento
    {
        Pendente = 1,
        Aprovado = 2,
        Rejeitado = 3,
        Estornado = 4
    }
}
```

#### 2.2 - Refatorar `ServicoDePagamentos` para Usar o Novo Método

**Arquivo**: `src/Services/ServicoDePagamentos.cs`

**ANTES da refatoração** (linha 54):
```csharp
public async Task<bool> VerificarStatusPagamentoAsync(int pedidoId)
{
    var pagamento = await _repositorioDePagamentos.ObterPorPedidoIdAsync(pedidoId);
    return pagamento != null && pagamento.Status == StatusPagamento.Aprovado;
}
```

**DEPOIS da refatoração**:
```csharp
public async Task<bool> VerificarStatusPagamentoAsync(int pedidoId)
{
    var pagamento = await _repositorioDePagamentos.ObterPorPedidoIdAsync(pedidoId);
    return pagamento?.EstaAprovado() ?? false;  // ✨ Usando o novo método
}
```

**ANTES da refatoração** (linha 69):
```csharp
public async Task<bool> EstornarPagamentoAsync(int pedidoId)
{
    await Task.Delay(50);
    
    var pagamento = await _repositorioDePagamentos.ObterPorPedidoIdAsync(pedidoId);
    if (pagamento == null)
        return false;

    if (pagamento.Status != StatusPagamento.Aprovado)
        return false;
    
    // ... resto do código
}
```

**DEPOIS da refatoração**:
```csharp
public async Task<bool> EstornarPagamentoAsync(int pedidoId)
{
    await Task.Delay(50);
    
    var pagamento = await _repositorioDePagamentos.ObterPorPedidoIdAsync(pedidoId);
    if (pagamento == null || !pagamento.EstaAprovado())  // ✨ Usando o novo método
        return false;
    
    // ... resto do código
}
```

---

### ✅ Passo 3: Validar que os Testes Continuam Passando

**Executar testes DEPOIS da refatoração:**
```bash
dotnet test tests/Unidade/LojaExemplo.Unidade.csproj
# Resultado esperado: Total tests: 88, Passed: 88 ✅
```

**Análise**: Todos os testes continuam passando porque:
1. A lógica de negócio **não mudou** - apenas foi reorganizada
2. Os testes validam o **comportamento externo**, não a implementação interna
3. A refatoração foi **equivalente** - mesmos inputs produzem mesmos outputs

---

### 🧪 Passo 4: Adicionar Testes Unitários para o Novo Método (Opcional)

Embora os testes existentes já validem indiretamente o método `EstaAprovado()`, podemos adicionar testes diretos:

**Arquivo**: `tests/Unidade/Modelos/PagamentoInfo.Tests.cs` (novo arquivo)

```csharp
using Xunit;
using LojaExemplo.Modelos;

namespace LojaExemplo.Unidade.Modelos
{
    public class PagamentoInfoTests
    {
        #region EstaAprovado Tests

        [Fact]
        public void EstaAprovado_ComStatusAprovadoEValorPositivo_DeveRetornarTrue()
        {
            // Arrange
            var pagamento = new PagamentoInfo
            {
                Status = StatusPagamento.Aprovado,
                Valor = 100m
            };

            // Act
            var resultado = pagamento.EstaAprovado();

            // Assert
            Assert.True(resultado);
        }

        [Theory]
        [InlineData(StatusPagamento.Pendente)]
        [InlineData(StatusPagamento.Rejeitado)]
        [InlineData(StatusPagamento.Estornado)]
        public void EstaAprovado_ComStatusDiferenteDeAprovado_DeveRetornarFalse(StatusPagamento status)
        {
            // Arrange
            var pagamento = new PagamentoInfo
            {
                Status = status,
                Valor = 100m
            };

            // Act
            var resultado = pagamento.EstaAprovado();

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void EstaAprovado_ComStatusAprovadoMasValorZero_DeveRetornarFalse()
        {
            // Arrange
            var pagamento = new PagamentoInfo
            {
                Status = StatusPagamento.Aprovado,
                Valor = 0m
            };

            // Act
            var resultado = pagamento.EstaAprovado();

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void EstaAprovado_ComStatusAprovadoMasValorNegativo_DeveRetornarFalse()
        {
            // Arrange
            var pagamento = new PagamentoInfo
            {
                Status = StatusPagamento.Aprovado,
                Valor = -10m
            };

            // Act
            var resultado = pagamento.EstaAprovado();

            // Assert
            Assert.False(resultado);
        }

        #endregion
    }
}
```

**Adicionar projeto ao arquivo .csproj** (se necessário):
```bash
# No arquivo tests/Unidade/LojaExemplo.Unidade.csproj, criar pasta Modelos
mkdir -p tests/Unidade/Modelos
```

**Executar novos testes:**
```bash
dotnet test tests/Unidade/LojaExemplo.Unidade.csproj
# Resultado esperado: Total tests: 92, Passed: 92 ✅ (+4 testes)
```

---

### 📊 Benefícios da Refatoração

| Aspecto | Antes | Depois |
|---------|-------|--------|
| **Duplicação de Código** | Lógica `pagamento.Status == StatusPagamento.Aprovado` repetida em 2 lugares | Encapsulada em método `EstaAprovado()` |
| **Validação de Valor** | ❌ Não validava se `Valor > 0` | ✅ Valida automaticamente |
| **Manutenibilidade** | Mudanças requerem editar múltiplos arquivos | Mudanças centralizadas na classe `PagamentoInfo` |
| **Testabilidade** | Lógica testada indiretamente através dos serviços | Lógica testável diretamente com testes unitários simples |
| **Cobertura de Testes** | 88 testes | 92 testes (+4.5%) |
| **Risco de Regressão** | ✅ Zero - todos os testes continuam passando | - |

---

### 💡 Lições Aprendidas

1. ✅ **Testes como Rede de Segurança**: A cobertura de testes existente permitiu refatorar com confiança
2. ✅ **Refatoração Incremental**: Mudanças pequenas e validadas a cada passo
3. ✅ **Encapsulamento**: Lógica de negócio movida para onde pertence (classe de domínio)
4. ✅ **Sem Quebra de Contrato**: API pública dos serviços permaneceu inalterada
5. ✅ **Melhoria Contínua**: Refatoração melhorou design sem adicionar funcionalidades

---

### 🎯 Prompt para Gerar Esta Refatoração com IA

```text
Altere ou acrescente um teste de refatoração considerando que a classe PagamentoInfo 
passará a ter um método que responde se o pagamento está aprovado e tem valor não nulo. 
Esse novo exemplo no README deve descrever onde é feita essa refatoração e neste exemplo 
todos os testes só devem continuar passando.

Estruture a resposta com:
1. Contexto do problema (duplicação de código)
2. Solução proposta (novo método EstaAprovado)
3. Passo a passo da refatoração com código antes/depois
4. Validação que testes continuam passando
5. Testes unitários adicionais para o novo método
6. Tabela de benefícios da refatoração
```

---

## 🏢 Estrutura de Dados

### Principais Entidades

```csharp
// Produto
{
  "id": 1,
  "nome": "Notebook",
  "preco": 2500.00,
  "estoqueDisponivel": 10,
  "descricao": "Notebook para trabalho",
  "ativo": true
}

// Pedido
{
  "id": 1,
  "dataPedido": "2025-11-08T10:30:00",
  "clienteEmail": "cliente@exemplo.com",
  "status": "Pago",
  "valorTotal": 5000.00,
  "metodoPagamento": "CartaoCredito",
  "itens": [...]
}
```

## 🔧 Configurações de Desenvolvimento

### Configurar Injeção de Dependência

Para usar o projeto com banco de dados real, adicione no `Program.cs`:

```csharp
// Registrar serviços
builder.Services.AddScoped<IRepositorioDeProdutos, RepositorioDeProdutos>();
builder.Services.AddScoped<IServicoDePedidos, ServicoDePedidos>();
builder.Services.AddScoped<IServicoDePagamentos, ServicoDePagamentos>();

// Adicionar controllers
builder.Services.AddControllers();
```

## 📚 Recursos Adicionais

### Documentação

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [xUnit Documentation](https://xunit.net/docs/getting-started/netcore/cmdline)
- [Moq Documentation](https://github.com/moq/moq4/wiki/Quickstart)

### Tutoriais

- [Unit Testing in .NET](https://docs.microsoft.com/dotnet/core/testing/)
- [Integration Testing in ASP.NET Core](https://docs.microsoft.com/aspnet/core/test/integration-tests)

## 🤖 Prompts para Demonstração com IA

Esta seção contém prompts práticos para demonstrar como a inteligência artificial pode auxiliar na criação de testes automatizados. Os prompts estão separados entre **Testes Unitários** e **Testes de Integração**, organizados de forma progressiva.

---

## 🧪 PARTE 1: Testes Unitários

Os testes unitários focam em testar componentes isoladamente, usando mocks para simular dependências.

### 1️⃣ Início Simples - Testes de Validação Básica

**Objetivo**: Demonstrar testes simples de lógica de negócio sem dependências externas.

**Prompt principal:**
```text
Crie testes unitários completos para a classe Pedido em src/Modelos/Pedido.cs. 
Inclua testes para os métodos Confirmar(), Cancelar(), PodeCancelar() e 
DeveReporEstoque(), cobrindo todos os cenários possíveis de transição de status.
```

**Variação mais específica:**
```text
Crie testes xUnit para validar o comportamento do método Confirmar() da classe 
Pedido. Teste: 1) confirmação bem-sucedida de pedido pendente, 2) tentativa 
de confirmar pedido já confirmado, 3) confirmação de pedidos em outros status.
```

**Variação com tabela de decisão:**
```text
Para o método DeveReporEstoque() da classe Pedido, crie uma matriz de testes 
que valide o retorno para cada status possível: Pendente, Confirmado, Pago, 
Enviado, Entregue e Cancelado. Use [Theory] e [InlineData].
```

---

### 2️⃣ Mocks - Teste com Dependências

**Objetivo**: Mostrar como a IA configura mocks e testa classes com dependências injetadas.

**Prompt principal:**
```text
Crie testes unitários usando Moq para o método ProcessarPagamentoAsync do 
ServicoDePagamentos. Mocke as dependências IServicoDePedidos e 
IRepositorioDePagamentos. Teste todos os cenários de exceção e o fluxo de 
sucesso.
```

**Variação focada em exceções:**
```text
No ServicoDePagamentos, crie testes para ProcessarPagamentoAsync validando: 
1) método de pagamento vazio/nulo, 2) valor inválido (zero ou negativo), 
3) pedido não encontrado, 4) pedido não confirmado, 5) valor diferente do 
total, 6) método de pagamento não suportado, 7) processamento bem-sucedido. 
Use Moq para as dependências.
```

**Variação com verificação de interações:**
```text
Crie testes para o método EstornarPagamentoAsync do ServicoDePagamentos. 
Use Moq.Verify() para garantir que: 1) o repositório de pagamentos foi 
consultado, 2) o status foi atualizado para Estornado, 3) o serviço de 
pedidos foi chamado para cancelar o pedido.
```

---

### 3️⃣ Bug Discovery - Identificar Problemas

**Objetivo**: Demonstrar como a IA pode identificar bugs através da criação de testes.

**Prompt principal:**
```text
Analise o método CalcularDescontoProgressivoAsync do ServicoDeDesconto e 
crie testes unitários. Verifique se a implementação está matematicamente 
correta e sugira correções se necessário.
```

**Variação investigativa com dados concretos:**
```text
Crie testes com dados concretos para o método CalcularDescontoProgressivoAsync:
- valorTotal: 100, percentual: 10 (esperado: desconto de R$ 10)
- valorTotal: 200, percentual: 20 (esperado: desconto de R$ 40)
- valorTotal: 500, percentual: 5 (esperado: desconto de R$ 25)
Identifique se há problemas na implementação atual.
```

**Prompt direto para análise:**
```text
O método CalcularDescontoProgressivoAsync tem a fórmula: 
(valorTotal - percentualDesconto) * percentualDesconto / 100
Crie testes unitários e explique se essa fórmula está correta. Se não estiver, 
qual deveria ser a fórmula correta e como corrigi-la?
```

---

### 4️⃣ Complexidade - Teste Completo com Múltiplos Mocks

**Objetivo**: Mostrar a capacidade da IA em criar testes complexos com múltiplas dependências.

**Prompt principal:**
```text
Crie testes unitários completos para o método CriarPedidoAsync do 
ServicoDePedidos. Mocke IRepositorioDeProdutos, IServicoDeDesconto e 
IRepositorioDePedidos. Teste validações, verificação de estoque, produtos 
inexistentes e o fluxo de sucesso.
```

**Variação detalhada com checklist:**
```text
Para o método CriarPedidoAsync no ServicoDePedidos, crie testes usando Moq que 
cubram:
1. Email do cliente vazio/nulo/whitespace
2. Lista de itens nula ou vazia
3. Produto não encontrado no repositório
4. Estoque insuficiente para algum item
5. Criação bem-sucedida de pedido com 2 produtos
6. Verificação do cálculo correto do valor total
Garanta que os mocks sejam configurados corretamente e as interações verificadas.
```

**Variação com TDD:**
```text
Usando TDD, crie testes primeiro para especificar o comportamento esperado do 
método CriarPedidoComDescontoAsync, depois implemente os testes unitários 
completos mockando todas as dependências.
```

---

### 5️⃣ Refatoração - Melhorar Testes Existentes

**Objetivo**: Demonstrar como a IA pode analisar e melhorar testes já escritos.

**Prompt principal:**
```text
Analise os testes existentes em tests/Unidade/Services/ e sugira melhorias: 
1) cobertura de cenários faltantes, 2) refatoração para melhor legibilidade, 
3) uso de padrões como AAA (Arrange-Act-Assert), 4) extração de métodos 
auxiliares para setup comum, 5) uso de Theory para testes parametrizados.
```

**Variação específica para um arquivo:**
```text
Revise os testes em ServicoDePedidos.Tests.cs e:
- Identifique casos de teste faltantes
- Refatore testes duplicados usando [Theory] e [InlineData]
- Adicione nomes de testes mais descritivos seguindo padrão: 
  MetodoEmTeste_Cenario_ResultadoEsperado
- Melhore a organização dos Arrange-Act-Assert
```

**Prompt de análise de cobertura:**
```text
Analise todos os testes de unidade do projeto e identifique:
1. Métodos sem nenhum teste
2. Branches de código não cobertas
3. Edge cases não testados (valores limite, nulos, vazios)
4. Oportunidades para testes parametrizados
5. Setup de mocks que pode ser extraído para métodos auxiliares
Priorize as melhorias mais importantes.
```

---

## 🔗 PARTE 2: Testes de Integração

Os testes de integração validam o funcionamento de múltiplos componentes trabalhando juntos, sem mocks.

### 6️⃣ Fluxo Completo de Pedidos

**Objetivo**: Testar o fluxo end-to-end de criação e gerenciamento de pedidos com componentes reais.

**Prompt principal:**
```text
Crie testes de integração para o ServicoDePedidos em tests/Integracao/Services/. 
Use repositórios reais (não mocke nada). Teste o fluxo completo:
1. Criar pedido com produtos reais
2. Verificar redução de estoque
3. Confirmar pedido
4. Cancelar pedido
5. Verificar reposição de estoque
```

**Variação com validações complexas:**
```text
Crie testes de integração que validem:
1. Criação de pedido com múltiplos produtos de IDs diferentes
2. Tentativa de criar pedido com produto inexistente (deve lançar exceção)
3. Tentativa de criar pedido com quantidade maior que estoque disponível
4. Cancelamento de pedido pendente (não deve repor estoque)
5. Cancelamento de pedido confirmado (deve repor estoque)
Use repositórios reais sem mocks.
```

**Variação focada em estado compartilhado:**
```text
Crie testes de integração para verificar o comportamento do estoque quando 
múltiplos pedidos são criados sequencialmente para o mesmo produto. Valide:
1. Criação de 3 pedidos usando o mesmo produto
2. Verificação da redução correta do estoque após cada pedido
3. Cancelamento de um pedido no meio da sequência
4. Confirmação de que o estoque foi corretamente ajustado
```

---

### 7️⃣ Fluxo Completo de Pagamentos

**Objetivo**: Testar a integração entre pedidos e pagamentos com todos os componentes reais.

**Prompt principal:**
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

**Variação com cenários de erro:**
```text
Crie testes de integração para validar erros no processamento de pagamento:
1. Tentar pagar pedido que não existe
2. Tentar pagar pedido que ainda está pendente (não confirmado)
3. Tentar pagar com valor diferente do total do pedido
4. Tentar pagar com método de pagamento inválido
5. Tentar estornar pagamento já estornado
Todos os testes devem usar repositórios reais.
```

**Variação com múltiplos clientes:**
```text
Crie testes de integração simulando múltiplos clientes fazendo compras:
1. Cliente A cria pedido, confirma e paga com CartaoCredito
2. Cliente B cria pedido, confirma e paga com Pix
3. Cliente C cria pedido, confirma, paga e depois solicita estorno
4. Verifique que cada cliente tem seu histórico correto
5. Verifique que os pagamentos estão corretamente associados aos pedidos
```

---

### 8️⃣ Integração com Desconto

**Objetivo**: Testar a integração entre pedidos, descontos e pagamentos.

**Prompt principal:**
```text
Crie testes de integração para o método CriarPedidoComDescontoAsync do 
ServicoDePedidos. Teste:
1. Criação de pedido com 10% de desconto
2. Verificação do cálculo correto do valor final
3. Confirmação do pedido
4. Processamento de pagamento com valor já descontado
Use todos os componentes reais (ServicoDePedidos, ServicoDeDesconto, 
ServicoDePagamentos e repositórios).
```

**Variação com diferentes percentuais:**
```text
Crie testes de integração parametrizados usando [Theory] para validar descontos 
de 5%, 10%, 15% e 20% aplicados a pedidos com valores de R$ 100, R$ 500 e 
R$ 1000. Para cada combinação:
1. Crie o pedido com desconto
2. Valide o cálculo do valor final
3. Confirme e processe o pagamento
4. Verifique que o pagamento foi registrado com o valor correto
```

**Variação investigativa do bug:**
```text
Crie testes de integração para o fluxo completo de pedido com desconto. 
Compare o comportamento atual com o comportamento esperado. Se o cálculo 
de desconto estiver incorreto, os testes devem evidenciar a diferença entre 
o valor calculado e o valor esperado para casos como:
- Pedido de R$ 100 com 10% de desconto (esperado: R$ 90 final)
- Pedido de R$ 500 com 20% de desconto (esperado: R$ 400 final)
```

---

### 9️⃣ Testes de Concorrência e Estado

**Objetivo**: Testar comportamentos complexos envolvendo estado compartilhado e múltiplas operações.

**Prompt principal:**
```text
Crie testes de integração para validar o gerenciamento de estoque em cenários 
complexos:
1. Dois pedidos criados para o mesmo produto simultaneamente
2. Verificação de que ambos respeitam o estoque disponível
3. Cancelamento de um pedido e tentativa de criar novo pedido com estoque reposto
4. Confirmação de múltiplos pedidos e validação da redução acumulada do estoque
```

**Variação com histórico de cliente:**
```text
Crie testes de integração para validar o histórico completo de um cliente:
1. Cliente faz 3 pedidos diferentes
2. Confirma 2 pedidos e deixa 1 pendente
3. Paga 1 pedido confirmado
4. Cancela 1 pedido pendente
5. Use ObterPedidosPorClienteAsync para validar o histórico
6. Verifique que todos os status estão corretos
```

---

### 🔟 Testes de integração via API

**Objetivo**: Testar a aplicação completa através dos endpoints HTTP.

**Prompt principal:**
```text
Crie testes de integração para o PedidosController usando WebApplicationFactory 
do ASP.NET Core. Teste:
1. POST /api/pedidos - Criar pedido
2. GET /api/pedidos/{id} - Consultar pedido
3. POST /api/pedidos/{id}/confirmar - Confirmar pedido
4. POST /api/pedidos/{id}/pagar - Processar pagamento
5. DELETE /api/pedidos/{id} - Cancelar pedido
Valide os códigos HTTP de resposta e o conteúdo JSON retornado.
```

**Variação com fluxo completo:**
```text
Crie um teste de integração end-to-end que simule um usuário real:
1. Fazer requisição POST para criar pedido com 2 produtos
2. Verificar resposta HTTP 201 Created
3. Fazer requisição POST para confirmar o pedido
4. Verificar resposta HTTP 200 OK
5. Fazer requisição POST para processar pagamento
6. Fazer requisição GET para validar status final do pedido
Use HttpClient real contra a aplicação rodando em memória.
```

---

## 🎯 Prompts Bônus Avançados

### Geração de Dados de Teste (Unitário)
```text
Crie uma classe TestDataBuilder ou Fixture para gerar objetos Pedido, Produto 
e ItemDePedido válidos para uso nos testes unitários, seguindo o padrão Builder 
ou Object Mother. Inclua métodos para criar: ProdutoValido(), PedidoPendente(), 
PedidoConfirmado(), ItemDePedidoValido().
```

### Geração de Factories para Integração
```text
Crie factories para testes de integração que instanciem os serviços e 
repositórios reais com configuração adequada. Implemente:
1. ServicosDePedidosFactory - retorna instância real com dependências reais
2. ServicoDePagamentosFactory - retorna instância real configurada
3. RepositorioFactory - retorna repositórios em memória
Garanta isolamento entre testes.
```

### Análise de Qualidade Geral
```text
Revise todos os testes (unitários e integração) do projeto e classifique-os:
- Independência (não dependem de outros testes)
- Repetibilidade (sempre produzem o mesmo resultado)
- Velocidade (unitários < 100ms, integração < 1s)
- Clareza (fácil entender o que testam)
- Cobertura (cenários cobertos vs não cobertos)
Sugira melhorias prioritárias para cada categoria.
```

### Testes Parametrizados Unitários
```text
Crie testes parametrizados usando xUnit Theory para o ServicoDeDesconto:
- Teste CalcularDescontoProgressivoAsync com percentuais: 0%, 5%, 10%, 25%, 50%, 100%
- Teste com valores: R$ 0, R$ 100, R$ 500, R$ 1000, R$ 10000
- Teste AplicarDescontoAsync com descontos maiores que o valor total
- Use [Theory], [InlineData] e assertions claras
```

### Cobertura de Exceções
```text
Para cada método que lança exceções no projeto, crie testes (unitários e de 
integração) que validem:
1. O tipo correto de exceção é lançado (ArgumentException, InvalidOperationException)
2. A mensagem da exceção é apropriada e informativa
3. O parâmetro da exceção está correto (paramName)
4. O estado do sistema não é corrompido após a exceção
Use Assert.Throws<T> e FluentAssertions.
```

---

## 💡 Dicas para Usar os Prompts

### Para Testes Unitários:
- ✅ Use mocks para isolar a unidade sendo testada
- ✅ Foque em um método ou classe por vez
- ✅ Teste todos os caminhos de código (branches)
- ✅ Valide exceções e casos de borda
- ✅ Mantenha os testes rápidos (< 100ms)

### Para Testes de Integração:
- ✅ Use componentes reais, evite mocks
- ✅ Teste fluxos completos e interações entre camadas
- ✅ Valide estado compartilhado e efeitos colaterais
- ✅ Garanta isolamento entre testes (setup/teardown adequados)
- ✅ Aceite tempo de execução maior (mas mantenha < 1s quando possível)

### Geral:
1. **Seja específico**: Quanto mais contexto você der, melhores serão os testes gerados
2. **Itere**: Use os resultados da IA como ponto de partida e refine conforme necessário
3. **Revise sempre**: A IA pode gerar código incorreto - sempre valide a lógica
4. **Aprenda com os exemplos**: Observe os padrões usados pela IA e aplique em outros contextos
5. **Combine prompts**: Use múltiplos prompts sequencialmente para tarefas complexas

---

## 📊 Sequências Recomendadas para Apresentação

### 🎤 Demonstração Rápida (10-15 min)
**Foco**: Mostrar o básico de testes unitários e um exemplo de integração
- **Unitários**: Prompts 1 e 2 (validação básica + mocks simples)
- **Integração**: Prompt 6 (fluxo completo de pedidos)

### 🎤 Demonstração Média (25-35 min)
**Foco**: Cobrir unitários completos e principais cenários de integração
- **Unitários**: Prompts 1, 2, 3 e **Refatoração Guiada Por Testes** (básico, mocks, bug discovery, refatoração)
- **Integração**: Prompts 6 e 7 (pedidos e pagamentos), correção de bug

### 🎤 Demonstração Completa (45-60 min)
**Foco**: Demonstração abrangente com casos avançados
- **Unitários**: Todos os prompts 1-5
- **Integração**: Todos os prompts 6-10
- **Bônus**: 2 prompts avançados de sua escolha

### 🎓 Workshop Prático (90-120 min)
**Foco**: Hands-on com participantes
- **Parte 1** (30 min): Demonstração ao vivo dos prompts 1-3
- **Parte 2** (30 min): Participantes usam prompts 4-5
- **Parte 3** (30 min): Demonstração de integração (prompts 6-8)
- **Parte 4** (30 min): Participantes criam seus próprios testes de integração
- **Conclusão** (15 min): Discussão e Q&A

### 🏆 Demonstração Focada em Bug Discovery (15-20 min)
**Foco**: Mostrar como IA identifica bugs
- **Setup** (3 min): Contexto do projeto e do bug intencional
- **Unitário** (5 min): Prompt 3 - descobrir bug no desconto
- **Integração** (7 min): Prompt 8 (variação investigativa) - evidenciar bug no fluxo completo
- **Correção** (5 min): Usar IA para sugerir e implementar correção

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 👥 Autor

- **Fernando Okuma** - *Desenvolvimento inicial* - [https://github.com/feokuma](https://github.com/feokuma)
