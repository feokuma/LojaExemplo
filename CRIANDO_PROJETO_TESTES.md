# Como Criar Projeto de Testes com GitHub Copilot

## 📋 Contexto

Este documento demonstra como usar o GitHub Copilot para automatizar a criação de projetos de teste em .NET.

---

## 💬 Prompt Utilizado

```plaintext
Crie um projeto de testes seguindo as seguintes instruções:
- O projeto deve ser criado dentro de um diretório chamado "LojaExemplo.Tests.Unidade" na pasta "tests"
- O projeto deve se chamar LojaExemplo.Tests.Unidade
- Utilizar NUnit como ferramenta de testes
- Adicione a referência do projeto LojaExemplo.csproj neste projeto de test
- Adicione o projeto de testes na solution
```

---

## 🎯 O que o Copilot Fez Automaticamente

### 1. Criação do Projeto de Testes
```bash
dotnet new nunit -o ./tests/LojaExemplo.Tests.Unidade -n LojaExemplo.Tests.Unidade
```
- Criou um projeto NUnit no diretório `tests/LojaExemplo.Tests.Unidade`
- Framework de testes utilizado: NUnit
- Estrutura básica pronta para uso

### 2. Referência ao Projeto Principal
```bash
dotnet add ./tests/LojaExemplo.Tests.Unidade/LojaExemplo.Tests.Unidade.csproj reference ./src/LojaExemplo.csproj
```
- Adicionou referência ao projeto principal (`src/LojaExemplo.csproj`)
- Permite acesso às classes e métodos do projeto principal

### 3. Adição à Solution
```bash
dotnet sln LojaExemplo.sln add ./tests/LojaExemplo.Tests.Unidade/LojaExemplo.Tests.Unidade.csproj
```
- Integrou o projeto de testes à solution existente
- Facilita o gerenciamento no Visual Studio/VS Code

---

## 📝 Anotações para a Palestra

### Pontos-Chave a Destacar:

1. **Simplicidade do Prompt**
   - Instrução clara e direta em português
   - Não precisa especificar comandos técnicos
   - O Copilot entende o contexto do workspace

2. **Inteligência Contextual**
   - Identificou automaticamente a estrutura do projeto
   - Escolheu NUnit como framework (conforme instrução)
   - Configurou caminhos relativos corretamente

3. **Automação Completa**
   - Executou 3 comandos em sequência
   - Configurou todas as dependências necessárias
   - Projeto pronto para começar a escrever testes

4. **Boas Práticas Aplicadas**
   - Projeto de testes em diretório separado (`tests/`)
   - Nomenclatura consistente (`.Tests.Unidade`)
   - Referências corretas configuradas

---

## 🔄 Alternativas de Prompt

Outras formas de pedir a mesma tarefa:

```
"Configure um projeto de testes unitários para o LojaExemplo"
```

```
"Preciso de um projeto xUnit para testar a aplicação"
```

```
"Adicione estrutura de testes unitários ao projeto"
```

---

## ✅ Resultado Final


Estrutura criada:
```
tests/
   LojaExemplo.Tests.Unidade/
      ├── LojaExemplo.Tests.Unidade.csproj
      ├── UnitTest1.cs
      └── Usings.cs
```

**Status:** ✅ Pronto para começar a escrever testes!

---

## 💡 Dicas para a Audiência

1. **Seja específico mas não técnico demais**
   - O Copilot entende intenções, não apenas comandos

2. **Use linguagem natural**
   - Português funciona perfeitamente
   - Descreva o que você quer, não como fazer

3. **Confie no contexto**
   - O Copilot analisa a estrutura do projeto
   - Ele conhece convenções e melhores práticas

4. **Valide o resultado**
   - Sempre verifique o que foi criado
   - Execute `dotnet build` para confirmar

---

## 🎤 Roteiro Sugerido para Apresentação

1. **Introdução** (30s)
   - "Vamos criar um projeto de testes em segundos"

2. **Demonstração do Prompt** (1min)
   - Mostrar o prompt simples
   - Destacar que está em português

3. **Execução e Explicação** (2min)
   - Mostrar os 3 comandos executados
   - Explicar cada passo brevemente

4. **Resultado** (30s)
   - Mostrar a estrutura criada
   - Executar `dotnet build` para validar

5. **Conclusão** (1min)
   - Economia de tempo
   - Redução de erros
   - Foco no que importa: escrever testes

---

## 📊 Métricas de Produtividade

- **Tempo Manual:** ~5-10 minutos
- **Tempo com Copilot:** ~30 segundos
- **Comandos Necessários:** 0 (Copilot executa automaticamente)
- **Chance de Erro:** Praticamente zero

---

## 🚀 Próximos Passos

Após criar o projeto, você pode pedir ao Copilot:

```
"Crie testes para a classe ServicoDePedidos"
```

```
"Adicione mocks usando Moq no projeto de testes"
```

```
"Configure code coverage para o projeto"
```
