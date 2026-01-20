# 📚 Guia Visual do Sistema - Fluxogramas e Processos

## 📚 Índice
- [Visão Geral](#visão-geral)
- [Fluxogramas de Processos](#fluxogramas-de-processos)
- [Diagramas de Navegação](#diagramas-de-navegação)
- [Status e Estados](#status-e-estados)
- [Hierarquia de Permissões](#hierarquia-de-permissões)

---

## 📚 Visão Geral

Este guia apresenta visualmente os principais fluxos e processos do sistema através de diagramas e fluxogramas.

---

## 📚 Fluxogramas de Processos

### 1📚 Processo Completo de Locação

```
INÍCIO
  ✅
  ✅
📚📚📚📚📚📚📚📚📚✅
✅ Cliente chega   ✅
✅ na agência      ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅      NÃO    📚📚📚📚📚📚📚📚
✅ Cliente está    📚📚📚📚📚📚📚📚 Cadastrar    ✅
✅ cadastrado✅     ✅              ✅ Cliente      ✅
📚📚📚📚📚📚📚📚📚✅              📚📚📚📚📚📚📚📚
         ✅ SIM                          ✅
         ✅                              ✅
         📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅      NÃO    📚📚📚📚📚📚📚📚
✅ CNH válida✅     📚📚📚📚📚📚📚📚 Solicitar    ✅
📚📚📚📚📚📚📚📚📚✅              ✅ CNH válida   ✅
         ✅ SIM                   📚📚📚📚📚📚📚📚
         ✅                              ✅
📚📚📚📚📚📚📚📚📚✅                     ✅
✅ Selecionar      ✅                     ✅
✅ Veículo         ✅                     ✅
📚📚📚📚📚📚📚📚📚✅                     ✅
         ✅                              ✅
         ✅                              ✅
📚📚📚📚📚📚📚📚📚✅      NÃO           ✅
✅ Veículo         📚📚📚📚📚📚📚📚📚📚📚
✅ disponível✅     ✅
📚📚📚📚📚📚📚📚📚✅
         ✅ SIM
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Definir período ✅
✅ de locação      ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Sistema calcula ✅
✅ valor total     ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Criar locação   ✅
✅ no sistema      ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Fazer vistoria  ✅
✅ de retirada     ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Gerar contrato  ✅
✅ de locação      ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Cliente assina  ✅
✅ contrato        ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅      NÃO    📚📚📚📚📚📚📚📚
✅ Pagamento       📚📚📚📚📚📚📚📚 Processar    ✅
✅ confirmado✅     ✅              ✅ pagamento    ✅
📚📚📚📚📚📚📚📚📚✅              📚📚📚📚📚📚📚📚
         ✅ SIM                          ✅
         ✅                              ✅
         📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Entregar chaves ✅
✅ ao cliente      ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Status veículo  ✅
✅ ✅ LOCADO        ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Cliente retira  ✅
✅ veículo         ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
    [FIM RETIRADA]
         ✅
         ✅ (Período de locação)
         ✅
         ✅
    [INÍCIO DEVOLUÇÃO]
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Cliente retorna ✅
✅ com veículo     ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Fazer vistoria  ✅
✅ de devolução    ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Registrar KM    ✅
✅ de devolução    ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅      SIM    📚📚📚📚📚📚📚📚
✅ Há novos danos✅ 📚📚📚📚📚📚📚📚 Calcular     ✅
📚📚📚📚📚📚📚📚📚✅              ✅ custo reparo ✅
         ✅ NÃO                   📚📚📚📚📚📚📚📚
         ✅                              ✅
         📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅      SIM    📚📚📚📚📚📚📚📚
✅ Há atraso na    📚📚📚📚📚📚📚📚 Calcular     ✅
✅ devolução✅      ✅              ✅ multa        ✅
📚📚📚📚📚📚📚📚📚✅              📚📚📚📚📚📚📚📚
         ✅ NÃO                          ✅
         ✅                              ✅
         📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Calcular valor  ✅
✅ final total     ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅      NÃO    📚📚📚📚📚📚📚📚
✅ Pagamento final 📚📚📚📚📚📚📚📚 Processar    ✅
✅ confirmado✅     ✅              ✅ pagamento    ✅
📚📚📚📚📚📚📚📚📚✅              📚📚📚📚📚📚📚📚
         ✅ SIM                          ✅
         ✅                              ✅
         📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Finalizar       ✅
✅ locação         ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅      SIM    📚📚📚📚📚📚📚📚
✅ Veículo precisa 📚📚📚📚📚📚📚📚 Status ✅     ✅
✅ manutenção✅     ✅              ✅ MANUTENÇÃO   ✅
📚📚📚📚📚📚📚📚📚✅              📚📚📚📚📚📚📚📚
         ✅ NÃO                          ✅
         ✅                              ✅
📚📚📚📚📚📚📚📚📚✅                     ✅
✅ Status veículo  📚📚📚📚📚📚📚📚📚📚📚✅
✅ ✅ DISPONÍVEL    ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
       [FIM]
```

---

### 2📚 Processo de Manutenção de Veículo

```
INÍCIO
  ✅
  ✅
📚📚📚📚📚📚📚📚📚✅      SIM    📚📚📚📚📚📚📚📚
✅ Veículo está    📚📚📚📚📚📚📚📚 Aguardar     ✅
✅ em locação✅     ✅              ✅ devolução    ✅
📚📚📚📚📚📚📚📚📚✅              📚📚📚📚📚📚📚📚
         ✅ NÃO                          ✅
         ✅                              ✅
         📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Alterar status  ✅
✅ ✅ MANUTENÇÃO    ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Criar registro  ✅
✅ de manutenção   ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Definir tipo:   ✅
✅ ✅ Preventiva    ✅
✅ ✅ Corretiva     ✅
✅ ✅ Urgente       ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Informar:       ✅
✅ - Descrição     ✅
✅ - KM atual      ✅
✅ - Custo estimado✅
✅ - Oficina       ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Enviar veículo  ✅
✅ para oficina    ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Status ✅        ✅
✅ EM ANDAMENTO    ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
    (Aguardando)
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Manutenção      ✅
✅ concluída✅      ✅
📚📚📚📚📚📚📚📚📚✅
         ✅ SIM
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Adicionar itens ✅
✅ e serviços      ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Registrar:      ✅
✅ - Peças trocadas✅
✅ - Custo real    ✅
✅ - NF oficina    ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Status ✅        ✅
✅ CONCLUÍDA       ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Atualizar KM    ✅
✅ do veículo      ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅      SIM    📚📚📚📚📚📚📚📚
✅ Veículo OK para 📚📚📚📚📚📚📚📚 Status ✅     ✅
✅ voltar frota✅   ✅              ✅ DISPONÍVEL   ✅
📚📚📚📚📚📚📚📚📚✅              📚📚📚📚📚📚📚📚
         ✅ NÃO                          ✅
         ✅                              ✅
📚📚📚📚📚📚📚📚📚✅                     ✅
✅ Aguardar nova   ✅                     ✅
✅ manutenção      ✅                     ✅
📚📚📚📚📚📚📚📚📚✅                     ✅
         ✅                              ✅
         ✅                              ✅
         📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
         ✅
         ✅
       [FIM]
```

---

### 3📚 Fluxo de Reserva de Viagem

```
INÍCIO
  ✅
  ✅
📚📚📚📚📚📚📚📚📚✅
✅ Cliente         ✅
✅ interessado     ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅      NÃO    📚📚📚📚📚📚📚📚
✅ Cliente         📚📚📚📚📚📚📚📚 Cadastrar    ✅
✅ cadastrado✅     ✅              ✅ cliente      ✅
📚📚📚📚📚📚📚📚📚✅              📚📚📚📚📚📚📚📚
         ✅ SIM                          ✅
         ✅                              ✅
         📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Consultar       ✅
✅ pacotes ativos  ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Cliente escolhe ✅
✅ pacote          ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Definir:        ✅
✅ - Data viagem   ✅
✅ - Qtd pessoas   ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Sistema calcula ✅
✅ valor total     ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅      SIM    📚📚📚📚📚📚📚📚
✅ Cliente quer    📚📚📚📚📚📚📚📚 Adicionar    ✅
✅ serviços extras📚              ✅ serviços     ✅
📚📚📚📚📚📚📚📚📚✅              📚📚📚📚📚📚📚📚
         ✅ NÃO                          ✅
         ✅                              ✅
         📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Recalcular      ✅
✅ valor total     ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Criar reserva   ✅
✅ Status: PENDENTE✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅      NÃO    📚📚📚📚📚📚📚📚
✅ Cliente confirma📚📚📚📚📚📚📚📚 Aguardar     ✅
✅ e paga✅         ✅              ✅ confirmação  ✅
📚📚📚📚📚📚📚📚📚✅              📚📚📚📚📚📚📚📚
         ✅ SIM                          ✅
         ✅                              ✅
         📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Status ✅        ✅
✅ CONFIRMADA      ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Enviar          ✅
✅ confirmação     ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
    (Aguardar data)
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Data da viagem  ✅
✅ chegou          ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Realizar        ✅
✅ viagem          ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
📚📚📚📚📚📚📚📚📚✅
✅ Status ✅        ✅
✅ REALIZADA       ✅
📚📚📚📚📚📚📚📚📚✅
         ✅
         ✅
       [FIM]
```

---

## 📚✅ Diagramas de Navegação

### 📚 Menu Principal do Sistema

```
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
✅           SISTEMA LITORAL SUL                ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
✅                                             ✅
✅  📚 INÍCIO                                  ✅
✅     📚✅ Dashboard                           ✅
✅                                             ✅
✅  📚 LOCAÇÃO                                 ✅
✅     📚✅ 📚 Clientes                         ✅
✅     ✅   📚✅ Ver Todos                       ✅
✅     ✅   📚✅ Novo Cliente                    ✅
✅     ✅   📚✅ Buscar                          ✅
✅     ✅                                       ✅
✅     📚✅ 📚 Veículos                         ✅
✅     ✅   📚✅ Ver Todos                       ✅
✅     ✅   📚✅ Novo Veículo                    ✅
✅     ✅   📚✅ Disponibilidade                 ✅
✅     ✅   📚✅ Status da Frota                 ✅
✅     ✅                                       ✅
✅     📚✅ 📚 Locações                         ✅
✅     ✅   📚✅ Ver Todas                       ✅
✅     ✅   📚✅ Nova Locação                    ✅
✅     ✅   📚✅ Ativas                          ✅
✅     ✅   📚✅ Atrasadas                       ✅
✅     ✅                                       ✅
✅     📚✅ 📚 Manutenções                      ✅
✅         📚✅ Ver Todas                       ✅
✅         📚✅ Nova Manutenção (Admin/Manager)✅
✅         📚✅ Relatório de Custos            ✅
✅                                             ✅
✅  📚 TURISMO                                 ✅
✅     📚✅ 📚 Pacotes de Viagem                ✅
✅     ✅   📚✅ Ver Todos                       ✅
✅     ✅   📚✅ Novo Pacote                     ✅
✅     ✅   📚✅ Ativos/Inativos                 ✅
✅     ✅                                       ✅
✅     📚✅ 📚 Reservas                         ✅
✅         📚✅ Ver Todas                       ✅
✅         📚✅ Nova Reserva                    ✅
✅         📚✅ Confirmadas                     ✅
✅         📚✅ Pendentes                       ✅
✅                                             ✅
✅  📚📚✅ GESTÃO (Admin/Manager)                 ✅
✅     📚✅ 📚 Funcionários                     ✅
✅     📚✅ 📚 Agências                         ✅
✅     📚✅ 📚 Usuários                         ✅
✅                                             ✅
✅  📚 RELATÓRIOS                              ✅
✅     📚✅ 📚 Receita                          ✅
✅     📚✅ 📚 Veículos Mais Alugados           ✅
✅     📚✅ 📚 Clientes Frequentes              ✅
✅     📚✅ 📚 Custos de Manutenção             ✅
✅     📚✅ 📚 Pacotes Mais Vendidos            ✅
✅                                             ✅
✅  📚 CONFIGURAÇÕES (Admin)                   ✅
✅     📚✅ 📚 Segurança                        ✅
✅     📚✅ 📚 Backup                           ✅
✅     📚✅ 📚 Notificações                     ✅
✅     📚✅ 📚 Logs do Sistema                  ✅
✅                                             ✅
✅  📚 MEU PERFIL                              ✅
✅     📚✅ 📚 Meus Dados                       ✅
✅     📚✅ 📚 Alterar Senha                    ✅
✅     📚✅ 📚 Sair                             ✅
✅                                             ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
```

---

## 📚 Status e Estados

### 📚 Status de Veículo

```
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
✅         CICLO DE STATUS DO VEÍCULO       ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅

         📚📚📚📚📚📚📚📚
    📚📚📚  DISPONÍVEL  📚📚📚
    ✅    📚📚📚📚📚📚📚📚    ✅
    ✅           ✅            ✅
    ✅           ✅            ✅
    ✅    📚📚📚📚📚📚📚📚    ✅
    ✅    ✅    LOCADO    ✅    ✅
    ✅    📚📚📚📚📚📚📚📚    ✅
    ✅           ✅            ✅
    ✅           ✅            ✅
    ✅    📚📚📚📚📚📚📚📚    ✅
    ✅    ✅ MANUTENÇÃO   ✅    ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
✅INATIVO ✅       ✅        ✅RESERVADO✅
📚📚📚📚📚       ✅        📚📚📚📚📚
    ✅     📚📚📚📚📚📚📚📚     ✅
    📚📚📚✅  DISPONÍVEL  📚📚📚✅
          📚📚📚📚📚📚📚📚

CORES:
📚 DISPONÍVEL  - Verde  - Pronto para locação
📚 LOCADO      - Azul   - Em uso por cliente
📚 MANUTENÇÃO  - Amarelo- Em reparo/revisão
📚 RESERVADO   - Roxo   - Reservado futuro
📚 INATIVO     - Vermelho- Fora de operação
```

### 📚 Status de Locação

```
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
✅        CICLO DE STATUS DA LOCAÇÃO        ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅

          📚📚📚📚📚📚📚📚
          ✅   CRIADA     ✅
          📚📚📚📚📚📚📚📚
                 ✅
                 ✅
          📚📚📚📚📚📚📚📚
          ✅    ATIVA     ✅
          📚📚📚📚📚📚📚📚
                 ✅
          📚📚📚📚📚📚📚📚
          ✅              ✅
   📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
   ✅  ATRASADA    📚  NO PRAZO    ✅
   📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
          📚📚📚📚📚📚📚📚✅
                 ✅
          📚📚📚📚📚📚📚📚
          ✅  FINALIZADA  ✅
          📚📚📚📚📚📚📚📚

CORES:
📚 ATIVA      - Verde  - Locação em andamento
📚 ATRASADA   - Vermelho- Passou data devolução
📚 FINALIZADA - Azul   - Concluída normalmente
✅ CANCELADA  - Cinza  - Cancelada antes da retirada
```

### 📚 Status de Reserva de Viagem

```
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
✅    CICLO DE STATUS DA RESERVA VIAGEM     ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅

          📚📚📚📚📚📚📚📚
          ✅   PENDENTE   ✅
          📚📚📚📚📚📚📚📚
                 ✅
          📚📚📚📚📚📚📚📚
          ✅              ✅
   📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
   ✅ CONFIRMADA   📚  CANCELADA   ✅
   📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
          ✅              ✅
          ✅            [FIM]
   📚📚📚📚📚📚📚📚
   ✅  REALIZADA   ✅
   📚📚📚📚📚📚📚📚
          ✅
        [FIM]

CORES:
📚 PENDENTE   - Amarelo - Aguardando confirmação
📚 CONFIRMADA - Verde   - Paga e confirmada
📚 REALIZADA  - Azul    - Viagem aconteceu
📚 CANCELADA  - Vermelho- Cliente cancelou
```

### 📚 Status de Manutenção

```
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
✅      CICLO DE STATUS DA MANUTENÇÃO       ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅

          📚📚📚📚📚📚📚📚
          ✅  AGENDADA    ✅
          📚📚📚📚📚📚📚📚
                 ✅
                 ✅
          📚📚📚📚📚📚📚📚
          ✅ EM ANDAMENTO ✅
          📚📚📚📚📚📚📚📚
                 ✅
          📚📚📚📚📚📚📚📚
          ✅              ✅
   📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
   ✅  CONCLUÍDA   📚  CANCELADA   ✅
   📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚

CORES:
📚 AGENDADA      - Amarelo - Programada
📚 EM ANDAMENTO  - Azul    - Em execução
📚 CONCLUÍDA     - Verde   - Finalizada
📚 CANCELADA     - Vermelho- Não realizada
```

---

## 📚 Hierarquia de Permissões

### 📚 Pirâmide de Acesso

```
                    📚📚📚📚📚📚📚
                    ✅   ADMIN    ✅
                    ✅  (Acesso   ✅
                    ✅   Total)   ✅
                    📚📚📚📚📚📚📚
                          ✅
              📚📚📚📚📚📚📚📚📚📚📚📚✅
              ✅                       ✅
        📚📚📚📚📚📚📚         📚📚📚📚📚📚📚
        ✅  MANAGER   ✅         ✅  MANAGER   ✅
        ✅ (Gerencial)✅         ✅ (Gerencial)✅
        📚📚📚📚📚📚📚         📚📚📚📚📚📚📚
              ✅                       ✅
    📚📚📚📚📚📚📚📚📚📚✅   📚📚📚📚📚📚📚📚📚✅
    ✅                   ✅   ✅                 ✅
📚📚📚📚📚         📚📚📚📚📚           📚📚📚📚📚✅
✅EMPLOYEE✅         ✅EMPLOYEE✅           ✅EMPLOYEE ✅
✅(Opera- ✅         ✅(Opera- ✅           ✅(Opera-  ✅
✅cional) ✅         ✅cional) ✅           ✅cional)  ✅
📚📚📚📚📚         📚📚📚📚📚           📚📚📚📚📚✅
    ✅                   ✅                     ✅
    📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
                ✅                  ✅
            📚📚📚📚📚         📚📚📚📚📚
            ✅  USER  ✅         ✅  USER  ✅
            ✅(Consul-✅         ✅(Consul-✅
            ✅  ta)   ✅         ✅  ta)   ✅
            📚📚📚📚📚         📚📚📚📚📚
```

### 📚 Matriz de Permissões Visual

```
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
✅         RECURSOS × ROLES (Permissões)               ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
✅   RECURSO    ✅ADMIN ✅ MANAGER ✅ EMPLOYEE ✅   USER   ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
✅ Clientes     ✅      ✅         ✅          ✅          ✅
✅  - Ver       ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
✅  - Criar     ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
✅  - Editar    ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
✅  - Excluir   ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
✅ Veículos     ✅      ✅         ✅          ✅          ✅
✅  - Ver       ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
✅  - Criar     ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
✅  - Editar    ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
✅  - Excluir   ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
✅  - Status    ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
✅ Locações     ✅      ✅         ✅          ✅          ✅
✅  - Ver       ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
✅  - Criar     ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
✅  - Editar    ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
✅  - Excluir   ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
✅ Manutenções  ✅      ✅         ✅          ✅          ✅
✅  - Ver       ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
✅  - Criar     ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
✅  - Editar    ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
✅  - Excluir   ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
✅  - Relatório ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
✅ Usuários     ✅      ✅         ✅          ✅          ✅
✅  - Ver       ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
✅  - Criar     ✅  ✅  ✅   ✅    ✅    ✅    ✅    ✅    ✅
✅  - Editar    ✅  ✅  ✅   ✅*   ✅    ✅    ✅    ✅    ✅
✅  - Excluir   ✅  ✅  ✅   ✅*   ✅    ✅    ✅    ✅    ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅

* Manager não pode editar/excluir Admin

LEGENDA:
✅ = Permitido
✅ = Negado
```

---

## 📚 Fluxo de Decisão - Permissões

```
Usuário tenta acessar recurso
         ✅
         ✅
    📚📚📚📚📚✅
    ✅Autenti- ✅     NÃO
    ✅cado✅    📚📚📚📚📚 Redirecionar para Login
    📚📚📚📚📚✅
         ✅ SIM
         ✅
    📚📚📚📚📚✅
    ✅Tem role ✅     NÃO
    ✅necessá- 📚📚📚📚📚 Acesso Negado (403)
    ✅ria✅     ✅
    📚📚📚📚📚✅
         ✅ SIM
         ✅
    📚📚📚📚📚✅
    ✅Recurso  ✅     NÃO
    ✅existe✅  📚📚📚📚📚 Não Encontrado (404)
    📚📚📚📚📚✅
         ✅ SIM
         ✅
    📚📚📚📚📚✅
    ✅Permis-  ✅     NÃO
    ✅são espe-📚📚📚📚📚 Acesso Negado (403)
    ✅cífica✅  ✅
    📚📚📚📚📚✅
         ✅ SIM
         ✅
   [ACESSO PERMITIDO]
```

---

## 📚 Responsividade e Adaptação

### 📚✅ Layout por Dispositivo

```
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
✅              DESKTOP (> 992px)              ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
✅  📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚  ✅
✅  ✅      ✅                               ✅  ✅
✅  ✅ SIDE ✅        CONTEÚDO PRINCIPAL     ✅  ✅
✅  ✅ BAR  ✅                               ✅  ✅
✅  ✅      ✅                               ✅  ✅
✅  ✅      ✅                               ✅  ✅
✅  📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚  ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚

📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
✅             TABLET (768px - 991px)          ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
✅  📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚  ✅
✅  ✅   [✅] MENU                           ✅  ✅
✅  📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚  ✅
✅  ✅                                      ✅  ✅
✅  ✅      CONTEÚDO PRINCIPAL              ✅  ✅
✅  ✅                                      ✅  ✅
✅  ✅                                      ✅  ✅
✅  📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚  ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚

📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
✅           SMARTPHONE (< 768px)              ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
✅  📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚  ✅
✅  ✅  [✅]  LOGO                           ✅  ✅
✅  📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚  ✅
✅  ✅                                      ✅  ✅
✅  ✅                                      ✅  ✅
✅  ✅   CONTEÚDO                           ✅  ✅
✅  ✅   EMPILHADO                          ✅  ✅
✅  ✅                                      ✅  ✅
✅  ✅                                      ✅  ✅
✅  📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚  ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚
```

---

## 📚 Paleta de Cores do Sistema

```
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
✅          CORES PRINCIPAIS                ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
✅                                         ✅
✅  📚 PRIMARY (Azul)                      ✅
✅  📚 #0d6efd (Bootstrap Primary)        ✅
✅  📚 Botões principais                  ✅
✅  📚 Links e destaques                  ✅
✅                                         ✅
✅  📚 SUCCESS (Verde)                     ✅
✅  📚 #198754 (Bootstrap Success)        ✅
✅  📚 Ações positivas                    ✅
✅  📚 Status "Disponível", "Confirmada"  ✅
✅  📚 Mensagens de sucesso               ✅
✅                                         ✅
✅  📚 WARNING (Amarelo)                   ✅
✅  📚 #ffc107 (Bootstrap Warning)        ✅
✅  📚 Alertas importantes                ✅
✅  📚 Status "Pendente", "Agendada"      ✅
✅  📚 Avisos ao usuário                  ✅
✅                                         ✅
✅  📚 DANGER (Vermelho)                   ✅
✅  📚 #dc3545 (Bootstrap Danger)         ✅
✅  📚 Ações destrutivas                  ✅
✅  📚 Status "Cancelada", "Atrasada"     ✅
✅  📚 Erros e validações                 ✅
✅                                         ✅
✅  📚 INFO (Azul Claro)                   ✅
✅  📚 #0dcaf0 (Bootstrap Info)           ✅
✅  📚 Informações gerais                 ✅
✅  📚 Status "Em Andamento"              ✅
✅  📚 Tooltips e ajudas                  ✅
✅                                         ✅
✅  ✅ SECONDARY (Cinza)                   ✅
✅  📚 #6c757d (Bootstrap Secondary)      ✅
✅  📚 Botões secundários                 ✅
✅  📚 Status "Inativa", "Cancelada"      ✅
✅  📚 Texto de apoio                     ✅
✅                                         ✅
📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚📚✅
```

---

## 📚 Resumo Visual

### ✅ Principais Fluxos Implementados

```
✅ AUTENTICAÇÃO
   📚✅ Login ✅ Dashboard ✅ Operação

✅ LOCAÇÃO
   📚✅ Cliente ✅ Veículo ✅ Período ✅ Vistoria ✅ Contrato ✅ Locação

✅ DEVOLUÇÃO
   📚✅ Vistoria ✅ KM ✅ Cálculos ✅ Pagamento ✅ Finalização

✅ MANUTENÇÃO
   📚✅ Status ✅ Registro ✅ Oficina ✅ Itens ✅ Conclusão ✅ Disponível

✅ RESERVA VIAGEM
   📚✅ Pacote ✅ Cliente ✅ Data ✅ Serviços ✅ Pagamento ✅ Confirmação

✅ GESTÃO
   📚✅ Usuários ✅ Permissões ✅ Relatórios ✅ Configurações
```

---

**Sistema:** Litoral Sul Locadora e Turismo  
**Versão:** 1.0  
**Última Atualização:** Outubro/2025  
**Tipo de Documento:** Guia Visual de Processos

