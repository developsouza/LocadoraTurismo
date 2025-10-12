# ?? Sistema de Locações de Veículos - Guia Completo

## ?? Índice
- [Visão Geral](#visão-geral)
- [Criar Nova Locação](#criar-nova-locação)
- [Processo de Locação](#processo-de-locação)
- [Devolução de Veículo](#devolução-de-veículo)
- [Documentos e Contratos](#documentos-e-contratos)
- [Vistoria](#vistoria)
- [Gestão de Locações](#gestão-de-locações)
- [Relatórios](#relatórios)
- [Permissões](#permissões)

---

## ?? Visão Geral

O sistema de locações gerencia todo o processo de aluguel de veículos, desde a reserva até a devolução, incluindo contratos, vistorias e pagamentos.

### ?? Funcionalidades Principais

? **Processo Completo de Locação**
- Seleção de cliente e veículo
- Definição de período de locação
- Cálculo automático de valores
- Geração de contrato
- Vistoria de retirada e devolução
- Registro de quilometragem

? **Controle Financeiro**
- Cálculo de diárias
- Valores de caução
- Extras e adicionais
- Multas por atraso
- Histórico de pagamentos

? **Documentação**
- Contrato de locação (PDF)
- Laudo de vistoria (PDF)
- Registro fotográfico
- Histórico completo

---

## ?? Criar Nova Locação

### ?? Como Acessar

**Opção 1 - Menu Lateral:**
```
Menu ? Locação ? Locações ? ? Nova Locação
```

**Opção 2 - A partir do Cliente:**
```
Clientes ? Detalhes do Cliente ? ?? Nova Locação
```

**Opção 3 - A partir do Veículo:**
```
Veículos ? Detalhes do Veículo ? ? Nova Locação
(apenas se veículo disponível)
```

**URL direta:** `/Locacoes/Create`

### ?? Pré-requisitos

Antes de criar locação, certifique-se que:

#### ? **Cliente**
- [x] Cliente cadastrado no sistema
- [x] CPF válido
- [x] Email e telefone atualizados
- [x] **CNH VÁLIDA** (não vencida)
- [x] Idade mínima 21 anos
- [x] Documentos enviados (CNH, RG, CPF)

#### ? **Veículo**
- [x] Veículo cadastrado
- [x] Status = "Disponível"
- [x] Sem manutenção pendente
- [x] Documentos em dia (CRLV, Seguro)
- [x] Disponível no período desejado

#### ? **Sistema**
- [x] Agência definida
- [x] Funcionário responsável
- [x] Período de locação válido

### ?? Formulário de Locação

#### 1?? **Seleção - Quem e O que**

| Campo | Descrição | Validação |
|-------|-----------|-----------|
| **Cliente** | Selecione o cliente | Obrigatório, CNH válida |
| **Veículo** | Selecione o veículo | Obrigatório, disponível |
| **Funcionário** | Responsável pela locação | Obrigatório |
| **Agência** | Local de retirada/devolução | Obrigatório |

#### 2?? **Período - Quando**

| Campo | Formato | Validação | Exemplo |
|-------|---------|-----------|---------|
| **Data de Retirada** | dd/MM/yyyy HH:mm | Obrigatório, data futura ou hoje | 15/12/2024 10:00 |
| **Data de Devolução** | dd/MM/yyyy HH:mm | Obrigatório, após retirada | 20/12/2024 18:00 |

> ?? **Sistema calcula automaticamente:** Quantidade de diárias

#### 3?? **Quilometragem**

| Campo | Descrição | Validação | Exemplo |
|-------|-----------|-----------|---------|
| **KM na Retirada** | Quilometragem atual do veículo | Obrigatório, ? 0 | 45.000 km |
| **KM na Devolução** | Preenchido na devolução | Automático | - |

> ?? **Automático:** Sistema preenche com KM atual do veículo

#### 4?? **Valores - Quanto**

| Campo | Cálculo | Exemplo |
|-------|---------|---------|
| **Valor da Diária** | Do cadastro do veículo | R$ 150,00 |
| **Quantidade de Diárias** | Calculado automaticamente | 5 diárias |
| **Valor Total** | Diária × Quantidade | R$ 750,00 |

> ?? **Cálculo Automático:** Sistema calcula baseado no período

#### 5?? **Observações**

| Campo | Descrição | Exemplo |
|-------|-----------|---------|
| **Observações** | Informações adicionais (opcional) | "Cliente solicitou seguro adicional" |

### ?? Passo a Passo - Criar Locação

**Exemplo prático:**

```
1. ACESSE: Locações ? Nova Locação

2. SELECIONE CLIENTE:
   Cliente: Maria da Silva
   CPF: 987.654.321-00
   ? CNH válida até: 10/08/2028

3. SELECIONE VEÍCULO:
   Veículo: Gol 1.0 Flex - ABC1D23
   ? Disponível
   Diária: R$ 150,00

4. DEFINA RESPONSÁVEIS:
   Funcionário: João Atendente
   Agência: Agência Centro

5. DEFINA PERÍODO:
   Retirada: 15/12/2024 10:00
   Devolução: 20/12/2024 18:00
   
   ?? Sistema calcula:
   ? 5 diárias completas
   ? Valor Total: R$ 750,00

6. QUILOMETRAGEM:
   KM Retirada: 45.000 (do veículo)

7. OBSERVAÇÕES:
   "Cliente preferencial, 5% desconto aplicado"

8. CLIQUE: "Criar Locação"

9. RESULTADO:
   ? Locação criada com sucesso!
   ? Veículo alterado para status "Locado"
   ? Contrato gerado
   ? Próximo: Fazer vistoria
```

### ? Validações Automáticas

Sistema valida automaticamente:

1. **Cliente:**
   - ? CNH válida (não vencida)
   - ? Idade mínima 21 anos
   - ? Cadastro completo

2. **Veículo:**
   - ? Status "Disponível"
   - ? Sem locação no período
   - ? Documentos em dia

3. **Período:**
   - ? Data devolução após retirada
   - ? Mínimo 1 diária
   - ? Período disponível

4. **Valores:**
   - ? Valor total > 0
   - ? Cálculo correto de diárias
   - ? Sem inconsistências

---

## ?? Processo de Locação

### ?? Fluxo Completo

```
1. RESERVA/AGENDAMENTO
   ?
2. CRIAÇÃO DA LOCAÇÃO
   ?
3. VISTORIA DE RETIRADA
   ?
4. GERAÇÃO DO CONTRATO
   ?
5. RETIRADA DO VEÍCULO
   ?
6. PERÍODO DE LOCAÇÃO
   ?
7. DEVOLUÇÃO DO VEÍCULO
   ?
8. VISTORIA DE DEVOLUÇÃO
   ?
9. CÁLCULO FINAL
   ?
10. FINALIZAÇÃO
```

### 1?? Criação da Locação

**Status:** Criada
**Ações:**
- [x] Preencher formulário
- [x] Validar dados
- [x] Salvar locação
- [x] Alterar status do veículo para "Locado"

**Próximo:** Vistoria de Retirada

### 2?? Vistoria de Retirada

**Objetivo:** Registrar estado do veículo antes da entrega

**Como fazer:**
```
Locações ? Detalhes da Locação ? ?? Vistoria de Retirada
```

**Itens a verificar:**
- ? Lataria (arranhões, amassados)
- ? Vidros (trincas, quebras)
- ? Pneus (estado, calibragem)
- ? Documentos no veículo
- ? Combustível (nível)
- ? Acessórios (macaco, estepe, extintor)
- ? Interior (bancos, painel)
- ? Limpeza geral

**Registro fotográfico:**
- ?? Frente
- ?? Traseira
- ?? Laterais
- ?? Painel
- ?? Detalhes específicos

### 3?? Geração do Contrato

**Automático após salvar locação**

```
Locações ? Detalhes ? ?? Gerar Contrato
```

**Contrato inclui:**
- ? Dados do cliente
- ? Dados do veículo
- ? Período da locação
- ? Valores e condições
- ? Responsabilidades
- ? Termos e condições
- ? Assinaturas

**Formato:** PDF para impressão

### 4?? Retirada do Veículo

**Checklist final:**
- [x] Contrato assinado
- [x] Vistoria aprovada
- [x] Pagamento confirmado (ou caução)
- [x] Cliente com CNH e documentos
- [x] Veículo limpo e abastecido

**Cliente recebe:**
- ?? Cópia do contrato
- ?? Laudo de vistoria
- ?? Chaves do veículo
- ?? Contato de emergência

### 5?? Período de Locação

**Sistema monitora:**
- ? Dias restantes
- ?? Data prevista de devolução
- ?? Status do veículo (Locado)
- ?? Alertas de vencimento

**Cliente pode:**
- ?? Solicitar extensão
- ?? Reportar problemas
- ?? Informar localização (futuro)

---

## ?? Devolução de Veículo

### ?? Como Registrar Devolução

**Locações ? Detalhes da Locação ? ?? Registrar Devolução**

### ?? Processo de Devolução

#### 1?? **Recepção do Veículo**

**Verificar:**
- ? Veículo retornou na data prevista
- ? Cliente trouxe documentos
- ? Chaves entregues

#### 2?? **Registrar Quilometragem**

```
KM na Devolução: 45.850 km

?? Sistema calcula:
   KM Retirada: 45.000 km
   KM Devolução: 45.850 km
   KM Rodados: 850 km
   Média: 170 km/dia
```

> ?? **Automático:** Sistema atualiza KM do veículo

#### 3?? **Vistoria de Devolução**

**Comparar com vistoria de retirada:**

```
Locações ? Detalhes ? ?? Vistoria de Devolução
```

**Verificar:**
- ? Novos danos?
- ? Estado de conservação
- ? Limpeza
- ? Combustível (mesmo nível)
- ? Acessórios completos

**Registrar:**
- ?? Estado geral: Bom / Regular / Ruim
- ?? Observações de danos
- ?? Fotos de novos danos
- ?? Cálculo de multas (se aplicável)

#### 4?? **Cálculo Final**

**Sistema calcula automaticamente:**

```
?? CÁLCULO DE VALORES:

Valor Inicial: R$ 750,00 (5 diárias)

+ Dias extras (se houver):
  Atraso: 2 dias × R$ 150,00 = R$ 300,00

+ Multas (se houver):
  Dano no para-choque: R$ 350,00
  Falta de combustível: R$ 80,00

- Descontos (se houver):
  Cliente fidelidade: -R$ 50,00

= VALOR TOTAL FINAL: R$ 1.430,00

Status: Aguardando Pagamento
```

#### 5?? **Finalização**

**Após confirmação de pagamento:**

```
1. Marcar pagamento como recebido
2. Alterar status veículo:
   De: "Locado"
   Para: "Disponível" (ou "Manutenção" se necessário)
3. Gerar recibo final
4. Enviar comprovante ao cliente
5. Arquivar documentação
```

### ? Situações Especiais

#### **Devolução Antecipada**

**Cliente devolveu antes do prazo:**

```
Locação: 5 diárias (R$ 750,00)
Devolveu: após 3 diárias

Opções:
1. Cobrar valor proporcional (3 dias)
2. Cobrar valor total (depende do contrato)
3. Gerar crédito para próxima locação

? Definir política comercial
```

#### **Devolução Atrasada**

**Cliente atrasou devolução:**

```
Previsto: 20/12/2024 18:00
Devolveu: 22/12/2024 10:00
Atraso: 2 dias

Cálculo:
Base (5 dias): R$ 750,00
Dias extras (2 dias): R$ 300,00
Multa por atraso: R$ 100,00

Total: R$ 1.150,00

?? Sistema alerta atraso automaticamente
```

#### **Veículo com Danos**

**Danos identificados na devolução:**

```
1. Documentar com fotos
2. Descrever danos detalhadamente
3. Calcular custo de reparo:
   - Orçamento oficial
   - Tabela de referência
4. Adicionar à conta do cliente
5. Programar manutenção
6. Acionar seguro (se aplicável)

Próximo: Veículo vai para "Manutenção"
```

---

## ?? Documentos e Contratos

### ?? Geração de Documentos

#### 1?? **Contrato de Locação**

**Acesso:**
```
Locações ? Detalhes ? ?? Gerar Contrato
```

**Conteúdo:**
```
CONTRATO DE LOCAÇÃO DE VEÍCULO

LOCADOR: Litoral Sul Locadora e Turismo
CNPJ: XX.XXX.XXX/XXXX-XX

LOCATÁRIO: [Nome do Cliente]
CPF: [CPF do Cliente]
CNH: [Número da CNH]

VEÍCULO:
Marca/Modelo: [Marca] [Modelo]
Placa: [Placa]
Ano: [Ano]
Cor: [Cor]

PERÍODO:
Retirada: [Data/Hora]
Devolução Prevista: [Data/Hora]

VALORES:
Diária: R$ [Valor]
Quantidade: [X] diárias
Total: R$ [Valor Total]

CONDIÇÕES:
- Quilometragem livre / limitada
- Seguro incluído / não incluído
- Franquia: R$ [Valor]
- Motorista adicional: Sim / Não

RESPONSABILIDADES:
[Cláusulas do contrato]

ASSINATURAS:
_________________________
Locador

_________________________
Locatário

Data: [Data]
```

**Formato:** PDF para impressão

#### 2?? **Laudo de Vistoria**

**Vistoria de Retirada:**
```
Locações ? Detalhes ? ?? Vistoria de Retirada
```

**Vistoria de Devolução:**
```
Locações ? Detalhes ? ?? Vistoria de Devolução
```

**Conteúdo:**
```
LAUDO DE VISTORIA - [RETIRADA/DEVOLUÇÃO]

Veículo: [Marca Modelo - Placa]
Cliente: [Nome]
Data: [Data/Hora]
KM: [Quilometragem]

CHECKLIST:

Carroceria:
[ ] Sem avarias
[ ] Arranhões: [Localização]
[ ] Amassados: [Localização]

Vidros:
[ ] Perfeitos
[ ] Trincados: [Qual]

Pneus:
[ ] Bom estado
[ ] Desgaste: [Nível]
[ ] Calibragem: OK

Interior:
[ ] Limpo
[ ] Conservado
[ ] Danos: [Descrição]

Combustível: [Nível]
Acessórios: [Todos presentes]

FOTOS:
[Imagens anexas]

OBSERVAÇÕES:
[Detalhes adicionais]

_________________________
Vistoriador

_________________________
Cliente
```

**Formato:** PDF com fotos

### ?? Envio de Documentos

**Opções:**
- ?? Email para cliente
- ?? WhatsApp (futuro)
- ?? Download direto
- ??? Impressão

---

## ?? Gestão de Locações

### ?? Lista de Locações

**Menu ? Locação ? Locações ? Ver Todas**

### ?? Filtros e Buscas

**Filtros disponíveis:**

| Filtro | Opções |
|--------|--------|
| **Status** | Ativa, Finalizada, Atrasada, Cancelada |
| **Período** | Data início/fim |
| **Cliente** | Nome ou CPF |
| **Veículo** | Modelo ou Placa |
| **Agência** | Agência específica |
| **Funcionário** | Responsável |

**Busca rápida:**
- Por cliente (nome/CPF)
- Por veículo (modelo/placa)
- Por número de locação

### ?? Visualização

#### **Card de Locação:**
```
?? Locação #12345
?????????????????????????????
?? Cliente: Maria da Silva
?? Veículo: Gol 1.0 - ABC1234
?? 15/12 a 20/12/2024 (5 dias)
?? R$ 750,00
?? Status: ATIVA

?? Devolução em: 3 dias

[Detalhes] [Vistoria] [Contrato]
```

#### **Indicadores de Status:**

| Status | Cor | Significado |
|--------|-----|-------------|
| **Ativa** | ?? Verde | Locação em andamento |
| **Atrasada** | ?? Vermelho | Passou data de devolução |
| **Finalizada** | ?? Azul | Concluída normalmente |
| **Cancelada** | ? Cinza | Cancelada antes da retirada |

### ?? Alertas

**Sistema alerta:**
- ?? Locação atrasada (passou data devolução)
- ?? Devolução hoje
- ?? Devolução em 1-2 dias
- ?? Vistoria pendente
- ?? Pagamento pendente

---

## ?? Relatórios

### ?? Como Acessar

**Menu ? Relatórios ? Locações**

### ?? Tipos de Relatórios

#### 1?? **Locações por Período**

```
RELATÓRIO: Locações - Dezembro/2024

Total de Locações: 45
Receita Total: R$ 67.500,00
Ticket Médio: R$ 1.500,00

Detalhamento:
Data       | Cliente        | Veículo      | Valor
15/12/2024 | Maria Silva   | Gol ABC1234  | R$ 750,00
16/12/2024 | João Santos   | HB20 DEF567  | R$ 900,00
...
```

**Filtros:**
- ?? Data início/fim
- ?? Agência
- ?? Cliente
- ?? Veículo
- ?? Status

#### 2?? **Receita por Veículo**

```
Veículo          | Locações | Dias | Receita
Gol ABC1234      | 12       | 85   | R$ 12.750,00
HB20 DEF5678     | 9        | 67   | R$ 10.050,00
Onix GHI9012     | 8        | 54   | R$ 8.100,00
```

#### 3?? **Clientes Frequentes**

```
Cliente          | Locações | Valor Total | Última Locação
Maria Silva      | 15       | R$ 22.500   | 15/12/2024
João Santos      | 12       | R$ 18.000   | 10/12/2024
Ana Costa        | 8        | R$ 12.000   | 05/12/2024
```

#### 4?? **Locações Ativas**

```
LOCAÇÕES ATIVAS - Hoje: 20/12/2024

Total Ativas: 7 veículos locados

Cliente     | Veículo     | Devolução   | Status
Maria       | Gol ABC     | Hoje 18:00  | ?? Devolve hoje
João        | HB20 DEF    | 22/12       | ?? No prazo
Ana         | Onix GHI    | 18/12       | ?? ATRASADA!
```

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
- Ver lista de locações
- Ver detalhes de locação
- Visualizar contratos
- Consultar histórico

### ?? **Criação e Edição**
**Quem pode:** Admin, Manager, Employee
- Criar nova locação
- Editar locação (antes da retirada)
- Registrar devolução
- Fazer vistoria
- Gerar contratos

### ??? **Cancelamento**
**Quem pode:** Admin, Manager
- Cancelar locação (antes da retirada)
- Ajustar valores
- Aplicar descontos

> ?? **Importante:** Locações em andamento ou finalizadas NÃO podem ser excluídas!

---

## ? Boas Práticas

### ?? Ao Criar Locação

? **Faça:**
- Verificar CNH do cliente (válida)
- Confirmar disponibilidade do veículo
- Validar período de locação
- Fazer vistoria completa
- Gerar contrato antes da entrega
- Registrar fotos do veículo

? **Evite:**
- Criar sem verificar CNH
- Locar veículo em manutenção
- Pular etapa de vistoria
- Entregar sem contrato assinado
- Não registrar danos existentes

### ?? Durante a Locação

? **Faça:**
- Monitorar data de devolução
- Manter contato com cliente (se atraso)
- Verificar alertas do sistema
- Estar pronto para devolução

? **Evite:**
- Ignorar atrasos
- Não avisar cliente sobre vencimento
- Descuidar do acompanhamento

### ?? Na Devolução

? **Faça:**
- Vistoria completa e detalhada
- Registrar quilometragem correta
- Documentar TODOS os danos (novos)
- Calcular valores corretamente
- Confirmar pagamento antes de liberar
- Alterar status do veículo

? **Evite:**
- Vistoria superficial
- Ignorar pequenos danos
- Liberar sem pagamento
- Esquecer de atualizar status
- Não documentar problemas

---

## ?? Casos de Uso

### Caso 1: Locação Simples

**Cenário:** Cliente retira carro para fim de semana

```
1. Cliente: Maria Silva (CNH válida)
2. Veículo: Gol ABC1234 (Disponível)
3. Período: 15/12 a 17/12 (3 dias)
4. Valor: R$ 450,00

Processo:
? Criar locação
? Vistoria de retirada (OK)
? Gerar contrato
? Cliente retira veículo
? Aguardar devolução (17/12)
? Cliente devolve no prazo
? Vistoria devolução (OK)
? Confirmar pagamento
? Finalizar locação
```

### Caso 2: Locação com Atraso

**Cenário:** Cliente atrasa devolução

```
Previsto: 20/12/2024
Devolveu: 22/12/2024 (2 dias atraso)

Cálculo:
5 dias originais: R$ 750,00
2 dias extras: R$ 300,00
Multa atraso: R$ 100,00
Total: R$ 1.150,00

Ações:
1. Sistema alerta atraso automaticamente
2. Contatar cliente
3. Registrar devolução
4. Calcular valores extras
5. Confirmar pagamento total
6. Finalizar
```

### Caso 3: Veículo com Danos

**Cenário:** Cliente devolveu com dano

```
Vistoria Devolução:
?? Dano identificado: Arranhão na lateral

Procedimento:
1. Documentar com fotos
2. Descrever dano detalhadamente
3. Fazer orçamento de reparo
4. Adicionar custo à conta:
   Locação: R$ 750,00
   Reparo: R$ 350,00
   Total: R$ 1.100,00
5. Negociar com cliente
6. Confirmar pagamento
7. Programar reparo
8. Veículo ? Status "Manutenção"
```

---

## ?? Solução de Problemas

### ? "CNH do cliente vencida"
**Solução:** 
- Não permitir locação
- Solicitar CNH atualizada
- Atualizar cadastro do cliente

### ? "Veículo indisponível"
**Solução:**
- Verificar status do veículo
- Consultar outras locações no período
- Sugerir veículo similar

### ? "Erro ao gerar contrato"
**Solução:**
- Verificar dados completos
- Confirmar template do contrato
- Tentar novamente

### ? "Cliente não devolveu"
**Solução:**
- Contatar cliente imediatamente
- Verificar localização (se disponível)
- Acionar seguradora (se necessário)
- Polícia (casos extremos)

---

## ?? Atalhos

### ?? Teclado
```
Ctrl + N     ? Nova locação
Ctrl + F     ? Buscar locação
Ctrl + P     ? Imprimir contrato
```

### ?? URLs
```
/Locacoes/Index          ? Todas as locações
/Locacoes/Create         ? Nova locação
/Locacoes/Details/{id}   ? Detalhes
```

---

## ?? Pronto para Usar!

O sistema de locações está **100% operacional**.

**Acesse:** Menu ? Locação ? Locações

---

**Desenvolvido para:** Litoral Sul Locadora e Turismo  
**Versão:** 1.0  
**Data:** Outubro/2025  
**Documentação relacionada:** 
- [Gestão de Clientes](CLIENTES_GUIA_COMPLETO.md)
- [Gestão de Veículos](VEICULOS_GUIA_COMPLETO.md)
- [Sistema de Manutenções](MANUTENCAO_GUIA_ACESSO.md)
