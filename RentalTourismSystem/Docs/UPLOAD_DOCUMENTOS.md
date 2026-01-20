# Funcionalidade de Upload de Documentos

## 📚 Descrição

Sistema completo de upload e gerenciamento de documentos para clientes, veículos e funcionários no RentalTourismSystem.

## ✅ Recursos Implementados

### 1. **Upload de Documentos**
- ✅ Suporte para PDF e imagens (JPG, JPEG, PNG, GIF, BMP)
- ✅ Limite de tamanho: 10MB por arquivo
- ✅ Validação de tipo e tamanho de arquivo
- ✅ Preview de imagens antes do upload
- ✅ Armazenamento organizado por tipo de entidade (clientes/veiculos)
- ✅ Geração de nomes únicos para evitar conflitos

### 2. **Gerenciamento de Documentos**
- ✅ Listagem de documentos enviados
- ✅ Visualização de imagens inline
- ✅ Download de documentos
- ✅ Exclusão de documentos (Admin/Manager)
- ✅ Metadados: nome, tipo, tamanho, data de upload, usuário

### 3. **Tipos de Documentos Suportados**

#### **Clientes:**
- CNH (Carteira Nacional de Habilitação)
- RG (Registro Geral)
- CPF
- Comprovante de Residência
- Foto de Perfil
- Outros

#### **Veículos:**
- CRLV (Certificado de Registro e Licenciamento do Veículo)
- Nota Fiscal
- Apólice de Seguro
- Comprovante de IPVA
- Fotos do Veículo
- Outros

#### **Funcionários:**
- CNH
- RG
- CPF
- Comprovante de Residência
- Contrato de Trabalho
- Carteira de Trabalho
- Foto de Perfil
- Outros

## 📚 Como Usar

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

A pasta será criada automaticamente pelo FileService se não existir.

### Passo 3: Acessar a Funcionalidade

#### **Para Clientes:**
1. Acesse `Clientes > Detalhes do Cliente`
2. Clique no botão "📚 Documentos"
3. Selecione o tipo de documento
4. Escolha o arquivo
5. Adicione uma descrição (opcional)
6. Clique em "Enviar Documento"

#### **Para Veículos:**
1. Acesse `Veículos > Detalhes do Veículo`
2. Clique no botão "📚 Documentos"
3. Selecione o tipo de documento
4. Escolha o arquivo
5. Adicione uma descrição (opcional)
6. Clique em "Enviar Documento"

## 📚 Permissões

### Upload de Documentos
- **Clientes:** Admin, Manager, Employee
- **Veículos:** Admin, Manager

### Visualização/Download
- **Todos:** Admin, Manager
- **Restrição:** Verificação adicional pode ser implementada para limitar acesso

### Exclusão
- **Apenas:** Admin, Manager

## 📚 Estrutura de Arquivos

```
RentalTourismSystem/
📚✅ Controllers/
✅   📚✅ DocumentosUploadController.cs          # Controller principal
📚✅ Models/
✅   📚✅ Documento.cs                            # Model do documento
📚✅ Services/
✅   📚✅ IFileService.cs                         # Interface do serviço
✅   📚✅ FileService.cs                          # Implementação do serviço
📚✅ Views/
✅   📚✅ DocumentosUpload/
✅       📚✅ UploadCliente.cshtml                # View para clientes
✅       📚✅ UploadVeiculo.cshtml                # View para veículos
📚✅ Data/
✅   📚✅ RentalTourismContext.cs                 # DbContext atualizado
📚✅ Migrations/
✅   📚✅ Scripts/
✅       📚✅ AdicionarTabelaDocumentos.sql       # Script de criação
📚✅ wwwroot/
    📚✅ uploads/                                 # Pasta de armazenamento
        📚✅ clientes/
        ✅   📚✅ 1/                              # Documentos do cliente ID 1
        ✅   📚✅ 2/                              # Documentos do cliente ID 2
        📚✅ veiculos/
            📚✅ 1/                              # Documentos do veículo ID 1
            📚✅ 2/                              # Documentos do veículo ID 2
```

## 📚 Configuração Técnica

### Serviço Registrado em Program.cs
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

### Índices Criados
- `IX_Documentos_DataUpload`
- `IX_Documentos_TipoDocumento`
- `IX_Documentos_ClienteId`
- `IX_Documentos_VeiculoId`
- `IX_Documentos_FuncionarioId`

## 📚 Campos da Tabela Documentos

| Campo | Tipo | Descrição |
|-------|------|-----------|
| Id | INT | Chave primária |
| NomeArquivo | NVARCHAR(200) | Nome original do arquivo |
| CaminhoArquivo | NVARCHAR(500) | Caminho relativo do arquivo |
| TipoDocumento | NVARCHAR(50) | Tipo do documento (CNH, RG, etc) |
| ContentType | NVARCHAR(100) | MIME type do arquivo |
| TamanhoBytes | BIGINT | Tamanho do arquivo em bytes |
| Descricao | NVARCHAR(500) | Descrição opcional |
| DataUpload | DATETIME2 | Data e hora do upload |
| UsuarioUpload | NVARCHAR(100) | Usuário que fez o upload |
| ClienteId | INT (nullable) | FK para Cliente |
| VeiculoId | INT (nullable) | FK para Veículo |
| FuncionarioId | INT (nullable) | FK para Funcionário |
| ApplicationUserId | NVARCHAR(450) (nullable) | FK para ApplicationUser |

## 📚 Recursos da Interface

### Preview de Arquivos
- ✅ Preview de imagens antes do upload
- ✅ Ícone para PDFs

### Lista de Documentos
- ✅ Cards com informações completas
- ✅ Ícones diferenciados por tipo
- ✅ Informações de tamanho formatadas
- ✅ Data e hora do upload
- ✅ Nome do usuário que fez o upload

### Ações Disponíveis
- 📚✅ Visualizar (imagens)
- 📚 Baixar (todos os tipos)
- 📚✅ Excluir (Admin/Manager)

## 📚 Segurança

1. **Validação de Arquivo:**
   - Extensões permitidas controladas
   - Tamanho máximo definido
   - Validação de MIME type

2. **Armazenamento Seguro:**
   - Nomes de arquivo únicos (GUID)
   - Organização por pastas
   - Fora do acesso direto via URL

3. **Controle de Acesso:**
   - Verificação de autenticação
   - Verificação de autorização por role
   - Logs de todas as operações

## 📚 Logs

Todas as operações são logadas:
- Upload de documentos
- Download de documentos
- Exclusão de documentos
- Tentativas de acesso não autorizado
- Erros durante operações

## 📚 Tratamento de Erros

- ✅ Validação de entrada
- ✅ Mensagens de erro amigáveis
- ✅ Logs detalhados para debugging
- ✅ Rollback em caso de falha
- ✅ Feedback visual para o usuário

## 📚 Extensibilidade

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

1. Adicione propriedade de navegação no Model:
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

## 📚 Melhorias Futuras

- [ ] Suporte para múltiplos uploads simultâneos
- [ ] Compressão automática de imagens
- [ ] Visualizador de PDF integrado
- [ ] Detecção automática de tipo de documento via OCR
- [ ] Versionamento de documentos
- [ ] Assinatura digital de documentos
- [ ] Notificações de documentos vencidos
- [ ] Integração com armazenamento em nuvem (Azure Blob, AWS S3)
- [ ] Thumbnail automático para imagens
- [ ] Busca por conteúdo de documentos

## 📚 Troubleshooting

### Erro: "Pasta não encontrada"
**Solução:** Certifique-se que a pasta `wwwroot/uploads` existe e tem permissões de escrita.

### Erro: "Arquivo muito grande"
**Solução:** Aumente o limite em `FileService.cs` (padrão: 10MB)

### Erro: "Tipo de arquivo não permitido"
**Solução:** Adicione a extensão no array `_extensoesPermitidasTodas`

### Erro: "Documento não encontrado"
**Solução:** Verifique se o arquivo físico existe e o caminho está correto no banco

## 📚 Licença

Este módulo faz parte do RentalTourismSystem e segue a mesma licença do projeto principal.

## 📚 Contribuidores

Desenvolvido como parte do sistema de locação e turismo integrado.

---

**Versão:** 1.0.0  
**Data:** Outubro 2025  
**Status:** ✅ Produção

