# ?? Sistema de Reservas de Viagem - Guia Completo

## ?? �ndice
- [Vis�o Geral](#vis�o-geral)
- [Pacotes de Viagem](#pacotes-de-viagem)
- [Criar Reserva](#criar-reserva)
- [Gerenciar Reservas](#gerenciar-reservas)
- [Servi�os Adicionais](#servi�os-adicionais)
- [Status e Workflow](#status-e-workflow)
- [Relat�rios](#relat�rios)
- [Permiss�es](#permiss�es)

---

## ?? Vis�o Geral

O sistema de reservas de viagem gerencia pacotes tur�sticos, permitindo que clientes reservem viagens completas com servi�os adicionais opcionais.

### ?? Funcionalidades Principais

? **Gest�o de Pacotes**
- Cadastro de destinos e roteiros
- Defini��o de pre�os e dura��o
- Controle de disponibilidade
- Ativa��o/desativa��o de pacotes

? **Reservas Completas**
- Sele��o de pacote e cliente
- Defini��o de data e quantidade de pessoas
- C�lculo autom�tico de valores
- Adi��o de servi�os extras

? **Controle de Status**
- Pendente ? Confirmada ? Realizada
- Cancelamento de reservas
- Hist�rico completo
- Alertas e notifica��es

---

## ?? Pacotes de Viagem

### ?? Como Acessar

**Menu Lateral ? Turismo ? Pacotes de Viagem**

OU

**URL direta:** `/PacotesViagens/Index`

### ?? Cadastrar Novo Pacote

**Pacotes ? ? Novo Pacote**

#### ?? Formul�rio de Cadastro

| Campo | Formato | Valida��o | Exemplo |
|-------|---------|-----------|---------|
| **Nome do Pacote** | Texto (m�x. 100 caracteres) | Obrigat�rio | Praias do Litoral Sul |
| **Descri��o** | Texto (m�x. 1000 caracteres) | Obrigat�rio | Tour completo pelas praias... |
| **Destino** | Texto (m�x. 100 caracteres) | Obrigat�rio | Guaruj�, SP |
| **Dura��o** | N�mero | Obrigat�rio | 3 |
| **Unidade de Tempo** | Sele��o | Obrigat�rio | dias / horas |
| **Pre�o por Pessoa** | Decimal (R$) | Obrigat�rio | R$ 850,00 |
| **Ativo** | Checkbox | Padr�o: Sim | ? Ativo |

### ?? Exemplo - Cadastro de Pacote

```
NOVO PACOTE DE VIAGEM

Nome: Litoral Norte Completo
Descri��o: 
  "Conhe�a as melhores praias do Litoral Norte de SP. 
   Inclui transporte, guia tur�stico e almo�o.
   Visita a Ubatuba, Caraguatatuba e Ilhabela."

Destino: Litoral Norte - SP
Dura��o: 2 dias
Unidade: dias

Pre�o: R$ 650,00 por pessoa

?? Ativo (dispon�vel para venda)

[Salvar Pacote]
```

### ?? Informa��es do Pacote

**Card do Pacote:**
```
?? Litoral Norte Completo
?????????????????????????????
?? Destino: Litoral Norte - SP
?? Dura��o: 2 dias
?? R$ 650,00 por pessoa

?? Estat�sticas:
   Reservas: 25
   Receita: R$ 48.750,00
   �ltima reserva: h� 3 dias

? ATIVO

[Detalhes] [Editar] [Desativar]
```

### ?? Editar Pacote

**Pacotes ? ?? Editar**

**Campos edit�veis:**
- ? Nome e descri��o
- ? Destino
- ? Dura��o e unidade
- ? Pre�o por pessoa
- ? Status (Ativo/Inativo)

> ?? **Importante:** Altera��es no pre�o N�O afetam reservas j� confirmadas!

### ?? Ativar/Desativar Pacote

**Por que desativar?**
- Pacote fora de temporada
- Destino temporariamente indispon�vel
- Manuten��o de informa��es
- Pacote sendo reformulado

**Efeitos:**
- ? N�o aparece para novas reservas
- ? Reservas existentes n�o s�o afetadas
- ? Hist�rico mantido

### ??? Excluir Pacote

**Regras:**
- ? Pode excluir: Pacote SEM reservas
- ? N�O pode excluir: Pacote COM reservas

> ?? **Dica:** Use "Desativar" ao inv�s de excluir pacotes com hist�rico

---

## ?? Criar Reserva

### ?? Como Acessar

**Op��o 1 - Menu:**
```
Menu ? Turismo ? Reservas ? ? Nova Reserva
```

**Op��o 2 - A partir do Cliente:**
```
Clientes ? Detalhes ? ?? Nova Reserva
```

**Op��o 3 - A partir do Pacote:**
```
Pacotes ? Detalhes ? ? Reservar
```

**URL direta:** `/ReservasViagens/Create`

### ?? Formul�rio de Reserva

#### 1?? **Sele��o - Quem e O que**

| Campo | Descri��o | Valida��o |
|-------|-----------|-----------|
| **Cliente** | Quem vai viajar | Obrigat�rio, cadastrado |
| **Pacote de Viagem** | Destino escolhido | Obrigat�rio, ativo |

#### 2?? **Quando e Quantos**

| Campo | Formato | Valida��o | Exemplo |
|-------|---------|-----------|---------|
| **Data da Viagem** | dd/MM/yyyy | Obrigat�rio, data futura | 25/12/2024 |
| **Quantidade de Pessoas** | N�mero | Obrigat�rio (1-50) | 4 pessoas |

#### 3?? **Valores**

| Campo | C�lculo | Exemplo |
|-------|---------|---------|
| **Pre�o por Pessoa** | Do pacote | R$ 650,00 |
| **Quantidade** | Informado | 4 pessoas |
| **Valor Total** | Pre�o � Quantidade | R$ 2.600,00 |

> ?? **C�lculo Autom�tico:** Sistema calcula valor total

#### 4?? **Complementos**

| Campo | Descri��o | Exemplo |
|-------|-----------|---------|
| **Observa��es** | Informa��es adicionais (opcional) | "Cliente solicitou hotel 5 estrelas" |

### ?? Passo a Passo - Criar Reserva

**Exemplo completo:**

```
1. ACESSE: Reservas ? Nova Reserva

2. SELECIONE CLIENTE:
   Cliente: Jo�o Santos
   CPF: 123.456.789-00
   ? Cadastro completo

3. SELECIONE PACOTE:
   Pacote: Litoral Norte Completo
   Destino: Litoral Norte - SP
   Dura��o: 2 dias
   Pre�o: R$ 650,00/pessoa
   ? Pacote ativo

4. DEFINA DATA E QUANTIDADE:
   Data da Viagem: 25/12/2024
   Quantidade: 4 pessoas
   
   ?? Sistema calcula:
   ? 4 � R$ 650,00 = R$ 2.600,00

5. OBSERVA��ES:
   "Cliente preferencial, fam�lia completa"

6. CLIQUE: "Criar Reserva"

7. RESULTADO:
   ? Reserva #12345 criada!
   ? Status: Pendente
   ? Valor: R$ 2.600,00
   
   Pr�ximo: Adicionar servi�os extras (opcional)
```

### ? Valida��es Autom�ticas

Sistema valida:

1. **Cliente:**
   - ? Cadastrado no sistema
   - ? Dados completos

2. **Pacote:**
   - ? Status ativo
   - ? Dispon�vel

3. **Data:**
   - ? Data futura
   - ? Formato v�lido

4. **Quantidade:**
   - ? Entre 1 e 50 pessoas
   - ? N�mero inteiro positivo

5. **Valores:**
   - ? Valor total > 0
   - ? C�lculo correto

---

## ?? Servi�os Adicionais

### ?? O que s�o?

Servi�os extras que podem ser adicionados � reserva:
- ?? Hospedagem premium
- ??? Refei��es especiais
- ?? Ingressos para atra��es
- ?? Transporte privativo
- ?? Fot�grafo profissional
- ?? Equipamentos especiais

### ? Como Adicionar

**Op��o 1 - Durante a Cria��o:**
```
Ap�s criar reserva ? "Adicionar Servi�os"
```

**Op��o 2 - Depois:**
```
Reservas ? Detalhes ? ? Adicionar Servi�o
```

### ?? Cadastrar Servi�o Adicional

| Campo | Descri��o | Exemplo |
|-------|-----------|---------|
| **Nome** | Descri��o do servi�o | "Hospedagem Hotel 5 Estrelas" |
| **Descri��o** | Detalhes | "2 di�rias no Hotel Paradiso" |
| **Pre�o** | Valor do servi�o | R$ 800,00 |

### ?? Exemplo

```
ADICIONAR SERVI�O ADICIONAL

Reserva: #12345
Cliente: Jo�o Santos
Pacote: Litoral Norte Completo

Servi�o: Hospedagem Premium
Descri��o: Hotel 5 estrelas, vista mar, caf� da manh�
Pre�o: R$ 800,00

[Adicionar]

RESUMO ATUALIZADO:
Pacote base: R$ 2.600,00
+ Hospedagem Premium: R$ 800,00
????????????????????????????????
TOTAL: R$ 3.400,00
```

### ?? Gerenciar Servi�os

**Visualizar:**
```
Reservas ? Detalhes ? Se��o "Servi�os Adicionais"

?? Servi�os Contratados:
1. Hospedagem Premium - R$ 800,00
2. Passeio de Barco - R$ 350,00
3. Jantar Especial - R$ 250,00

Total Servi�os: R$ 1.400,00
Total Geral: R$ 4.000,00
```

**Editar:**
- ?? Alterar descri��o
- ?? Ajustar pre�o
- ??? Remover servi�o

---

## ?? Gerenciar Reservas

### ?? Lista de Reservas

**Menu ? Turismo ? Reservas**

### ?? Filtros e Buscas

**Filtros dispon�veis:**

| Filtro | Op��es |
|--------|--------|
| **Status** | Pendente, Confirmada, Realizada, Cancelada |
| **Per�odo** | Data da viagem |
| **Cliente** | Nome ou CPF |
| **Pacote** | Destino |
| **Data Reserva** | Quando foi reservado |

### ?? Card de Reserva

```
?? Reserva #12345
?????????????????????????????
?? Cliente: Jo�o Santos
?? Pacote: Litoral Norte Completo
?? Viagem: 25/12/2024
?? 4 pessoas
?? R$ 3.400,00 (com servi�os)

?? Status: CONFIRMADA
??? Faltam 15 dias

[Detalhes] [Editar] [Cancelar]
```

### ?? Indicadores de Status

| Status | Cor | Descri��o |
|--------|-----|-----------|
| **Pendente** | ?? Amarelo | Aguardando confirma��o |
| **Confirmada** | ?? Verde | Reserva confirmada e paga |
| **Realizada** | ?? Azul | Viagem j� aconteceu |
| **Cancelada** | ?? Vermelho | Reserva cancelada |

### ?? Detalhes da Reserva

**Reservas ? Detalhes**

**Informa��es exibidas:**

```
DETALHES DA RESERVA #12345

?? INFORMA��ES GERAIS
?????????????????????????????
Data da Reserva: 10/12/2024
Status Atual: Confirmada
Dias at� a viagem: 15

?? CLIENTE
?????????????????????????????
Nome: Jo�o Santos
CPF: 123.456.789-00
Telefone: (13) 98765-4321
Email: joao@email.com

?? PACOTE
?????????????????????????????
Pacote: Litoral Norte Completo
Destino: Litoral Norte - SP
Dura��o: 2 dias
Data da Viagem: 25/12/2024

?? PARTICIPANTES
?????????????????????????????
Quantidade: 4 pessoas
Valor por pessoa: R$ 650,00
Subtotal: R$ 2.600,00

?? SERVI�OS ADICIONAIS
?????????????????????????????
1. Hospedagem Premium: R$ 800,00
2. Passeio de Barco: R$ 350,00

Total Servi�os: R$ 1.150,00

?? RESUMO FINANCEIRO
?????????????????????????????
Pacote base: R$ 2.600,00
Servi�os: R$ 1.150,00
????????????????????????????????
TOTAL: R$ 3.750,00

Status Pagamento: ? Pago

?? OBSERVA��ES
?????????????????????????????
Cliente preferencial, fam�lia completa.
Solicitou quarto com vista para o mar.
```

### ?? Editar Reserva

**Quando permitido:**
- ? Status: Pendente
- ? Viagem futura (n�o iniciada)
- ? Mais de 2 dias antes da viagem

**Campos edit�veis:**
- Data da viagem
- Quantidade de pessoas
- Observa��es
- Adicionar/remover servi�os

**N�O edit�vel:**
- Cliente (criar nova reserva)
- Pacote (criar nova reserva)

### ? Cancelar Reserva

**Regras de cancelamento:**

```
Pode cancelar quando:
? Status: Pendente ou Confirmada
? Viagem futura
? M�nimo 2 dias antes da viagem

N�o pode cancelar:
? Viagem j� realizada
? Menos de 2 dias para viagem
? J� cancelada
```

**Processo:**
```
1. Reservas ? Detalhes
2. Clicar em "? Cancelar Reserva"
3. Confirmar a��o
4. Sistema:
   - Altera status para "Cancelada"
   - Registra data de cancelamento
   - Mant�m hist�rico
   - Libera vaga do pacote
```

---

## ?? Status e Workflow

### ?? Ciclo de Vida da Reserva

```
1. CRIA��O
   ?
2. PENDENTE (aguardando confirma��o)
   ?
3. CONFIRMADA (pagamento confirmado)
   ?
4. REALIZADA (viagem aconteceu)

OU

   CANCELADA (em qualquer momento antes da viagem)
```

### ?? Detalhes de Cada Status

#### 1?? **PENDENTE** ??

**Quando:**
- Reserva rec�m-criada
- Aguardando confirma��o de pagamento

**A��es poss�veis:**
- ? Confirmar (ap�s pagamento)
- ? Editar
- ? Cancelar
- ? Adicionar servi�os

**Sistema alerta se:**
- Pendente h� mais de 7 dias
- Faltam menos de 7 dias para viagem

#### 2?? **CONFIRMADA** ??

**Quando:**
- Pagamento confirmado
- Cliente confirmou presen�a

**A��es poss�veis:**
- ? Adicionar servi�os
- ? Editar (limitado)
- ? Cancelar (com condi��es)

**Sistema alerta:**
- 7 dias antes: "Viagem se aproxima"
- 3 dias antes: "Confirmar preparativos"
- 1 dia antes: "Viagem amanh�!"

#### 3?? **REALIZADA** ??

**Quando:**
- Data da viagem passou
- Viagem foi conclu�da

**Caracter�sticas:**
- ?? N�o edit�vel
- ?? Entra nas estat�sticas
- ? Poss�vel avaliar (futuro)

#### 4?? **CANCELADA** ??

**Quando:**
- Cliente cancelou
- Motivos operacionais
- N�o confirmado a tempo

**Caracter�sticas:**
- ?? N�o edit�vel
- ?? Motivo registrado
- ?? N�o conta em receita

### ?? Mudan�as de Status

**Manual:**
```
Reservas ? Detalhes ? ?? Alterar Status

Status atual: Pendente
Novo status: Confirmada

Motivo: Pagamento confirmado via PIX

[Confirmar Altera��o]
```

**Autom�tico:**
- ?? Viagem passou ? Status vira "Realizada"
- ? Timeout ? Pendente por muito tempo ? Alerta

---

## ?? Relat�rios

### ?? Como Acessar

**Menu ? Relat�rios ? Reservas de Viagem**

### ?? Tipos de Relat�rios

#### 1?? **Reservas por Per�odo**

```
RELAT�RIO: Reservas - Dezembro/2024

Total Reservas: 35
Receita Total: R$ 91.000,00
Ticket M�dio: R$ 2.600,00

Por Status:
? Confirmadas: 28 (80%)
?? Pendentes: 5 (14%)
?? Canceladas: 2 (6%)

Detalhamento:
Data Viagem | Cliente       | Pacote          | Pessoas | Valor
25/12/2024  | Jo�o Santos  | Litoral Norte   | 4       | R$ 3.400,00
26/12/2024  | Maria Silva  | Praias do Sul   | 2       | R$ 1.700,00
...
```

#### 2?? **Pacotes Mais Vendidos**

```
Ranking de Pacotes - Ano 2024

Pacote               | Reservas | Pessoas | Receita
Litoral Norte        | 45       | 156     | R$ 101.400,00
Praias do Sul        | 38       | 98      | R$ 83.300,00
Trilhas e Cachoeiras | 32       | 112     | R$ 89.600,00
Serra da Mantiqueira | 28       | 84      | R$ 67.200,00
```

#### 3?? **Receita por M�s**

```
M�s          | Reservas | Receita      | Crescimento
Janeiro      | 42       | R$ 105.000   | -
Fevereiro    | 38       | R$ 95.000    | -9.5%
Mar�o        | 45       | R$ 112.500   | +18.4%
...
Dezembro     | 52       | R$ 130.000   | +15.6%

Total Ano: R$ 1.248.000
M�dia Mensal: R$ 104.000
```

#### 4?? **An�lise de Clientes**

```
Clientes Frequentes

Cliente          | Reservas | Valor Total | �ltima Viagem
Jo�o Santos      | 8        | R$ 24.000   | 15/12/2024
Maria Silva      | 6        | R$ 18.500   | 10/12/2024
Ana Costa        | 5        | R$ 15.000   | 05/12/2024

Novos Clientes: 45 (m�s atual)
Taxa Recompra: 35%
```

### ?? Filtros Dispon�veis

**Todos os relat�rios:**
- ?? Per�odo (data viagem ou reserva)
- ?? Status da reserva
- ?? Cliente espec�fico
- ?? Pacote espec�fico
- ?? Quantidade de pessoas

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
- Ver lista de reservas
- Ver detalhes de reserva
- Consultar pacotes
- Ver relat�rios b�sicos

### ?? **Cria��o e Edi��o**
**Quem pode:** Admin, Manager, Employee
- Criar nova reserva
- Editar reserva (antes da viagem)
- Adicionar servi�os
- Confirmar pagamento
- Alterar status

### ??? **Cancelamento e Exclus�o**
**Quem pode:** Admin, Manager
- Cancelar reserva
- Excluir pacote SEM reservas
- Ajustar valores
- Aplicar descontos

### ?? **Gest�o de Pacotes**
**Quem pode:** Admin, Manager
- Criar pacotes
- Editar pacotes
- Ativar/desativar
- Definir pre�os

---

## ? Boas Pr�ticas

### ?? Pacotes

? **Fa�a:**
- Descri��o clara e detalhada
- Pre�os competitivos e justos
- Atualizar informa��es regularmente
- Desativar pacotes fora de temporada
- Manter hist�rico de reservas

? **Evite:**
- Descri��es vagas
- Pre�os desatualizados
- Excluir pacotes com reservas
- Deixar inativos sem motivo

### ?? Reservas

? **Fa�a:**
- Confirmar dados do cliente
- Validar data da viagem
- Adicionar observa��es relevantes
- Confirmar pagamento antes de confirmar reserva
- Manter cliente informado

? **Evite:**
- Reservar para data passada
- Confirmar sem pagamento
- Ignorar observa��es do cliente
- Deixar pendente por muito tempo
- Esquecer de adicionar servi�os solicitados

### ?? Status

? **Fa�a:**
- Atualizar status prontamente
- Registrar motivo de cancelamento
- Confirmar ap�s pagamento
- Marcar como realizada ap�s viagem

? **Evite:**
- Status desatualizado
- Cancelar sem registrar motivo
- Confirmar sem pagamento
- Esquecer de atualizar

---

## ?? Casos de Uso

### Caso 1: Reserva Simples

```
Cliente: Maria Silva
Pacote: Praias do Sul (R$ 850/pessoa)
Data: 01/01/2025
Pessoas: 2

Processo:
1. Criar reserva
2. Calcular: 2 � R$ 850 = R$ 1.700
3. Status: Pendente
4. Cliente paga
5. Confirmar reserva
6. Aguardar data da viagem
7. Ap�s 01/01 ? Status: Realizada
```

### Caso 2: Reserva com Servi�os

```
Cliente: Jo�o Santos
Pacote: Litoral Norte (R$ 650/pessoa)
Data: 25/12/2024
Pessoas: 4
Base: R$ 2.600

Servi�os:
+ Hospedagem: R$ 800
+ Passeio de Barco: R$ 350

Total: R$ 3.750

Processo:
1. Criar reserva base
2. Adicionar servi�os
3. Recalcular total
4. Confirmar pagamento
5. Status: Confirmada
```

### Caso 3: Cancelamento

```
Reserva #12345
Status: Confirmada
Viagem: 25/12/2024
Hoje: 15/12/2024 (10 dias antes)

Cliente solicitou cancelamento

? Pode cancelar (mais de 2 dias)

Processo:
1. Verificar pol�tica de cancelamento
2. Calcular reembolso (se aplic�vel)
3. Cancelar reserva
4. Registrar motivo
5. Processar reembolso
6. Status: Cancelada
```

---

## ?? Solu��o de Problemas

### ? "Data da viagem deve ser futura"
**Solu��o:** Selecionar data posterior a hoje

### ? "Pacote n�o est� ativo"
**Solu��o:** 
- Ativar pacote primeiro
- Ou escolher outro pacote ativo

### ? "N�o � poss�vel cancelar"
**Solu��o:**
- Viagem muito pr�xima (< 2 dias)
- Ou viagem j� realizada
- Pol�tica n�o permite

### ? "Quantidade inv�lida"
**Solu��o:**
- M�nimo: 1 pessoa
- M�ximo: 50 pessoas
- Usar n�mero inteiro

---

## ?? Atalhos

### ?? Teclado
```
Ctrl + N     ? Nova reserva
Ctrl + F     ? Buscar reserva
Ctrl + P     ? Imprimir
```

### ?? URLs
```
/ReservasViagens/Index          ? Todas as reservas
/ReservasViagens/Create         ? Nova reserva
/PacotesViagens/Index           ? Pacotes
```

---

## ?? Pronto para Usar!

O sistema de reservas est� **100% operacional**.

**Acesse:** Menu ? Turismo ? Reservas

---

**Desenvolvido para:** Litoral Sul Locadora e Turismo  
**Vers�o:** 1.0  
**Data:** Outubro/2025  
**Documenta��o relacionada:** 
- [Gest�o de Clientes](CLIENTES_GUIA_COMPLETO.md)
- [Sistema de Loca��es](LOCACOES_GUIA_COMPLETO.md)
