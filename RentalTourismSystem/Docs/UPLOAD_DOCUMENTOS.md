# Funcionalidade de Upload de Documentos

## ?? Descri��o

Sistema completo de upload e gerenciamento de documentos para clientes, ve�culos e funcion�rios no RentalTourismSystem.

## ? Recursos Implementados

### 1. **Upload de Documentos**
- ? Suporte para PDF e imagens (JPG, JPEG, PNG, GIF, BMP)
- ? Limite de tamanho: 10MB por arquivo
- ? Valida��o de tipo e tamanho de arquivo
- ? Preview de imagens antes do upload
- ? Armazenamento organizado por tipo de entidade (clientes/veiculos)
- ? Gera��o de nomes �nicos para evitar conflitos

### 2. **Gerenciamento de Documentos**
- ? Listagem de documentos enviados
- ? Visualiza��o de imagens inline
- ? Download de documentos
- ? Exclus�o de documentos (Admin/Manager)
- ? Metadados: nome, tipo, tamanho, data de upload, usu�rio

### 3. **Tipos de Documentos Suportados**

#### **Clientes:**
- CNH (Carteira Nacional de Habilita��o)
- RG (Registro Geral)
- CPF
- Comprovante de Resid�ncia
- Foto de Perfil
- Outros

#### **Ve�culos:**
- CRLV (Certificado de Registro e Licenciamento do Ve�culo)
- Nota Fiscal
- Ap�lice de Seguro
- Comprovante de IPVA
- Fotos do Ve�culo
- Outros

#### **Funcion�rios:**
- CNH
- RG
- CPF
- Comprovante de Resid�ncia
- Contrato de Trabalho
- Carteira de Trabalho
- Foto de Perfil
- Outros

## ?? Como Usar

### Passo 1: Executar o Script SQL

Execute o script de migration para criar a tabela de documentos:

```sql
-- Executar em: RentalTourismSystem\Migrations\Scripts\AdicionarTabelaDocumentos.sql
```

Ou utilize a migration do Entity Framework:

```bash
dotnet ef database update --project RentalTourismSystem
```

### Passo 2: Verificar Pasta de Upload

Certifique-se de que a pasta existe:
```
RentalTourismSystem\wwwroot\uploads\
```

A pasta ser� criada automaticamente pelo FileService se n�o existir.

### Passo 3: Acessar a Funcionalidade

#### **Para Clientes:**
1. Acesse `Clientes > Detalhes do Cliente`
2. Clique no bot�o "?? Documentos"
3. Selecione o tipo de documento
4. Escolha o arquivo
5. Adicione uma descri��o (opcional)
6. Clique em "Enviar Documento"

#### **Para Ve�culos:**
1. Acesse `Ve�culos > Detalhes do Ve�culo`
2. Clique no bot�o "?? Documentos"
3. Selecione o tipo de documento
4. Escolha o arquivo
5. Adicione uma descri��o (opcional)
6. Clique em "Enviar Documento"

## ?? Permiss�es

### Upload de Documentos
- **Clientes:** Admin, Manager, Employee
- **Ve�culos:** Admin, Manager

### Visualiza��o/Download
- **Todos:** Admin, Manager
- **Restri��o:** Verifica��o adicional pode ser implementada para limitar acesso

### Exclus�o
- **Apenas:** Admin, Manager

## ?? Estrutura de Arquivos

```
RentalTourismSystem/
??? Controllers/
?   ??? DocumentosUploadController.cs          # Controller principal
??? Models/
?   ??? Documento.cs                            # Model do documento
??? Services/
?   ??? IFileService.cs                         # Interface do servi�o
?   ??? FileService.cs                          # Implementa��o do servi�o
??? Views/
?   ??? DocumentosUpload/
?       ??? UploadCliente.cshtml                # View para clientes
?       ??? UploadVeiculo.cshtml                # View para ve�culos
??? Data/
?   ??? RentalTourismContext.cs                 # DbContext atualizado
??? Migrations/
?   ??? Scripts/
?       ??? AdicionarTabelaDocumentos.sql       # Script de cria��o
??? wwwroot/
    ??? uploads/                                 # Pasta de armazenamento
        ??? clientes/
        ?   ??? 1/                              # Documentos do cliente ID 1
        ?   ??? 2/                              # Documentos do cliente ID 2
        ??? veiculos/
            ??? 1/                              # Documentos do ve�culo ID 1
            ??? 2/                              # Documentos do ve�culo ID 2
```

## ?? Configura��o T�cnica

### Servi�o Registrado em Program.cs
```csharp
builder.Services.AddScoped<IFileService, FileService>();
```

### Relacionamentos no Banco de Dados
```sql
Documento (1) -> (*) Cliente  [CASCADE DELETE]
Documento (1) -> (*) Veiculo  [CASCADE DELETE]
Documento (1) -> (*) Funcionario  [CASCADE DELETE]
Documento (1) -> (*) ApplicationUser  [SET NULL]
```

### �ndices Criados
- `IX_Documentos_DataUpload`
- `IX_Documentos_TipoDocumento`
- `IX_Documentos_ClienteId`
- `IX_Documentos_VeiculoId`
- `IX_Documentos_FuncionarioId`

## ?? Campos da Tabela Documentos

| Campo | Tipo | Descri��o |
|-------|------|-----------|
| Id | INT | Chave prim�ria |
| NomeArquivo | NVARCHAR(200) | Nome original do arquivo |
| CaminhoArquivo | NVARCHAR(500) | Caminho relativo do arquivo |
| TipoDocumento | NVARCHAR(50) | Tipo do documento (CNH, RG, etc) |
| ContentType | NVARCHAR(100) | MIME type do arquivo |
| TamanhoBytes | BIGINT | Tamanho do arquivo em bytes |
| Descricao | NVARCHAR(500) | Descri��o opcional |
| DataUpload | DATETIME2 | Data e hora do upload |
| UsuarioUpload | NVARCHAR(100) | Usu�rio que fez o upload |
| ClienteId | INT (nullable) | FK para Cliente |
| VeiculoId | INT (nullable) | FK para Ve�culo |
| FuncionarioId | INT (nullable) | FK para Funcion�rio |
| ApplicationUserId | NVARCHAR(450) (nullable) | FK para ApplicationUser |

## ?? Recursos da Interface

### Preview de Arquivos
- ? Preview de imagens antes do upload
- ? �cone para PDFs

### Lista de Documentos
- ? Cards com informa��es completas
- ? �cones diferenciados por tipo
- ? Informa��es de tamanho formatadas
- ? Data e hora do upload
- ? Nome do usu�rio que fez o upload

### A��es Dispon�veis
- ??? Visualizar (imagens)
- ?? Baixar (todos os tipos)
- ??? Excluir (Admin/Manager)

## ?? Seguran�a

1. **Valida��o de Arquivo:**
   - Extens�es permitidas controladas
   - Tamanho m�ximo definido
   - Valida��o de MIME type

2. **Armazenamento Seguro:**
   - Nomes de arquivo �nicos (GUID)
   - Organiza��o por pastas
   - Fora do acesso direto via URL

3. **Controle de Acesso:**
   - Verifica��o de autentica��o
   - Verifica��o de autoriza��o por role
   - Logs de todas as opera��es

## ?? Logs

Todas as opera��es s�o logadas:
- Upload de documentos
- Download de documentos
- Exclus�o de documentos
- Tentativas de acesso n�o autorizado
- Erros durante opera��es

## ?? Tratamento de Erros

- ? Valida��o de entrada
- ? Mensagens de erro amig�veis
- ? Logs detalhados para debugging
- ? Rollback em caso de falha
- ? Feedback visual para o usu�rio

## ?? Extensibilidade

### Para adicionar novo tipo de documento:

1. Adicione em `TipoDocumentoEnum.cs`:
```csharp
public const string NomeTipo = "Nome do Tipo";
```

2. Adicione na lista do tipo apropriado:
```csharp
public static List<string> ObterTiposCliente()
{
    return new List<string> { ..., NomeTipo, ... };
}
```

### Para adicionar upload para outra entidade:

1. Adicione propriedade de navega��o no Model:
```csharp
public virtual ICollection<Documento> Documentos { get; set; }
```

2. Configure relacionamento no `DbContext`:
```csharp
modelBuilder.Entity<Documento>()
    .HasOne(e => e.MinhaEntidade)
    .WithMany(m => m.Documentos)
    .HasForeignKey(e => e.MinhaEntidadeId)
    .OnDelete(DeleteBehavior.Cascade);
```

3. Crie action no controller e view correspondente

## ?? Melhorias Futuras

- [ ] Suporte para m�ltiplos uploads simult�neos
- [ ] Compress�o autom�tica de imagens
- [ ] Visualizador de PDF integrado
- [ ] Detec��o autom�tica de tipo de documento via OCR
- [ ] Versionamento de documentos
- [ ] Assinatura digital de documentos
- [ ] Notifica��es de documentos vencidos
- [ ] Integra��o com armazenamento em nuvem (Azure Blob, AWS S3)
- [ ] Thumbnail autom�tico para imagens
- [ ] Busca por conte�do de documentos

## ?? Troubleshooting

### Erro: "Pasta n�o encontrada"
**Solu��o:** Certifique-se que a pasta `wwwroot/uploads` existe e tem permiss�es de escrita.

### Erro: "Arquivo muito grande"
**Solu��o:** Aumente o limite em `FileService.cs` (padr�o: 10MB)

### Erro: "Tipo de arquivo n�o permitido"
**Solu��o:** Adicione a extens�o no array `_extensoesPermitidasTodas`

### Erro: "Documento n�o encontrado"
**Solu��o:** Verifique se o arquivo f�sico existe e o caminho est� correto no banco

## ?? Licen�a

Este m�dulo faz parte do RentalTourismSystem e segue a mesma licen�a do projeto principal.

## ?? Contribuidores

Desenvolvido como parte do sistema de loca��o e turismo integrado.

---

**Vers�o:** 1.0.0  
**Data:** Outubro 2025  
**Status:** ? Produ��o
