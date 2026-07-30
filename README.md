# Controle de Comprovantes Financeiros

Controle do ciclo de vida de comprovantes de pagamento: recebimento, validação (ou inconsistência) e integração simulada com o ERP.

Projeto único: interface (Razor Pages) e API (Controllers + Swagger) no mesmo processo, mesma porta.

## Tecnologias

- .NET 8 / C#
- ASP.NET Core Razor Pages + Web API
- EF Core + SQL Server (LocalDB), com Migrations
- Swagger / OpenAPI
- Bootstrap 5
- Serilog

## Pré-requisitos

- .NET 8 SDK
- SQL Server LocalDB

## Como executar

```
cd ComprovantesApp
dotnet restore
dotnet run
```

Ao iniciar, aplica as migrations e popula dados de exemplo se o banco estiver vazio. Sem passo manual de banco.

- Interface: `http://localhost:5000`
- Swagger: `http://localhost:5000/swagger`

## Arquitetura

Razor Pages e Controllers usam os mesmos `Services` (`FornecedorService`, `ComprovanteService`) — regra de negócio não é duplicada.

- `RegraDeNegocioException`: nas Pages vira mensagem pro usuário; nos Controllers vira `400` com `{ "mensagem": "..." }`.
- DTOs de requisição (`FornecedorRequest`, `ComprovanteRequest`, `InconsistenciaRequest`) usados só pelos Controllers, pra API não aceitar campos como `Status`/`DataCadastro` direto do cliente.
- Fornecedor não é excluído, só inativado.
- Índice único (`FornecedorId` + `NumeroDocumento`) no banco + validação no service.
- Mudança de status gera registro em `Historicos`.
- Schema em EF Core Migrations, aplicado automaticamente. Dados de exemplo via `DbInitializer`.
- Ações destrutivas pedem confirmação em modal.
- Serilog: ações de negócio + requisições HTTP, em console e `logs/`.

### Novas migrations

```
dotnet ef migrations add NomeDaMigration
dotnet ef database update
```

(`dotnet run` já aplica pendentes sozinho)

## Endpoints

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/fornecedores` | Lista fornecedores |
| GET | `/api/fornecedores/{id}` | Obtém um fornecedor |
| POST | `/api/fornecedores` | Cadastra fornecedor |
| PUT | `/api/fornecedores/{id}` | Atualiza fornecedor |
| PATCH | `/api/fornecedores/{id}/inativar` | Inativa fornecedor |
| PATCH | `/api/fornecedores/{id}/ativar` | Reativa fornecedor |
| GET | `/api/comprovantes` | Lista comprovantes (filtros: `fornecedorId`, `numeroDocumento`, `status`, `emissaoDe`, `emissaoAte`) |
| GET | `/api/comprovantes/{id}` | Obtém um comprovante |
| GET | `/api/comprovantes/{id}/historico` | Histórico |
| POST | `/api/comprovantes` | Cadastra comprovante |
| PUT | `/api/comprovantes/{id}` | Atualiza (bloqueado se já integrado) |
| DELETE | `/api/comprovantes/{id}` | Exclui (só status Recebido) |
| PATCH | `/api/comprovantes/{id}/validar` | Marca como Validado |
| PATCH | `/api/comprovantes/{id}/inconsistencia` | Marca inconsistência (corpo: `{ "motivo": "..." }`) |
| POST | `/api/comprovantes/{id}/integrar` | Integra ao ERP |
| PATCH | `/api/comprovantes/{id}/cancelar` | Cancela (corpo: `{ "motivo": "..." }`) |

## Pendências

- Testes automatizados
- Paginação na listagem
- Autenticação e usuário responsável por ação
- Controle de concorrência (RowVersion)
