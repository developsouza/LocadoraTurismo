# 🔧 Sistema de Manutenção Veicular - Guia de Acesso

## 📍 Como Acessar o Sistema de Manutenção

### 🎯 Menu Principal (Sidebar)

O sistema de manutenção está disponível no menu lateral esquerdo:

```
📂 Locação
   ├── 👥 Clientes
   ├── 🚗 Veículos
   ├── 📄 Locações
   │   ├── Ver Todas
   │   └── Nova Locação
   └── 🔧 Manutenções  ← NOVO!
       ├── Ver Todas
       ├── Nova Manutenção (Admin/Manager)
       └── Relatório de Custos (Admin/Manager)
```

### 🚗 A partir de Veículos

#### **1. Lista de Veículos** (`/Veiculos/Index`)
Cada veículo na lista possui um botão de **Manutenções**:
- 🔍 Detalhes
- ✏️ Editar (Admin/Manager)
- **🔧 Manutenções (Admin/Manager)** ← NOVO!
- ➕ Nova Locação (se disponível)

#### **2. Detalhes do Veículo** (`/Veiculos/Details/{id}`)
No painel de **Ações Rápidas** (lado direito):
- ➕ Nova Locação (se disponível)
- **📜 Histórico de Manutenções** ← NOVO!
- **🔧 Nova Manutenção (Admin/Manager)** ← NOVO!
- 🔄 Alterar Status (Admin/Manager)
- 📅 Verificar Disponibilidade

## 🎯 Fluxo de Uso Completo

### 📊 Cenário 1: Visualizar Histórico de Manutenções

1. **Opção A - Pelo Menu:**
   - Menu Lateral → Manutenções → Ver Todas
   - Filtrar por veículo desejado

2. **Opção B - Pela Lista de Veículos:**
   - Veículos → Clicar no botão 🔧 do veículo desejado

3. **Opção C - Pelos Detalhes do Veículo:**
   - Veículos → Detalhes → Histórico de Manutenções

**Resultado:**
- Visualização completa do histórico
- Total de manutenções realizadas
- Custo total e médio
- Última manutenção
- Detalhamento de cada manutenção

### 🆕 Cenário 2: Cadastrar Nova Manutenção

1. **Opção A - Pelo Menu:**
   - Menu Lateral → Manutenções → Nova Manutenção
   - Selecionar o veículo

2. **Opção B - Pelos Detalhes do Veículo:**
   - Veículos → Detalhes → Painel "Ações Rápidas" → Nova Manutenção
   - *(Veículo já pré-selecionado)*

3. **Preencher dados:**
   - Tipo de manutenção (Troca de Óleo, Revisão, etc.)
   - Status (Agendada, Em Andamento, etc.)
   - Data agendada
   - Quilometragem atual
   - Descrição
   - Custos (geral, peças, mão de obra)
   - Oficina e observações

4. **Adicionar itens/serviços** (opcional):
   - Após criar a manutenção
   - Adicionar peças e serviços
   - Informar quantidade, valor, fornecedor

### 📈 Cenário 3: Consultar Relatório de Custos

1. **Acesso:**
   - Menu Lateral → Manutenções → Relatório de Custos
   - OU: Manutenções → Ver Todas → Relatório

2. **Filtros disponíveis:**
   - Período (data início/fim)
   - Veículo específico
   - Tipo de manutenção

3. **Visualizações:**
   - Estatísticas resumidas
   - Custos por tipo de manutenção
   - Custos por veículo
   - Gráficos e percentuais
   - Detalhamento completo

## 🔑 Permissões de Acesso

### 👁️ Visualização
- **Todos os usuários autenticados** podem:
  - Ver lista de manutenções
  - Ver detalhes de manutenções
  - Ver histórico por veículo

### ✏️ Edição
- **Admin e Manager** podem:
  - Criar novas manutenções
  - Editar manutenções existentes
  - Adicionar/editar itens de manutenção
  - Gerar relatórios

### 🗑️ Exclusão
- **Apenas Admin** pode:
  - Excluir manutenções
  - Excluir itens de manutenção

## 📋 Funcionalidades Principais

### ✅ Controle de Manutenções
- ✔️ Agendamento e acompanhamento
- ✔️ Status (Agendada, Em Andamento, Concluída, Cancelada)
- ✔️ Tipos predefinidos (17 categorias)
- ✔️ Manutenções preventivas e urgentes
- ✔️ Garantias de serviços
- ✔️ Oficinas e notas fiscais

### 💰 Controle de Custos
- ✔️ Custo total (automático)
- ✔️ Custo de peças
- ✔️ Custo de mão de obra
- ✔️ Detalhamento por item
- ✔️ Histórico completo de gastos

### 🔄 Integração com Veículos
- ✔️ Atualização automática de status do veículo
- ✔️ Atualização de quilometragem
- ✔️ Histórico centralizado
- ✔️ Acesso rápido a partir dos veículos

### 📊 Relatórios
- ✔️ Custos por período
- ✔️ Custos por tipo de manutenção
- ✔️ Custos por veículo
- ✔️ Análise percentual
- ✔️ Exportação e impressão

## 🎨 Interface do Usuário

### 🌈 Códigos de Cores
- 🟡 **Amarelo/Warning:** Manutenções agendadas
- 🔵 **Azul/Info:** Manutenções em andamento
- 🟢 **Verde/Success:** Manutenções concluídas
- 🔴 **Vermelho/Danger:** Manutenções canceladas/urgentes
- 🔵 **Azul/Primary:** Manutenções preventivas

### 🏷️ Badges e Marcadores
- **Preventiva:** Badge azul com escudo
- **Urgente:** Badge vermelho com triângulo de alerta
- **Em Garantia:** Badge verde com certificado

## 📱 Navegação Rápida

### 🔗 Links Diretos
```
/Manutencoes/Index                    → Lista completa
/Manutencoes/Create                   → Nova manutenção
/Manutencoes/Details/{id}             → Detalhes
/Manutencoes/HistoricoVeiculo/{id}    → Histórico por veículo
/Manutencoes/Relatorio                → Relatório gerencial
```

### ⌨️ Atalhos do Teclado
- `Ctrl+K`: Busca global (pode buscar veículos)
- `Ctrl+B`: Toggle sidebar (desktop)
- `ESC`: Fechar modals

## 📊 Dados Pré-cadastrados

### Status de Manutenção (5 opções):
1. Agendada
2. Em Andamento
3. Concluída
4. Cancelada
5. Pendente Aprovação

### Tipos de Manutenção (17 categorias):
1. Troca de Óleo
2. Revisão Periódica
3. Troca de Pneus
4. Alinhamento e Balanceamento
5. Freios
6. Suspensão
7. Ar Condicionado
8. Bateria
9. Sistema Elétrico
10. Motor
11. Câmbio
12. Funilaria
13. Pintura
14. Vidros
15. Limpeza Completa
16. Inspeção Veicular
17. Outros

## 🎯 Melhores Práticas

### ✅ Recomendações
1. **Cadastrar manutenções regularmente**
   - Manter histórico atualizado
   - Programar preventivas

2. **Usar tipos corretos**
   - Facilita análises posteriores
   - Melhora organização

3. **Adicionar itens detalhados**
   - Facilita controle de estoque
   - Melhora rastreabilidade

4. **Manter custos atualizados**
   - Facilita análise financeira
   - Melhora planejamento

5. **Usar observações**
   - Documentar problemas
   - Registrar recomendações

## 🆘 Suporte

### ❓ Perguntas Frequentes

**P: Como alterar o status de uma manutenção?**
R: Acesse Manutenções → Editar → Altere o status desejado

**P: Posso excluir uma manutenção já concluída?**
R: Apenas usuários Admin podem excluir manutenções

**P: Como ver o total gasto com um veículo?**
R: Veículos → Detalhes → Histórico de Manutenções (mostra total e média)

**P: Posso adicionar itens depois de criar a manutenção?**
R: Sim! Acesse Detalhes da Manutenção → Adicionar Item

**P: Como fazer uma manutenção preventiva?**
R: Ao criar/editar, marque a opção "Manutenção Preventiva"

## 🎉 Pronto para Usar!

O sistema de manutenção está **100% integrado** e pronto para uso.

Acesse pelo menu **Locação → Manutenções** ou diretamente pelos **Veículos**!

---

**Desenvolvido para:** Litoral Sul Locadora e Turismo  
**Versão:** 1.0  
**Data:** Outubro/2025
