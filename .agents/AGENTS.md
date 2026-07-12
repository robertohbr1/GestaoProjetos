# Regras do Projeto — Gestão de Projetos e Demandas

As seguintes regras e diretrizes de desenvolvimento devem ser seguidas estritamente neste projeto.

---

## 🎨 Acessibilidade (Accessibility) e Tipografia

Toda interface web desenvolvida no projeto frontend deve seguir as diretrizes de acessibilidade WCAG (Web Content Accessibility Guidelines):

1.  **Família de Fontes:**
    *   Utilizar exclusivamente a pilha de fontes legíveis configurada no projeto: `'Inter', system-ui, -apple-system, sans-serif`.
    *   Evitar o uso de variações muito finas (font-weight < 400) em blocos de texto principais ou tabelas de dados.

2.  **Tamanhos de Fonte Mínimos:**
    *   **Texto Geral, Células de Tabela e Formulários:** Mínimo de `15px`.
    *   **Metadados, Legendas, Rótulos e Counters:** Mínimo absoluto de `13px` (preferencialmente `14px`).
    *   Nunca utilizar fontes de tamanho `11px` ou `12px` em elementos visíveis de texto.

3.  **Contraste de Cores:**
    *   Todas as cores de texto e ícones funcionais devem manter uma taxa de contraste em relação ao fundo escuro que atenda ao nível **WCAG AA** (mínimo de 4.5:1) e, idealmente, **WCAG AAA** (7:1).
    *   Não utilizar opacidades baixas (ex: `rgba(255,255,255,0.4)`) diretamente sobre fundos escuros para textos com função de leitura.

4.  **Espaçamento e Legibilidade:**
    *   Todos os elementos de texto corrido, descrições de demandas e histórico devem possuir `line-height` de no mínimo `1.5` ou `1.6` para facilitar a leitura.
    *   Tabelas e listas devem conter espaçamento interno (padding) adequado para evitar congestionamento visual.

---

## 🔐 Segurança e Mapeamento de Papéis (RBAC)

Devido às convenções de serialização de Claims em tokens JWT do ASP.NET Core Web API:

1.  **Robustez na Decodificação de Claims:**
    *   Ao ler as propriedades do token JWT no cliente (Angular), o interpretador deve verificar tanto as chaves curtas geradas por handlers específicos quanto as URIs XML completas geradas pelo pipeline padrão do .NET.
    *   Mapeamento obrigatório para:
        *   **Nome:** `decoded.unique_name` ou `decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']`
        *   **Cargo/Perfil:** `decoded.role` ou `decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']`
        *   **Identificador:** `decoded.nameid` ou `decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']`
