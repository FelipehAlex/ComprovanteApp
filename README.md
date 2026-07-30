# Controle de Comprovantes Financeiros

Aplicação para controle do ciclo de vida de comprovantes de pagamento enviados por hotéis e fornecedores: recebimento, validação (ou registro de inconsistência) e integração simulada com o ERP.

## Tecnologias

- .NET 8 / C#
- ASP.NET Core Razor Pages
- Entity Framework Core + SQL Server (LocalDB)
- Bootstrap 5 (CDN)
- Serilog (log em console e arquivo)

## Pré-requisitos

- .NET 8 SDK
- SQL Server LocalDB (incluído na instalação padrão do Visual Studio; caso não esteja disponível, instalar o componente "SQL Server Express LocalDB" pelo Visual Studio Installer)

## Como executar

1. Criar o banco a partir do script `database_setup.sql`, que cria a estrutura e insere dados de exemplo (fornecedores e comprovantes em diferentes status):

```
sqlcmd -S "(localdb)\mssqllocaldb" -i database_setup.sql
```

2. A connection string em `appsettings.json` já aponta para o LocalDB:

```
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=comprovantes_db;Trusted_Connection=True;"
```

3. Restaurar e executar o projeto:

```
cd ComprovantesApp
dotnet restore
dotnet run
```

A aplicação fica disponível em `http://localhost:5000`, já com os dados de exemplo carregados.

## Arquitetura
Cada ação de negócio (validar, marcar inconsistência, integrar, excluir) é um handler de página, sem dependência de JavaScript além das confirmações de ação.

As regras de negócio ficam isoladas em `Services` (`FornecedorService`, `ComprovanteService`), injetados por interface. As páginas apenas chamam o service correspondente e tratam o resultado.

Outros pontos:

- `RegraDeNegocioException` separa erro de regra de negócio (exibido como mensagem para o usuário) de erro inesperado.
- Fornecedor não é excluído, apenas inativado, para preservar o vínculo com comprovantes já cadastrados.
- Índice único composto (`FornecedorId` + `NumeroDocumento`) no banco, além da validação equivalente no service.
- Toda mudança de status gera um registro em `Historicos`, consultável em `/Comprovantes/Historico/{id}`.
- Banco criado via script SQL, sem uso de migrations do EF Core.
- Ações destrutivas ou irreversíveis (excluir comprovante, integrar ao ERP, inativar fornecedor) pedem confirmação através de um modal do Bootstrap, reaproveitado entre as páginas.
- Serilog registra as ações de negócio (cadastro, validação, inconsistência, integração, exclusão) com dados estruturados em campos separados, além das requisições HTTP via `UseSerilogRequestLogging()`. Logs em console e em `logs/` (um arquivo por dia).

## Pendências

- Testes automatizados das regras de negócio.
- Paginação na listagem de comprovantes.
- Autenticação e registro do usuário responsável por cada ação.
- Controle de concorrência otimista (RowVersion).
