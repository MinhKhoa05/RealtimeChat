# Design System & UI/UX Reference Guide

This guide documents how to consult curated references and maintain the project-specific living design system (`MASTER.md`).

---

## 1. Golden Rule: Always Derive from References or Code

> [!IMPORTANT]
> **Never invent arbitrary colors, fonts, or styles.**
> When establishing or updating a design system:
> - If code exists: Extract the project's actual CSS custom properties, Tailwind theme tokens, and component classes.
> - If starting greenfield or standardizing: **Agent must inspect the relevant reference CSVs** (`data/products.csv`, `data/colors.csv`, `data/styles.csv`, `data/typography.csv`) to look up concrete, curated values matching the product archetype. Never hardcode random colors or guess typography.

---

## 2. Living Design System Workflow (`MASTER.md`)

`MASTER.md` (stored at `docs/design-system/MASTER.md` or `design-system/MASTER.md`) serves as the **single source of truth** for a project's visual identity, design tokens, component standards, and layout conventions.

### When to Consult `MASTER.md`
- **If `MASTER.md` exists:** Treat it as your **primary source of truth**. Read it before implementing UI to extract established colors, typography, spacing, border radii, and component variants. Only consult external datasets (`data/`) when additional guidance is needed (e.g. specialized UX guidelines), without overriding established project decisions.
- **If `MASTER.md` is missing:** Adhere to the project's existing CSS custom properties, Tailwind theme, and UI components. Consult the reference datasets below when design guidance is needed. **Do not create `MASTER.md` automatically.**

### Initializing `MASTER.md` (Strictly Opt-In)
Only create or update `MASTER.md` when the user **explicitly requests** establishing, harmonizing, or documenting a design system:

- **Case A: Existing Codebase with Inconsistent Styling**
  1. **Audit existing styles:** Scan `globals.css` / `index.css`, `tailwind.config.*`, and existing components for established CSS variables, Tailwind utility patterns, font families, and color choices.
  2. **Lookup curated standards to harmonize:** If styles are inconsistent (e.g. conflicting shades of blue, random radii like `rounded-sm` vs `rounded-2xl`, arbitrary margins like `mt-[13px]`), inspect [`data/colors.csv`](../data/colors.csv), [`data/styles.csv`](../data/styles.csv), and [`data/typography.csv`](../data/typography.csv) to map and normalize chaotic values into a clean, unified token set matching the product type.
  3. **Save `MASTER.md`:** Write out the standardized tokens and document the established conventions.

- **Case B: Greenfield / Brand-New Project**
  1. **Identify product type:** Look up the product archetype in [`data/products.csv`](../data/products.csv) & [`data/ui-reasoning.csv`](../data/ui-reasoning.csv).
  2. **Retrieve concrete reference tokens:**
     - Primary, Secondary, Background, Card, Border, Ring, Destructive hex values ➔ Read directly from [`data/colors.csv`](../data/colors.csv).
     - Recommended style, border radius, and elevation formula ➔ Read directly from [`data/styles.csv`](../data/styles.csv).
     - Heading & Body font pairing, Google Fonts URL, and CSS import ➔ Read directly from [`data/typography.csv`](../data/typography.csv).
  3. **Save `MASTER.md`:** Populate the template with the retrieved values.

---

## 3. Structure Template for `MASTER.md`

When generating `MASTER.md`, populate all bracketed fields with concrete values retrieved from code or the reference datasets:

```markdown
# Project Design System (MASTER.md)
> Status: Active | Product Type: [Product Type from data/products.csv] | Last Updated: [YYYY-MM-DD] - [Summary of change]

## 1. Visual Direction & Style
- **Aesthetic:** [Style Category from data/styles.csv matching archetype]
- **Best For:** [Context from styles.csv]
- **Key Effects:** [Effects & Animation from styles.csv / ui-reasoning.csv]
- **Border Radius:** [Derived from design system variables in styles.csv]
- **Shadows & Elevation:** [Shadow formula from styles.csv]

## 2. Semantic Color Tokens
*(Values retrieved from data/colors.csv for this Product Type)*

| Role | CSS Variable | Hex / Value | Purpose & Notes |
|---|---|---|---|
| Primary | `--color-primary` | [Primary hex from colors.csv] | Main brand action, primary CTAs |
| On Primary | `--color-on-primary` | [On Primary hex from colors.csv] | Text/icon on primary background |
| Secondary | `--color-secondary` | [Secondary hex from colors.csv] | Supporting buttons, secondary badges |
| Background | `--color-background` | [Background hex from colors.csv] | Canvas/page background |
| Foreground | `--color-foreground` | [Foreground hex from colors.csv] | Primary text color |
| Surface / Card | `--color-card` | [Card hex from colors.csv] | Card and container surface |
| Muted | `--color-muted` | [Muted hex from colors.csv] | Muted backgrounds, table row hover |
| Muted Text | `--color-muted-foreground` | [Muted Foreground from colors.csv] | Labels, secondary text |
| Border | `--color-border` | [Border hex from colors.csv] | Card dividers, input borders |
| Destructive | `--color-destructive` | [Destructive hex from colors.csv] | Error states, delete actions |
| Ring | `--color-ring` | [Ring hex from colors.csv] | Focus ring outline for a11y |

## 3. Typography Scale
*(Pairing retrieved from data/typography.csv)*

- **Heading Font:** [Heading Font name] | **Body Font:** [Body Font name]
- **Google Fonts Import:** [CSS Import from typography.csv]
- **Tailwind Config:** [Tailwind Config snippet from typography.csv]
- **Type Hierarchy:**
  - Display / H1: `2rem` (32px), bold, tracking-tight
  - H2: `1.5rem` (24px), font-semibold
  - H3: `1.25rem` (20px), font-medium
  - Body: `1rem` (16px), regular, line-height 1.5
  - Small / Helper: `0.875rem` (14px), text-muted

## 4. Spacing & Layout
- **Spacing Scale:** Systematic 4px / 0.25rem increments (p-2, p-4, p-6, gap-3, gap-4).
- **Container Max-Widths:** Content container `max-w-6xl` (1152px), Form container `max-w-md`.
- **Breakpoints:** Project defaults (`sm: 640px`, `md: 768px`, `lg: 1024px`, `xl: 1280px`).

## 5. Component Patterns
- **Buttons:** Primary (`bg-primary text-on-primary`), Ghost/Outline for secondary actions.
- **Inputs:** Standard height (h-10), ring on focus (`focus-visible:ring-2 focus-visible:ring-primary`).
- **Cards:** Clean border (`border border-border bg-card rounded-lg p-4 sm:p-6`).
```

---

## 4. Curated UI/UX Reference Catalog

All reference datasets are stored as concise CSV files in `data/`. When design decisions are needed, read the corresponding CSV directly:

| Domain | File | Content & Usage |
|---|---|---|
| **Product Archetypes** | [`data/products.csv`](../data/products.csv) | **13 web application patterns** (SaaS, Social & Chat, E-commerce, Dashboards, Admin Systems, AI Copilots, Portfolios, etc.) with layout structures and color palette focus. |
| **Semantic Colors** | [`data/colors.csv`](../data/colors.csv) | **13 semantic palette token sets** with tested hex values for Primary, On Primary, Secondary, Background, Card, Muted, Border, Destructive, and Ring. |
| **Visual Styles** | [`data/styles.csv`](../data/styles.csv) | **20 practical styles** (Minimalism & Swiss Style, Soft UI Evolution, Bento Grids, Clean Modern E-commerce, High-Contrast Dark Mode, Glassmorphism, Corporate Clean, etc.) with color formulas, effects, and technical checklists. |
| **Typography** | [`data/typography.csv`](../data/typography.csv) | **15 curated font pairings** (Heading + Body) with Google Fonts URLs, CSS imports, and 100% full Vietnamese / Latin Extended support. |
| **Design Reasoning** | [`data/ui-reasoning.csv`](../data/ui-reasoning.csv) | **13 archetype decision matrices** with recommended patterns, style priorities, color moods, typography moods, and anti-patterns. |
| **UX Guidelines** | [`data/ux-guidelines.csv`](../data/ux-guidelines.csv) | **110 web usability and WCAG accessibility guidelines** categorized by Interaction, Form, Layout, Navigation, and Feedback with good/bad code snippets. |
| **Tech Stacks** | `data/stacks/` | Targeted guidelines for **React 19** (`react.csv`), **Next.js** (`nextjs.csv`), **shadcn/ui** (`shadcn.csv`), and **Tailwind CSS** (`html-tailwind.csv`). Consult only when the project uses that specific stack. |

---

## 5. Visual Quality & Avoiding the "AI Aesthetic"

AI-generated UI often defaults to generic, clichéd patterns. Be intentional with styling:

| AI Default | Why It Is a Problem | Production Quality |
|---|---|---|
| **Purple/indigo everything** | Models default to visually "safe" palettes, making every app look identical | Use the project's actual brand and semantic palette from `colors.csv` |
| **Excessive gradients** | Gradients add visual noise and clash with most design systems | Flat or purposeful subtle tints matching the design system |
| **Rounded everything (`rounded-2xl`)** | Maximum rounding signals "friendly" but ignores hierarchy of corner radii | Consistent border-radius from `styles.csv` (`rounded-md` or `rounded-lg`) |
| **Generic hero sections** | Template-driven layout with no connection to content or user need | Content-first, purpose-driven layouts matching `products.csv` |
| **Lorem ipsum-style copy** | Placeholder text hides layout problems that real content reveals (wrapping, overflow) | Realistic, domain-appropriate copy |
| **Oversized padding everywhere** | Equal generous padding destroys visual hierarchy and wastes screen space | Systematic spacing scale matching the content density |
| **Stock card grids** | Uniform grids ignore information priority and scanning patterns | Purpose-driven layouts with distinct hierarchy |
| **Shadow-heavy design** | Layered shadows add depth that competes with content | Subtle border-based contrast or purposeful shadows from `styles.csv` |
