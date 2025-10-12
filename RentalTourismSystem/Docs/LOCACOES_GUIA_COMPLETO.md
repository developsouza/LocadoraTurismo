# ?? Sistema de Loca��es de Ve�culos - Guia Completo

## ?? �ndice
- [Vis�o Geral](#vis�o-geral)
- [Criar Nova Loca��o](#criar-nova-loca��o)
- [Processo de Loca��o](#processo-de-loca��o)
- [Devolu��o de Ve�culo](#devolu��o-de-ve�culo)
- [Documentos e Contratos](#documentos-e-contratos)
- [Vistoria](#vistoria)
- [Gest�o de Loca��es](#gest�o-de-loca��es)
- [Relat�rios](#relat�rios)
- [Permiss�es](#permiss�es)

---

## ?? Vis�o Geral

O sistema de loca��es gerencia todo o processo de aluguel de ve�culos, desde a reserva at� a devolu��o, incluindo contratos, vistorias e pagamentos.

### ?? Funcionalidades Principais

? **Processo Completo de Loca��o**
- Sele��o de cliente e ve�culo
- Defini��o de per�odo de loca��o
- C�lculo autom�tico de valores
- Gera��o de contrato
- Vistoria de retirada e devolu��o
- Registro de quilometragem

? **Controle Financeiro**
- C�lculo de di�rias
- Valores de cau��o
- Extras e adicionais
- Multas por atraso
- Hist�rico de pagamentos

? **Documenta��o**
- Contrato de loca��o (PDF)
- Laudo de vistoria (PDF)
- Registro fotogr�fico
- Hist�rico completo

---

## ?? Criar Nova Loca��o

### ?? Como Acessar

**Op��o 1 - Menu Lateral:**
```
Menu ? Loca��o ? Loca��es ? ? Nova Loca��o
```

**Op��o 2 - A partir do Cliente:**
```
Clientes ? Detalhes do Cliente ? ?? Nova Loca��o
```

**Op��o 3 - A partir do Ve�culo:**
```
Ve�culos ? Detalhes do Ve�culo ? ? Nova Loca��o
(apenas se ve�culo dispon�vel)
```

**URL direta:** `/Locacoes/Create`

### ?? Pr�-requisitos

Antes de criar loca��o, certifique-se que:

#### ? **Cliente**
- [x] Cliente cadastrado no sistema
- [x] CPF v�lido
- [x] Email e telefone atualizados
- [x] **CNH V�LIDA** (n�o vencida)
- [x] Idade m�nima 21 anos
- [x] Documentos enviados (CNH, RG, CPF)

#### ? **Ve�culo**
- [x] Ve�culo cadastrado
- [x] Status = "Dispon�vel"
- [x] Sem manuten��o pendente
- [x] Documentos em dia (CRLV, Seguro)
- [x] Dispon�vel no per�odo desejado

#### ? **Sistema**
- [x] Ag�ncia definida
- [x] Funcion�rio respons�vel
- [x] Per�odo de loca��o v�lido

### ?? Formul�rio de Loca��o

#### 1?? **Sele��o - Quem e O que**

| Campo | Descri��o | Valida��o |
|-------|-----------|-----------|
| **Cliente** | Selecione o cliente | Obrigat�rio, CNH v�lida |
| **Ve�culo** | Selecione o ve�culo | Obrigat�rio, dispon�vel |
| **Funcion�rio** | Respons�vel pela loca��o | Obrigat�rio |
| **Ag�ncia** | Local de retirada/devolu��o | Obrigat�rio |

#### 2?? **Per�odo - Quando**

| Campo | Formato | Valida��o | Exemplo |
|-------|---------|-----------|---------|
| **Data de Retirada** | dd/MM/yyyy HH:mm | Obrigat�rio, data futura ou hoje | 15/12/2024 10:00 |
| **Data de Devolu��o** | dd/MM/yyyy HH:mm | Obrigat�rio, ap�s retirada | 20/12/2024 18:00 |

> ?? **Sistema calcula automaticamente:** Quantidade de di�rias

#### 3?? **Quilometragem**

| Campo | Descri��o | Valida��o | Exemplo |
|-------|-----------|-----------|---------|
| **KM na Retirada** | Quilometragem atual do ve�culo | Obrigat�rio, ? 0 | 45.000 km |
| **KM na Devolu��o** | Preenchido na devolu��o | Autom�tico | - |

> ?? **Autom�tico:** Sistema preenche com KM atual do ve�culo

#### 4?? **Valores - Quanto**

| Campo | C�lculo | Exemplo |
|-------|---------|---------|
| **Valor da Di�ria** | Do cadastro do ve�culo | R$ 150,00 |
| **Quantidade de Di�rias** | Calculado automaticamente | 5 di�rias |
| **Valor Total** | Di�ria � Quantidade | R$ 750,00 |

> ?? **C�lculo Autom�tico:** Sistema calcula baseado no per�odo

#### 5?? **Observa��es**

| Campo | Descri��o | Exemplo |
|-------|-----------|---------|
| **Observa��es** | Informa��es adicionais (opcional) | "Cliente solicitou seguro adicional" |

### ?? Passo a Passo - Criar Loca��o

**Exemplo pr�tico:**

```
1. ACESSE: Loca��es ? Nova Loca��o

2. SELECIONE CLIENTE:
   Cliente: Maria da Silva
   CPF: 987.654.321-00
   ? CNH v�lida at�: 10/08/2028

3. SELECIONE VE�CULO:
   Ve�culo: Gol 1.0 Flex - ABC1D23
   ? Dispon�vel
   Di�ria: R$ 150,00

4. DEFINA RESPONS�VEIS:
   Funcion�rio: Jo�o Atendente
   Ag�ncia: Ag�ncia Centro

5. DEFINA PER�ODO:
   Retirada: 15/12/2024 10:00
   Devolu��o: 20/12/2024 18:00
   
   ?? Sistema calcula:
   ? 5 di�rias completas
   ? Valor Total: R$ 750,00

6. QUILOMETRAGEM:
   KM Retirada: 45.000 (do ve�culo)

7. OBSERVA��ES:
   "Cliente preferencial, 5% desconto aplicado"

8. CLIQUE: "Criar Loca��o"

9. RESULTADO:
   ? Loca��o criada com sucesso!
   ? Ve�culo alterado para status "Locado"
   ? Contrato gerado
   ? Pr�ximo: Fazer vistoria
```

### ? Valida��es Autom�ticas

Sistema valida automaticamente:

1. **Cliente:**
   - ? CNH v�lida (n�o vencida)
   - ? Idade m�nima 21 anos
   - ? Cadastro completo

2. **Ve�culo:**
   - ? Status "Dispon�vel"
   - ? Sem loca��o no per�odo
   - ? Documentos em dia

3. **Per�odo:**
   - ? Data devolu��o ap�s retirada
   - ? M�nimo 1 di�ria
   - ? Per�odo dispon�vel

4. **Valores:**
   - ? Valor total > 0
   - ? C�lculo correto de di�rias
   - ? Sem inconsist�ncias

---

## ?? Processo de Loca��o

### ?? Fluxo Completo

```
1. RESERVA/AGENDAMENTO
   ?
2. CRIA��O DA LOCA��O
   ?
3. VISTORIA DE RETIRADA
   ?
4. GERA��O DO CONTRATO
   ?
5. RETIRADA DO VE�CULO
   ?
6. PER�ODO DE LOCA��O
   ?
7. DEVOLU��O DO VE�CULO
   ?
8. VISTORIA DE DEVOLU��O
   ?
9. C�LCULO FINAL
   ?
10. FINALIZA��O
```

### 1?? Cria��o da Loca��o

**Status:** Criada
**A��es:**
- [x] Preencher formul�rio
- [x] Validar dados
- [x] Salvar loca��o
- [x] Alterar status do ve�culo para "Locado"

**Pr�ximo:** Vistoria de Retirada

### 2?? Vistoria de Retirada

**Objetivo:** Registrar estado do ve�culo antes da entrega

**Como fazer:**
```
Loca��es ? Detalhes da Loca��o ? ?? Vistoria de Retirada
```

**Itens a verificar:**
- ? Lataria (arranh�es, amassados)
- ? Vidros (trincas, quebras)
- ? Pneus (estado, calibragem)
- ? Documentos no ve�culo
- ? Combust�vel (n�vel)
- ? Acess�rios (macaco, estepe, extintor)
- ? Interior (bancos, painel)
- ? Limpeza geral

**Registro fotogr�fico:**
- ?? Frente
- ?? Traseira
- ?? Laterais
- ?? Painel
- ?? Detalhes espec�ficos

### 3?? Gera��o do Contrato

**Autom�tico ap�s salvar loca��o**

```
Loca��es ? Detalhes ? ?? Gerar Contrato
```

**Contrato inclui:**
- ? Dados do cliente
- ? Dados do ve�culo
- ? Per�odo da loca��o
- ? Valores e condi��es
- ? Responsabilidades
- ? Termos e condi��es
- ? Assinaturas

**Formato:** PDF para impress�o

### 4?? Retirada do Ve�culo

**Checklist final:**
- [x] Contrato assinado
- [x] Vistoria aprovada
- [x] Pagamento confirmado (ou cau��o)
- [x] Cliente com CNH e documentos
- [x] Ve�culo limpo e abastecido

**Cliente recebe:**
- ?? C�pia do contrato
- ?? Laudo de vistoria
- ?? Chaves do ve�culo
- ?? Contato de emerg�ncia

### 5?? Per�odo de Loca��o

**Sistema monitora:**
- ? Dias restantes
- ?? Data prevista de devolu��o
- ?? Status do ve�culo (Locado)
- ?? Alertas de vencimento

**Cliente pode:**
- ?? Solicitar extens�o
- ?? Reportar problemas
- ?? Informar localiza��o (futuro)

---

## ?? Devolu��o de Ve�culo

### ?? Como Registrar Devolu��o

**Loca��es ? Detalhes da Loca��o ? ?? Registrar Devolu��o**

### ?? Processo de Devolu��o

#### 1?? **Recep��o do Ve�culo**

**Verificar:**
- ? Ve�culo retornou na data prevista
- ? Cliente trouxe documentos
- ? Chaves entregues

#### 2?? **Registrar Quilometragem**

```
KM na Devolu��o: 45.850 km

?? Sistema calcula:
   KM Retirada: 45.000 km
   KM Devolu��o: 45.850 km
   KM Rodados: 850 km
   M�dia: 170 km/dia
```

> ?? **Autom�tico:** Sistema atualiza KM do ve�culo

#### 3?? **Vistoria de Devolu��o**

**Comparar com vistoria de retirada:**

```
Loca��es ? Detalhes ? ?? Vistoria de Devolu��o
```

**Verificar:**
- ? Novos danos?
- ? Estado de conserva��o
- ? Limpeza
- ? Combust�vel (mesmo n�vel)
- ? Acess�rios completos

**Registrar:**
- ?? Estado geral: Bom / Regular / Ruim
- ?? Observa��es de danos
- ?? Fotos de novos danos
- ?? C�lculo de multas (se aplic�vel)

#### 4?? **C�lculo Final**

**Sistema calcula automaticamente:**

```
?? C�LCULO DE VALORES:

Valor Inicial: R$ 750,00 (5 di�rias)

+ Dias extras (se houver):
  Atraso: 2 dias � R$ 150,00 = R$ 300,00

+ Multas (se houver):
  Dano no para-choque: R$ 350,00
  Falta de combust�vel: R$ 80,00

- Descontos (se houver):
  Cliente fidelidade: -R$ 50,00

= VALOR TOTAL FINAL: R$ 1.430,00

Status: Aguardando Pagamento
```

#### 5?? **Finaliza��o**

**Ap�s confirma��o de pagamento:**

```
1. Marcar pagamento como recebido
2. Alterar status ve�culo:
   De: "Locado"
   Para: "Dispon�vel" (ou "Manuten��o" se necess�rio)
3. Gerar recibo final
4. Enviar comprovante ao cliente
5. Arquivar documenta��o
```

### ? Situa��es Especiais

#### **Devolu��o Antecipada**

**Cliente devolveu antes do prazo:**

```
Loca��o: 5 di�rias (R$ 750,00)
Devolveu: ap�s 3 di�rias

Op��es:
1. Cobrar valor proporcional (3 dias)
2. Cobrar valor total (depende do contrato)
3. Gerar cr�dito para pr�xima loca��o

? Definir pol�tica comercial
```

#### **Devolu��o Atrasada**

**Cliente atrasou devolu��o:**

```
Previsto: 20/12/2024 18:00
Devolveu: 22/12/2024 10:00
Atraso: 2 dias

C�lculo:
Base (5 dias): R$ 750,00
Dias extras (2 dias): R$ 300,00
Multa por atraso: R$ 100,00

Total: R$ 1.150,00

?? Sistema alerta atraso automaticamente
```

#### **Ve�culo com Danos**

**Danos identificados na devolu��o:**

```
1. Documentar com fotos
2. Descrever danos detalhadamente
3. Calcular custo de reparo:
   - Or�amento oficial
   - Tabela de refer�ncia
4. Adicionar � conta do cliente
5. Programar manuten��o
6. Acionar seguro (se aplic�vel)

Pr�ximo: Ve�culo vai para "Manuten��o"
```

---

## ?? Documentos e Contratos

### ?? Gera��o de Documentos

#### 1?? **Contrato de Loca��o**

**Acesso:**
```
Loca��es ? Detalhes ? ?? Gerar Contrato
```

**Conte�do:**
```
CONTRATO DE LOCA��O DE VE�CULO

LOCADOR: Litoral Sul Locadora e Turismo
CNPJ: XX.XXX.XXX/XXXX-XX

LOCAT�RIO: [Nome do Cliente]
CPF: [CPF do Cliente]
CNH: [N�mero da CNH]

VE�CULO:
Marca/Modelo: [Marca] [Modelo]
Placa: [Placa]
Ano: [Ano]
Cor: [Cor]

PER�ODO:
Retirada: [Data/Hora]
Devolu��o Prevista: [Data/Hora]

VALORES:
Di�ria: R$ [Valor]
Quantidade: [X] di�rias
Total: R$ [Valor Total]

CONDI��ES:
- Quilometragem livre / limitada
- Seguro inclu�do / n�o inclu�do
- Franquia: R$ [Valor]
- Motorista adicional: Sim / N�o

RESPONSABILIDADES:
[Cl�usulas do contrato]

ASSINATURAS:
_________________________
Locador

_________________________
Locat�rio

Data: [Data]
```

**Formato:** PDF para impress�o

#### 2?? **Laudo de Vistoria**

**Vistoria de Retirada:**
```
Loca��es ? Detalhes ? ?? Vistoria de Retirada
```

**Vistoria de Devolu��o:**
```
Loca��es ? Detalhes ? ?? Vistoria de Devolu��o
```

**Conte�do:**
```
LAUDO DE VISTORIA - [RETIRADA/DEVOLU��O]

Ve�culo: [Marca Modelo - Placa]
Cliente: [Nome]
Data: [Data/Hora]
KM: [Quilometragem]

CHECKLIST:

Carroceria:
[ ] Sem avarias
[ ] Arranh�es: [Localiza��o]
[ ] Amassados: [Localiza��o]

Vidros:
[ ] Perfeitos
[ ] Trincados: [Qual]

Pneus:
[ ] Bom estado
[ ] Desgaste: [N�vel]
[ ] Calibragem: OK

Interior:
[ ] Limpo
[ ] Conservado
[ ] Danos: [Descri��o]

Combust�vel: [N�vel]
Acess�rios: [Todos presentes]

FOTOS:
[Imagens anexas]

OBSERVA��ES:
[Detalhes adicionais]

_________________________
Vistoriador

_________________________
Cliente
```

**Formato:** PDF com fotos

### ?? Envio de Documentos

**Op��es:**
- ?? Email para cliente
- ?? WhatsApp (futuro)
- ?? Download direto
- ??? Impress�o

---

## ?? Gest�o de Loca��es

### ?? Lista de Loca��es

**Menu ? Loca��o ? Loca��es ? Ver Todas**

### ?? Filtros e Buscas

**Filtros dispon�veis:**

| Filtro | Op��es |
|--------|--------|
| **Status** | Ativa, Finalizada, Atrasada, Cancelada |
| **Per�odo** | Data in�cio/fim |
| **Cliente** | Nome ou CPF |
| **Ve�culo** | Modelo ou Placa |
| **Ag�ncia** | Ag�ncia espec�fica |
| **Funcion�rio** | Respons�vel |

**Busca r�pida:**
- Por cliente (nome/CPF)
- Por ve�culo (modelo/placa)
- Por n�mero de loca��o

### ?? Visualiza��o

#### **Card de Loca��o:**
```
?? Loca��o #12345
?????????????????????????????
?? Cliente: Maria da Silva
?? Ve�culo: Gol 1.0 - ABC1234
?? 15/12 a 20/12/2024 (5 dias)
?? R$ 750,00
?? Status: ATIVA

?? Devolu��o em: 3 dias

[Detalhes] [Vistoria] [Contrato]
```

#### **Indicadores de Status:**

| Status | Cor | Significado |
|--------|-----|-------------|
| **Ativa** | ?? Verde | Loca��o em andamento |
| **Atrasada** | ?? Vermelho | Passou data de devolu��o |
| **Finalizada** | ?? Azul | Conclu�da normalmente |
| **Cancelada** | ? Cinza | Cancelada antes da retirada |

### ?? Alertas

**Sistema alerta:**
- ?? Loca��o atrasada (passou data devolu��o)
- ?? Devolu��o hoje
- ?? Devolu��o em 1-2 dias
- ?? Vistoria pendente
- ?? Pagamento pendente

---

## ?? Relat�rios

### ?? Como Acessar

**Menu ? Relat�rios ? Loca��es**

### ?? Tipos de Relat�rios

#### 1?? **Loca��es por Per�odo**

```
RELAT�RIO: Loca��es - Dezembro/2024

Total de Loca��es: 45
Receita Total: R$ 67.500,00
Ticket M�dio: R$ 1.500,00

Detalhamento:
Data       | Cliente        | Ve�culo      | Valor
15/12/2024 | Maria Silva   | Gol ABC1234  | R$ 750,00
16/12/2024 | Jo�o Santos   | HB20 DEF567  | R$ 900,00
...
```

**Filtros:**
- ?? Data in�cio/fim
- ?? Ag�ncia
- ?? Cliente
- ?? Ve�culo
- ?? Status

#### 2?? **Receita por Ve�culo**

```
Ve�culo          | Loca��es | Dias | Receita
Gol ABC1234      | 12       | 85   | R$ 12.750,00
HB20 DEF5678     | 9        | 67   | R$ 10.050,00
Onix GHI9012     | 8        | 54   | R$ 8.100,00
```

#### 3?? **Clientes Frequentes**

```
Cliente          | Loca��es | Valor Total | �ltima Loca��o
Maria Silva      | 15       | R$ 22.500   | 15/12/2024
Jo�o Santos      | 12       | R$ 18.000   | 10/12/2024
Ana Costa        | 8        | R$ 12.000   | 05/12/2024
```

#### 4?? **Loca��es Ativas**

```
LOCA��ES ATIVAS - Hoje: 20/12/2024

Total Ativas: 7 ve�culos locados

Cliente     | Ve�culo     | Devolu��o   | Status
Maria       | Gol ABC     | Hoje 18:00  | ?? Devolve hoje
Jo�o        | HB20 DEF    | 22/12       | ?? No prazo
Ana         | Onix GHI    | 18/12       | ?? ATRASADA!
```

### ?? Exporta��o

**Formatos:**
- ?? Excel (.xlsx)
- ?? PDF
- ?? CSV
- ??? Impress�o

---

## ?? Permiss�es de Acesso

### ??? **Visualiza��o**
**Quem pode:** Todos os usu�rios autenticados
- Ver lista de loca��es
- Ver detalhes de loca��o
- Visualizar contratos
- Consultar hist�rico

### ?? **Cria��o e Edi��o**
**Quem pode:** Admin, Manager, Employee
- Criar nova loca��o
- Editar loca��o (antes da retirada)
- Registrar devolu��o
- Fazer vistoria
- Gerar contratos

### ??? **Cancelamento**
**Quem pode:** Admin, Manager
- Cancelar loca��o (antes da retirada)
- Ajustar valores
- Aplicar descontos

> ?? **Importante:** Loca��es em andamento ou finalizadas N�O podem ser exclu�das!

---

## ? Boas Pr�ticas

### ?? Ao Criar Loca��o

? **Fa�a:**
- Verificar CNH do cliente (v�lida)
- Confirmar disponibilidade do ve�culo
- Validar per�odo de loca��o
- Fazer vistoria completa
- Gerar contrato antes da entrega
- Registrar fotos do ve�culo

? **Evite:**
- Criar sem verificar CNH
- Locar ve�culo em manuten��o
- Pular etapa de vistoria
- Entregar sem contrato assinado
- N�o registrar danos existentes

### ?? Durante a Loca��o

? **Fa�a:**
- Monitorar data de devolu��o
- Manter contato com cliente (se atraso)
- Verificar alertas do sistema
- Estar pronto para devolu��o

? **Evite:**
- Ignorar atrasos
- N�o avisar cliente sobre vencimento
- Descuidar do acompanhamento

### ?? Na Devolu��o

? **Fa�a:**
- Vistoria completa e detalhada
- Registrar quilometragem correta
- Documentar TODOS os danos (novos)
- Calcular valores corretamente
- Confirmar pagamento antes de liberar
- Alterar status do ve�culo

? **Evite:**
- Vistoria superficial
- Ignorar pequenos danos
- Liberar sem pagamento
- Esquecer de atualizar status
- N�o documentar problemas

---

## ?? Casos de Uso

### Caso 1: Loca��o Simples

**Cen�rio:** Cliente retira carro para fim de semana

```
1. Cliente: Maria Silva (CNH v�lida)
2. Ve�culo: Gol ABC1234 (Dispon�vel)
3. Per�odo: 15/12 a 17/12 (3 dias)
4. Valor: R$ 450,00

Processo:
? Criar loca��o
? Vistoria de retirada (OK)
? Gerar contrato
? Cliente retira ve�culo
? Aguardar devolu��o (17/12)
? Cliente devolve no prazo
? Vistoria devolu��o (OK)
? Confirmar pagamento
? Finalizar loca��o
```

### Caso 2: Loca��o com Atraso

**Cen�rio:** Cliente atrasa devolu��o

```
Previsto: 20/12/2024
Devolveu: 22/12/2024 (2 dias atraso)

C�lculo:
5 dias originais: R$ 750,00
2 dias extras: R$ 300,00
Multa atraso: R$ 100,00
Total: R$ 1.150,00

A��es:
1. Sistema alerta atraso automaticamente
2. Contatar cliente
3. Registrar devolu��o
4. Calcular valores extras
5. Confirmar pagamento total
6. Finalizar
```

### Caso 3: Ve�culo com Danos

**Cen�rio:** Cliente devolveu com dano

```
Vistoria Devolu��o:
?? Dano identificado: Arranh�o na lateral

Procedimento:
1. Documentar com fotos
2. Descrever dano detalhadamente
3. Fazer or�amento de reparo
4. Adicionar custo � conta:
   Loca��o: R$ 750,00
   Reparo: R$ 350,00
   Total: R$ 1.100,00
5. Negociar com cliente
6. Confirmar pagamento
7. Programar reparo
8. Ve�culo ? Status "Manuten��o"
```

---

## ?? Solu��o de Problemas

### ? "CNH do cliente vencida"
**Solu��o:** 
- N�o permitir loca��o
- Solicitar CNH atualizada
- Atualizar cadastro do cliente

### ? "Ve�culo indispon�vel"
**Solu��o:**
- Verificar status do ve�culo
- Consultar outras loca��es no per�odo
- Sugerir ve�culo similar

### ? "Erro ao gerar contrato"
**Solu��o:**
- Verificar dados completos
- Confirmar template do contrato
- Tentar novamente

### ? "Cliente n�o devolveu"
**Solu��o:**
- Contatar cliente imediatamente
- Verificar localiza��o (se dispon�vel)
- Acionar seguradora (se necess�rio)
- Pol�cia (casos extremos)

---

## ?? Atalhos

### ?? Teclado
```
Ctrl + N     ? Nova loca��o
Ctrl + F     ? Buscar loca��o
Ctrl + P     ? Imprimir contrato
```

### ?? URLs
```
/Locacoes/Index          ? Todas as loca��es
/Locacoes/Create         ? Nova loca��o
/Locacoes/Details/{id}   ? Detalhes
```

---

## ?? Pronto para Usar!

O sistema de loca��es est� **100% operacional**.

**Acesse:** Menu ? Loca��o ? Loca��es

---

**Desenvolvido para:** Litoral Sul Locadora e Turismo  
**Vers�o:** 1.0  
**Data:** Outubro/2025  
**Documenta��o relacionada:** 
- [Gest�o de Clientes](CLIENTES_GUIA_COMPLETO.md)
- [Gest�o de Ve�culos](VEICULOS_GUIA_COMPLETO.md)
- [Sistema de Manuten��es](MANUTENCAO_GUIA_ACESSO.md)
