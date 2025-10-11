IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [Agencias] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(100) NOT NULL,
        [CNPJ] nvarchar(20) NULL,
        [Endereco] nvarchar(200) NOT NULL,
        [Cidade] nvarchar(100) NULL,
        [Telefone] nvarchar(20) NOT NULL,
        [Email] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Agencias] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [Clientes] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(100) NOT NULL,
        [CPF] nvarchar(14) NOT NULL,
        [Telefone] nvarchar(20) NOT NULL,
        [Email] nvarchar(100) NOT NULL,
        [Endereco] nvarchar(200) NOT NULL,
        [CEP] nvarchar(10) NULL,
        [DataNascimento] datetime2 NOT NULL,
        [EstadoCivil] nvarchar(50) NULL,
        [Profissao] nvarchar(100) NULL,
        [CNH] nvarchar(20) NULL,
        [ValidadeCNH] datetime2 NULL,
        [CategoriaCNH] nvarchar(5) NULL,
        [DataCadastro] datetime2 NOT NULL,
        CONSTRAINT [PK_Clientes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [PacotesViagens] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(100) NOT NULL,
        [Descricao] nvarchar(1000) NOT NULL,
        [Destino] nvarchar(100) NOT NULL,
        [Duracao] int NOT NULL,
        [UnidadeTempo] nvarchar(max) NOT NULL,
        [Preco] decimal(10,2) NOT NULL,
        [Ativo] bit NOT NULL,
        [DataCriacao] datetime2 NOT NULL,
        CONSTRAINT [PK_PacotesViagens] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [StatusCarros] (
        [Id] int NOT NULL IDENTITY,
        [Status] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_StatusCarros] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [StatusReservaViagens] (
        [Id] int NOT NULL IDENTITY,
        [Status] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_StatusReservaViagens] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [Funcionarios] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(100) NOT NULL,
        [Cpf] nvarchar(14) NOT NULL,
        [Telefone] nvarchar(20) NOT NULL,
        [Email] nvarchar(100) NOT NULL,
        [Cargo] nvarchar(50) NOT NULL,
        [Salario] decimal(10,2) NOT NULL,
        [DataAdmissao] datetime2 NOT NULL,
        [AgenciaId] int NOT NULL,
        CONSTRAINT [PK_Funcionarios] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Funcionarios_Agencias_AgenciaId] FOREIGN KEY ([AgenciaId]) REFERENCES [Agencias] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [Veiculos] (
        [Id] int NOT NULL IDENTITY,
        [Marca] nvarchar(50) NOT NULL,
        [Modelo] nvarchar(50) NOT NULL,
        [Ano] int NOT NULL,
        [Placa] nvarchar(10) NOT NULL,
        [Cor] nvarchar(50) NOT NULL,
        [Combustivel] nvarchar(30) NOT NULL,
        [Cambio] nvarchar(30) NOT NULL,
        [ValorDiaria] decimal(10,2) NOT NULL,
        [Quilometragem] int NOT NULL,
        [DataCadastro] datetime2 NOT NULL,
        [StatusCarroId] int NOT NULL,
        [AgenciaId] int NOT NULL,
        CONSTRAINT [PK_Veiculos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Veiculos_Agencias_AgenciaId] FOREIGN KEY ([AgenciaId]) REFERENCES [Agencias] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Veiculos_StatusCarros_StatusCarroId] FOREIGN KEY ([StatusCarroId]) REFERENCES [StatusCarros] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [ReservasViagens] (
        [Id] int NOT NULL IDENTITY,
        [DataReserva] datetime2 NOT NULL,
        [DataViagem] datetime2 NOT NULL,
        [Quantidade] int NOT NULL,
        [ValorTotal] decimal(10,2) NOT NULL,
        [Observacoes] nvarchar(500) NULL,
        [ClienteId] int NOT NULL,
        [PacoteViagemId] int NOT NULL,
        [StatusReservaViagemId] int NOT NULL,
        [DataCriacao] datetime2 NOT NULL,
        [UltimaAtualizacao] datetime2 NULL,
        CONSTRAINT [PK_ReservasViagens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ReservasViagens_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ReservasViagens_PacotesViagens_PacoteViagemId] FOREIGN KEY ([PacoteViagemId]) REFERENCES [PacotesViagens] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ReservasViagens_StatusReservaViagens_StatusReservaViagemId] FOREIGN KEY ([StatusReservaViagemId]) REFERENCES [StatusReservaViagens] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [NomeCompleto] nvarchar(100) NOT NULL,
        [Cpf] nvarchar(14) NULL,
        [DataCadastro] datetime2 NOT NULL,
        [Ativo] bit NOT NULL,
        [Cargo] nvarchar(100) NULL,
        [AgenciaId] int NULL,
        [FuncionarioId] int NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUsers_Agencias_AgenciaId] FOREIGN KEY ([AgenciaId]) REFERENCES [Agencias] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_AspNetUsers_Funcionarios_FuncionarioId] FOREIGN KEY ([FuncionarioId]) REFERENCES [Funcionarios] ([Id]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [Locacoes] (
        [Id] int NOT NULL IDENTITY,
        [DataRetirada] datetime2 NOT NULL,
        [DataDevolucao] datetime2 NOT NULL,
        [DataDevolucaoReal] datetime2 NULL,
        [ValorTotal] decimal(10,2) NOT NULL,
        [Observacoes] nvarchar(500) NULL,
        [QuilometragemDevolucao] int NOT NULL,
        [ClienteId] int NOT NULL,
        [VeiculoId] int NOT NULL,
        [FuncionarioId] int NOT NULL,
        [AgenciaId] int NOT NULL,
        CONSTRAINT [PK_Locacoes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Locacoes_Agencias_AgenciaId] FOREIGN KEY ([AgenciaId]) REFERENCES [Agencias] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Locacoes_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Locacoes_Funcionarios_FuncionarioId] FOREIGN KEY ([FuncionarioId]) REFERENCES [Funcionarios] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Locacoes_Veiculos_VeiculoId] FOREIGN KEY ([VeiculoId]) REFERENCES [Veiculos] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [ServicosAdicionais] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(100) NOT NULL,
        [Descricao] nvarchar(500) NOT NULL,
        [Preco] decimal(10,2) NOT NULL,
        [ReservaViagemId] int NOT NULL,
        CONSTRAINT [PK_ServicosAdicionais] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ServicosAdicionais_ReservasViagens_ReservaViagemId] FOREIGN KEY ([ReservaViagemId]) REFERENCES [ReservasViagens] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(128) NOT NULL,
        [ProviderKey] nvarchar(128) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(128) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [Documentos] (
        [Id] int NOT NULL IDENTITY,
        [NomeArquivo] nvarchar(200) NOT NULL,
        [CaminhoArquivo] nvarchar(500) NOT NULL,
        [TipoDocumento] nvarchar(50) NOT NULL,
        [ContentType] nvarchar(100) NULL,
        [TamanhoBytes] bigint NOT NULL,
        [Descricao] nvarchar(500) NULL,
        [DataUpload] datetime2 NOT NULL,
        [UsuarioUpload] nvarchar(100) NULL,
        [ClienteId] int NULL,
        [VeiculoId] int NULL,
        [FuncionarioId] int NULL,
        [ApplicationUserId] nvarchar(450) NULL,
        CONSTRAINT [PK_Documentos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Documentos_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Documentos_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Documentos_Funcionarios_FuncionarioId] FOREIGN KEY ([FuncionarioId]) REFERENCES [Funcionarios] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Documentos_Veiculos_VeiculoId] FOREIGN KEY ([VeiculoId]) REFERENCES [Veiculos] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE TABLE [Notificacoes] (
        [Id] int NOT NULL IDENTITY,
        [Titulo] nvarchar(200) NOT NULL,
        [Mensagem] nvarchar(1000) NOT NULL,
        [Tipo] nvarchar(50) NOT NULL,
        [Categoria] nvarchar(100) NULL,
        [LinkAcao] nvarchar(max) NULL,
        [TextoLinkAcao] nvarchar(max) NULL,
        [ClienteId] int NULL,
        [VeiculoId] int NULL,
        [LocacaoId] int NULL,
        [ReservaId] int NULL,
        [Lida] bit NOT NULL,
        [DataCriacao] datetime2 NOT NULL,
        [DataLeitura] datetime2 NULL,
        [Prioridade] int NOT NULL,
        CONSTRAINT [PK_Notificacoes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Notificacoes_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Notificacoes_Locacoes_LocacaoId] FOREIGN KEY ([LocacaoId]) REFERENCES [Locacoes] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Notificacoes_ReservasViagens_ReservaId] FOREIGN KEY ([ReservaId]) REFERENCES [ReservasViagens] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Notificacoes_Veiculos_VeiculoId] FOREIGN KEY ([VeiculoId]) REFERENCES [Veiculos] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CNPJ', N'Cidade', N'Email', N'Endereco', N'Nome', N'Telefone') AND [object_id] = OBJECT_ID(N'[Agencias]'))
        SET IDENTITY_INSERT [Agencias] ON;
    EXEC(N'INSERT INTO [Agencias] ([Id], [CNPJ], [Cidade], [Email], [Endereco], [Nome], [Telefone])
    VALUES (1, N''12.345.678/0001-90'', N''João Pessoa - PB'', N''central@rentaltourism.com.br'', N''Rua Principal, 123 - Centro'', N''Agência Central'', N''(83) 3221-1234'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CNPJ', N'Cidade', N'Email', N'Endereco', N'Nome', N'Telefone') AND [object_id] = OBJECT_ID(N'[Agencias]'))
        SET IDENTITY_INSERT [Agencias] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Ativo', N'DataCriacao', N'Descricao', N'Destino', N'Duracao', N'Nome', N'Preco', N'UnidadeTempo') AND [object_id] = OBJECT_ID(N'[PacotesViagens]'))
        SET IDENTITY_INSERT [PacotesViagens] ON;
    EXEC(N'INSERT INTO [PacotesViagens] ([Id], [Ativo], [DataCriacao], [Descricao], [Destino], [Duracao], [Nome], [Preco], [UnidadeTempo])
    VALUES (1, CAST(1 AS bit), ''2024-01-15T10:00:00.0000000'', N''3 dias e 2 noites na Praia do Forte com café da manhã incluso'', N''Praia do Forte - BA'', 3, N''Pacote Praia do Forte'', 850.0, N''dias''),
    (2, CAST(1 AS bit), ''2024-01-20T14:30:00.0000000'', N''5 dias na Serra da Mantiqueira com todas as refeições'', N''Serra da Mantiqueira - MG'', 5, N''Pacote Serra da Mantiqueira'', 1200.0, N''dias''),
    (3, CAST(1 AS bit), ''2024-02-01T09:15:00.0000000'', N''Emocionante passeio de buggy pelas dunas de Natal com parada para banho de lagoa'', N''Genipabu - RN'', 4, N''Passeio de Buggy nas Dunas'', 120.0, N''horas''),
    (4, CAST(1 AS bit), ''2024-02-10T11:45:00.0000000'', N''Mergulho com cilindro nos recifes de coral de Porto de Galinhas, equipamentos inclusos'', N''Porto de Galinhas - PE'', 3, N''Mergulho em Recife de Coral'', 180.0, N''horas'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Ativo', N'DataCriacao', N'Descricao', N'Destino', N'Duracao', N'Nome', N'Preco', N'UnidadeTempo') AND [object_id] = OBJECT_ID(N'[PacotesViagens]'))
        SET IDENTITY_INSERT [PacotesViagens] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Status') AND [object_id] = OBJECT_ID(N'[StatusCarros]'))
        SET IDENTITY_INSERT [StatusCarros] ON;
    EXEC(N'INSERT INTO [StatusCarros] ([Id], [Status])
    VALUES (1, N''Disponível''),
    (2, N''Alugado''),
    (3, N''Manutenção''),
    (4, N''Indisponível'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Status') AND [object_id] = OBJECT_ID(N'[StatusCarros]'))
        SET IDENTITY_INSERT [StatusCarros] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Status') AND [object_id] = OBJECT_ID(N'[StatusReservaViagens]'))
        SET IDENTITY_INSERT [StatusReservaViagens] ON;
    EXEC(N'INSERT INTO [StatusReservaViagens] ([Id], [Status])
    VALUES (1, N''Pendente''),
    (2, N''Confirmada''),
    (3, N''Cancelada''),
    (4, N''Realizada'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Status') AND [object_id] = OBJECT_ID(N'[StatusReservaViagens]'))
        SET IDENTITY_INSERT [StatusReservaViagens] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_AgenciaId] ON [AspNetUsers] ([AgenciaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AspNetUsers_Cpf] ON [AspNetUsers] ([Cpf]) WHERE [Cpf] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AspNetUsers_FuncionarioId] ON [AspNetUsers] ([FuncionarioId]) WHERE [FuncionarioId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Clientes_CPF] ON [Clientes] ([CPF]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Clientes_Email] ON [Clientes] ([Email]) WHERE [Email] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Documentos_ApplicationUserId] ON [Documentos] ([ApplicationUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Documentos_ClienteId] ON [Documentos] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Documentos_DataUpload] ON [Documentos] ([DataUpload]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Documentos_FuncionarioId] ON [Documentos] ([FuncionarioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Documentos_TipoDocumento] ON [Documentos] ([TipoDocumento]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Documentos_VeiculoId] ON [Documentos] ([VeiculoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Funcionarios_AgenciaId] ON [Funcionarios] ([AgenciaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Funcionarios_Cpf] ON [Funcionarios] ([Cpf]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Funcionarios_Email] ON [Funcionarios] ([Email]) WHERE [Email] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Locacoes_AgenciaId] ON [Locacoes] ([AgenciaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Locacoes_ClienteId] ON [Locacoes] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Locacoes_FuncionarioId] ON [Locacoes] ([FuncionarioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Locacoes_VeiculoId] ON [Locacoes] ([VeiculoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Notificacoes_Categoria] ON [Notificacoes] ([Categoria]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Notificacoes_ClienteId] ON [Notificacoes] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Notificacoes_DataCriacao] ON [Notificacoes] ([DataCriacao]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Notificacoes_Lida] ON [Notificacoes] ([Lida]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Notificacoes_LocacaoId] ON [Notificacoes] ([LocacaoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Notificacoes_ReservaId] ON [Notificacoes] ([ReservaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Notificacoes_Tipo] ON [Notificacoes] ([Tipo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Notificacoes_VeiculoId] ON [Notificacoes] ([VeiculoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_ReservasViagens_ClienteId] ON [ReservasViagens] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_ReservasViagens_PacoteViagemId] ON [ReservasViagens] ([PacoteViagemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_ReservasViagens_StatusReservaViagemId] ON [ReservasViagens] ([StatusReservaViagemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_ServicosAdicionais_ReservaViagemId] ON [ServicosAdicionais] ([ReservaViagemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Veiculos_AgenciaId] ON [Veiculos] ([AgenciaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Veiculos_Placa] ON [Veiculos] ([Placa]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    CREATE INDEX [IX_Veiculos_StatusCarroId] ON [Veiculos] ([StatusCarroId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251011144548_AdicionarDocumentos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251011144548_AdicionarDocumentos', N'9.0.9');
END;

COMMIT;
GO

