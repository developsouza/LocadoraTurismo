# ?? Correção de Emojis nos Documentos

## ? Problema

Os emojis nos documentos Markdown estão aparecendo como "??" ou caracteres quebrados no visualizador.

## ?? Causa

Os arquivos `.md` foram salvos com encoding incorreto (não UTF-8 ou UTF-8 sem BOM).

## ? Soluções

### Solução 1: Recriar Arquivos com Emojis Corretos (Recomendado)

Os emojis originais foram:

| Corrompido | Emoji Correto | Código |
|------------|---------------|--------|
| ?? | ?? | `:rocket:` |
| ?? | ?? | `:books:` |
| ?? | ?? | `:book:` |
| ?? | ?? | `:closed_lock_with_key:` |
| ?? | ?? | `:busts_in_silhouette:` |
| ?? | ?? | `:car:` |
| ?? | ?? | `:clipboard:` |
| ?? | ?? | `:wrench:` |
| ?? | ?? | `:airplane:` |
| ?? | ?? | `:paperclip:` |
| ?? | ?? | `:computer:` |
| ?? | ?? | `:art:` |
| ?? | ?? | `:gear:` |
| ?? | ? | `:white_check_mark:` |
| ?? | ? | `:x:` |
| ?? | ?? | `:warning:` |
| ?? | ?? | `:bulb:` |
| ?? | ?? | `:bar_chart:` |
| ?? | ?? | `:trophy:` |
| ?? | ????? | `:man_office_worker:` |
| ?? | ?? | `:necktie:` |
| ???? | ????? | `:man_technologist:` |

### Solução 2: Script PowerShell de Correção Automática

Execute este comando no PowerShell (como Administrador):

```powershell
# Navegar até a pasta do projeto
cd C:\Sistemas\LocadoraTurismo\RentalTourismSystem

# Executar script de correção
.\CorrigirEmojis.ps1
```

### Solução 3: Correção Manual com Visual Studio Code

1. Abra o arquivo `.md` no VS Code
2. No canto inferior direito, clique no encoding atual
3. Selecione "Save with Encoding"
4. Escolha "UTF-8 with BOM"
5. Salve o arquivo
6. Repita para cada arquivo

### Solução 4: Usar HTML Entities

Se os emojis não funcionarem, use HTML entities:

```markdown
# &#128640; Título com emoji (foguete)
```

Códigos úteis:
- ?? = `&#128640;`
- ?? = `&#128218;`
- ? = `&#9989;`
- ? = `&#10060;`

## ?? Script de Correção Completa

Crie um arquivo `CorrigirEmojis.ps1`:

```powershell
# Mapeamento de emojis corrompidos
$emojiMap = @{
    '??' = '??'  # rocket
    # Adicione mais mapeamentos conforme necessário
}

$docsPath = ".\Docs"

Get-ChildItem -Path $docsPath -Filter "*.md" | ForEach-Object {
    $content = Get-Content $_.FullName -Raw -Encoding UTF8
    
    # Substituir emojis corrompidos
    foreach ($key in $emojiMap.Keys) {
        $content = $content -replace [regex]::Escape($key), $emojiMap[$key]
    }
    
    # Salvar com UTF-8 BOM
    $utf8WithBom = New-Object System.Text.UTF8Encoding $true
    [System.IO.File]::WriteAllText($_.FullName, $content, $utf8WithBom)
    
    Write-Host "Corrigido: $($_.Name)" -ForegroundColor Green
}
```

## ?? Garantir UTF-8 no Navegador

O visualizador já inclui estas meta tags:

```html
<meta charset="UTF-8">
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
```

## ? Verificação

Para verificar se foi corrigido:

1. Acesse `/Documentacao` no navegador
2. Abra qualquer documento
3. Verifique se os emojis aparecem corretamente
4. Se ainda aparecer "??", tente as soluções acima

## ?? Prevenção Futura

Ao criar novos documentos:

1. **VS Code**: Sempre salvar como "UTF-8 with BOM"
2. **Notepad++**: Encoding > UTF-8-BOM
3. **PowerShell**: Use sempre `New-Object System.Text.UTF8Encoding $true`

## ?? Ainda com Problemas?

Se os emojis ainda não aparecerem:

1. **Limpe o cache do navegador**: Ctrl + Shift + Delete
2. **Teste em navegador diferente**: Chrome, Edge, Firefox
3. **Verifique a fonte do sistema**: Alguns emojis precisam de fontes específicas
4. **Use alternativas**: Substitua emojis por ícones Font Awesome

---

**Última Atualização**: Janeiro 2025
