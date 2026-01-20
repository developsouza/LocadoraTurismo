# 📚 Sistema de Gestão de Clientes - Guia Completo

## 📚 Índice
- [Visão Geral](#visão-geral)
- [Cadastro de Clientes](#cadastro-de-clientes)
- [Consulta e Filtros](#consulta-e-filtros)
- [Edição e Atualização](#edição-e-atualização)
- [Documentos do Cliente](#documentos-do-cliente)
- [Histórico e Relatórios](#histórico-e-relatórios)
- [Permissões](#permissões)
- [Boas Práticas](#boas-práticas)

---

## 📚 Visão Geral

O módulo de gestão de clientes é o coração do sistema, permitindo o cadastro completo e gerenciamento de dados dos clientes da locadora e agência de turismo.

### 📚 Funcionalidades Principais

✅ **Cadastro Completo**
- Dados pessoais (nome, CPF, telefone, email)
- Endereço e CEP
- Data de nascimento com validação de idade
- Estado civil e profissão
- Documentação (CNH com validade)

✅ **Gerenciamento**
- Visualização de todos os clientes
- Busca e filtros avançados
- Edição de informações
- Upload de documentos
- Histórico de locações e reservas

✅ **Integração**
- Vinculação com locações de veículos
- Vinculação com reservas de viagem
- Upload de documentos (CNH, RG, CPF, etc.)
- Geração de relatórios

---

## 📚 Cadastro de Clientes

### 📚 Como Acessar
**Menu Lateral ✅ Locação ✅ Clientes ✅ ✅ Novo Cliente**

OU

**URL direta:** `/Clientes/Create`

### 📚 Campos Obrigatórios

#### 1📚 **Dados Pessoais**
| Campo | Formato | Validação | Exemplo |
|-------|---------|-----------|---------|
| **Nome Completo** | Texto (máx. 100 caracteres) | Obrigatório | João Silva Santos |
| **CPF** | 000.000.000-00 | Obrigatório, validação de CPF | 123.456.789-00 |
| **Telefone** | (00) 00000-0000 | Obrigatório | (13) 98765-4321 |
| **Email** | email@dominio.com | Obrigatório, formato válido | joao@email.com |
| **Data de Nascimento** | dd/MM/yyyy | Obrigatório, entre 21 e 100 anos | 15/03/1990 |

#### 2📚 **Endereço**
| Campo | Formato | Validação | Exemplo |
|-------|---------|-----------|---------|
| **Endereço Completo** | Texto (máx. 200 caracteres) | Obrigatório | Rua das Flores, 123, Centro |
| **CEP** | 00000-000 | Opcional | 11700-000 |

#### 3📚 **Informações Complementares**
| Campo | Formato | Validação | Exemplo |
|-------|---------|-----------|---------|
| **Estado Civil** | Seleção | Opcional | Casado, Solteiro, etc. |
| **Profissão** | Texto (máx. 100 caracteres) | Opcional | Engenheiro Civil |

#### 4📚 **Documentação (CNH)**
| Campo | Formato | Validação | Exemplo |
|-------|---------|-----------|---------|
| **Número da CNH** | Texto (máx. 20 caracteres) | Opcional* | 12345678900 |
| **Validade da CNH** | dd/MM/yyyy | Obrigatório se CNH informada | 15/12/2027 |
| **Categoria CNH** | Texto (máx. 5 caracteres) | Opcional | B, AB, etc. |

> 📚 **Importante:** Se informar número da CNH, a validade torna-se obrigatória e deve ser uma data futura!

### 📚 Validações Automáticas

#### ✅ **CPF**
- Validação de dígitos verificadores
- Não permite CPF com todos os números iguais
- Aceita formato com ou sem pontuação

#### ✅ **Idade**
- Mínima: 21 anos
- Máxima: 100 anos
- Cálculo automático da idade atual

#### ✅ **CNH**
- Validade deve ser data futura
- Se informar CNH, validade é obrigatória
- Indica visualmente CNH válida/vencida

#### ✅ **Email**
- Formato válido (exemplo@dominio.com)
- Único no sistema

### 📚 Passo a Passo - Cadastro

1. **Acesse o formulário de cadastro**
   - Menu ✅ Clientes ✅ Novo Cliente

2. **Preencha os dados pessoais**
   ```
   Nome: Maria da Silva
   CPF: 987.654.321-00
   Telefone: (13) 99876-5432
   Email: maria@email.com
   Data de Nascimento: 20/05/1985
   ```

3. **Preencha o endereço**
   ```
   Endereço: Av. Atlântica, 456, Jardim Praiano
   CEP: 11700-123
   ```

4. **Informações complementares (opcional)**
   ```
   Estado Civil: Solteira
   Profissão: Médica
   ```

5. **Documentação (opcional)**
   ```
   Número CNH: 98765432100
   Validade CNH: 10/08/2028
   Categoria: AB
   ```

6. **Clique em "Salvar"**
   - Sistema valida todos os campos
   - Exibe mensagem de sucesso
   - Redireciona para lista de clientes

---

## 📚 Consulta e Filtros

### 📚 Como Acessar
**Menu Lateral ✅ Locação ✅ Clientes**

OU

**URL direta:** `/Clientes/Index`

### 📚 Busca Rápida

**Campo de busca no topo da lista:**
- Busca por nome
- Busca por CPF
- Busca por email
- Busca por telefone

**Como usar:**
```
Digite: "Silva" ✅ Encontra todos os clientes com "Silva" no nome
Digite: "123.456" ✅ Encontra cliente com CPF contendo esses números
Digite: "maria@" ✅ Encontra emails começando com "maria@"
```

### 📚 Informações Exibidas na Lista

Cada cliente na lista mostra:

| Informação | Descrição | Exemplo |
|------------|-----------|---------|
| **Nome** | Nome completo | Maria da Silva |
| **CPF** | CPF formatado | 987.654.321-00 |
| **Email** | Email de contato | maria@email.com |
| **Telefone** | Telefone formatado | (13) 99876-5432 |
| **Idade** | Calculada automaticamente | 38 anos |
| **CNH Status** | ✅ Válida / 📚 Vencida / ✅ Sem CNH | ✅ Válida até 2028 |
| **Total Locações** | Quantidade de locações | 5 locações |
| **Total Reservas** | Quantidade de reservas | 3 reservas |

### 📚 Indicadores Visuais

#### **Status da CNH**
- 📚 **Verde:** CNH válida
- 📚 **Amarelo:** CNH vence em menos de 30 dias
- 📚 **Vermelho:** CNH vencida
- ✅ **Cinza:** Sem CNH cadastrada

#### **Atividade do Cliente**
- 📚 **Badge azul:** Cliente com locações ativas
- 📚 **Badge roxo:** Cliente com reservas futuras
- 📚 **Badge verde:** Cliente ativo (locações + reservas)
- ✅ **Sem badge:** Cliente sem atividades recentes

---

## 📚 Edição e Atualização

### 📚 Como Acessar

**Opção 1 - Pela Lista:**
- Clientes ✅ Botão "📚 Editar" ao lado do cliente

**Opção 2 - Pelos Detalhes:**
- Clientes ✅ Detalhes ✅ Botão "Editar"

### 📚 Campos Editáveis

**✅ Permitido Editar:**
- Nome completo
- Telefone
- Email
- Endereço e CEP
- Estado civil e profissão
- CNH (número, validade, categoria)

**📚 Não Editável:**
- CPF (identificador único)
- Data de nascimento (para manter histórico)
- Data de cadastro (registro histórico)

### 📚 Validações na Edição

1. **Email único:** Não pode usar email já cadastrado por outro cliente
2. **CNH válida:** Se alterar CNH, a nova validade deve ser futura
3. **Idade:** Não pode alterar data de nascimento para fora do range (21-100 anos)
4. **Telefone:** Deve manter formato válido

### 📚 Exemplo de Atualização

**Cenário:** Atualizar telefone e CNH do cliente

1. Acesse a edição do cliente
2. Altere o telefone:
   ```
   De: (13) 99876-5432
   Para: (13) 98765-4321
   ```
3. Atualize a CNH:
   ```
   Número CNH: 11111111111 (novo documento)
   Validade CNH: 20/01/2030 (nova validade)
   Categoria: AB
   ```
4. Clique em "Salvar Alterações"
5. Sistema valida e confirma a atualização

---

## 📚 Documentos do Cliente

### 📚 Como Acessar
**Clientes ✅ Detalhes do Cliente ✅ Botão "📚 Documentos"**

OU

**URL direta:** `/DocumentosUpload/UploadCliente/{id}`

### 📚 Tipos de Documentos Suportados

| Tipo | Descrição | Formato |
|------|-----------|---------|
| **CNH** | Carteira Nacional de Habilitação | PDF, Imagem |
| **RG** | Registro Geral | PDF, Imagem |
| **CPF** | Cadastro de Pessoa Física | PDF, Imagem |
| **Comprovante Residência** | Conta de luz, água, etc. | PDF, Imagem |
| **Foto de Perfil** | Foto 3x4 | Imagem |
| **Outros** | Documentos diversos | PDF, Imagem |

### 📚 Como Fazer Upload

1. **Acesse a área de documentos do cliente**
2. **Selecione o tipo de documento** (dropdown)
3. **Clique em "Escolher arquivo"**
4. **Selecione o arquivo do seu computador**
   - Formatos: PDF, JPG, JPEG, PNG, GIF, BMP
   - Tamanho máximo: 10MB
5. **Adicione uma descrição** (opcional)
6. **Clique em "Enviar Documento"**

### 📚✅ Visualização de Documentos

**Lista de documentos mostra:**
- 📚 Nome do arquivo
- 📚✅ Tipo do documento
- 📚 Tamanho do arquivo
- 📚 Data de upload
- 📚 Usuário que fez upload
- 📚 Descrição (se houver)

**Ações disponíveis:**
- 📚✅ **Visualizar** (para imagens)
- 📚 **Baixar** (todos os tipos)
- 📚✅ **Excluir** (Admin/Manager)

### 📚 Documentação Completa

Para informações detalhadas sobre upload de documentos, consulte:
📚 **[UPLOAD_DOCUMENTOS.md](UPLOAD_DOCUMENTOS.md)**

---

## 📚 Histórico e Relatórios

### 📚 Visualização de Detalhes

**Clientes ✅ Detalhes do Cliente**

### 📚 Informações Disponíveis

#### 1📚 **Resumo do Cliente**
```
Nome: Maria da Silva
CPF: 987.654.321-00
Email: maria@email.com
Telefone: (13) 99876-5432
Idade: 38 anos
Cadastrado em: 15/01/2024
```

#### 2📚 **Status da CNH**
```
CNH: 98765432100
Categoria: AB
Validade: 10/08/2028
Status: ✅ Válida (vence em 1.250 dias)
```

#### 3📚 **Estatísticas**
```
📚 Total de Locações: 5
📚 Total de Reservas: 3
📚 Valor Total Gasto: R$ 8.450,00
📚 Ticket Médio: R$ 1.056,25
```

#### 4📚 **Histórico de Locações**
- Lista das últimas locações
- Veículo locado
- Período da locação
- Valor pago
- Status da locação

#### 5📚 **Histórico de Reservas**
- Lista das reservas de viagem
- Pacote contratado
- Data da viagem
- Quantidade de pessoas
- Valor total
- Status da reserva

### 📚 Ações Rápidas

**Painel lateral direito:**

```
📚 Nova Locação
   ✅ Criar locação para este cliente

📚 Nova Reserva
   ✅ Criar reserva de viagem

📚 Documentos
   ✅ Gerenciar documentos do cliente

📚 Editar
   ✅ Editar informações do cliente

📚 Enviar Email
   ✅ Enviar comunicação (futuro)
```

---

## 📚 Permissões de Acesso

### 📚✅ **Visualização**
**Quem pode:** Todos os usuários autenticados
- Ver lista de clientes
- Ver detalhes do cliente
- Ver histórico de locações/reservas
- Consultar documentos

### 📚 **Criação e Edição**
**Quem pode:** Admin, Manager, Employee
- Cadastrar novos clientes
- Editar informações de clientes
- Fazer upload de documentos

### 📚✅ **Exclusão**
**Quem pode:** Admin, Manager
- Excluir clientes SEM vinculações
- Excluir documentos de clientes

> 📚 **Importante:** Clientes com locações ou reservas NÃO podem ser excluídos!

### 📚 Regras de Negócio

#### **Não é possível excluir cliente se:**
1. Possui locações cadastradas (ativas ou finalizadas)
2. Possui reservas de viagem (confirmadas ou pendentes)
3. Possui documentos anexados

#### **Para excluir um cliente:**
1. Verificar se não há vinculações
2. Excluir documentos (se houver)
3. Então excluir o cliente

---

## ✅ Boas Práticas

### 📚 Cadastro

✅ **Faça:**
- Sempre preencher todos os campos obrigatórios
- Validar CPF antes de salvar
- Confirmar dados de contato (telefone/email)
- Verificar validade da CNH antes de aceitar
- Solicitar documentos complementares
- Manter dados atualizados

✅ **Evite:**
- Cadastrar clientes sem CNH válida para locações
- Usar telefones/emails incorretos
- Duplicar cadastros (verificar CPF antes)
- Deixar campos importantes em branco
- Cadastrar menores de 21 anos

### 📚 Documentação

✅ **Faça:**
- Upload de CNH, RG e CPF sempre que possível
- Verificar qualidade da imagem/PDF
- Adicionar descrição aos documentos
- Manter documentos atualizados (CNH renovada)
- Organizar por tipo de documento

✅ **Evite:**
- Arquivos muito grandes (>10MB)
- Imagens ilegíveis ou de baixa qualidade
- Documentos vencidos
- Misturar tipos de documentos

### 📚 Consultas

✅ **Faça:**
- Usar busca para encontrar clientes rapidamente
- Verificar CNH antes de criar locação
- Consultar histórico do cliente
- Verificar inadimplência (futuro)
- Manter contato atualizado

✅ **Evite:**
- Criar locação para cliente com CNH vencida
- Ignorar alertas do sistema
- Não verificar histórico de problemas

### 📚 Manutenção

✅ **Faça:**
- Atualizar dados quando cliente informar mudanças
- Renovar CNH quando cliente trouxer nova
- Limpar cadastros duplicados
- Revisar dados periodicamente
- Manter documentação completa

✅ **Evite:**
- Deixar dados desatualizados
- Manter CNH vencida no cadastro
- Ignorar solicitações de atualização
- Acumular documentos desnecessários

---

## 📚 Casos de Uso Comuns

### Caso 1: Cliente sem CNH
**Situação:** Cliente quer fazer reserva de viagem (não precisa de CNH)

```
✅ Permitido:
- Cadastrar cliente sem CNH
- Criar reserva de viagem
- Fazer upload de RG e CPF

✅ Não Permitido:
- Criar locação de veículo (requer CNH válida)
```

### Caso 2: CNH Vencida
**Situação:** Cliente com CNH vencida no sistema

```
📚 Sistema alerta: CNH VENCIDA

Ações:
1. Solicitar nova CNH ao cliente
2. Fazer upload da nova CNH (documento)
3. Atualizar número e validade no cadastro
4. Após atualização, liberar para locação
```

### Caso 3: Atualização de Dados
**Situação:** Cliente mudou de endereço e telefone

```
Passos:
1. Clientes ✅ Buscar cliente
2. Clicar em "Editar"
3. Atualizar endereço e telefone
4. Salvar alterações
5. Sistema registra histórico de alteração
```

### Caso 4: Cliente Frequente
**Situação:** Ver histórico completo de cliente antigo

```
1. Clientes ✅ Detalhes do cliente
2. Ver estatísticas:
   - Total gasto: R$ 15.000
   - Locações: 12
   - Reservas: 8
3. Analisar padrões de consumo
4. Oferecer promoções personalizadas (futuro)
```

---

## 📚 Solução de Problemas

### ✅ Erro: "CPF já cadastrado"
**Causa:** Já existe cliente com este CPF
**Solução:** 
- Buscar cliente existente
- Atualizar dados do cliente existente
- Não criar duplicata

### ✅ Erro: "CNH vencida"
**Causa:** Data de validade da CNH é anterior à data atual
**Solução:**
- Solicitar CNH atualizada ao cliente
- Atualizar dados no sistema
- Fazer upload da nova CNH

### ✅ Erro: "Idade inválida"
**Causa:** Cliente tem menos de 21 ou mais de 100 anos
**Solução:**
- Verificar data de nascimento
- Clientes menores de 21 não podem ser cadastrados
- Confirmar data correta

### ✅ Erro: "Email inválido"
**Causa:** Formato de email incorreto
**Solução:**
- Verificar formato: exemplo@dominio.com
- Confirmar email com cliente
- Usar email válido e ativo

### ✅ Erro: "Não é possível excluir"
**Causa:** Cliente possui locações ou reservas
**Solução:**
- Cliente com histórico não pode ser excluído
- Apenas inativar (futuro)
- Manter registro histórico

---

## 📚 Atalhos e Dicas

### 📚 Atalhos de Teclado
```
Ctrl + K     ✅ Busca global (buscar cliente)
Ctrl + N     ✅ Novo cliente (em breve)
Ctrl + S     ✅ Salvar (nos formulários)
Esc          ✅ Cancelar/Fechar modal
```

### 📚 Links Rápidos
```
/Clientes/Index          ✅ Lista de clientes
/Clientes/Create         ✅ Novo cliente
/Clientes/Details/{id}   ✅ Detalhes do cliente
/Clientes/Edit/{id}      ✅ Editar cliente
```

### 📚 Dicas Profissionais

1. **Organize por Status de CNH**
   - Filtre clientes com CNH próxima ao vencimento
   - Entre em contato proativamente

2. **Use Histórico para Upsell**
   - Clientes frequentes merecem benefícios
   - Oferece pacotes premium

3. **Mantenha Documentação Completa**
   - Reduz problemas legais
   - Agiliza processos

4. **Atualize Contatos Regularmente**
   - Email e telefone atualizados
   - Facilita comunicação

---

## 📚 Suporte

### ✅ Dúvidas Frequentes

**P: Posso cadastrar cliente menor de 21 anos✅**
R: Não. A idade mínima é 21 anos para locações.

**P: CNH é obrigatória para todos os clientes✅**
R: Não. Apenas para clientes que vão alugar veículos.

**P: Posso usar o mesmo email para vários clientes✅**
R: Não. Cada cliente deve ter email único.

**P: Como faço backup dos documentos✅**
R: Os documentos são salvos no servidor. Consulte o administrador.

**P: Posso recuperar cliente excluído✅**
R: Não. Exclusões são permanentes. Por isso a restrição.

---

## 📚 Pronto para Usar!

O sistema de gestão de clientes está **100% operacional**.

**Próximos passos:**
1. Cadastre seus primeiros clientes
2. Faça upload dos documentos
3. Crie locações e reservas
4. Acompanhe o histórico

**Acesse:** Menu ✅ Locação ✅ Clientes

---

**Desenvolvido para:** Litoral Sul Locadora e Turismo  
**Versão:** 1.0  
**Data:** Outubro/2025  
**Documentação relacionada:** 
- [Upload de Documentos](UPLOAD_DOCUMENTOS.md)
- [Sistema de Manutenções](MANUTENCAO_GUIA_ACESSO.md)

