# Report Schedule

API em ASP.NET Core (.NET 8) para agendamento de relatórios, com persistência em MySQL via Entity Framework Core.

## Rotas

- `GET /health`
- CRUD de agendamentos: `GET/POST/PUT/PATCH/DELETE /api/report-schedules`

## Frameworks e bibliotecas

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8
- Pomelo.EntityFrameworkCore.MySql (provider MySQL)
- Swagger / OpenAPI (Swashbuckle)

## Banco de dados (MySQL via Docker)

O projeto inclui um `docker-compose.yml` na raiz para subir MySQL e a API.

- Subir serviços:
  - `docker-compose up -d --build`

A API recebe a connection string via variável de ambiente (ex.: `ConnectionStrings__DefaultConnection`). O `.env` é usado pelo Compose e não deve ser commitado.

## Migrações (EF Core)

Os comandos abaixo devem ser executados no diretório do projeto (onde está o `.csproj`).

1) Criar a migration inicial:

- `dotnet ef migrations add InitialCreate`

2) Aplicar no banco:

- `dotnet ef database update`

### Como informar a connection string (sem colocar senha no repositório)

Opção A) Variável de ambiente (exemplo):

- `export ConnectionStrings__DefaultConnection="server=localhost;port=3306;database=report_schedule;user=root;password=root"`

Opção B) User Secrets (recomendado para rodar local):

- `dotnet user-secrets init`
- `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;port=3306;database=report_schedule;user=root;password=root"`

> No Docker, normalmente você não precisa ajustar `appsettings.json`, porque o Compose injeta as variáveis de ambiente.

## Testes

Os testes unitários ficam no projeto `ReportSchedule.Tests`.

- Rodar os testes:
  - `dotnet test`

- Exibir os nomes dos testes no console (saída detalhada):
  - `dotnet test --logger "console;verbosity=detailed"`

- Listar os testes (sem executar):
  - `dotnet test --list-tests`

- Gerar relatório TRX (útil para CI):
  - `dotnet test --logger "trx;LogFileName=test_results.trx"`
