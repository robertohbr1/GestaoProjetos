# Sistema de Gerenciamento de Demandas e Projetos (TI Interna)

Este é um sistema Full-Stack desenvolvido para o controle e gestão de demandas, bugs, requisitos e timesheet da equipe de TI interna. 

A solução utiliza **C# (.NET 10 Web API)** no backend com banco de dados **SQL Server** gerenciado via **Entity Framework Core**, e **Angular 22** no frontend com estilo premium escuro, glassmorphism e **Angular Material**.

---

## 🚀 Funcionalidades Principais

*   **Autenticação e Autorização (RBAC):** Login com geração e validação de tokens JWT. Bloqueio de funcionalidades por perfil (Administrador, Desenvolvedor e Colaborador).
*   **Gestão de Projetos:** CRUD completo de projetos e escopos (restrito a administradores).
*   **Gestão de Demandas (Issues):** Grid tabular dinâmico com ordenação, paginação, busca global por termos e **5 filtros obrigatórios** de pesquisa.
*   **Reordenação de Prioridades e Status:** Alteração rápida de prioridades (Baixa, Média, Alta, Crítica) e de status da demanda diretamente na listagem por menus pop-up.
*   **Timesheet (Apontamento de Horas):** Lançamento diário de horas trabalhadas por demanda com controle de acumulado.
*   **Colaboração Integrada:** Thread de comentários estruturada por demanda e upload de anexos de evidências (prints, PDFs, Word).
*   **Log de Auditoria:** Timeline detalhada registrando automaticamente o histórico de alterações (mudança de responsável, status, datas e prazos).
*   **Dashboard de Relatórios:** Gráficos e painéis rápidos de demandas concluídas em período customizado, carga de trabalho atual por desenvolvedor, e controle de pendências com alertas visuais de prazo vencido/próximo.

---

## 🔑 Credenciais para Teste (Banco Semeado)

O sistema conta com um inicializador que migra o banco de dados e semeia os usuários de teste padrão na primeira execução:

| Perfil | Usuário | Senha | Acesso |
| :--- | :--- | :--- | :--- |
| **Administrador** | `admin` | `admin123` | Acesso Total (CRUD de Projetos, Deletar Demandas) |
| **Desenvolvedor** | `dev1` | `dev123` | Lançar horas, alterar status e prioridades |
| **Colaborador** | `collab1` | `collab123` | Comentar, anexar arquivos e visualizar relatórios |

---

## 📦 Estrutura de Diretórios

```
c:\Projetos\GestaoProjetos\
├── backend\
│   ├── GestaoProjetos.slnx
│   └── GestaoProjetos.Api\
│       ├── Controllers\          # Endpoints REST (Auth, Projects, Issues, etc.)
│       ├── Domain\               # Entidades de domínio e Enums
│       ├── Infrastructure\       # AppDbContext, configurações do EF e migrations
│       ├── Application\          # DTOs e Services (Lógica de negócios e auditoria)
│       └── appsettings.json      # Configuração de conexão com banco e JWT
└── frontend\
    └── gestao-projetos-ui\       # Aplicação Angular 22 (SPA com Angular Material)
        ├── src\app\
        │   ├── core\             # AuthService, AuthGuard e Interceptors HTTP (JWT)
        │   ├── shared\           # Modelos de interfaces TS e Serviços globais
        │   ├── features\         # Telas (Login, Projetos, Demandas, Dashboard)
        │   └── layout\           # Shell com Sidenav e Toolbar
        └── src\styles.scss       # Temática visual dark customizada
```

---

## 🛠️ Como Executar o Projeto

### Pré-requisitos
*   [.NET 10 SDK](https://dotnet.microsoft.com/download)
*   [Node.js 22+](https://nodejs.org/)
*   SQL Server ativo localmente ou instância remota configurada.

---

### Executando o Backend (C# Web API)

1. Entre no diretório do backend:
   ```bash
   cd backend/GestaoProjetos.Api
   ```

2. Certifique-se de que a string de conexão no arquivo `appsettings.json` está correta. A padrão é:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=master;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

3. Execute o comando para rodar o projeto:
   ```bash
   dotnet run
   ```
   > O servidor iniciará em `http://localhost:5151`. A documentação e testes rápidos de API (Scalar) estarão acessíveis em `http://localhost:5151/scalar/v1`. O banco de dados será gerado automaticamente.

---

### Executando o Frontend (Angular 22)

1. Abra um novo terminal e navegue até a pasta do frontend:
   ```bash
   cd frontend/gestao-projetos-ui
   ```

2. Instale as dependências:
   ```bash
   npm install
   ```

3. Execute o servidor de desenvolvimento:
   ```bash
   npm start
   ```
   > A aplicação estará disponível em `http://localhost:4200/`. Faça login utilizando uma das credenciais informadas acima.

---

## 🎨 Design System e Estilo
O frontend foi customizado com uma paleta de cores moderna em tons de azul escuro neon e roxo, aplicando transparências (glassmorphism) e desfoques de fundo nas janelas e tabelas. O feedback visual de prazos nas pendências e a exibição de badges ajudam o gestor a agir rápido nas prioridades.
