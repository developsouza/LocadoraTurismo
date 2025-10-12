# ?? Sistema de Gest�o de Clientes - Guia Completo

## ?? �ndice
- [Vis�o Geral](#vis�o-geral)
- [Cadastro de Clientes](#cadastro-de-clientes)
- [Consulta e Filtros](#consulta-e-filtros)
- [Edi��o e Atualiza��o](#edi��o-e-atualiza��o)
- [Documentos do Cliente](#documentos-do-cliente)
- [Hist�rico e Relat�rios](#hist�rico-e-relat�rios)
- [Permiss�es](#permiss�es)
- [Boas Pr�ticas](#boas-pr�ticas)

---

## ?? Vis�o Geral

O m�dulo de gest�o de clientes � o cora��o do sistema, permitindo o cadastro completo e gerenciamento de dados dos clientes da locadora e ag�ncia de turismo.

### ?? Funcionalidades Principais

? **Cadastro Completo**
- Dados pessoais (nome, CPF, telefone, email)
- Endere�o e CEP
- Data de nascimento com valida��o de idade
- Estado civil e profiss�o
- Documenta��o (CNH com validade)

? **Gerenciamento**
- Visualiza��o de todos os clientes
- Busca e filtros avan�ados
- Edi��o de informa��es
- Upload de documentos
- Hist�rico de loca��es e reservas

? **Integra��o**
- Vincula��o com loca��es de ve�culos
- Vincula��o com reservas de viagem
- Upload de documentos (CNH, RG, CPF, etc.)
- Gera��o de relat�rios

---

## ?? Cadastro de Clientes

### ?? Como Acessar
**Menu Lateral ? Loca��o ? Clientes ? ? Novo Cliente**

OU

**URL direta:** `/Clientes/Create`

### ?? Campos Obrigat�rios

#### 1?? **Dados Pessoais**
| Campo | Formato | Valida��o | Exemplo |
|-------|---------|-----------|---------|
| **Nome Completo** | Texto (m�x. 100 caracteres) | Obrigat�rio | Jo�o Silva Santos |
| **CPF** | 000.000.000-00 | Obrigat�rio, valida��o de CPF | 123.456.789-00 |
| **Telefone** | (00) 00000-0000 | Obrigat�rio | (13) 98765-4321 |
| **Email** | email@dominio.com | Obrigat�rio, formato v�lido | joao@email.com |
| **Data de Nascimento** | dd/MM/yyyy | Obrigat�rio, entre 21 e 100 anos | 15/03/1990 |

#### 2?? **Endere�o**
| Campo | Formato | Valida��o | Exemplo |
|-------|---------|-----------|---------|
| **Endere�o Completo** | Texto (m�x. 200 caracteres) | Obrigat�rio | Rua das Flores, 123, Centro |
| **CEP** | 00000-000 | Opcional | 11700-000 |

#### 3?? **Informa��es Complementares**
| Campo | Formato | Valida��o | Exemplo |
|-------|---------|-----------|---------|
| **Estado Civil** | Sele��o | Opcional | Casado, Solteiro, etc. |
| **Profiss�o** | Texto (m�x. 100 caracteres) | Opcional | Engenheiro Civil |

#### 4?? **Documenta��o (CNH)**
| Campo | Formato | Valida��o | Exemplo |
|-------|---------|-----------|---------|
| **N�mero da CNH** | Texto (m�x. 20 caracteres) | Opcional* | 12345678900 |
| **Validade da CNH** | dd/MM/yyyy | Obrigat�rio se CNH informada | 15/12/2027 |
| **Categoria CNH** | Texto (m�x. 5 caracteres) | Opcional | B, AB, etc. |

> ?? **Importante:** Se informar n�mero da CNH, a validade torna-se obrigat�ria e deve ser uma data futura!

### ?? Valida��es Autom�ticas

#### ? **CPF**
- Valida��o de d�gitos verificadores
- N�o permite CPF com todos os n�meros iguais
- Aceita formato com ou sem pontua��o

#### ? **Idade**
- M�nima: 21 anos
- M�xima: 100 anos
- C�lculo autom�tico da idade atual

#### ? **CNH**
- Validade deve ser data futura
- Se informar CNH, validade � obrigat�ria
- Indica visualmente CNH v�lida/vencida

#### ? **Email**
- Formato v�lido (exemplo@dominio.com)
- �nico no sistema

### ?? Passo a Passo - Cadastro

1. **Acesse o formul�rio de cadastro**
   - Menu ? Clientes ? Novo Cliente

2. **Preencha os dados pessoais**
   ```
   Nome: Maria da Silva
   CPF: 987.654.321-00
   Telefone: (13) 99876-5432
   Email: maria@email.com
   Data de Nascimento: 20/05/1985
   ```

3. **Preencha o endere�o**
   ```
   Endere�o: Av. Atl�ntica, 456, Jardim Praiano
   CEP: 11700-123
   ```

4. **Informa��es complementares (opcional)**
   ```
   Estado Civil: Solteira
   Profiss�o: M�dica
   ```

5. **Documenta��o (opcional)**
   ```
   N�mero CNH: 98765432100
   Validade CNH: 10/08/2028
   Categoria: AB
   ```

6. **Clique em "Salvar"**
   - Sistema valida todos os campos
   - Exibe mensagem de sucesso
   - Redireciona para lista de clientes

---

## ?? Consulta e Filtros

### ?? Como Acessar
**Menu Lateral ? Loca��o ? Clientes**

OU

**URL direta:** `/Clientes/Index`

### ?? Busca R�pida

**Campo de busca no topo da lista:**
- Busca por nome
- Busca por CPF
- Busca por email
- Busca por telefone

**Como usar:**
```
Digite: "Silva" ? Encontra todos os clientes com "Silva" no nome
Digite: "123.456" ? Encontra cliente com CPF contendo esses n�meros
Digite: "maria@" ? Encontra emails come�ando com "maria@"
```

### ?? Informa��es Exibidas na Lista

Cada cliente na lista mostra:

| Informa��o | Descri��o | Exemplo |
|------------|-----------|---------|
| **Nome** | Nome completo | Maria da Silva |
| **CPF** | CPF formatado | 987.654.321-00 |
| **Email** | Email de contato | maria@email.com |
| **Telefone** | Telefone formatado | (13) 99876-5432 |
| **Idade** | Calculada automaticamente | 38 anos |
| **CNH Status** | ? V�lida / ?? Vencida / ? Sem CNH | ? V�lida at� 2028 |
| **Total Loca��es** | Quantidade de loca��es | 5 loca��es |
| **Total Reservas** | Quantidade de reservas | 3 reservas |

### ?? Indicadores Visuais

#### **Status da CNH**
- ?? **Verde:** CNH v�lida
- ?? **Amarelo:** CNH vence em menos de 30 dias
- ?? **Vermelho:** CNH vencida
- ? **Cinza:** Sem CNH cadastrada

#### **Atividade do Cliente**
- ?? **Badge azul:** Cliente com loca��es ativas
- ?? **Badge roxo:** Cliente com reservas futuras
- ?? **Badge verde:** Cliente ativo (loca��es + reservas)
- ? **Sem badge:** Cliente sem atividades recentes

---

## ?? Edi��o e Atualiza��o

### ?? Como Acessar

**Op��o 1 - Pela Lista:**
- Clientes ? Bot�o "?? Editar" ao lado do cliente

**Op��o 2 - Pelos Detalhes:**
- Clientes ? Detalhes ? Bot�o "Editar"

### ?? Campos Edit�veis

**? Permitido Editar:**
- Nome completo
- Telefone
- Email
- Endere�o e CEP
- Estado civil e profiss�o
- CNH (n�mero, validade, categoria)

**?? N�o Edit�vel:**
- CPF (identificador �nico)
- Data de nascimento (para manter hist�rico)
- Data de cadastro (registro hist�rico)

### ?? Valida��es na Edi��o

1. **Email �nico:** N�o pode usar email j� cadastrado por outro cliente
2. **CNH v�lida:** Se alterar CNH, a nova validade deve ser futura
3. **Idade:** N�o pode alterar data de nascimento para fora do range (21-100 anos)
4. **Telefone:** Deve manter formato v�lido

### ?? Exemplo de Atualiza��o

**Cen�rio:** Atualizar telefone e CNH do cliente

1. Acesse a edi��o do cliente
2. Altere o telefone:
   ```
   De: (13) 99876-5432
   Para: (13) 98765-4321
   ```
3. Atualize a CNH:
   ```
   N�mero CNH: 11111111111 (novo documento)
   Validade CNH: 20/01/2030 (nova validade)
   Categoria: AB
   ```
4. Clique em "Salvar Altera��es"
5. Sistema valida e confirma a atualiza��o

---

## ?? Documentos do Cliente

### ?? Como Acessar
**Clientes ? Detalhes do Cliente ? Bot�o "?? Documentos"**

OU

**URL direta:** `/DocumentosUpload/UploadCliente/{id}`

### ?? Tipos de Documentos Suportados

| Tipo | Descri��o | Formato |
|------|-----------|---------|
| **CNH** | Carteira Nacional de Habilita��o | PDF, Imagem |
| **RG** | Registro Geral | PDF, Imagem |
| **CPF** | Cadastro de Pessoa F�sica | PDF, Imagem |
| **Comprovante Resid�ncia** | Conta de luz, �gua, etc. | PDF, Imagem |
| **Foto de Perfil** | Foto 3x4 | Imagem |
| **Outros** | Documentos diversos | PDF, Imagem |

### ?? Como Fazer Upload

1. **Acesse a �rea de documentos do cliente**
2. **Selecione o tipo de documento** (dropdown)
3. **Clique em "Escolher arquivo"**
4. **Selecione o arquivo do seu computador**
   - Formatos: PDF, JPG, JPEG, PNG, GIF, BMP
   - Tamanho m�ximo: 10MB
5. **Adicione uma descri��o** (opcional)
6. **Clique em "Enviar Documento"**

### ??? Visualiza��o de Documentos

**Lista de documentos mostra:**
- ?? Nome do arquivo
- ??? Tipo do documento
- ?? Tamanho do arquivo
- ?? Data de upload
- ?? Usu�rio que fez upload
- ?? Descri��o (se houver)

**A��es dispon�veis:**
- ??? **Visualizar** (para imagens)
- ?? **Baixar** (todos os tipos)
- ??? **Excluir** (Admin/Manager)

### ?? Documenta��o Completa

Para informa��es detalhadas sobre upload de documentos, consulte:
?? **[UPLOAD_DOCUMENTOS.md](UPLOAD_DOCUMENTOS.md)**

---

## ?? Hist�rico e Relat�rios

### ?? Visualiza��o de Detalhes

**Clientes ? Detalhes do Cliente**

### ?? Informa��es Dispon�veis

#### 1?? **Resumo do Cliente**
```
Nome: Maria da Silva
CPF: 987.654.321-00
Email: maria@email.com
Telefone: (13) 99876-5432
Idade: 38 anos
Cadastrado em: 15/01/2024
```

#### 2?? **Status da CNH**
```
CNH: 98765432100
Categoria: AB
Validade: 10/08/2028
Status: ? V�lida (vence em 1.250 dias)
```

#### 3?? **Estat�sticas**
```
?? Total de Loca��es: 5
?? Total de Reservas: 3
?? Valor Total Gasto: R$ 8.450,00
?? Ticket M�dio: R$ 1.056,25
```

#### 4?? **Hist�rico de Loca��es**
- Lista das �ltimas loca��es
- Ve�culo locado
- Per�odo da loca��o
- Valor pago
- Status da loca��o

#### 5?? **Hist�rico de Reservas**
- Lista das reservas de viagem
- Pacote contratado
- Data da viagem
- Quantidade de pessoas
- Valor total
- Status da reserva

### ?? A��es R�pidas

**Painel lateral direito:**

```
?? Nova Loca��o
   ? Criar loca��o para este cliente

?? Nova Reserva
   ? Criar reserva de viagem

?? Documentos
   ? Gerenciar documentos do cliente

?? Editar
   ? Editar informa��es do cliente

?? Enviar Email
   ? Enviar comunica��o (futuro)
```

---

## ?? Permiss�es de Acesso

### ??? **Visualiza��o**
**Quem pode:** Todos os usu�rios autenticados
- Ver lista de clientes
- Ver detalhes do cliente
- Ver hist�rico de loca��es/reservas
- Consultar documentos

### ?? **Cria��o e Edi��o**
**Quem pode:** Admin, Manager, Employee
- Cadastrar novos clientes
- Editar informa��es de clientes
- Fazer upload de documentos

### ??? **Exclus�o**
**Quem pode:** Admin, Manager
- Excluir clientes SEM vincula��es
- Excluir documentos de clientes

> ?? **Importante:** Clientes com loca��es ou reservas N�O podem ser exclu�dos!

### ?? Regras de Neg�cio

#### **N�o � poss�vel excluir cliente se:**
1. Possui loca��es cadastradas (ativas ou finalizadas)
2. Possui reservas de viagem (confirmadas ou pendentes)
3. Possui documentos anexados

#### **Para excluir um cliente:**
1. Verificar se n�o h� vincula��es
2. Excluir documentos (se houver)
3. Ent�o excluir o cliente

---

## ? Boas Pr�ticas

### ?? Cadastro

? **Fa�a:**
- Sempre preencher todos os campos obrigat�rios
- Validar CPF antes de salvar
- Confirmar dados de contato (telefone/email)
- Verificar validade da CNH antes de aceitar
- Solicitar documentos complementares
- Manter dados atualizados

? **Evite:**
- Cadastrar clientes sem CNH v�lida para loca��es
- Usar telefones/emails incorretos
- Duplicar cadastros (verificar CPF antes)
- Deixar campos importantes em branco
- Cadastrar menores de 21 anos

### ?? Documenta��o

? **Fa�a:**
- Upload de CNH, RG e CPF sempre que poss�vel
- Verificar qualidade da imagem/PDF
- Adicionar descri��o aos documentos
- Manter documentos atualizados (CNH renovada)
- Organizar por tipo de documento

? **Evite:**
- Arquivos muito grandes (>10MB)
- Imagens ileg�veis ou de baixa qualidade
- Documentos vencidos
- Misturar tipos de documentos

### ?? Consultas

? **Fa�a:**
- Usar busca para encontrar clientes rapidamente
- Verificar CNH antes de criar loca��o
- Consultar hist�rico do cliente
- Verificar inadimpl�ncia (futuro)
- Manter contato atualizado

? **Evite:**
- Criar loca��o para cliente com CNH vencida
- Ignorar alertas do sistema
- N�o verificar hist�rico de problemas

### ?? Manuten��o

? **Fa�a:**
- Atualizar dados quando cliente informar mudan�as
- Renovar CNH quando cliente trouxer nova
- Limpar cadastros duplicados
- Revisar dados periodicamente
- Manter documenta��o completa

? **Evite:**
- Deixar dados desatualizados
- Manter CNH vencida no cadastro
- Ignorar solicita��es de atualiza��o
- Acumular documentos desnecess�rios

---

## ?? Casos de Uso Comuns

### Caso 1: Cliente sem CNH
**Situa��o:** Cliente quer fazer reserva de viagem (n�o precisa de CNH)

```
? Permitido:
- Cadastrar cliente sem CNH
- Criar reserva de viagem
- Fazer upload de RG e CPF

? N�o Permitido:
- Criar loca��o de ve�culo (requer CNH v�lida)
```

### Caso 2: CNH Vencida
**Situa��o:** Cliente com CNH vencida no sistema

```
?? Sistema alerta: CNH VENCIDA

A��es:
1. Solicitar nova CNH ao cliente
2. Fazer upload da nova CNH (documento)
3. Atualizar n�mero e validade no cadastro
4. Ap�s atualiza��o, liberar para loca��o
```

### Caso 3: Atualiza��o de Dados
**Situa��o:** Cliente mudou de endere�o e telefone

```
Passos:
1. Clientes ? Buscar cliente
2. Clicar em "Editar"
3. Atualizar endere�o e telefone
4. Salvar altera��es
5. Sistema registra hist�rico de altera��o
```

### Caso 4: Cliente Frequente
**Situa��o:** Ver hist�rico completo de cliente antigo

```
1. Clientes ? Detalhes do cliente
2. Ver estat�sticas:
   - Total gasto: R$ 15.000
   - Loca��es: 12
   - Reservas: 8
3. Analisar padr�es de consumo
4. Oferecer promo��es personalizadas (futuro)
```

---

## ?? Solu��o de Problemas

### ? Erro: "CPF j� cadastrado"
**Causa:** J� existe cliente com este CPF
**Solu��o:** 
- Buscar cliente existente
- Atualizar dados do cliente existente
- N�o criar duplicata

### ? Erro: "CNH vencida"
**Causa:** Data de validade da CNH � anterior � data atual
**Solu��o:**
- Solicitar CNH atualizada ao cliente
- Atualizar dados no sistema
- Fazer upload da nova CNH

### ? Erro: "Idade inv�lida"
**Causa:** Cliente tem menos de 21 ou mais de 100 anos
**Solu��o:**
- Verificar data de nascimento
- Clientes menores de 21 n�o podem ser cadastrados
- Confirmar data correta

### ? Erro: "Email inv�lido"
**Causa:** Formato de email incorreto
**Solu��o:**
- Verificar formato: exemplo@dominio.com
- Confirmar email com cliente
- Usar email v�lido e ativo

### ? Erro: "N�o � poss�vel excluir"
**Causa:** Cliente possui loca��es ou reservas
**Solu��o:**
- Cliente com hist�rico n�o pode ser exclu�do
- Apenas inativar (futuro)
- Manter registro hist�rico

---

## ?? Atalhos e Dicas

### ?? Atalhos de Teclado
```
Ctrl + K     ? Busca global (buscar cliente)
Ctrl + N     ? Novo cliente (em breve)
Ctrl + S     ? Salvar (nos formul�rios)
Esc          ? Cancelar/Fechar modal
```

### ?? Links R�pidos
```
/Clientes/Index          ? Lista de clientes
/Clientes/Create         ? Novo cliente
/Clientes/Details/{id}   ? Detalhes do cliente
/Clientes/Edit/{id}      ? Editar cliente
```

### ?? Dicas Profissionais

1. **Organize por Status de CNH**
   - Filtre clientes com CNH pr�xima ao vencimento
   - Entre em contato proativamente

2. **Use Hist�rico para Upsell**
   - Clientes frequentes merecem benef�cios
   - Oferece pacotes premium

3. **Mantenha Documenta��o Completa**
   - Reduz problemas legais
   - Agiliza processos

4. **Atualize Contatos Regularmente**
   - Email e telefone atualizados
   - Facilita comunica��o

---

## ?? Suporte

### ? D�vidas Frequentes

**P: Posso cadastrar cliente menor de 21 anos?**
R: N�o. A idade m�nima � 21 anos para loca��es.

**P: CNH � obrigat�ria para todos os clientes?**
R: N�o. Apenas para clientes que v�o alugar ve�culos.

**P: Posso usar o mesmo email para v�rios clientes?**
R: N�o. Cada cliente deve ter email �nico.

**P: Como fa�o backup dos documentos?**
R: Os documentos s�o salvos no servidor. Consulte o administrador.

**P: Posso recuperar cliente exclu�do?**
R: N�o. Exclus�es s�o permanentes. Por isso a restri��o.

---

## ?? Pronto para Usar!

O sistema de gest�o de clientes est� **100% operacional**.

**Pr�ximos passos:**
1. Cadastre seus primeiros clientes
2. Fa�a upload dos documentos
3. Crie loca��es e reservas
4. Acompanhe o hist�rico

**Acesse:** Menu ? Loca��o ? Clientes

---

**Desenvolvido para:** Litoral Sul Locadora e Turismo  
**Vers�o:** 1.0  
**Data:** Outubro/2025  
**Documenta��o relacionada:** 
- [Upload de Documentos](UPLOAD_DOCUMENTOS.md)
- [Sistema de Manuten��es](MANUTENCAO_GUIA_ACESSO.md)
