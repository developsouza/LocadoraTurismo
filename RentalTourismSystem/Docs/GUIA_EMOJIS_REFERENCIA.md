# ?? Guia de Emojis - Referência de Substituição

## ?? Problema Identificado

Os emojis nos arquivos Markdown foram corrompidos e aparecem como "??" no navegador.

## ? Solução: Mapeamento de Emojis

### ?? Emojis por Contexto

Baseando-se no contexto dos documentos, aqui está o mapeamento dos emojis que deveriam ser usados:

#### ?? Documentação e Guias
- **??** antes de "Guia" = ?? (`:books:`)
- **??** antes de "Documentação" = ?? (`:books:`)
- **??** antes de "README" = ?? (`:book:`)
- **??** antes de "Índice" = ?? (`:clipboard:`)

#### ?? Início Rápido
- **??** antes de "Início Rápido" = ?? (`:rocket:`)
- **??** antes de "Guia de Início Rápido" = ?? (`:rocket:`)
- **?** antes de "15 Minutos" = ? (`:zap:`)

#### ?? Segurança
- **??** antes de "Autenticação" = ?? (`:closed_lock_with_key:`)
- **??** antes de "Segurança" = ?? (`:lock:`)

#### ?? Gestão
- **??** antes de "Gestão de Clientes" = ?? (`:busts_in_silhouette:`)
- **??** antes de "Gestão de Veículos" = ?? (`:car:`)
- **??** antes de "Locações" = ?? (`:clipboard:`)
- **??** antes de "Manutenções" = ?? (`:wrench:`)
- **??** antes de "Reservas" = ?? (`:airplane:`)
- **??** antes de "Upload" = ?? (`:paperclip:`)

#### ?? Técnico
- **??** antes de "Referência Técnica" = ?? (`:computer:`)
- **???** antes de "Arquitetura" = ??? (`:building_construction:`)
- **??** antes de "APIs" = ?? (`:electric_plug:`)

#### ?? Visual
- **??** antes de "Fluxogramas" = ?? (`:art:`)
- **???** antes de "Diagrama" = ?? (`:bar_chart:`)

#### ? Status e Ações
- **?** antes de "Pronto" = ? (`:white_check_mark:`)
- **?** antes de "Login" = ? (`:white_check_mark:`)
- **?** antes de "Sistema" = ? (`:white_check_mark:`)
- **?** antes de "Banco" = ? (`:white_check_mark:`)
- **?** antes de "Você" = ? (`:white_check_mark:`)

#### ?? Interface
- **??** antes de "Interface" = ?? (`:iphone:`)
- **??** antes de "Mobile" = ?? (`:iphone:`)
- **??** antes de "Menu" = ?? (`:dart:`)

#### ?? Configuração
- **??** antes de "Configurar" = ?? (`:gear:`)
- **??** antes de "Passo" = ?? (`:1234:`)

#### ?? Níveis e Perfis
- **?????** antes de "ADMINISTRADOR" = ????? (`:man_office_worker:`)
- **??** antes de "GERENTE" = ?? (`:necktie:`)
- **?????** antes de "FUNCIONÁRIO" = ?? (`:bust_in_silhouette:`)
- **??** antes de "DESENVOLVEDOR" = ????? (`:man_technologist:`)

#### ?? Estatísticas
- **??** antes de "Estatísticas" = ?? (`:bar_chart:`)
- **??** antes de "Métricas" = ?? (`:chart_with_upwards_trend:`)

#### ?? Suporte
- **??** antes de "Suporte" = ?? (`:telephone_receiver:`)
- **??** antes de "Contato" = ?? (`:telephone_receiver:`)
- **??** antes de "Email" = ?? (`:email:`)
- **??** antes de "WhatsApp" = ?? (`:speech_balloon:`)

#### ?? Conclusão
- **??** antes de "Conclusão" = ?? (`:dart:`)
- **??** antes de "Próximos Passos" = ?? (`:rocket:`)

### ?? Como Corrigir Manualmente

1. Abra cada arquivo `.md`
2. Procure por "??" ou "?"
3. Veja o contexto (palavra após o emoji)
4. Substitua pelo emoji correto da lista acima
5. Salve como UTF-8 with BOM

### ?? Alternativa: Usar Ícones Font Awesome

Se os emojis não funcionarem, você pode usar ícones HTML:

```html
<!-- Em vez de emojis, use Font Awesome -->
<i class="fas fa-rocket"></i> Guia de Início Rápido
<i class="fas fa-book"></i> Documentação
<i class="fas fa-lock"></i> Segurança
<i class="fas fa-users"></i> Gestão de Clientes
<i class="fas fa-car"></i> Gestão de Veículos
```

Esses ícones já estão incluídos no sistema via CDN.

## ?? Lista Completa de Substituições Sugeridas

### Para copiar e colar no editor:

```
?? Documentação ? ?? Documentação
?? Guia ? ?? Guia
?? README ? ?? README
?? Índice ? ?? Índice
?? Início Rápido ? ?? Início Rápido
? 15 Minutos ? ? 15 Minutos
?? Autenticação ? ?? Autenticação
?? Segurança ? ?? Segurança
?? Gestão de Clientes ? ?? Gestão de Clientes
?? Gestão de Veículos ? ?? Gestão de Veículos
?? Locações ? ?? Locações
?? Manutenções ? ?? Manutenções
?? Reservas ? ?? Reservas
?? Upload ? ?? Upload
?? Referência Técnica ? ?? Referência Técnica
?? Fluxogramas ? ?? Fluxogramas
? Pronto ? ? Pronto
?? Próximos Passos ? ?? Próximos Passos
```

## ?? Preview de Emojis no Sistema

O visualizador de documentação do sistema já está configurado para UTF-8.
Após corrigir os arquivos, eles aparecerão corretamente.

## ? Status Atual

- ? Acentos corrigidos (ç, ã, é, etc.)
- ?? Emojis precisam ser manualmente substituídos
- ? Encoding UTF-8 com BOM aplicado
- ? Meta tags UTF-8 adicionadas ao visualizador

---

**Última Atualização**: Janeiro 2025  
**Status**: Emojis necessitam correção manual
