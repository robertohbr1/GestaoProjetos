# Regras do Projeto — Gestão de Projetos e Demandas

As seguintes regras e diretrizes de desenvolvimento devem ser seguidas **estritamente** neste projeto.

---

## 🎨 Acessibilidade WCAG AA — Regras Obrigatórias

Todo código frontend Angular deve atender ao nível **WCAG 2.1 AA** de acessibilidade sem exceções.

### 1. Família de Fontes

- Usar **exclusivamente**: `'Inter', system-ui, -apple-system, sans-serif`
- Nunca usar `font-weight < 400` em texto de leitura
- A fonte Inter **deve** ser carregada no `index.html` via Google Fonts

### 2. Tamanhos de Fonte Mínimos (aplicar via `!important` quando necessário)

| Contexto | Tamanho Mínimo |
|---|---|
| Texto geral, parágrafos, descrições | `15px` |
| Células de tabela (`mat-cell`) | `15px` |
| Labels de formulário (`mdc-floating-label`) | `15px` |
| Valores de inputs e selects | `15px` |
| Itens de menu e dropdown | `15px` |
| Metadados, datas, legendas secundárias | `14px` |
| Rótulos de badges, counters, chips | `13px` mínimo |
| **PROIBIDO** | `11px`, `12px` — NUNCA usar |

### 3. Contraste de Cores — WCAG AA (fundo escuro `#080c14`)

O projeto usa **tema escuro**. Cores de texto **devem** garantir contraste mínimo 4.5:1 sobre `#080c14` ou `#0f1626`:

| Uso | Cor Mínima Aprovada | Contraste |
|---|---|---|
| Texto principal | `#f9fafb` (`--text-primary`) | 19:1 ✅ |
| Texto secundário | `#d1d5db` (`--text-secondary`) | 11.2:1 ✅ |
| Texto auxiliar/muted | `#9ca3af` (`--text-muted`) | 5.9:1 ✅ |
| Link/botão ativo | `#60a5fa` (azul claro) | 5.7:1 ✅ |
| Badge verde (Done) | `#34d399` | 7.2:1 ✅ |
| **PROIBIDO** | `rgba(255,255,255,0.3)` ou menos | < 2:1 ❌ |
| **PROIBIDO** | Cor escura sobre fundo escuro | ❌ |

### 4. Overrides Obrigatórios para Angular Material (MDC)

O projeto usa `indigo-pink.css` (tema **claro**) como base. Todo componente sobre fundo escuro **deve** ser sobrescrito explicitamente. Regras obrigatórias no `styles.scss`:

- **`mat-button` e `mat-icon-button` inline** (ex: status/prioridade na tabela): forçar `color: inherit !important` e garantir que o texto do botão herde a cor do contexto de tema escuro.
- **`mat-select`**: `--mat-select-trigger-text-color`, `.mat-mdc-select-value`, `.mdc-list-item__primary-text` devem ser `var(--text-primary)`
- **`mdc-floating-label`**: sempre `color: var(--text-secondary)` com `font-size: 15px`
- **`mdc-text-field`**: background `rgba(255,255,255,0.05)`, não menor
- **`mat-menu`**: fundo `#1a2240` com texto `var(--text-primary)`
- **`mat-button` sem cor**: o padrão MDC aplica cor escura — sempre definir explicitamente com `color: var(--text-primary)`

### 5. Buttons do tipo "trigger" em tabelas

Botões `mat-button` usados como triggers (ex: seleção de status/prioridade diretamente na tabela) **devem ter** cor de texto explícita:

```scss
// ❌ ERRADO — herda cor escura do tema claro
.priority-trigger-btn { font-size: 14px; }

// ✅ CORRETO — força cor legível no tema escuro
.priority-trigger-btn,
.status-trigger-btn {
  color: inherit !important;  // herda da classe .priority-* ou .status-*
  font-size: 15px !important;
  font-weight: 500 !important;
}
```

### 6. Espaçamento e Legibilidade

- `line-height` mínimo de `1.6` para texto corrido
- `padding` mínimo de `12px 16px` em células de tabela com texto
- `gap` mínimo de `8px` entre elementos agrupados

### 7. Foco de Teclado

- Sempre definir `*:focus-visible { outline: 2px solid var(--primary-color); outline-offset: 2px; }`
- Nunca usar `outline: none` sem alternativa visual equivalente

### 8. ARIA e Semântica

- Todo `mat-icon-button` sem texto visível **deve** ter `aria-label` ou `title`
- Usar `lang="pt-BR"` no `<html>`
- Usar `<h1>` único por página (no título principal com `.text-gradient`)

---

## 🔐 Segurança e Mapeamento de Papéis (RBAC)

Devido às convenções de serialização de Claims em tokens JWT do ASP.NET Core Web API:

### Robustez na Decodificação de Claims

Ao ler as propriedades do token JWT no cliente (Angular), o interpretador deve verificar tanto as chaves curtas quanto as URIs XML completas:

- **Nome:** `decoded.unique_name` ou `decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']`
- **Cargo/Perfil:** `decoded.role` ou `decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']`
- **Identificador:** `decoded.nameid` ou `decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']`

---

## 🛡️ Backend — Serialização de Enums

O backend C# **deve sempre** configurar `JsonStringEnumConverter` no `AddControllers()`:

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
```

Isso garante que o Angular envie `"Backlog"`, `"Angular"` etc. e o backend deserialize corretamente.
