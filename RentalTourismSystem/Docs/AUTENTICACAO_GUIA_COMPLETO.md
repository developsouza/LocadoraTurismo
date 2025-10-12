# ?? Sistema de Autenticação e Controle de Acesso - Guia Completo

## ?? Índice
- [Visão Geral](#visão-geral)
- [Tipos de Usuários](#tipos-de-usuários)
- [Login e Registro](#login-e-registro)
- [Gerenciamento de Usuários](#gerenciamento-de-usuários)
- [Permissões por Módulo](#permissões-por-módulo)
- [Segurança](#segurança)
- [Recuperação de Senha](#recuperação-de-senha)

---

## ?? Visão Geral

O sistema utiliza **ASP.NET Core Identity** para autenticação e autorização baseada em roles (funções), garantindo acesso seguro e controlado a todas as funcionalidades.

### ?? Recursos de Segurança

? **Autenticação Robusta**
- Login com email e senha
- Validação de credenciais
- Sessão segura (8 horas)
- Proteção contra ataques

? **Autorização Baseada em Roles**
- 4 níveis de acesso
- Permissões granulares
- Controle por módulo
- Hierarquia de privilégios

? **Gestão de Usuários**
- Cadastro de novos usuários
- Edição de informações
- Alteração de senha
- Bloqueio e desbloqueio

---

## ?? Tipos de Usuários

### ?? **Admin (Administrador)**

**Privilégios TOTAIS:**
- ? Acesso completo ao sistema
- ? Gerenciar todos os usuários
- ? Criar, editar e excluir TUDO
- ? Acessar relatórios gerenciais
- ? Configurações do sistema
- ? Backup e restore
- ? Logs e auditoria

**Acesso aos módulos:**
```
? Clientes (CRUD completo)
? Veículos (CRUD completo)
? Locações (CRUD completo)
? Manutenções (CRUD completo)
? Reservas (CRUD completo)
? Pacotes (CRUD completo)
? Funcionários (CRUD completo)
? Agências (CRUD completo)
? Usuários (CRUD completo)
? Relatórios (todos)
? Documentos (upload/exclusão)
```

**Usuário padrão:**
```
Email: admin@litoralsul.com.br
Senha: Admin@123456
```

### ?? **Manager (Gerente)**

**Privilégios GERENCIAIS:**
- ? Gerenciar operações diárias
- ? Criar e editar registros principais
- ? Aprovar locações e reservas
- ? Acessar relatórios operacionais
- ?? NÃO gerencia usuários Admin
- ?? NÃO acessa configurações críticas

**Acesso aos módulos:**
```
? Clientes (CRUD completo)
? Veículos (CRUD completo)
? Locações (CRUD completo)
? Manutenções (CRUD completo)
? Reservas (CRUD completo)
? Pacotes (CRUD completo)
? Funcionários (CRUD)
? Agências (CRUD)
? Usuários (apenas visualização)
? Relatórios (operacionais)
? Documentos (upload/exclusão)
```

### ?? **Employee (Funcionário)**

**Privilégios OPERACIONAIS:**
- ? Criar locações e reservas
- ? Cadastrar clientes
- ? Fazer vistorias
- ? Upload de documentos
- ?? NÃO exclui registros
- ?? NÃO gerencia usuários
- ?? Relatórios limitados

**Acesso aos módulos:**
```
? Clientes (criar/editar)
??? Veículos (visualização)
? Locações (criar/editar/finalizar)
??? Manutenções (visualização)
? Reservas (criar/editar)
??? Pacotes (visualização)
??? Funcionários (visualização)
??? Agências (visualização)
? Usuários (sem acesso)
??? Relatórios (básicos)
? Documentos (upload, sem exclusão)
```

### ?? **User (Usuário Básico)**

**Privilégios LIMITADOS:**
- ??? Visualização de informações
- ??? Consultas básicas
- ? Sem permissões de edição
- ? Sem acesso a dados sensíveis

**Acesso aos módulos:**
```
??? Clientes (visualização)
??? Veículos (visualização)
??? Locações (visualização)
??? Manutenções (visualização)
??? Reservas (visualização)
??? Pacotes (visualização)
? Funcionários (sem acesso)
? Agências (sem acesso)
? Usuários (sem acesso)
? Relatórios (sem acesso)
? Documentos (sem acesso)
```

---

## ?? Login e Registro

### ?? Tela de Login

**Acesso:** `/Account/Login`

**Formulário:**
```
???????????????????????????????????
?   LITORAL SUL                   ?
?   Locadora e Turismo            ?
?                                 ?
?   Email:                        ?
?   [___________________________] ?
?                                 ?
?   Senha:                        ?
?   [___________________________] ?
?                                 ?
?   [ ] Lembrar-me               ?
?                                 ?
?   [       ENTRAR       ]        ?
?                                 ?
?   Esqueceu a senha?             ?
???????????????????????????????????
```

### ? Validações de Login

**Sistema valida:**
1. **Email:**
   - ? Formato válido
   - ? Cadastrado no sistema
   - ? Conta ativa

2. **Senha:**
   - ? Senha correta
   - ? Conta não bloqueada
   - ? Tentativas de login

### ?? Segurança do Login

**Proteções implementadas:**

1. **Bloqueio por Tentativas**
   - Máximo: 5 tentativas erradas
   - Bloqueio: 5 minutos
   - Reset automático após sucesso

2. **Sessão Segura**
   - Duração: 8 horas
   - Cookie HttpOnly
   - HTTPS obrigatório (produção)
   - Anti-CSRF token

3. **Validação de Senha**
   - Mínimo 6 caracteres
   - Pelo menos 1 maiúscula
   - Pelo menos 1 minúscula
   - Pelo menos 1 número

### ?? Registro de Novo Usuário

**Acesso:** `/Account/Register`

> ?? **Importante:** Apenas Admin e Manager podem criar novos usuários!

**Formulário:**
```
CADASTRAR NOVO USUÁRIO

Nome Completo:
[_________________________________]

Email:
[_________________________________]

Senha:
[_________________________________]

Confirmar Senha:
[_________________________________]

Função (Role):
[ Selecione ? ]
  - Admin
  - Manager
  - Employee
  - User

[    CADASTRAR    ]  [  CANCELAR  ]
```

### ? Validações de Registro

**Campos obrigatórios:**
- ? Nome completo
- ? Email válido e único
- ? Senha forte
- ? Confirmação de senha
- ? Função (role)

**Regras de senha:**
```
? Mínimo 6 caracteres
? Pelo menos 1 letra maiúscula
? Pelo menos 1 letra minúscula
? Pelo menos 1 número
? Caracteres especiais: opcional
```

**Exemplo de senha válida:**
```
? Admin@123456
? Manager2024
? Employee123
```

---

## ????? Gerenciamento de Usuários

### ?? Como Acessar

**Menu ? Administração ? Gerenciar Usuários**

OU

**URL direta:** `/Account/ManageUsers`

> ?? **Acesso:** Apenas Admin e Manager

### ?? Lista de Usuários

**Visualização:**
```
???????????????????????????????????????????????????
?  GERENCIAR USUÁRIOS                    [+ Novo] ?
???????????????????????????????????????????????????
?                                                 ?
?  ?? João Silva                                  ?
?  ?? joao@litoralsul.com.br                     ?
?  ??? Role: Manager                              ?
?  ?? Cadastrado: 15/01/2024                     ?
?  ? Status: Ativo                              ?
?  [?? Editar] [?? Bloquear] [??? Excluir]      ?
?                                                 ?
???????????????????????????????????????????????????
?                                                 ?
?  ?? Maria Santos                                ?
?  ?? maria@litoralsul.com.br                    ?
?  ??? Role: Employee                             ?
?  ?? Cadastrado: 20/02/2024                     ?
?  ? Status: Ativo                              ?
?  [?? Editar] [?? Bloquear] [??? Excluir]      ?
?                                                 ?
???????????????????????????????????????????????????
```

### ?? Editar Usuário

**Acesso:** Lista ? Botão "?? Editar"

**Campos editáveis:**
```
EDITAR USUÁRIO

Nome: [João Silva                    ]
Email: [joao@litoralsul.com.br      ]
Role: [Manager ?]

Status: ( ) Ativo  ( ) Bloqueado

[  SALVAR  ]  [  CANCELAR  ]
```

**Permissões para edição:**
- ? **Admin:** Pode editar TODOS os usuários
- ?? **Manager:** Pode editar Employee e User (NÃO Admin)
- ? **Employee/User:** Sem permissão

### ?? Alterar Senha

**Opção 1 - Próprio Usuário:**
```
Menu ? Perfil ? Alterar Senha

Senha Atual:
[_________________________________]

Nova Senha:
[_________________________________]

Confirmar Nova Senha:
[_________________________________]

[  ALTERAR SENHA  ]
```

**Opção 2 - Admin/Manager alterando:**
```
Gerenciar Usuários ? Editar ? Redefinir Senha

Nova Senha:
[_________________________________]

Confirmar Senha:
[_________________________________]

[  REDEFINIR  ]
```

### ?? Bloquear/Desbloquear Usuário

**Quando bloquear:**
- Suspeita de acesso não autorizado
- Funcionário afastado temporariamente
- Violação de política de uso
- Solicitação de segurança

**Como fazer:**
```
Gerenciar Usuários ? Selecionar usuário ? ?? Bloquear

Motivo do bloqueio:
[_________________________________]

[  CONFIRMAR BLOQUEIO  ]
```

**Efeitos:**
- ? Usuário não consegue fazer login
- ?? Mensagem: "Conta bloqueada. Entre em contato com administrador"
- ?? Bloqueio registrado em log

**Para desbloquear:**
```
Gerenciar Usuários ? Usuário bloqueado ? ?? Desbloquear

[  DESBLOQUEAR  ]
```

### ??? Excluir Usuário

**Regras:**
- ? Pode excluir: Usuários sem vínculos
- ? NÃO pode excluir: Usuário logado
- ? NÃO pode excluir: Admin principal
- ?? Manager não pode excluir Admin

**Como fazer:**
```
Gerenciar Usuários ? Selecionar ? ??? Excluir

?? ATENÇÃO!
Esta ação não pode ser desfeita.

Deseja realmente excluir o usuário?
Nome: João Silva
Email: joao@litoralsul.com.br

[  CONFIRMAR EXCLUSÃO  ]  [  CANCELAR  ]
```

---

## ?? Permissões por Módulo

### ?? Tabela Completa de Permissões

| Módulo | Admin | Manager | Employee | User |
|--------|-------|---------|----------|------|
| **Clientes** |  |  |  |  |
| Visualizar | ? | ? | ? | ? |
| Criar | ? | ? | ? | ? |
| Editar | ? | ? | ? | ? |
| Excluir | ? | ? | ? | ? |
| **Veículos** |  |  |  |  |
| Visualizar | ? | ? | ? | ? |
| Criar | ? | ? | ? | ? |
| Editar | ? | ? | ? | ? |
| Excluir | ? | ? | ? | ? |
| Alterar Status | ? | ? | ? | ? |
| **Locações** |  |  |  |  |
| Visualizar | ? | ? | ? | ? |
| Criar | ? | ? | ? | ? |
| Editar | ? | ? | ? | ? |
| Excluir | ? | ? | ? | ? |
| Finalizar | ? | ? | ? | ? |
| **Manutenções** |  |  |  |  |
| Visualizar | ? | ? | ? | ? |
| Criar | ? | ? | ? | ? |
| Editar | ? | ? | ? | ? |
| Excluir | ? | ? | ? | ? |
| Relatórios | ? | ? | ? | ? |
| **Reservas** |  |  |  |  |
| Visualizar | ? | ? | ? | ? |
| Criar | ? | ? | ? | ? |
| Editar | ? | ? | ? | ? |
| Cancelar | ? | ? | ? | ? |
| **Pacotes** |  |  |  |  |
| Visualizar | ? | ? | ? | ? |
| Criar | ? | ? | ? | ? |
| Editar | ? | ? | ? | ? |
| Excluir | ? | ? | ? | ? |
| **Documentos** |  |  |  |  |
| Visualizar | ? | ? | ? | ? |
| Upload | ? | ? | ? | ? |
| Download | ? | ? | ? | ? |
| Excluir | ? | ? | ? | ? |
| **Funcionários** |  |  |  |  |
| Visualizar | ? | ? | ? | ? |
| Criar | ? | ? | ? | ? |
| Editar | ? | ? | ? | ? |
| Excluir | ? | ? | ? | ? |
| **Agências** |  |  |  |  |
| Visualizar | ? | ? | ? | ? |
| Criar | ? | ? | ? | ? |
| Editar | ? | ? | ? | ? |
| Excluir | ? | ? | ? | ? |
| **Usuários** |  |  |  |  |
| Visualizar | ? | ? | ? | ? |
| Criar | ? | ? | ? | ? |
| Editar | ? | ?* | ? | ? |
| Excluir | ? | ?* | ? | ? |
| **Relatórios** |  |  |  |  |
| Gerenciais | ? | ? | ? | ? |
| Operacionais | ? | ? | ? | ? |
| Financeiros | ? | ? | ? | ? |

> \* Manager não pode editar/excluir Admin

### ?? Página de Acesso Negado

**Quando usuário tenta acessar sem permissão:**

```
???????????????????????????????????
?   ?? ACESSO NEGADO              ?
?                                 ?
?   Você não tem permissão        ?
?   para acessar esta página.     ?
?                                 ?
?   Entre em contato com o        ?
?   administrador do sistema.     ?
?                                 ?
?   [  VOLTAR AO INÍCIO  ]        ?
???????????????????????????????????
```

**URL:** `/Account/AccessDenied`

---

## ?? Segurança

### ??? Recursos de Segurança

#### 1?? **Proteção de Senha**

**Requisitos:**
```
? Mínimo 6 caracteres
? Letra maiúscula
? Letra minúscula
? Número
? Hash bcrypt (não armazena texto puro)
```

**Exemplos:**
```
? Admin@123456    (válida)
? Manager2024     (válida)
? Employee123     (válida)
? admin           (muito fraca)
? 123456          (sem letras)
? senha           (sem números/maiúscula)
```

#### 2?? **Bloqueio de Conta**

**Configuração:**
- Máximo tentativas: 5
- Tempo de bloqueio: 5 minutos
- Reset após sucesso: Sim
- Permitido para novos: Sim

**Cenário:**
```
Tentativa 1: Senha errada ?
Tentativa 2: Senha errada ?
Tentativa 3: Senha errada ?
Tentativa 4: Senha errada ?
Tentativa 5: Senha errada ?
? CONTA BLOQUEADA POR 5 MINUTOS
```

#### 3?? **Sessão Segura**

**Configurações:**
- Duração: 8 horas
- Cookie: HttpOnly ?
- SameSite: Lax ?
- Secure: Sim (produção) ?
- Sliding: Sim ?

**Renovação:**
- Atividade = Renova sessão
- Inativo 8h = Logout automático

#### 4?? **Proteção CSRF**

**Anti-Forgery Token:**
```html
<!-- Automaticamente incluído em formulários -->
<form method="post">
    @Html.AntiForgeryToken()
    ...
</form>
```

**Headers de segurança:**
```
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
Strict-Transport-Security: max-age=31536000
```

#### 5?? **Rate Limiting**

**Limites por política:**

| Recurso | Limite | Janela |
|---------|--------|--------|
| API Geral | 100 req | 1 min |
| Validação CPF | 20 req | 1 min |
| Dashboard | 300 req | 1 min |

**Resposta ao exceder:**
```
HTTP 429 Too Many Requests

{
  "error": "Muitas requisições. Tente novamente em 60 segundos."
}
```

### ?? Auditoria e Logs

**Sistema registra:**
- ? Todas as tentativas de login
- ? Criação/edição de usuários
- ? Alterações de permissões
- ? Bloqueios de conta
- ? Acessos negados
- ? Operações críticas

**Localização dos logs:**
```
RentalTourismSystem/logs/rental-tourism-YYYY-MM-DD.txt
```

**Exemplo de log:**
```
2024-10-20 14:35:12.456 [INFO] Login bem-sucedido: admin@litoralsul.com.br
2024-10-20 14:37:45.123 [WARN] Tentativa de login falha: funcionario@email.com
2024-10-20 15:10:33.789 [INFO] Usuário criado: maria@litoralsul.com.br por admin@litoralsul.com.br
2024-10-20 16:22:15.456 [ERROR] Acesso negado: /Account/ManageUsers por user@email.com
```

---

## ?? Recuperação de Senha

### ?? Esqueci Minha Senha

**Acesso:** Tela de Login ? "Esqueceu a senha?"

**Processo:**
```
1. Clicar em "Esqueceu a senha?"

2. Informar email cadastrado:
   [_________________________________]
   
   [  ENVIAR LINK DE RECUPERAÇÃO  ]

3. Sistema envia email com link

4. Clicar no link recebido

5. Definir nova senha:
   Nova Senha: [________________]
   Confirmar: [________________]
   
   [  REDEFINIR SENHA  ]

6. Senha alterada!
   ? Fazer login com nova senha
```

> ?? **Importante:** Link de recuperação válido por 24 horas

### ?? Alterar Senha (Logado)

**Menu ? Meu Perfil ? Alterar Senha**

```
ALTERAR SENHA

Senha Atual:
[_________________________________]

Nova Senha:
[_________________________________]

Confirmar Nova Senha:
[_________________________________]

[  ALTERAR SENHA  ]
```

**Validações:**
- ? Senha atual correta
- ? Nova senha atende requisitos
- ? Confirmação coincide
- ? Nova senha diferente da atual

---

## ?? Casos de Uso

### Caso 1: Primeiro Acesso

```
Sistema instalado, usar credenciais padrão:

Email: admin@litoralsul.com.br
Senha: Admin@123456

? Fazer login
? TROCAR SENHA imediatamente
? Criar outros usuários
```

### Caso 2: Novo Funcionário

```
Admin/Manager:
1. Menu ? Gerenciar Usuários
2. Clicar "+ Novo Usuário"
3. Preencher:
   Nome: Maria Santos
   Email: maria@litoralsul.com.br
   Senha: Employee123
   Role: Employee
4. Salvar

Funcionário:
1. Receber credenciais
2. Fazer login
3. Alterar senha no primeiro acesso
4. Começar a trabalhar
```

### Caso 3: Bloqueio por Segurança

```
Situação: Suspeita de acesso não autorizado

Admin:
1. Gerenciar Usuários
2. Localizar usuário suspeito
3. Clicar "?? Bloquear"
4. Informar motivo: "Tentativa de acesso suspeita"
5. Confirmar

Resultado:
? Usuário não consegue logar
? Ver mensagem de bloqueio
? Deve entrar em contato com admin
```

### Caso 4: Funcionário Saiu da Empresa

```
Admin/Manager:
1. Gerenciar Usuários
2. Localizar ex-funcionário
3. Opção 1: Bloquear (manter histórico)
   OU
4. Opção 2: Excluir (se sem vínculos)

Recomendado: BLOQUEAR
? Mantém logs
? Mantém auditoria
? Pode reativar se necessário
```

---

## ?? Solução de Problemas

### ? "Email ou senha incorretos"
**Soluções:**
- Verificar caps lock
- Confirmar email correto
- Resetar senha se necessário
- Contatar administrador

### ? "Conta bloqueada"
**Soluções:**
- Aguardar 5 minutos (bloqueio automático)
- Contatar administrador para desbloqueio manual
- Verificar se foi bloqueado por segurança

### ? "Sessão expirada"
**Soluções:**
- Fazer login novamente
- Marcar "Lembrar-me" para sessão mais longa
- Verificar se não ultrapassou 8 horas de inatividade

### ? "Acesso negado"
**Soluções:**
- Verificar se tem permissão para a ação
- Contatar manager/admin para solicitar permissão
- Confirmar se está com a role correta

---

## ? Boas Práticas

### ?? Segurança

? **Faça:**
- Trocar senha padrão imediatamente
- Usar senhas fortes e únicas
- Fazer logout ao sair
- Não compartilhar credenciais
- Reportar acessos suspeitos

? **Evite:**
- Senhas fracas (123456, senha, etc.)
- Mesma senha em vários sistemas
- Deixar sessão aberta em PC público
- Anotar senha em local visível
- Compartilhar conta com colegas

### ?? Gestão de Usuários

? **Faça:**
- Criar usuários com role apropriada
- Revisar permissões regularmente
- Bloquear usuários inativos
- Manter registro de alterações
- Documentar motivos de bloqueio

? **Evite:**
- Dar privilégios além do necessário
- Criar Admin desnecessariamente
- Deixar contas de ex-funcionários ativas
- Excluir sem analisar impacto
- Ignorar alertas de segurança

---

## ?? Contato e Suporte

**Para problemas de acesso:**
- ?? Email: suporte@litoralsul.com.br
- ?? Telefone: (13) 3XXX-XXXX
- ?? Falar com administrador do sistema

**Horário de atendimento:**
- Segunda a Sexta: 8h às 18h
- Sábado: 8h às 12h

---

## ?? Pronto para Usar!

O sistema de autenticação está **100% configurado e seguro**.

**Primeiro acesso:**
1. Usar credenciais padrão do Admin
2. Trocar senha imediatamente
3. Criar demais usuários
4. Começar a operar!

---

**Desenvolvido para:** Litoral Sul Locadora e Turismo  
**Versão:** 1.0  
**Data:** Outubro/2025  
**Tecnologia:** ASP.NET Core Identity 8.0
