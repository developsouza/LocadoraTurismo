using RentalTourismSystem.Models.ViewModels;
using Markdig;
using System.Text;
using System.Text.RegularExpressions;

namespace RentalTourismSystem.Services
{
    public class DocumentacaoService : IDocumentacaoService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DocumentacaoService> _logger;
        private readonly string _docsPath;
        private static readonly MarkdownPipeline _markdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        // Mapeamento de documentos com metadados
        private static readonly Dictionary<string, DocumentoMetadata> _documentosMetadata = new()
        {
            ["INDEX"] = new("INDEX.md", "?? �ndice Master da Documenta��o", "�ndice completo de toda a documenta��o do sistema", "?? �ndice", "fas fa-book", 10, new[] { "Admin", "Manager", "Employee", "Developer" }),
            ["README"] = new("README.md", "?? Guia Principal", "README e �ndice geral de navega��o", "?? �ndice", "fas fa-home", 15, new[] { "Admin", "Manager", "Employee", "Developer" }),
            ["GUIA_INICIO_RAPIDO"] = new("GUIA_INICIO_RAPIDO.md", "?? Guia de In�cio R�pido", "15 minutos para estar operacional", "?? In�cio R�pido", "fas fa-rocket", 15, new[] { "Admin", "Manager", "Employee" }),
            ["AUTENTICACAO_GUIA_COMPLETO"] = new("AUTENTICACAO_GUIA_COMPLETO.md", "?? Autentica��o e Seguran�a", "Controle de acesso e permiss�es", "?? Seguran�a", "fas fa-lock", 30, new[] { "Admin", "Manager" }),
            ["CLIENTES_GUIA_COMPLETO"] = new("CLIENTES_GUIA_COMPLETO.md", "?? Gest�o de Clientes", "Cadastro e gerenciamento de clientes", "?? Gest�o", "fas fa-users", 20, new[] { "Admin", "Manager", "Employee" }),
            ["VEICULOS_GUIA_COMPLETO"] = new("VEICULOS_GUIA_COMPLETO.md", "?? Gest�o de Ve�culos", "Controle completo da frota", "?? Gest�o", "fas fa-car", 25, new[] { "Admin", "Manager", "Employee" }),
            ["LOCACOES_GUIA_COMPLETO"] = new("LOCACOES_GUIA_COMPLETO.md", "?? Sistema de Loca��es", "Processo completo de loca��o", "?? Gest�o", "fas fa-file-contract", 30, new[] { "Admin", "Manager", "Employee" }),
            ["MANUTENCAO_GUIA_ACESSO"] = new("MANUTENCAO_GUIA_ACESSO.md", "?? Sistema de Manuten��es", "Controle de manuten��es e custos", "?? Gest�o", "fas fa-tools", 20, new[] { "Admin", "Manager" }),
            ["RESERVAS_VIAGEM_GUIA_COMPLETO"] = new("RESERVAS_VIAGEM_GUIA_COMPLETO.md", "?? Reservas de Viagem", "Gest�o de turismo e pacotes", "?? Gest�o", "fas fa-plane", 20, new[] { "Admin", "Manager", "Employee" }),
            ["UPLOAD_DOCUMENTOS"] = new("UPLOAD_DOCUMENTOS.md", "?? Upload de Documentos", "Sistema de gest�o documental", "?? Gest�o", "fas fa-upload", 15, new[] { "Admin", "Manager", "Employee" }),
            ["REFERENCIA_TECNICA"] = new("REFERENCIA_TECNICA.md", "?? Refer�ncia T�cnica", "Documenta��o para desenvolvedores", "??? T�cnico", "fas fa-code", 45, new[] { "Developer", "Admin" }),
            ["GUIA_VISUAL_FLUXOGRAMAS"] = new("GUIA_VISUAL_FLUXOGRAMAS.md", "?? Guia Visual de Fluxogramas", "Diagramas e processos visuais", "?? Visual", "fas fa-project-diagram", 15, new[] { "Admin", "Manager", "Employee", "Developer" }),
            ["RESUMO_DOCUMENTACAO"] = new("RESUMO_DOCUMENTACAO.md", "?? Resumo da Documenta��o", "Vis�o geral resumida", "?? �ndice", "fas fa-list-alt", 10, new[] { "Admin", "Manager", "Employee" }),

            // Documentos sobre o sistema de documenta��o
            ["START_HERE"] = new("START_HERE.md", "? COMECE AQUI - Acesso R�pido", "In�cio r�pido em 1 minuto", "?? In�cio R�pido", "fas fa-star", 1, new[] { "Admin", "Manager", "Employee", "Developer" }),
            ["SISTEMA_DOCUMENTACAO_README"] = new("SISTEMA_DOCUMENTACAO_README.md", "?? Sistema de Documenta��o - README", "Guia completo do sistema de documenta��o integrado", "??? T�cnico", "fas fa-book-reader", 20, new[] { "Developer", "Admin" }),
            ["GUIA_RAPIDO_DOCUMENTACAO"] = new("GUIA_RAPIDO_DOCUMENTACAO.md", "?? Guia R�pido - Documenta��o", "Como usar o sistema de documenta��o", "?? In�cio R�pido", "fas fa-map-marked-alt", 10, new[] { "Admin", "Manager", "Employee", "Developer" }),
            ["GALERIA_VISUAL_DOCUMENTACAO"] = new("GALERIA_VISUAL_DOCUMENTACAO.md", "?? Galeria Visual - Documenta��o", "Screenshots e exemplos visuais do sistema", "?? Visual", "fas fa-images", 15, new[] { "Admin", "Manager", "Employee", "Developer" }),
            ["RESUMO_IMPLEMENTACAO"] = new("RESUMO_IMPLEMENTACAO.md", "? Resumo da Implementa��o", "Resumo executivo completo do projeto", "?? �ndice", "fas fa-check-circle", 15, new[] { "Admin", "Manager", "Developer" }),
            ["INDEX_DOCUMENTACAO_SISTEMA"] = new("INDEX_DOCUMENTACAO_SISTEMA.md", "?? �ndice do Sistema de Documenta��o", "�ndice completo dos arquivos criados", "?? �ndice", "fas fa-list", 10, new[] { "Admin", "Manager", "Employee", "Developer" })
        };

        public DocumentacaoService(IWebHostEnvironment env, ILogger<DocumentacaoService> logger)
        {
            _env = env;
            _logger = logger;
            _docsPath = Path.Combine(_env.ContentRootPath, "Docs");
        }

        public ListaDocumentosViewModel ObterListaDocumentos()
        {
            var resultado = new ListaDocumentosViewModel();

            foreach (var (id, metadata) in _documentosMetadata)
            {
                var caminhoCompleto = Path.Combine(_docsPath, metadata.NomeArquivo);
                
                if (File.Exists(caminhoCompleto))
                {
                    var doc = new DocumentoViewModel
                    {
                        Id = id,
                        Titulo = metadata.Titulo,
                        Descricao = metadata.Descricao,
                        Categoria = metadata.Categoria,
                        Icone = metadata.Icone,
                        TempoLeitura = metadata.TempoLeitura,
                        PerfisSugeridos = metadata.PerfisSugeridos.ToList(),
                        UltimaAtualizacao = File.GetLastWriteTime(caminhoCompleto)
                    };

                    resultado.Documentos.Add(doc);

                    if (!resultado.DocumentosPorCategoria.ContainsKey(doc.Categoria))
                    {
                        resultado.DocumentosPorCategoria[doc.Categoria] = new List<DocumentoViewModel>();
                    }
                    resultado.DocumentosPorCategoria[doc.Categoria].Add(doc);
                }
            }

            // Ordenar por categoria e t�tulo
            foreach (var categoria in resultado.DocumentosPorCategoria.Keys.ToList())
            {
                resultado.DocumentosPorCategoria[categoria] = 
                    resultado.DocumentosPorCategoria[categoria]
                        .OrderBy(d => d.Titulo)
                        .ToList();
            }

            return resultado;
        }

        public DocumentoViewModel? ObterDocumento(string id)
        {
            if (!_documentosMetadata.TryGetValue(id.ToUpperInvariant(), out var metadata))
            {
                _logger.LogWarning("Documento n�o encontrado no mapeamento: {DocumentoId}", id);
                return null;
            }

            var caminhoCompleto = Path.Combine(_docsPath, metadata.NomeArquivo);

            if (!File.Exists(caminhoCompleto))
            {
                _logger.LogWarning("Arquivo de documento n�o encontrado: {Caminho}", caminhoCompleto);
                return null;
            }

            try
            {
                var conteudoMarkdown = File.ReadAllText(caminhoCompleto, Encoding.UTF8);
                var conteudoHtml = Markdown.ToHtml(conteudoMarkdown, _markdownPipeline);

                // Processar links internos para apontar para o visualizador
                conteudoHtml = ProcessarLinksInternos(conteudoHtml);

                return new DocumentoViewModel
                {
                    Id = id,
                    Titulo = metadata.Titulo,
                    Descricao = metadata.Descricao,
                    ConteudoMarkdown = conteudoMarkdown,
                    ConteudoHtml = conteudoHtml,
                    Categoria = metadata.Categoria,
                    Icone = metadata.Icone,
                    TempoLeitura = metadata.TempoLeitura,
                    PerfisSugeridos = metadata.PerfisSugeridos.ToList(),
                    UltimaAtualizacao = File.GetLastWriteTime(caminhoCompleto),
                    Tags = ExtrairTags(conteudoMarkdown)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar documento: {DocumentoId}", id);
                return null;
            }
        }

        public List<ResultadoBuscaDocumentacao> BuscarNaDocumentacao(string termo)
        {
            var resultados = new List<ResultadoBuscaDocumentacao>();
            var termoLower = termo.ToLowerInvariant();

            foreach (var (id, metadata) in _documentosMetadata)
            {
                var caminhoCompleto = Path.Combine(_docsPath, metadata.NomeArquivo);
                
                if (!File.Exists(caminhoCompleto))
                    continue;

                try
                {
                    var conteudo = File.ReadAllText(caminhoCompleto, Encoding.UTF8);
                    var conteudoLower = conteudo.ToLowerInvariant();

                    if (conteudoLower.Contains(termoLower))
                    {
                        var relevancia = CalcularRelevancia(conteudo, termo, metadata);
                        var trechoDestacado = ExtrairTrechoDestacado(conteudo, termo);

                        resultados.Add(new ResultadoBuscaDocumentacao
                        {
                            DocumentoId = id,
                            Titulo = metadata.Titulo,
                            TrechoDestacado = trechoDestacado,
                            Categoria = metadata.Categoria,
                            Relevancia = relevancia
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao buscar no documento: {DocumentoId}", id);
                }
            }

            return resultados.OrderByDescending(r => r.Relevancia).ToList();
        }

        public (byte[]? conteudo, string nomeArquivo) ObterArquivoParaDownload(string id)
        {
            if (!_documentosMetadata.TryGetValue(id.ToUpperInvariant(), out var metadata))
            {
                return (null, string.Empty);
            }

            var caminhoCompleto = Path.Combine(_docsPath, metadata.NomeArquivo);

            if (!File.Exists(caminhoCompleto))
            {
                return (null, string.Empty);
            }

            try
            {
                var conteudo = File.ReadAllBytes(caminhoCompleto);
                return (conteudo, metadata.NomeArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao preparar arquivo para download: {DocumentoId}", id);
                return (null, string.Empty);
            }
        }

        public List<DocumentoViewModel> ObterDocumentosPorPerfil(string perfil)
        {
            var documentos = new List<DocumentoViewModel>();

            foreach (var (id, metadata) in _documentosMetadata)
            {
                if (metadata.PerfisSugeridos.Contains(perfil, StringComparer.OrdinalIgnoreCase))
                {
                    var doc = ObterDocumento(id);
                    if (doc != null)
                    {
                        documentos.Add(doc);
                    }
                }
            }

            return documentos.OrderBy(d => d.Categoria).ThenBy(d => d.Titulo).ToList();
        }

        private string ProcessarLinksInternos(string html)
        {
            // Converter links .md para links do visualizador
            var pattern = @"href=""([^""]+\.md)(#[^""]*)?""";
            return Regex.Replace(html, pattern, match =>
            {
                var arquivo = match.Groups[1].Value;
                var anchor = match.Groups[2].Value;
                var id = Path.GetFileNameWithoutExtension(arquivo).ToUpperInvariant();
                
                if (_documentosMetadata.ContainsKey(id))
                {
                    return $"href=\"/Documentacao/Visualizar/{id}{anchor}\"";
                }
                
                return match.Value;
            });
        }

        private List<string> ExtrairTags(string conteudo)
        {
            var tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            
            // Extrair palavras-chave comuns
            var palavrasChave = new[] { 
                "login", "senha", "permiss�o", "cliente", "ve�culo", "loca��o", 
                "manuten��o", "reserva", "pacote", "relat�rio", "upload", "documento" 
            };

            foreach (var palavra in palavrasChave)
            {
                if (conteudo.Contains(palavra, StringComparison.OrdinalIgnoreCase))
                {
                    tags.Add(palavra);
                }
            }

            return tags.Take(10).ToList();
        }

        private int CalcularRelevancia(string conteudo, string termo, DocumentoMetadata metadata)
        {
            var relevancia = 0;
            var termoLower = termo.ToLowerInvariant();

            // Busca no t�tulo (peso maior)
            if (metadata.Titulo.Contains(termo, StringComparison.OrdinalIgnoreCase))
                relevancia += 100;

            // Busca na descri��o
            if (metadata.Descricao.Contains(termo, StringComparison.OrdinalIgnoreCase))
                relevancia += 50;

            // Contar ocorr�ncias no conte�do
            var ocorrencias = Regex.Matches(conteudo, Regex.Escape(termoLower), RegexOptions.IgnoreCase).Count;
            relevancia += Math.Min(ocorrencias * 5, 100);

            return relevancia;
        }

        private string ExtrairTrechoDestacado(string conteudo, string termo)
        {
            var index = conteudo.IndexOf(termo, StringComparison.OrdinalIgnoreCase);
            
            if (index == -1)
                return conteudo.Length > 150 ? conteudo.Substring(0, 150) + "..." : conteudo;

            var inicio = Math.Max(0, index - 75);
            var tamanho = Math.Min(150, conteudo.Length - inicio);
            var trecho = conteudo.Substring(inicio, tamanho);

            if (inicio > 0)
                trecho = "..." + trecho;
            if (inicio + tamanho < conteudo.Length)
                trecho += "...";

            // Destacar o termo encontrado
            trecho = Regex.Replace(
                trecho, 
                Regex.Escape(termo), 
                $"<mark class=\"bg-warning\">{termo}</mark>", 
                RegexOptions.IgnoreCase
            );

            return trecho;
        }

        private record DocumentoMetadata(
            string NomeArquivo,
            string Titulo,
            string Descricao,
            string Categoria,
            string Icone,
            int TempoLeitura,
            string[] PerfisSugeridos
        );
    }
}
