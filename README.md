# Controle de Comprovantes Financeiros

Aplicação para controle do ciclo de vida de comprovantes de pagamento enviados por hotéis e fornecedores: recebimento, validação (ou registro de inconsistência) e integração simulada com o ERP.

Um único projeto reúne interface (Razor Pages) e API (Controllers + Swagger), rodando no mesmo processo e na mesma porta — sem precisar iniciar dois projetos separados.

## Tecnologias

- .NET 8 / C#
- ASP.NET Core Razor Pages + Web API (Controllers), no mesmo projeto
- Entity Framework Core + SQL Server (LocalDB), com Migrations
- Swagger / OpenAPI (Swashbuckle)
- Bootstrap 5 (CDN)
- Serilog (log em console e arquivo)

## Pré-requisitos

- .NET 8 SDK
- SQL Server LocalDB (incluído na instalação padrão do Visual Studio; caso não esteja disponível, instalar o componente "SQL Server Express LocalDB" pelo Visual Studio Installer)

## Como executar

```
cd ComprovantesApp
dotnet restore
dotnet run
```

Ao iniciar, a aplicação aplica as migrations pendentes e, se o banco ainda não tiver nenhum fornecedor cadastrado, insere dados de exemplo automaticamente — não é necessário rodar nenhum script SQL manualmente.

- Interface: `http://localhost:5000` (a raiz redireciona para `/Comprovantes`)
- Swagger: `http://localhost:5000/swagger` (também tem um link na barra de navegação)

## Arquitetura

A interface (Razor Pages) e a API (Controllers) usam exatamente os mesmos `Services` (`FornecedorService`, `ComprovanteService`) — nenhuma regra de negócio é duplicada entre as duas camadas de apresentação. As Razor Pages chamam os Services diretamente; os Controllers fazem o mesmo, só que expondo o resultado como JSON, documentado no Swagger.

Outros pontos:

- `RegraDeNegocioException` é tratada de dois jeitos: nas Razor Pages, vira uma mensagem exibida ao usuário (`TempData`/`ModelState`); nos Controllers, vira `400 Bad Request` com `{ "mensagem": "..." }`.
- DTOs de requisição (`FornecedorRequest`, `ComprovanteRequest`, `InconsistenciaRequest`) são usados só pelos Controllers, para não expor campos controlados internamente (`Status`, `DataCadastro`) na API.
- Fornecedor não é excluído, apenas inativado, para preservar o vínculo com comprovantes já cadastrados.
- Índice único composto (`FornecedorId` + `NumeroDocumento`) no banco, além da validação equivalente no service.
- Toda mudança de status gera um registro em `Historicos`.
- Schema do banco versionado via EF Core Migrations (`Migrations/`), aplicadas automaticamente ao iniciar. Dados de exemplo inseridos por um `DbInitializer`, executado uma única vez.
- Ações destrutivas ou irreversíveis (excluir, integrar, inativar) pedem confirmação através de um modal do Bootstrap.
- Serilog registra as ações de negócio e as requisições HTTP. Logs em console e em `logs/`.

### Criando novas migrations

Qualquer alteração nos `Models` ou no `AppDbContext` exige uma nova migration:

```
dotnet ef migrations add NomeDaMigration
dotnet ef database update
```

(o `dotnet run` já aplica migrations pendentes automaticamente, então o `database update` manual normalmente não é necessário em desenvolvimento)

## Endpoints da API

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/fornecedores` | Lista fornecedores |
| GET | `/api/fornecedores/{id}` | Obtém um fornecedor |
| POST | `/api/fornecedores` | Cadastra fornecedor |
| PUT | `/api/fornecedores/{id}` | Atualiza fornecedor |
| PATCH | `/api/fornecedores/{id}/inativar` | Inativa fornecedor (sem excluir) |
| PATCH | `/api/fornecedores/{id}/ativar` | Reativa fornecedor |
| GET | `/api/comprovantes` | Lista comprovantes (filtros: `fornecedorId`, `numeroDocumento`, `status`, `emissaoDe`, `emissaoAte`) |
| GET | `/api/comprovantes/{id}` | Obtém um comprovante |
| GET | `/api/comprovantes/{id}/historico` | Histórico de um comprovante |
| POST | `/api/comprovantes` | Cadastra comprovante |
| PUT | `/api/comprovantes/{id}` | Atualiza comprovante (bloqueado se já integrado) |
| DELETE | `/api/comprovantes/{id}` | Exclui comprovante (apenas status Recebido) |
| PATCH | `/api/comprovantes/{id}/validar` | Marca como Validado |
| PATCH | `/api/comprovantes/{id}/inconsistencia` | Marca como Com inconsistência (corpo: `{ "motivo": "..." }`) |
| POST | `/api/comprovantes/{id}/integrar` | Integra ao ERP (simulado) |

## Pendências

- Testes automatizados das regras de negócio.
- Paginação na listagem de comprovantes.
- Autenticação (ex.: JWT) e registro do usuário responsável por cada ação.
- Controle de concorrência otimista (RowVersion).
