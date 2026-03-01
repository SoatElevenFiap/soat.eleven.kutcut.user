# Soat Eleven Kutcut User

Microserviço responsável por gerenciar e autenticar usuários do sistema Kutcut.

## 📋 Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Arquitetura](#arquitetura)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Pré-requisitos](#pré-requisitos)
- [Configuração e Execução](#configuração-e-execução)
- [Endpoints da API](#endpoints-da-api)
- [Testes](#testes)
- [Variáveis de Configuração](#variáveis-de-configuração)

## 🎯 Sobre o Projeto

Este microserviço é responsável por todo o gerenciamento de usuários e autenticação no sistema Kutcut. Ele fornece funcionalidades para criação, consulta, atualização e exclusão de usuários, além de autenticação via JWT.

## 🚀 Tecnologias Utilizadas

- **.NET 10.0** - Framework principal
- **PostgreSQL** - Banco de dados relacional
- **Entity Framework Core 10.0** - ORM para acesso a dados
- **Redis** - Cache distribuído
- **JWT Bearer** - Autenticação e autorização
- **Azure Key Vault** - Gerenciamento de secrets
- **FluentValidation** - Validação de dados
- **Swagger/OpenAPI** - Documentação da API
- **Docker & Docker Compose** - Containerização
- **XUnit** - Framework de testes unitários
- **Moq** - Framework de mock para testes
- **Coverlet** - Cobertura de código

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture** e está organizado em 4 camadas:

### 1. **API Layer** (`Soat.Eleven.Kutcut.User.Api`)
- Ponto de entrada da aplicação
- Controllers para exposição dos endpoints REST
- Configurações de middleware, autenticação e injeção de dependências
- Health checks
- Swagger para documentação

### 2. **Application Layer** (`Soat.Eleven.Kutcut.User.Application`)
- Lógica de negócio e casos de uso
- Handlers para processamento de comandos
- DTOs (Data Transfer Objects) de entrada e saída
- Validadores usando FluentValidation
- Interfaces de serviços

### 3. **Domain Layer** (`Soat.Eleven.Kutcut.User.Domain`)
- Entidades do domínio
- Regras de negócio
- Enums
- Interfaces de repositórios

### 4. **Infrastructure Layer** (`Soat.Eleven.Kutcut.User.Infra`)
- Contexto do Entity Framework
- Implementação de repositórios
- Serviços de infraestrutura (JWT, Redis, etc.)
- Migrations do banco de dados

## 📁 Estrutura do Projeto

```
soat.eleven.kutcut.user/
├── docker-compose.yml
├── README.md
└── src/
    ├── Soat.Eleven.Kutcut.User.Api/
    │   ├── Controllers/
    │   │   ├── AuthController.cs
    │   │   └── UserController.cs
    │   ├── Configurations/
    │   │   ├── KeyVaultConfiguration.cs
    │   │   ├── RegisterConfigurations.cs
    │   │   └── SwaggerConfiguration.cs
    │   ├── appsettings.json
    │   └── Program.cs
    ├── Soat.Eleven.Kutcut.User.Application/
    │   ├── DTOs/
    │   │   ├── Inputs/
    │   │   └── Outputs/
    │   ├── Handlers/
    │   │   ├── LoginHandle.cs
    │   │   └── UserHandler.cs
    │   ├── Interfaces/
    │   │   ├── ILoginHandle.cs
    │   │   └── IUserHandler.cs
    │   └── Validators/
    ├── Soat.Eleven.Kutcut.User.Domain/
    │   ├── Entities/
    │   ├── Enums/
    │   └── Interfaces/
    ├── Soat.Eleven.Kutcut.User.Infra/
    │   ├── Context/
    │   ├── Migrations/
    │   ├── Repositories/
    │   └── Services/
    └── Soat.Eleven.Kutcut.User.Tests/
        ├── UnitTestes/
        │   ├── Handlers/
        │   ├── Services/
        │   └── Validators/
        └── TestResults/
```

## 📦 Pré-requisitos

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet)
- [Docker](https://www.docker.com/) e [Docker Compose](https://docs.docker.com/compose/)
- [PostgreSQL 14+](https://www.postgresql.org/) (ou via Docker)
- [Redis](https://redis.io/) (ou via Docker)

## ⚙️ Configuração e Execução

### 1. Clone o repositório

```bash
git clone https://github.com/seu-usuario/soat.eleven.kutcut.user.git
cd soat.eleven.kutcut.user
```

### 2. Suba os serviços de infraestrutura com Docker Compose

```bash
docker-compose up -d
```

Isso irá iniciar:
- PostgreSQL na porta 5432
- Redis na porta 6379

### 3. Execute as migrations do banco de dados

```bash
cd src/Soat.Eleven.Kutcut.User.Api
dotnet ef database update
```

### 4. Execute a aplicação

```bash
dotnet run --project src/Soat.Eleven.Kutcut.User.Api/Soat.Eleven.Kutcut.Users.Api.csproj
```

A aplicação estará disponível em:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger**: http://localhost:5000/swagger

## 🔌 Endpoints da API

### Autenticação

#### `POST /api/Auth`
Realiza a autenticação de um usuário e retorna um token JWT.

**Request Body:**
```json
{
  "email": "usuario@example.com",
  "password": "senha123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600
}
```

### Usuários

#### `POST /api/User`
Cria um novo usuário no sistema.

**Request Body:**
```json
{
  "name": "João Silva",
  "email": "joao@example.com",
  "password": "senha123"
}
```

#### `GET /api/User/{id}`
Busca um usuário por ID.

#### `GET /api/User`
Lista todos os usuários (requer autenticação).

#### `PUT /api/User/{id}`
Atualiza os dados de um usuário (requer autenticação).

#### `DELETE /api/User/{id}`
Remove um usuário do sistema (requer autenticação).

### Health Check

#### `GET /health`
Verifica o status da aplicação.

**Response:**
```json
{
  "status": "Healthy"
}
```

## 🧪 Testes

O projeto possui testes de unidade com cobertura de código configurada.

### Executar todos os testes

```bash
dotnet test src/Soat.Eleven.Kutcut.User.Tests/Soat.Eleven.Kutcut.Users.Tests.csproj
```

### Executar testes com cobertura de código

```bash
dotnet test src/Soat.Eleven.Kutcut.User.Tests/Soat.Eleven.Kutcut.Users.Tests.csproj /p:CollectCoverage=true
```

Os relatórios de cobertura serão gerados em:
- `src/Soat.Eleven.Kutcut.User.Tests/TestResults/coverage.cobertura.xml`
- `src/Soat.Eleven.Kutcut.User.Tests/TestResults/coverage.opencover.xml`

### Configuração de Cobertura

O projeto está configurado para exigir:
- **80% de cobertura mínima** em linhas, branches e métodos
- Inclui: Application, Domain e Infra
- Exclui: Testes, Migrations, Context, Configurations

## 🔧 Variáveis de Configuração

### appsettings.json

```json
{
  "ConnectionStrings": {
    "PostgresConnectionString": "Host=localhost;Database=user_kutcut_db;Username=kutcut_user;Password=sua_senha",
    "Redis": "localhost:6379"
  },
  "JwtSettings": {
    "SecretKey": "sua_chave_secreta_jwt"
  },
  "SaltKey": "sua_chave_salt",
  "KeyVault": {
    "Name": "nome-do-keyvault"
  }
}
```

### Variáveis de Ambiente do Docker Compose

- `POSTGRES_USER`: Usuário do PostgreSQL
- `POSTGRES_PASSWORD`: Senha do PostgreSQL
- `POSTGRES_DB`: Nome do banco de dados

## 📝 Dependências Principais

### API
- Microsoft.AspNetCore.OpenApi
- Microsoft.EntityFrameworkCore.Design
- Npgsql.EntityFrameworkCore.PostgreSQL
- Swashbuckle.AspNetCore
- Azure.Extensions.AspNetCore.Configuration.Secrets
- Azure.Identity

### Application
- FluentValidation

### Infrastructure
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.EntityFrameworkCore
- Npgsql.EntityFrameworkCore.PostgreSQL
- StackExchange.Redis

### Tests
- xUnit
- Moq
- coverlet.msbuild
- coverlet.collector

## 📄 Licença

Este projeto é privado e pertence à equipe SOAT Eleven.

---

Desenvolvido com ❤️ pela equipe SOAT Eleven
