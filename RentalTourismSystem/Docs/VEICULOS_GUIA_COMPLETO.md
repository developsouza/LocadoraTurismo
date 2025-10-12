# ?? Sistema de Gest�o de Ve�culos - Guia Completo

## ?? �ndice
- [Vis�o Geral](#vis�o-geral)
- [Cadastro de Ve�culos](#cadastro-de-ve�culos)
- [Gerenciamento de Status](#gerenciamento-de-status)
- [Manuten��es](#manuten��es)
- [Disponibilidade](#disponibilidade)
- [Documentos do Ve�culo](#documentos-do-ve�culo)
- [Relat�rios](#relat�rios)
- [Permiss�es](#permiss�es)

---

## ?? Vis�o Geral

O m�dulo de gest�o de ve�culos permite controle completo da frota, desde o cadastro at� o acompanhamento de manuten��es, loca��es e disponibilidade.

### ?? Funcionalidades Principais

? **Gest�o Completa da Frota**
- Cadastro detalhado de ve�culos
- Controle de status (Dispon�vel, Locado, Manuten��o, etc.)
- Hist�rico de loca��es
- Hist�rico de manuten��es
- C�lculo de disponibilidade

? **Controle Operacional**
- Quilometragem atual
- �ltima manuten��o
- Pr�xima manuten��o prevista
- Alertas de manuten��o preventiva
- Gest�o de documentos (CRLV, Seguro, etc.)

? **Integra��o**
- Sistema de loca��es
- Sistema de manuten��es
- Sistema de relat�rios
- Upload de documentos

---

## ?? Cadastro de Ve�culos

### ?? Como Acessar
**Menu Lateral ? Loca��o ? Ve�culos ? ? Novo Ve�culo**

OU

**URL direta:** `/Veiculos/Create`

### ?? Campos do Formul�rio

#### 1?? **Informa��es B�sicas**

| Campo | Formato | Valida��o | Exemplo |
|-------|---------|-----------|---------|
| **Marca** | Texto (m�x. 50 caracteres) | Obrigat�rio | Volkswagen |
| **Modelo** | Texto (m�x. 50 caracteres) | Obrigat�rio | Gol 1.0 Flex |
| **Ano** | N�mero | Obrigat�rio (1990-2030) | 2023 |
| **Placa** | XXX-0000 ou XXX0X00 | Obrigat�rio, formato Mercosul ou antigo | ABC-1234 ou ABC1D23 |
| **Cor** | Texto (m�x. 50 caracteres) | Obrigat�rio | Branco |

#### 2?? **Especifica��es T�cnicas**

| Campo | Formato | Valida��o | Exemplo |
|-------|---------|-----------|---------|
| **Combust�vel** | Sele��o | Obrigat�rio | Gasolina, �lcool, Flex, Diesel, El�trico, H�brido |
| **C�mbio** | Sele��o | Obrigat�rio | Manual, Autom�tico, Automatizado, CVT |
| **Quilometragem** | N�mero inteiro | Obrigat�rio (? 0) | 45.000 |

#### 3?? **Valores**

| Campo | Formato | Valida��o | Exemplo |
|-------|---------|-----------|---------|
| **Valor da Di�ria** | Decimal (R$) | Obrigat�rio | R$ 150,00 |
| **Valor de Mercado** | Decimal (R$) | Opcional | R$ 55.000,00 |

#### 4?? **Localiza��o e Status**

| Campo | Formato | Valida��o | Exemplo |
|-------|---------|-----------|---------|
| **Ag�ncia** | Sele��o | Obrigat�rio | Ag�ncia Centro |
| **Status** | Sele��o | Obrigat�rio | Dispon�vel |

### ?? Status do Ve�culo

| Status | Descri��o | Cor | Permite Loca��o? |
|--------|-----------|-----|------------------|
| **Dispon�vel** | Ve�culo pronto para loca��o | ?? Verde | ? Sim |
| **Locado** | Ve�culo em loca��o ativa | ?? Azul | ? N�o |
| **Manuten��o** | Ve�culo em manuten��o | ?? Amarelo | ? N�o |
| **Reservado** | Ve�culo reservado para loca��o futura | ?? Roxo | ? N�o |
| **Inativo** | Ve�culo fora de opera��o | ?? Vermelho | ? N�o |

### ?? Passo a Passo - Cadastro

1. **Acesse o formul�rio**
   - Menu ? Ve�culos ? Novo Ve�culo

2. **Preencha as informa��es b�sicas**
   ```
   Marca: Volkswagen
   Modelo: Gol 1.0 Flex
   Ano: 2023
   Placa: ABC1D23
   Cor: Branco
   ```

3. **Especifica��es t�cnicas**
   ```
   Combust�vel: Flex
   C�mbio: Manual
   Quilometragem: 5.000 km
   ```

4. **Valores**
   ```
   Valor da Di�ria: R$ 150,00
   Valor de Mercado: R$ 55.000,00
   ```

5. **Localiza��o**
   ```
   Ag�ncia: Ag�ncia Centro
   Status: Dispon�vel
   ```

6. **Clique em "Salvar"**
   - Sistema valida os dados
   - Ve�culo � cadastrado
   - Redireciona para lista de ve�culos

---

## ?? Gerenciamento de Status

### ?? Como Alterar Status

**Op��o 1 - Detalhes do Ve�culo:**
```
Ve�culos ? Detalhes ? Painel "A��es R�pidas" ? ?? Alterar Status
```

**Op��o 2 - Lista de Ve�culos:**
```
Ve�culos ? Bot�o de a��o ? Alterar Status
```

### ?? Quando Alterar Status

#### ?? **Para DISPON�VEL**
**Quando:**
- Ve�culo retorna de loca��o
- Manuten��o foi conclu�da
- Ve�culo foi reativado

**Pr�-requisitos:**
- ? N�o estar em loca��o ativa
- ? Manuten��o conclu�da (se aplic�vel)
- ? Vistoria aprovada

#### ?? **Para LOCADO**
**Quando:**
- Cliente retira ve�culo para loca��o

**Pr�-requisitos:**
- ? Status anterior: Dispon�vel ou Reservado
- ? Loca��o criada no sistema
- ? Cliente com CNH v�lida

> ?? **Autom�tico:** Sistema altera para "Locado" automaticamente ao criar loca��o!

#### ?? **Para MANUTEN��O**
**Quando:**
- Ve�culo precisa de reparo
- Revis�o preventiva agendada
- Problema identificado

**Pr�-requisitos:**
- ? N�o estar em loca��o
- ? Criar registro de manuten��o

#### ?? **Para RESERVADO**
**Quando:**
- Cliente faz reserva antecipada
- Ve�culo ser� usado em evento espec�fico

**Pr�-requisitos:**
- ? Status anterior: Dispon�vel
- ? Per�odo definido

#### ?? **Para INATIVO**
**Quando:**
- Ve�culo vendido
- Ve�culo em sinistro grave
- Aguardando decis�o sobre destino

**Pr�-requisitos:**
- ? N�o estar em loca��o
- ? Sem manuten��es pendentes

### ?? Fluxo de Status

```mermaid
graph LR
    A[Dispon�vel] --> B[Locado]
    B --> A
    A --> C[Manuten��o]
    C --> A
    A --> D[Reservado]
    D --> B
    A --> E[Inativo]
    E --> A
    C --> E
```

### ?? Mudan�as Autom�ticas de Status

| Evento | Status Anterior | Status Novo | Autom�tico? |
|--------|-----------------|-------------|-------------|
| Criar loca��o | Dispon�vel | Locado | ? Sim |
| Finalizar loca��o | Locado | Dispon�vel | ?? Manual recomendado |
| Criar manuten��o | Qualquer | Manuten��o | ? Sim (opcional) |
| Concluir manuten��o | Manuten��o | Dispon�vel | ?? Manual |

---

## ?? Manuten��es

### ?? Acesso ao Sistema de Manuten��es

**Op��o 1 - Menu Lateral:**
```
Menu ? Loca��o ? Manuten��es
```

**Op��o 2 - Lista de Ve�culos:**
```
Ve�culos ? Bot�o "?? Manuten��es" (Admin/Manager)
```

**Op��o 3 - Detalhes do Ve�culo:**
```
Ve�culos ? Detalhes ? Painel "A��es R�pidas"
? ?? Hist�rico de Manuten��es
? ?? Nova Manuten��o
```

### ?? Informa��es de Manuten��o

#### **No Card do Ve�culo:**
```
?? Manuten��es:
   - Total: 8 manuten��es
   - �ltima: Troca de �leo (h� 15 dias)
   - Pr�xima: Revis�o 10.000km (prevista)
   - Custo Total: R$ 4.850,00
   - Custo M�dio: R$ 606,25
```

#### **Hist�rico Completo:**
- Lista todas as manuten��es
- Tipo, data, custo
- Quilometragem na manuten��o
- Status (Agendada, Conclu�da, etc.)
- Oficina e respons�vel
- Observa��es e garantia

### ?? Tipos de Manuten��o

1. **Preventiva** ???
   - Revis�es programadas
   - Troca de �leo peri�dica
   - Alinhamento/balanceamento
   - Inspe��o veicular

2. **Corretiva** ??
   - Problemas identificados
   - Reparos necess�rios
   - Substitui��o de pe�as

3. **Urgente** ??
   - Problemas cr�ticos
   - Seguran�a comprometida
   - Ve�culo parado

### ?? Alertas de Manuten��o

**Sistema alerta quando:**
- ?? Ve�culo atingiu quilometragem de revis�o
- ?? �ltima manuten��o h� mais de X dias
- ?? Manuten��o urgente pendente
- ?? Manuten��o agendada se aproximando

### ?? Documenta��o Completa

Para informa��es detalhadas sobre manuten��es:
?? **[MANUTENCAO_GUIA_ACESSO.md](MANUTENCAO_GUIA_ACESSO.md)**

---

## ?? Disponibilidade

### ?? Verificar Disponibilidade

**Ve�culos ? Detalhes ? Bot�o "?? Verificar Disponibilidade"**

### ?? Consulta de Per�odo

**Formul�rio de verifica��o:**
```
Data Inicial: 01/12/2024
Data Final: 05/12/2024

[Verificar Disponibilidade]
```

**Resultado:**
```
? Ve�culo DISPON�VEL no per�odo selecionado
   01/12 a 05/12/2024

   Sem loca��es agendadas
   Sem manuten��es programadas
```

OU

```
? Ve�culo INDISPON�VEL no per�odo

   Motivo: Loca��o ativa
   Cliente: Jo�o Silva
   Per�odo: 30/11 a 10/12/2024
   
   Alternativa: Buscar outro ve�culo similar
```

### ?? Calend�rio de Ocupa��o

**Informa��es exibidas:**
- ?? Dias dispon�veis
- ?? Dias locados
- ?? Dias em manuten��o
- ?? Dias reservados
- ? Dias inativos

### ?? Regras de Disponibilidade

**Ve�culo est� dispon�vel quando:**
- ? Status = "Dispon�vel"
- ? Sem loca��o no per�odo
- ? Sem manuten��o agendada
- ? Sem reserva confirmada

**Ve�culo N�O est� dispon�vel quando:**
- ? Em loca��o ativa
- ? Em manuten��o
- ? Status = Inativo
- ? Reservado para outro cliente

---

## ?? Documentos do Ve�culo

### ?? Como Acessar
**Ve�culos ? Detalhes ? Bot�o "?? Documentos"**

OU

**URL direta:** `/DocumentosUpload/UploadVeiculo/{id}`

### ?? Tipos de Documentos

| Tipo | Descri��o | Renova��o |
|------|-----------|-----------|
| **CRLV** | Certificado de Registro e Licenciamento | Anual |
| **Nota Fiscal** | Nota fiscal de compra do ve�culo | �nica |
| **Ap�lice de Seguro** | Documento do seguro | Anual |
| **IPVA** | Comprovante de pagamento do IPVA | Anual |
| **Fotos do Ve�culo** | Fotos externas e internas | Conforme necess�rio |
| **Outros** | Documentos diversos | Vari�vel |

### ?? Upload de Documentos

1. **Acesse �rea de documentos**
2. **Selecione tipo de documento**
   - CRLV, Seguro, IPVA, Fotos, etc.
3. **Escolha o arquivo**
   - PDF ou Imagem
   - M�ximo 10MB
4. **Adicione descri��o**
   - Exemplo: "CRLV 2024", "Seguro renovado at� 12/2025"
5. **Envie o documento**

### ?? Alertas de Documenta��o

**Sistema alerta quando:**
- ?? CRLV vencido
- ?? Seguro vencido
- ?? IPVA a vencer (30 dias)
- ?? Documentos faltantes

### ?? Checklist de Documenta��o

**Documentos obrigat�rios:**
- ? CRLV v�lido
- ? Seguro em dia
- ? IPVA quitado
- ? Fotos atualizadas (opcional)

### ?? Documenta��o Completa

Para informa��es detalhadas sobre upload de documentos:
?? **[UPLOAD_DOCUMENTOS.md](UPLOAD_DOCUMENTOS.md)**

---

## ?? Relat�rios

### ?? Como Acessar
**Menu ? Relat�rios ? Ve�culos**

### ?? Tipos de Relat�rios

#### 1?? **Ve�culos Mais Alugados**
```
Ranking de ve�culos por quantidade de loca��es

Top 5:
1. Gol 1.0 - ABC1234 (45 loca��es)
2. HB20 1.0 - DEF5678 (38 loca��es)
3. Onix 1.0 - GHI9012 (32 loca��es)
...
```

**Filtros:**
- Per�odo (data in�cio/fim)
- Ag�ncia
- Status

#### 2?? **Receita por Ve�culo**
```
Ve�culo          | Loca��es | Dias | Receita Total | Receita M�dia/Dia
Gol ABC1234      | 12       | 156  | R$ 23.400,00  | R$ 150,00
HB20 DEF5678     | 8        | 104  | R$ 15.600,00  | R$ 150,00
...
```

#### 3?? **Status da Frota**
```
?? Status atual da frota:
   
   ?? Dispon�vel: 15 ve�culos (60%)
   ?? Locado: 7 ve�culos (28%)
   ?? Manuten��o: 2 ve�culos (8%)
   ?? Reservado: 1 ve�culo (4%)
   ?? Inativo: 0 ve�culos (0%)
   
   Total: 25 ve�culos
```

#### 4?? **Custos de Manuten��o**
```
Ve�culo          | Manuten��es | Custo Total | Custo M�dio
Gol ABC1234      | 8           | R$ 4.850,00 | R$ 606,25
HB20 DEF5678     | 5           | R$ 3.200,00 | R$ 640,00
...
```

**Ver relat�rio completo:**
?? **Manuten��es ? Relat�rio de Custos**

#### 5?? **Quilometragem**
```
Ve�culo          | KM Atual | KM Inicial | KM Rodados | M�dia KM/Dia
Gol ABC1234      | 45.000   | 5.000      | 40.000     | 150 km
HB20 DEF5678     | 35.000   | 10.000     | 25.000     | 120 km
...
```

### ?? Filtros Dispon�veis

**Todos os relat�rios permitem filtrar por:**
- ?? Per�odo (data in�cio/fim)
- ?? Ag�ncia espec�fica
- ?? Ve�culo espec�fico
- ?? Status do ve�culo
- ??? Tipo de combust�vel
- ?? Tipo de c�mbio

### ?? Exporta��o

**Formatos dispon�veis:**
- ?? Excel (.xlsx)
- ?? PDF
- ?? CSV
- ??? Impress�o direta

---

## ?? Permiss�es de Acesso

### ??? **Visualiza��o**
**Quem pode:** Todos os usu�rios autenticados
- Ver lista de ve�culos
- Ver detalhes do ve�culo
- Consultar disponibilidade
- Ver hist�rico de loca��es
- Ver documentos

### ?? **Cria��o e Edi��o**
**Quem pode:** Admin, Manager
- Cadastrar novos ve�culos
- Editar informa��es
- Alterar status
- Fazer upload de documentos
- Criar manuten��es

### ??? **Exclus�o**
**Quem pode:** Apenas Admin
- Excluir ve�culos SEM hist�rico
- Excluir documentos

> ?? **Importante:** Ve�culos com loca��es ou manuten��es N�O podem ser exclu�dos!

### ?? Regras de Neg�cio

#### **N�o � poss�vel excluir ve�culo se:**
1. Possui loca��es cadastradas
2. Possui manuten��es registradas
3. Est� em loca��o ativa
4. Possui documentos anexados

#### **Para excluir um ve�culo:**
1. Verificar aus�ncia de vincula��es
2. Excluir documentos
3. Confirmar exclus�o

---

## ? Boas Pr�ticas

### ?? Cadastro

? **Fa�a:**
- Cadastrar ve�culos com todas as informa��es
- Validar placa (Mercosul ou antiga)
- Definir valor de di�ria competitivo
- Atribuir � ag�ncia correta
- Fazer upload de fotos e documentos

? **Evite:**
- Deixar campos importantes em branco
- Usar placas inv�lidas
- Cadastrar sem documenta��o
- Esquecer de definir ag�ncia

### ?? Status

? **Fa�a:**
- Atualizar status ao iniciar loca��o
- Marcar "Manuten��o" quando necess�rio
- Retornar para "Dispon�vel" ap�s revis�o
- Usar "Reservado" para compromissos futuros

? **Evite:**
- Deixar status desatualizado
- Locar ve�culo em manuten��o
- Esquecer de marcar manuten��es

### ?? Manuten��o

? **Fa�a:**
- Registrar TODAS as manuten��es
- Programar manuten��es preventivas
- Acompanhar quilometragem
- Atualizar custos reais
- Manter hist�rico completo

? **Evite:**
- Adiar manuten��es preventivas
- N�o registrar reparos
- Ignorar alertas do sistema
- Rodar al�m da quilometragem recomendada

### ?? Documenta��o

? **Fa�a:**
- Manter CRLV atualizado
- Renovar seguro antes do vencimento
- Pagar IPVA em dia
- Upload de documentos obrigat�rios
- Fotos de todos os �ngulos

? **Evite:**
- Operar com documentos vencidos
- Atrasar renova��es
- Falta de comprovantes
- Documenta��o incompleta

---

## ?? Casos de Uso Comuns

### Caso 1: Cadastrar Novo Ve�culo

**Cen�rio:** Locadora comprou ve�culo novo

```
1. Menu ? Ve�culos ? Novo Ve�culo
2. Preencher dados:
   - Marca: Volkswagen
   - Modelo: Polo 1.0 TSI
   - Ano: 2024
   - Placa: XYZ1A23
   - Cor: Prata
   - Combust�vel: Gasolina
   - C�mbio: Autom�tico
   - KM: 50 (zero km)
   - Di�ria: R$ 180,00
   - Valor Mercado: R$ 75.000,00
   - Ag�ncia: Centro
   - Status: Dispon�vel

3. Salvar ve�culo

4. Fazer upload de documentos:
   - Nota Fiscal de compra
   - CRLV
   - Seguro
   - Fotos do ve�culo

5. Ve�culo pronto para loca��o!
```

### Caso 2: Ve�culo em Manuten��o

**Cen�rio:** Ve�culo apresentou problema

```
1. Detalhes do Ve�culo
2. Alterar Status ? Manuten��o
3. Nova Manuten��o:
   - Tipo: Freios
   - Status: Em Andamento
   - Data: Hoje
   - KM Atual: 45.000
   - Descri��o: "Troca de pastilhas e discos"
   - Custo estimado: R$ 850,00
   - Oficina: Auto Center Silva

4. Aguardar conclus�o
5. Atualizar manuten��o ? Conclu�da
6. Alterar Status ? Dispon�vel
7. Ve�culo liberado para loca��o
```

### Caso 3: Verificar Disponibilidade

**Cen�rio:** Cliente quer alugar para final de semana

```
1. Buscar ve�culo desejado
2. Detalhes ? Verificar Disponibilidade
3. Informar per�odo:
   - Data Inicial: 15/12/2024
   - Data Final: 17/12/2024
4. Verificar

Resultado:
? Dispon�vel
   ? Criar loca��o

OU

? Indispon�vel (j� locado)
   ? Sugerir ve�culo similar
   ? Buscar outra data
```

### Caso 4: Atualizar Quilometragem

**Cen�rio:** Ve�culo retornou de loca��o

```
1. Detalhes da Loca��o
2. Registrar devolu��o
3. Informar KM na devolu��o: 47.500
4. Sistema atualiza KM do ve�culo automaticamente

5. Verificar se atingiu KM de manuten��o:
   Se SIM ? Programar revis�o
   Se N�O ? Marcar dispon�vel
```

---

## ?? Solu��o de Problemas

### ? Erro: "Placa j� cadastrada"
**Causa:** J� existe ve�culo com esta placa
**Solu��o:** 
- Buscar ve�culo existente
- Verificar se � duplicata
- Corrigir placa se incorreta

### ? Erro: "N�o pode alterar status"
**Causa:** Ve�culo em loca��o ativa
**Solu��o:**
- Finalizar loca��o primeiro
- Verificar se cliente j� devolveu
- Registrar devolu��o

### ? Erro: "N�o � poss�vel excluir"
**Causa:** Ve�culo possui hist�rico de loca��es
**Solu��o:**
- Ve�culos com hist�rico n�o podem ser exclu�dos
- Use status "Inativo" ao inv�s de excluir
- Mantenha registro hist�rico

### ? Erro: "Ve�culo indispon�vel para loca��o"
**Causa:** Status diferente de "Dispon�vel"
**Solu��o:**
- Verificar status atual
- Se em manuten��o, aguardar conclus�o
- Se locado, aguardar devolu��o
- Alterar status se apropriado

---

## ?? Atalhos e Dicas

### ?? Atalhos de Teclado
```
Ctrl + K     ? Busca global (buscar ve�culo)
Ctrl + N     ? Novo ve�culo (em breve)
Ctrl + S     ? Salvar (nos formul�rios)
Esc          ? Cancelar/Fechar modal
```

### ?? Links R�pidos
```
/Veiculos/Index                    ? Lista de ve�culos
/Veiculos/Create                   ? Novo ve�culo
/Veiculos/Details/{id}             ? Detalhes
/Manutencoes/HistoricoVeiculo/{id} ? Hist�rico manuten��es
```

### ?? Dicas Profissionais

1. **Organize por Ag�ncia**
   - Mantenha ve�culos na ag�ncia correta
   - Facilita controle e transfer�ncias

2. **Manuten��o Preventiva**
   - Siga o manual do fabricante
   - Previne problemas maiores
   - Reduz custos a longo prazo

3. **Documenta��o em Dia**
   - Evita multas e problemas legais
   - CRLV, Seguro e IPVA sempre v�lidos

4. **Fotos Atualizadas**
   - Facilita identifica��o
   - Importante para vistorias
   - Documenta estado do ve�culo

5. **Controle de Quilometragem**
   - Acompanhe KM rodados
   - Programe manuten��es baseadas em KM
   - Calcule custos por quil�metro

---

## ?? Perguntas Frequentes

**P: Posso alterar a placa de um ve�culo?**
R: Sim, mas apenas Admin. Certifique-se que � uma corre��o v�lida.

**P: Como transfiro ve�culo entre ag�ncias?**
R: Edite o ve�culo e altere a ag�ncia. Sistema registra a transfer�ncia.

**P: Posso excluir ve�culo com loca��es antigas?**
R: N�o. Use status "Inativo" ao inv�s de excluir.

**P: Como sei quando fazer manuten��o?**
R: Sistema alerta baseado em quilometragem e �ltima manuten��o.

**P: Preciso cadastrar fotos do ve�culo?**
R: Opcional, mas altamente recomendado para vistorias.

---

## ?? Pronto para Usar!

O sistema de gest�o de ve�culos est� **100% operacional**.

**Pr�ximos passos:**
1. Cadastre sua frota
2. Configure status adequados
3. Fa�a upload de documentos
4. Programe manuten��es preventivas
5. Inicie as loca��es!

**Acesse:** Menu ? Loca��o ? Ve�culos

---

**Desenvolvido para:** Litoral Sul Locadora e Turismo  
**Vers�o:** 1.0  
**Data:** Outubro/2025  
**Documenta��o relacionada:** 
- [Sistema de Manuten��es](MANUTENCAO_GUIA_ACESSO.md)
- [Upload de Documentos](UPLOAD_DOCUMENTOS.md)
- [Sistema de Loca��es](LOCACOES_GUIA_COMPLETO.md)
