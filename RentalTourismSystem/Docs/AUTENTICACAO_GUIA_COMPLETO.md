# ?? Sistema de Autentica��o e Controle de Acesso - Guia Completo

## ?? �ndice
- [Vis�o Geral](#vis�o-geral)
- [Tipos de Usu�rios](#tipos-de-usu�rios)
- [Login e Registro](#login-e-registro)
- [Gerenciamento de Usu�rios](#gerenciamento-de-usu�rios)
- [Permiss�es por M�dulo](#permiss�es-por-m�dulo)
- [Seguran�a](#seguran�a)
- [Recupera��o de Senha](#recupera��o-de-senha)

---

## ?? Vis�o Geral

O sistema utiliza **ASP.NET Core Identity** para autentica��o e autoriza��o baseada em roles (fun��es), garantindo acesso seguro e controlado a todas as funcionalidades.

### ?? Recursos de Seguran�a

? **Autentica��o Robusta**
- Login com email e senha
- Valida��o de credenciais
- Sess�o segura (8 horas)
- Prote��o contra ataques

? **Autoriza��o Baseada em Roles**
- 4 n�veis de acesso
- Permiss�es granulares
- Controle por m�dulo
- Hierarquia de privil�gios

? **Gest�o de Usu�rios**
- Cadastro de novos usu�rios
- Edi��o de informa��es
- Altera��o de senha
- Bloqueio e desbloqueio

---

## ?? Tipos de Usu�rios

### ?? **Admin (Administrador)**

**Privil�gios TOTAIS:**
- ? Acesso completo ao sistema
- ? Gerenciar todos os usu�rios
- ? Criar, editar e excluir TUDO
- ? Acessar relat�rios gerenciais
- ? Configura��es do sistema
- ? Backup e restore
- ? Logs e auditoria

**Acesso aos m�dulos:**
```
? Clientes (CRUD completo)
? Ve�culos (CRUD completo)
? Loca��es (CRUD completo)
? Manuten��es (CRUD completo)
? Reservas (CRUD completo)
? Pacotes (CRUD completo)
? Funcion�rios (CRUD completo)
? Ag�ncias (CRUD completo)
? Usu�rios (CRUD completo)
? Relat�rios (todos)
? Documentos (upload/exclus�o)
```

**Usu�rio padr�o:**
```
Email: admin@litoralsul.com.br
Senha: Admin@123456
```

### ?? **Manager (Gerente)**

**Privil�gios GERENCIAIS:**
- ? Gerenciar opera��es di�rias
- ? Criar e editar registros principais
- ? Aprovar loca��es e reservas
- ? Acessar relat�rios operacionais
- ?? N�O gerencia usu�rios Admin
- ?? N�O acessa configura��es cr�ticas

**Acesso aos m�dulos:**
```
? Clientes (CRUD completo)
? Ve�culos (CRUD completo)
? Loca��es (CRUD completo)
? Manuten��es (CRUD completo)
? Reservas (CRUD completo)
? Pacotes (CRUD completo)
? Funcion�rios (CRUD)
? Ag�ncias (CRUD)
? Usu�rios (apenas visualiza��o)
? Relat�rios (operacionais)
? Documentos (upload/exclus�o)
```

### ?? **Employee (Funcion�rio)**

**Privil�gios OPERACIONAIS:**
- ? Criar loca��es e reservas
- ? Cadastrar clientes
- ? Fazer vistorias
- ? Upload de documentos
- ?? N�O exclui registros
- ?? N�O gerencia usu�rios
- ?? Relat�rios limitados

**Acesso aos m�dulos:**
```
? Clientes (criar/editar)
??? Ve�culos (visualiza��o)
? Loca��es (criar/editar/finalizar)
??? Manuten��es (visualiza��o)
? Reservas (criar/editar)
??? Pacotes (visualiza��o)
??? Funcion�rios (visualiza��o)
??? Ag�ncias (visualiza��o)
? Usu�rios (sem acesso)
??? Relat�rios (b�sicos)
? Documentos (upload, sem exclus�o)
```

### ?? **User (Usu�rio B�sico)**

**Privil�gios LIMITADOS:**
- ??? Visualiza��o de informa��es
- ??? Consultas b�sicas
- ? Sem permiss�es de edi��o
- ? Sem acesso a dados sens�veis

**Acesso aos m�dulos:**
```
??? Clientes (visualiza��o)
??? Ve�culos (visualiza��o)
??? Loca��es (visualiza��o)
??? Manuten��es (visualiza��o)
??? Reservas (visualiza��o)
??? Pacotes (visualiza��o)
? Funcion�rios (sem acesso)
? Ag�ncias (sem acesso)
? Usu�rios (sem acesso)
? Relat�rios (sem acesso)
? Documentos (sem acesso)
```

---

## ?? Login e Registro

### ?? Tela de Login

**Acesso:** `/Account/Login`

**Formul�rio:**
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

### ? Valida��es de Login

**Sistema valida:**
1. **Email:**
   - ? Formato v�lido
   - ? Cadastrado no sistema
   - ? Conta ativa

2. **Senha:**
   - ? Senha correta
   - ? Conta n�o bloqueada
   - ? Tentativas de login

### ?? Seguran�a do Login

**Prote��es implementadas:**

1. **Bloqueio por Tentativas**
   - M�ximo: 5 tentativas erradas
   - Bloqueio: 5 minutos
   - Reset autom�tico ap�s sucesso

2. **Sess�o Segura**
   - Dura��o: 8 horas
   - Cookie HttpOnly
   - HTTPS obrigat�rio (produ��o)
   - Anti-CSRF token

3. **Valida��o de Senha**
   - M�nimo 6 caracteres
   - Pelo menos 1 mai�scula
   - Pelo menos 1 min�scula
   - Pelo menos 1 n�mero

### ?? Registro de Novo Usu�rio

**Acesso:** `/Account/Register`

> ?? **Importante:** Apenas Admin e Manager podem criar novos usu�rios!

**Formul�rio:**
```
CADASTRAR NOVO USU�RIO

Nome Completo:
[_________________________________]

Email:
[_________________________________]

Senha:
[_________________________________]

Confirmar Senha:
[_________________________________]

Fun��o (Role):
[ Selecione ? ]
  - Admin
  - Manager
  - Employee
  - User

[    CADASTRAR    ]  [  CANCELAR  ]
```

### ? Valida��es de Registro

**Campos obrigat�rios:**
- ? Nome completo
- ? Email v�lido e �nico
- ? Senha forte
- ? Confirma��o de senha
- ? Fun��o (role)

**Regras de senha:**
```
? M�nimo 6 caracteres
? Pelo menos 1 letra mai�scula
? Pelo menos 1 letra min�scula
? Pelo menos 1 n�mero
? Caracteres especiais: opcional
```

**Exemplo de senha v�lida:**
```
? Admin@123456
? Manager2024
? Employee123
```

---

## ????? Gerenciamento de Usu�rios

### ?? Como Acessar

**Menu ? Administra��o ? Gerenciar Usu�rios**

OU

**URL direta:** `/Account/ManageUsers`

> ?? **Acesso:** Apenas Admin e Manager

### ?? Lista de Usu�rios

**Visualiza��o:**
```
???????????????????????????????????????????????????
?  GERENCIAR USU�RIOS                    [+ Novo] ?
???????????????????????????????????????????????????
?                                                 ?
?  ?? Jo�o Silva                                  ?
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

### ?? Editar Usu�rio

**Acesso:** Lista ? Bot�o "?? Editar"

**Campos edit�veis:**
```
EDITAR USU�RIO

Nome: [Jo�o Silva                    ]
Email: [joao@litoralsul.com.br      ]
Role: [Manager ?]

Status: ( ) Ativo  ( ) Bloqueado

[  SALVAR  ]  [  CANCELAR  ]
```

**Permiss�es para edi��o:**
- ? **Admin:** Pode editar TODOS os usu�rios
- ?? **Manager:** Pode editar Employee e User (N�O Admin)
- ? **Employee/User:** Sem permiss�o

### ?? Alterar Senha

**Op��o 1 - Pr�prio Usu�rio:**
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

**Op��o 2 - Admin/Manager alterando:**
```
Gerenciar Usu�rios ? Editar ? Redefinir Senha

Nova Senha:
[_________________________________]

Confirmar Senha:
[_________________________________]

[  REDEFINIR  ]
```

### ?? Bloquear/Desbloquear Usu�rio

**Quando bloquear:**
- Suspeita de acesso n�o autorizado
- Funcion�rio afastado temporariamente
- Viola��o de pol�tica de uso
- Solicita��o de seguran�a

**Como fazer:**
```
Gerenciar Usu�rios ? Selecionar usu�rio ? ?? Bloquear

Motivo do bloqueio:
[_________________________________]

[  CONFIRMAR BLOQUEIO  ]
```

**Efeitos:**
- ? Usu�rio n�o consegue fazer login
- ?? Mensagem: "Conta bloqueada. Entre em contato com administrador"
- ?? Bloqueio registrado em log

**Para desbloquear:**
```
Gerenciar Usu�rios ? Usu�rio bloqueado ? ?? Desbloquear

[  DESBLOQUEAR  ]
```

### ??? Excluir Usu�rio

**Regras:**
- ? Pode excluir: Usu�rios sem v�nculos
- ? N�O pode excluir: Usu�rio logado
- ? N�O pode excluir: Admin principal
- ?? Manager n�o pode excluir Admin

**Como fazer:**
```
Gerenciar Usu�rios ? Selecionar ? ??? Excluir

?? ATEN��O!
Esta a��o n�o pode ser desfeita.

Deseja realmente excluir o usu�rio?
Nome: Jo�o Silva
Email: joao@litoralsul.com.br

[  CONFIRMAR EXCLUS�O  ]  [  CANCELAR  ]
```

---

## ?? Permiss�es por M�dulo

### ?? Tabela Completa de Permiss�es

| M�dulo | Admin | Manager | Employee | User |
|--------|-------|---------|----------|------|
| **Clientes** |  |  |  |  |
| Visualizar | ? | ? | ? | ? |
| Criar | ? | ? | ? | ? |
| Editar | ? | ? | ? | ? |
| Excluir | ? | ? | ? | ? |
| **Ve�culos** |  |  |  |  |
| Visualizar | ? | ? | ? | ? |
| Criar | ? | ? | ? | ? |
| Editar | ? | ? | ? | ? |
| Excluir | ? | ? | ? | ? |
| Alterar Status | ? | ? | ? | ? |
| **Loca��es** |  |  |  |  |
| Visualizar | ? | ? | ? | ? |
| Criar | ? | ? | ? | ? |
| Editar | ? | ? | ? | ? |
| Excluir | ? | ? | ? | ? |
| Finalizar | ? | ? | ? | ? |
| **Manuten��es** |  |  |  |  |
| Visualizar | ? | ? | ? | ? |
| Criar | ? | ? | ? | ? |
| Editar | ? | ? | ? | ? |
| Excluir | ? | ? | ? | ? |
| Relat�rios | ? | ? | ? | ? |
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
| **Funcion�rios** |  |  |  |  |
| Visualizar | ? | ? | ? | ? |
| Criar | ? | ? | ? | ? |
| Editar | ? | ? | ? | ? |
| Excluir | ? | ? | ? | ? |
| **Ag�ncias** |  |  |  |  |
| Visualizar | ? | ? | ? | ? |
| Criar | ? | ? | ? | ? |
| Editar | ? | ? | ? | ? |
| Excluir | ? | ? | ? | ? |
| **Usu�rios** |  |  |  |  |
| Visualizar | ? | ? | ? | ? |
| Criar | ? | ? | ? | ? |
| Editar | ? | ?* | ? | ? |
| Excluir | ? | ?* | ? | ? |
| **Relat�rios** |  |  |  |  |
| Gerenciais | ? | ? | ? | ? |
| Operacionais | ? | ? | ? | ? |
| Financeiros | ? | ? | ? | ? |

> \* Manager n�o pode editar/excluir Admin

### ?? P�gina de Acesso Negado

**Quando usu�rio tenta acessar sem permiss�o:**

```
???????????????????????????????????
?   ?? ACESSO NEGADO              ?
?                                 ?
?   Voc� n�o tem permiss�o        ?
?   para acessar esta p�gina.     ?
?                                 ?
?   Entre em contato com o        ?
?   administrador do sistema.     ?
?                                 ?
?   [  VOLTAR AO IN�CIO  ]        ?
???????????????????????????????????
```

**URL:** `/Account/AccessDenied`

---

## ?? Seguran�a

### ??? Recursos de Seguran�a

#### 1?? **Prote��o de Senha**

**Requisitos:**
```
? M�nimo 6 caracteres
? Letra mai�scula
? Letra min�scula
? N�mero
? Hash bcrypt (n�o armazena texto puro)
```

**Exemplos:**
```
? Admin@123456    (v�lida)
? Manager2024     (v�lida)
? Employee123     (v�lida)
? admin           (muito fraca)
? 123456          (sem letras)
? senha           (sem n�meros/mai�scula)
```

#### 2?? **Bloqueio de Conta**

**Configura��o:**
- M�ximo tentativas: 5
- Tempo de bloqueio: 5 minutos
- Reset ap�s sucesso: Sim
- Permitido para novos: Sim

**Cen�rio:**
```
Tentativa 1: Senha errada ?
Tentativa 2: Senha errada ?
Tentativa 3: Senha errada ?
Tentativa 4: Senha errada ?
Tentativa 5: Senha errada ?
? CONTA BLOQUEADA POR 5 MINUTOS
```

#### 3?? **Sess�o Segura**

**Configura��es:**
- Dura��o: 8 horas
- Cookie: HttpOnly ?
- SameSite: Lax ?
- Secure: Sim (produ��o) ?
- Sliding: Sim ?

**Renova��o:**
- Atividade = Renova sess�o
- Inativo 8h = Logout autom�tico

#### 4?? **Prote��o CSRF**

**Anti-Forgery Token:**
```html
<!-- Automaticamente inclu�do em formul�rios -->
<form method="post">
    @Html.AntiForgeryToken()
    ...
</form>
```

**Headers de seguran�a:**
```
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
Strict-Transport-Security: max-age=31536000
```

#### 5?? **Rate Limiting**

**Limites por pol�tica:**

| Recurso | Limite | Janela |
|---------|--------|--------|
| API Geral | 100 req | 1 min |
| Valida��o CPF | 20 req | 1 min |
| Dashboard | 300 req | 1 min |

**Resposta ao exceder:**
```
HTTP 429 Too Many Requests

{
  "error": "Muitas requisi��es. Tente novamente em 60 segundos."
}
```

### ?? Auditoria e Logs

**Sistema registra:**
- ? Todas as tentativas de login
- ? Cria��o/edi��o de usu�rios
- ? Altera��es de permiss�es
- ? Bloqueios de conta
- ? Acessos negados
- ? Opera��es cr�ticas

**Localiza��o dos logs:**
```
RentalTourismSystem/logs/rental-tourism-YYYY-MM-DD.txt
```

**Exemplo de log:**
```
2024-10-20 14:35:12.456 [INFO] Login bem-sucedido: admin@litoralsul.com.br
2024-10-20 14:37:45.123 [WARN] Tentativa de login falha: funcionario@email.com
2024-10-20 15:10:33.789 [INFO] Usu�rio criado: maria@litoralsul.com.br por admin@litoralsul.com.br
2024-10-20 16:22:15.456 [ERROR] Acesso negado: /Account/ManageUsers por user@email.com
```

---

## ?? Recupera��o de Senha

### ?? Esqueci Minha Senha

**Acesso:** Tela de Login ? "Esqueceu a senha?"

**Processo:**
```
1. Clicar em "Esqueceu a senha?"

2. Informar email cadastrado:
   [_________________________________]
   
   [  ENVIAR LINK DE RECUPERA��O  ]

3. Sistema envia email com link

4. Clicar no link recebido

5. Definir nova senha:
   Nova Senha: [________________]
   Confirmar: [________________]
   
   [  REDEFINIR SENHA  ]

6. Senha alterada!
   ? Fazer login com nova senha
```

> ?? **Importante:** Link de recupera��o v�lido por 24 horas

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

**Valida��es:**
- ? Senha atual correta
- ? Nova senha atende requisitos
- ? Confirma��o coincide
- ? Nova senha diferente da atual

---

## ?? Casos de Uso

### Caso 1: Primeiro Acesso

```
Sistema instalado, usar credenciais padr�o:

Email: admin@litoralsul.com.br
Senha: Admin@123456

? Fazer login
? TROCAR SENHA imediatamente
? Criar outros usu�rios
```

### Caso 2: Novo Funcion�rio

```
Admin/Manager:
1. Menu ? Gerenciar Usu�rios
2. Clicar "+ Novo Usu�rio"
3. Preencher:
   Nome: Maria Santos
   Email: maria@litoralsul.com.br
   Senha: Employee123
   Role: Employee
4. Salvar

Funcion�rio:
1. Receber credenciais
2. Fazer login
3. Alterar senha no primeiro acesso
4. Come�ar a trabalhar
```

### Caso 3: Bloqueio por Seguran�a

```
Situa��o: Suspeita de acesso n�o autorizado

Admin:
1. Gerenciar Usu�rios
2. Localizar usu�rio suspeito
3. Clicar "?? Bloquear"
4. Informar motivo: "Tentativa de acesso suspeita"
5. Confirmar

Resultado:
? Usu�rio n�o consegue logar
? Ver mensagem de bloqueio
? Deve entrar em contato com admin
```

### Caso 4: Funcion�rio Saiu da Empresa

```
Admin/Manager:
1. Gerenciar Usu�rios
2. Localizar ex-funcion�rio
3. Op��o 1: Bloquear (manter hist�rico)
   OU
4. Op��o 2: Excluir (se sem v�nculos)

Recomendado: BLOQUEAR
? Mant�m logs
? Mant�m auditoria
? Pode reativar se necess�rio
```

---

## ?? Solu��o de Problemas

### ? "Email ou senha incorretos"
**Solu��es:**
- Verificar caps lock
- Confirmar email correto
- Resetar senha se necess�rio
- Contatar administrador

### ? "Conta bloqueada"
**Solu��es:**
- Aguardar 5 minutos (bloqueio autom�tico)
- Contatar administrador para desbloqueio manual
- Verificar se foi bloqueado por seguran�a

### ? "Sess�o expirada"
**Solu��es:**
- Fazer login novamente
- Marcar "Lembrar-me" para sess�o mais longa
- Verificar se n�o ultrapassou 8 horas de inatividade

### ? "Acesso negado"
**Solu��es:**
- Verificar se tem permiss�o para a a��o
- Contatar manager/admin para solicitar permiss�o
- Confirmar se est� com a role correta

---

## ? Boas Pr�ticas

### ?? Seguran�a

? **Fa�a:**
- Trocar senha padr�o imediatamente
- Usar senhas fortes e �nicas
- Fazer logout ao sair
- N�o compartilhar credenciais
- Reportar acessos suspeitos

? **Evite:**
- Senhas fracas (123456, senha, etc.)
- Mesma senha em v�rios sistemas
- Deixar sess�o aberta em PC p�blico
- Anotar senha em local vis�vel
- Compartilhar conta com colegas

### ?? Gest�o de Usu�rios

? **Fa�a:**
- Criar usu�rios com role apropriada
- Revisar permiss�es regularmente
- Bloquear usu�rios inativos
- Manter registro de altera��es
- Documentar motivos de bloqueio

? **Evite:**
- Dar privil�gios al�m do necess�rio
- Criar Admin desnecessariamente
- Deixar contas de ex-funcion�rios ativas
- Excluir sem analisar impacto
- Ignorar alertas de seguran�a

---

## ?? Contato e Suporte

**Para problemas de acesso:**
- ?? Email: suporte@litoralsul.com.br
- ?? Telefone: (13) 3XXX-XXXX
- ?? Falar com administrador do sistema

**Hor�rio de atendimento:**
- Segunda a Sexta: 8h �s 18h
- S�bado: 8h �s 12h

---

## ?? Pronto para Usar!

O sistema de autentica��o est� **100% configurado e seguro**.

**Primeiro acesso:**
1. Usar credenciais padr�o do Admin
2. Trocar senha imediatamente
3. Criar demais usu�rios
4. Come�ar a operar!

---

**Desenvolvido para:** Litoral Sul Locadora e Turismo  
**Vers�o:** 1.0  
**Data:** Outubro/2025  
**Tecnologia:** ASP.NET Core Identity 8.0
