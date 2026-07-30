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

-- Fornecedores
-- TipoFornecedor: 1 = Hotel, 2 = Fornecedor
INSERT INTO Fornecedores (Nome, Cnpj, TipoFornecedor, Ativo) VALUES
('Hotel Ipanema Palace', '12.345.678/0001-90', 1, 1),
('Hotel Serra Verde', '23.456.789/0001-01', 1, 1),
('Transportadora Rota Certa', '34.567.890/0001-12', 2, 1),
('Locadora VeloCar', '45.678.901/0001-23', 2, 0);
GO

-- Comprovantes
-- Status: 1 = Recebido, 2 = Em validação, 3 = Validado, 4 = Com inconsistência, 5 = Integrado ao ERP
INSERT INTO Comprovantes
    (NumeroDocumento, FornecedorId, DataEmissao, DataVencimento, Valor, Descricao, Status, DataCadastro, DataValidacao, DataIntegracao, ObservacaoInconsistencia)
VALUES
    ('NF-1001', 1, DATEADD(DAY, -10, GETDATE()), DATEADD(DAY, 5, GETDATE()), 1580.50, 'Hospedagem - evento corporativo', 1, DATEADD(DAY, -10, GETDATE()), NULL, NULL, NULL),
    ('NF-2002', 2, DATEADD(DAY, -20, GETDATE()), DATEADD(DAY, -5, GETDATE()), 940.00, 'Diárias - visita técnica', 3, DATEADD(DAY, -20, GETDATE()), DATEADD(DAY, -18, GETDATE()), NULL, NULL),
    ('NF-3003', 3, DATEADD(DAY, -30, GETDATE()), DATEADD(DAY, -15, GETDATE()), 320.75, 'Transporte de equipe', 5, DATEADD(DAY, -30, GETDATE()), DATEADD(DAY, -28, GETDATE()), DATEADD(DAY, -25, GETDATE()), NULL),
    ('NF-4004', 1, DATEADD(DAY, -3, GETDATE()), DATEADD(DAY, 10, GETDATE()), 210.00, 'Ajuste de diária - valor divergente do contrato', 4, DATEADD(DAY, -3, GETDATE()), NULL, NULL, 'Valor cobrado divergente do contratado com o hotel.');
GO

-- Histórico
INSERT INTO Historicos (ComprovanteId, DataHora, Acao, Descricao) VALUES
(1, DATEADD(DAY, -10, GETDATE()), 'Cadastro', 'Comprovante cadastrado.'),

(2, DATEADD(DAY, -20, GETDATE()), 'Cadastro', 'Comprovante cadastrado.'),
(2, DATEADD(DAY, -18, GETDATE()), 'Validação', 'Comprovante validado.'),

(3, DATEADD(DAY, -30, GETDATE()), 'Cadastro', 'Comprovante cadastrado.'),
(3, DATEADD(DAY, -28, GETDATE()), 'Validação', 'Comprovante validado.'),
(3, DATEADD(DAY, -25, GETDATE()), 'Integração ao ERP', 'Comprovante integrado ao ERP (simulado).'),

(4, DATEADD(DAY, -3, GETDATE()), 'Cadastro', 'Comprovante cadastrado.'),
(4, DATEADD(DAY, -2, GETDATE()), 'Inconsistência registrada', 'Valor cobrado divergente do contratado com o hotel.');
GO
