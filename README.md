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

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 👥 Autor

- **Fernando Okuma** - *Desenvolvimento inicial* - [https://github.com/feokuma](https://github.com/feokuma)
