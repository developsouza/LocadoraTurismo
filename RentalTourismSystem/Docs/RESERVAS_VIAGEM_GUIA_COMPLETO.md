# ?? Sistema de Reservas de Viagem - Guia Completo

## ?? Índice
- [Visão Geral](#visão-geral)
- [Pacotes de Viagem](#pacotes-de-viagem)
- [Criar Reserva](#criar-reserva)
- [Gerenciar Reservas](#gerenciar-reservas)
- [Serviços Adicionais](#serviços-adicionais)
- [Status e Workflow](#status-e-workflow)
- [Relatórios](#relatórios)
- [Permissões](#permissões)

---

## ?? Visão Geral

O sistema de reservas de viagem gerencia pacotes turísticos, permitindo que clientes reservem viagens completas com serviços adicionais opcionais.

### ?? Funcionalidades Principais

? **Gestão de Pacotes**
- Cadastro de destinos e roteiros
- Definição de preços e duração
- Controle de disponibilidade
- Ativação/desativação de pacotes

? **Reservas Completas**
- Seleção de pacote e cliente
- Definição de data e quantidade de pessoas
- Cálculo automático de valores
- Adição de serviços extras

? **Controle de Status**
- Pendente ? Confirmada ? Realizada
- Cancelamento de reservas
- Histórico completo
- Alertas e notificações

---

## ?? Pacotes de Viagem

### ?? Como Acessar

**Menu Lateral ? Turismo ? Pacotes de Viagem**

OU

**URL direta:** `/PacotesViagens/Index`

### ?? Cadastrar Novo Pacote

**Pacotes ? ? Novo Pacote**

#### ?? Formulário de Cadastro

| Campo | Formato | Validação | Exemplo |
|-------|---------|-----------|---------|
| **Nome do Pacote** | Texto (máx. 100 caracteres) | Obrigatório | Praias do Litoral Sul |
| **Descrição** | Texto (máx. 1000 caracteres) | Obrigatório | Tour completo pelas praias... |
| **Destino** | Texto (máx. 100 caracteres) | Obrigatório | Guarujá, SP |
| **Duração** | Número | Obrigatório | 3 |
| **Unidade de Tempo** | Seleção | Obrigatório | dias / horas |
| **Preço por Pessoa** | Decimal (R$) | Obrigatório | R$ 850,00 |
| **Ativo** | Checkbox | Padrão: Sim | ? Ativo |

### ?? Exemplo - Cadastro de Pacote

```
NOVO PACOTE DE VIAGEM

Nome: Litoral Norte Completo
Descrição: 
  "Conheça as melhores praias do Litoral Norte de SP. 
   Inclui transporte, guia turístico e almoço.
   Visita a Ubatuba, Caraguatatuba e Ilhabela."

Destino: Litoral Norte - SP
Duração: 2 dias
Unidade: dias

Preço: R$ 650,00 por pessoa

?? Ativo (disponível para venda)

[Salvar Pacote]
```

### ?? Informações do Pacote

**Card do Pacote:**
```
?? Litoral Norte Completo
?????????????????????????????
?? Destino: Litoral Norte - SP
?? Duração: 2 dias
?? R$ 650,00 por pessoa

?? Estatísticas:
   Reservas: 25
   Receita: R$ 48.750,00
   Última reserva: há 3 dias

? ATIVO

[Detalhes] [Editar] [Desativar]
```

### ?? Editar Pacote

**Pacotes ? ?? Editar**

**Campos editáveis:**
- ? Nome e descrição
- ? Destino
- ? Duração e unidade
- ? Preço por pessoa
- ? Status (Ativo/Inativo)

> ?? **Importante:** Alterações no preço NÃO afetam reservas já confirmadas!

### ?? Ativar/Desativar Pacote

**Por que desativar?**
- Pacote fora de temporada
- Destino temporariamente indisponível
- Manutenção de informações
- Pacote sendo reformulado

**Efeitos:**
- ? Não aparece para novas reservas
- ? Reservas existentes não são afetadas
- ? Histórico mantido

### ??? Excluir Pacote

**Regras:**
- ? Pode excluir: Pacote SEM reservas
- ? NÃO pode excluir: Pacote COM reservas

> ?? **Dica:** Use "Desativar" ao invés de excluir pacotes com histórico

---

## ?? Criar Reserva

### ?? Como Acessar

**Opção 1 - Menu:**
```
Menu ? Turismo ? Reservas ? ? Nova Reserva
```

**Opção 2 - A partir do Cliente:**
```
Clientes ? Detalhes ? ?? Nova Reserva
```

**Opção 3 - A partir do Pacote:**
```
Pacotes ? Detalhes ? ? Reservar
```

**URL direta:** `/ReservasViagens/Create`

### ?? Formulário de Reserva

#### 1?? **Seleção - Quem e O que**

| Campo | Descrição | Validação |
|-------|-----------|-----------|
| **Cliente** | Quem vai viajar | Obrigatório, cadastrado |
| **Pacote de Viagem** | Destino escolhido | Obrigatório, ativo |

#### 2?? **Quando e Quantos**

| Campo | Formato | Validação | Exemplo |
|-------|---------|-----------|---------|
| **Data da Viagem** | dd/MM/yyyy | Obrigatório, data futura | 25/12/2024 |
| **Quantidade de Pessoas** | Número | Obrigatório (1-50) | 4 pessoas |

#### 3?? **Valores**

| Campo | Cálculo | Exemplo |
|-------|---------|---------|
| **Preço por Pessoa** | Do pacote | R$ 650,00 |
| **Quantidade** | Informado | 4 pessoas |
| **Valor Total** | Preço × Quantidade | R$ 2.600,00 |

> ?? **Cálculo Automático:** Sistema calcula valor total

#### 4?? **Complementos**

| Campo | Descrição | Exemplo |
|-------|-----------|---------|
| **Observações** | Informações adicionais (opcional) | "Cliente solicitou hotel 5 estrelas" |

### ?? Passo a Passo - Criar Reserva

**Exemplo completo:**

```
1. ACESSE: Reservas ? Nova Reserva

2. SELECIONE CLIENTE:
   Cliente: João Santos
   CPF: 123.456.789-00
   ? Cadastro completo

3. SELECIONE PACOTE:
   Pacote: Litoral Norte Completo
   Destino: Litoral Norte - SP
   Duração: 2 dias
   Preço: R$ 650,00/pessoa
   ? Pacote ativo

4. DEFINA DATA E QUANTIDADE:
   Data da Viagem: 25/12/2024
   Quantidade: 4 pessoas
   
   ?? Sistema calcula:
   ? 4 × R$ 650,00 = R$ 2.600,00

5. OBSERVAÇÕES:
   "Cliente preferencial, família completa"

6. CLIQUE: "Criar Reserva"

7. RESULTADO:
   ? Reserva #12345 criada!
   ? Status: Pendente
   ? Valor: R$ 2.600,00
   
   Próximo: Adicionar serviços extras (opcional)
```

### ? Validações Automáticas

Sistema valida:

1. **Cliente:**
   - ? Cadastrado no sistema
   - ? Dados completos

2. **Pacote:**
   - ? Status ativo
   - ? Disponível

3. **Data:**
   - ? Data futura
   - ? Formato válido

4. **Quantidade:**
   - ? Entre 1 e 50 pessoas
   - ? Número inteiro positivo

5. **Valores:**
   - ? Valor total > 0
   - ? Cálculo correto

---

## ?? Serviços Adicionais

### ?? O que são?

Serviços extras que podem ser adicionados à reserva:
- ?? Hospedagem premium
- ??? Refeições especiais
- ?? Ingressos para atrações
- ?? Transporte privativo
- ?? Fotógrafo profissional
- ?? Equipamentos especiais

### ? Como Adicionar

**Opção 1 - Durante a Criação:**
```
Após criar reserva ? "Adicionar Serviços"
```

**Opção 2 - Depois:**
```
Reservas ? Detalhes ? ? Adicionar Serviço
```

### ?? Cadastrar Serviço Adicional

| Campo | Descrição | Exemplo |
|-------|-----------|---------|
| **Nome** | Descrição do serviço | "Hospedagem Hotel 5 Estrelas" |
| **Descrição** | Detalhes | "2 diárias no Hotel Paradiso" |
| **Preço** | Valor do serviço | R$ 800,00 |

### ?? Exemplo

```
ADICIONAR SERVIÇO ADICIONAL

Reserva: #12345
Cliente: João Santos
Pacote: Litoral Norte Completo

Serviço: Hospedagem Premium
Descrição: Hotel 5 estrelas, vista mar, café da manhã
Preço: R$ 800,00

[Adicionar]

RESUMO ATUALIZADO:
Pacote base: R$ 2.600,00
+ Hospedagem Premium: R$ 800,00
????????????????????????????????
TOTAL: R$ 3.400,00
```

### ?? Gerenciar Serviços

**Visualizar:**
```
Reservas ? Detalhes ? Seção "Serviços Adicionais"

?? Serviços Contratados:
1. Hospedagem Premium - R$ 800,00
2. Passeio de Barco - R$ 350,00
3. Jantar Especial - R$ 250,00

Total Serviços: R$ 1.400,00
Total Geral: R$ 4.000,00
```

**Editar:**
- ?? Alterar descrição
- ?? Ajustar preço
- ??? Remover serviço

---

## ?? Gerenciar Reservas

### ?? Lista de Reservas

**Menu ? Turismo ? Reservas**

### ?? Filtros e Buscas

**Filtros disponíveis:**

| Filtro | Opções |
|--------|--------|
| **Status** | Pendente, Confirmada, Realizada, Cancelada |
| **Período** | Data da viagem |
| **Cliente** | Nome ou CPF |
| **Pacote** | Destino |
| **Data Reserva** | Quando foi reservado |

### ?? Card de Reserva

```
?? Reserva #12345
?????????????????????????????
?? Cliente: João Santos
?? Pacote: Litoral Norte Completo
?? Viagem: 25/12/2024
?? 4 pessoas
?? R$ 3.400,00 (com serviços)

?? Status: CONFIRMADA
??? Faltam 15 dias

[Detalhes] [Editar] [Cancelar]
```

### ?? Indicadores de Status

| Status | Cor | Descrição |
|--------|-----|-----------|
| **Pendente** | ?? Amarelo | Aguardando confirmação |
| **Confirmada** | ?? Verde | Reserva confirmada e paga |
| **Realizada** | ?? Azul | Viagem já aconteceu |
| **Cancelada** | ?? Vermelho | Reserva cancelada |

### ?? Detalhes da Reserva

**Reservas ? Detalhes**

**Informações exibidas:**

```
DETALHES DA RESERVA #12345

?? INFORMAÇÕES GERAIS
?????????????????????????????
Data da Reserva: 10/12/2024
Status Atual: Confirmada
Dias até a viagem: 15

?? CLIENTE
?????????????????????????????
Nome: João Santos
CPF: 123.456.789-00
Telefone: (13) 98765-4321
Email: joao@email.com

?? PACOTE
?????????????????????????????
Pacote: Litoral Norte Completo
Destino: Litoral Norte - SP
Duração: 2 dias
Data da Viagem: 25/12/2024

?? PARTICIPANTES
?????????????????????????????
Quantidade: 4 pessoas
Valor por pessoa: R$ 650,00
Subtotal: R$ 2.600,00

?? SERVIÇOS ADICIONAIS
?????????????????????????????
1. Hospedagem Premium: R$ 800,00
2. Passeio de Barco: R$ 350,00

Total Serviços: R$ 1.150,00

?? RESUMO FINANCEIRO
?????????????????????????????
Pacote base: R$ 2.600,00
Serviços: R$ 1.150,00
????????????????????????????????
TOTAL: R$ 3.750,00

Status Pagamento: ? Pago

?? OBSERVAÇÕES
?????????????????????????????
Cliente preferencial, família completa.
Solicitou quarto com vista para o mar.
```

### ?? Editar Reserva

**Quando permitido:**
- ? Status: Pendente
- ? Viagem futura (não iniciada)
- ? Mais de 2 dias antes da viagem

**Campos editáveis:**
- Data da viagem
- Quantidade de pessoas
- Observações
- Adicionar/remover serviços

**NÃO editável:**
- Cliente (criar nova reserva)
- Pacote (criar nova reserva)

### ? Cancelar Reserva

**Regras de cancelamento:**

```
Pode cancelar quando:
? Status: Pendente ou Confirmada
? Viagem futura
? Mínimo 2 dias antes da viagem

Não pode cancelar:
? Viagem já realizada
? Menos de 2 dias para viagem
? Já cancelada
```

**Processo:**
```
1. Reservas ? Detalhes
2. Clicar em "? Cancelar Reserva"
3. Confirmar ação
4. Sistema:
   - Altera status para "Cancelada"
   - Registra data de cancelamento
   - Mantém histórico
   - Libera vaga do pacote
```

---

## ?? Status e Workflow

### ?? Ciclo de Vida da Reserva

```
1. CRIAÇÃO
   ?
2. PENDENTE (aguardando confirmação)
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
- Reserva recém-criada
- Aguardando confirmação de pagamento

**Ações possíveis:**
- ? Confirmar (após pagamento)
- ? Editar
- ? Cancelar
- ? Adicionar serviços

**Sistema alerta se:**
- Pendente há mais de 7 dias
- Faltam menos de 7 dias para viagem

#### 2?? **CONFIRMADA** ??

**Quando:**
- Pagamento confirmado
- Cliente confirmou presença

**Ações possíveis:**
- ? Adicionar serviços
- ? Editar (limitado)
- ? Cancelar (com condições)

**Sistema alerta:**
- 7 dias antes: "Viagem se aproxima"
- 3 dias antes: "Confirmar preparativos"
- 1 dia antes: "Viagem amanhã!"

#### 3?? **REALIZADA** ??

**Quando:**
- Data da viagem passou
- Viagem foi concluída

**Características:**
- ?? Não editável
- ?? Entra nas estatísticas
- ? Possível avaliar (futuro)

#### 4?? **CANCELADA** ??

**Quando:**
- Cliente cancelou
- Motivos operacionais
- Não confirmado a tempo

**Características:**
- ?? Não editável
- ?? Motivo registrado
- ?? Não conta em receita

### ?? Mudanças de Status

**Manual:**
```
Reservas ? Detalhes ? ?? Alterar Status

Status atual: Pendente
Novo status: Confirmada

Motivo: Pagamento confirmado via PIX

[Confirmar Alteração]
```

**Automático:**
- ?? Viagem passou ? Status vira "Realizada"
- ? Timeout ? Pendente por muito tempo ? Alerta

---

## ?? Relatórios

### ?? Como Acessar

**Menu ? Relatórios ? Reservas de Viagem**

### ?? Tipos de Relatórios

#### 1?? **Reservas por Período**

```
RELATÓRIO: Reservas - Dezembro/2024

Total Reservas: 35
Receita Total: R$ 91.000,00
Ticket Médio: R$ 2.600,00

Por Status:
? Confirmadas: 28 (80%)
?? Pendentes: 5 (14%)
?? Canceladas: 2 (6%)

Detalhamento:
Data Viagem | Cliente       | Pacote          | Pessoas | Valor
25/12/2024  | João Santos  | Litoral Norte   | 4       | R$ 3.400,00
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

#### 3?? **Receita por Mês**

```
Mês          | Reservas | Receita      | Crescimento
Janeiro      | 42       | R$ 105.000   | -
Fevereiro    | 38       | R$ 95.000    | -9.5%
Março        | 45       | R$ 112.500   | +18.4%
...
Dezembro     | 52       | R$ 130.000   | +15.6%

Total Ano: R$ 1.248.000
Média Mensal: R$ 104.000
```

#### 4?? **Análise de Clientes**

```
Clientes Frequentes

Cliente          | Reservas | Valor Total | Última Viagem
João Santos      | 8        | R$ 24.000   | 15/12/2024
Maria Silva      | 6        | R$ 18.500   | 10/12/2024
Ana Costa        | 5        | R$ 15.000   | 05/12/2024

Novos Clientes: 45 (mês atual)
Taxa Recompra: 35%
```

### ?? Filtros Disponíveis

**Todos os relatórios:**
- ?? Período (data viagem ou reserva)
- ?? Status da reserva
- ?? Cliente específico
- ?? Pacote específico
- ?? Quantidade de pessoas

### ?? Exportação

**Formatos:**
- ?? Excel (.xlsx)
- ?? PDF
- ?? CSV
- ??? Impressão

---

## ?? Permissões de Acesso

### ??? **Visualização**
**Quem pode:** Todos os usuários autenticados
- Ver lista de reservas
- Ver detalhes de reserva
- Consultar pacotes
- Ver relatórios básicos

### ?? **Criação e Edição**
**Quem pode:** Admin, Manager, Employee
- Criar nova reserva
- Editar reserva (antes da viagem)
- Adicionar serviços
- Confirmar pagamento
- Alterar status

### ??? **Cancelamento e Exclusão**
**Quem pode:** Admin, Manager
- Cancelar reserva
- Excluir pacote SEM reservas
- Ajustar valores
- Aplicar descontos

### ?? **Gestão de Pacotes**
**Quem pode:** Admin, Manager
- Criar pacotes
- Editar pacotes
- Ativar/desativar
- Definir preços

---

## ? Boas Práticas

### ?? Pacotes

? **Faça:**
- Descrição clara e detalhada
- Preços competitivos e justos
- Atualizar informações regularmente
- Desativar pacotes fora de temporada
- Manter histórico de reservas

? **Evite:**
- Descrições vagas
- Preços desatualizados
- Excluir pacotes com reservas
- Deixar inativos sem motivo

### ?? Reservas

? **Faça:**
- Confirmar dados do cliente
- Validar data da viagem
- Adicionar observações relevantes
- Confirmar pagamento antes de confirmar reserva
- Manter cliente informado

? **Evite:**
- Reservar para data passada
- Confirmar sem pagamento
- Ignorar observações do cliente
- Deixar pendente por muito tempo
- Esquecer de adicionar serviços solicitados

### ?? Status

? **Faça:**
- Atualizar status prontamente
- Registrar motivo de cancelamento
- Confirmar após pagamento
- Marcar como realizada após viagem

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
2. Calcular: 2 × R$ 850 = R$ 1.700
3. Status: Pendente
4. Cliente paga
5. Confirmar reserva
6. Aguardar data da viagem
7. Após 01/01 ? Status: Realizada
```

### Caso 2: Reserva com Serviços

```
Cliente: João Santos
Pacote: Litoral Norte (R$ 650/pessoa)
Data: 25/12/2024
Pessoas: 4
Base: R$ 2.600

Serviços:
+ Hospedagem: R$ 800
+ Passeio de Barco: R$ 350

Total: R$ 3.750

Processo:
1. Criar reserva base
2. Adicionar serviços
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
1. Verificar política de cancelamento
2. Calcular reembolso (se aplicável)
3. Cancelar reserva
4. Registrar motivo
5. Processar reembolso
6. Status: Cancelada
```

---

## ?? Solução de Problemas

### ? "Data da viagem deve ser futura"
**Solução:** Selecionar data posterior a hoje

### ? "Pacote não está ativo"
**Solução:** 
- Ativar pacote primeiro
- Ou escolher outro pacote ativo

### ? "Não é possível cancelar"
**Solução:**
- Viagem muito próxima (< 2 dias)
- Ou viagem já realizada
- Política não permite

### ? "Quantidade inválida"
**Solução:**
- Mínimo: 1 pessoa
- Máximo: 50 pessoas
- Usar número inteiro

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

O sistema de reservas está **100% operacional**.

**Acesse:** Menu ? Turismo ? Reservas

---

**Desenvolvido para:** Litoral Sul Locadora e Turismo  
**Versão:** 1.0  
**Data:** Outubro/2025  
**Documentação relacionada:** 
- [Gestão de Clientes](CLIENTES_GUIA_COMPLETO.md)
- [Sistema de Locações](LOCACOES_GUIA_COMPLETO.md)
