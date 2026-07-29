IF DB_ID('comprovantes_db') IS NULL
BEGIN
    CREATE DATABASE comprovantes_db;
END
GO

USE comprovantes_db;
GO

CREATE TABLE Fornecedores (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(150) NOT NULL,
    Cnpj NVARCHAR(18) NOT NULL,
    TipoFornecedor INT NOT NULL,
    Ativo BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE Comprovantes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NumeroDocumento NVARCHAR(30) NOT NULL,
    FornecedorId INT NOT NULL,
    DataEmissao DATETIME2 NOT NULL,
    DataVencimento DATETIME2 NOT NULL,
    Valor DECIMAL(18,2) NOT NULL,
    Descricao NVARCHAR(500) NULL,
    Status INT NOT NULL,
    DataCadastro DATETIME2 NOT NULL,
    DataValidacao DATETIME2 NULL,
    DataIntegracao DATETIME2 NULL,
    ObservacaoInconsistencia NVARCHAR(500) NULL,
    CONSTRAINT FK_Comprovantes_Fornecedores FOREIGN KEY (FornecedorId)
        REFERENCES Fornecedores(Id) ON DELETE NO ACTION,
    CONSTRAINT UQ_Comprovantes_Fornecedor_Numero UNIQUE (FornecedorId, NumeroDocumento)
);
GO

CREATE TABLE Historicos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ComprovanteId INT NOT NULL,
    DataHora DATETIME2 NOT NULL,
    Acao NVARCHAR(100) NOT NULL,
    Descricao NVARCHAR(500) NULL,
    CONSTRAINT FK_Historicos_Comprovantes FOREIGN KEY (ComprovanteId)
        REFERENCES Comprovantes(Id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_Comprovantes_FornecedorId ON Comprovantes(FornecedorId);
CREATE INDEX IX_Historicos_ComprovanteId ON Historicos(ComprovanteId);
GO
