# React Component Specifications & State Patterns

Load this file on demand when implementing or reviewing common React UI components (Buttons, Inputs/Forms, Cards, Badges, Alerts, Dialogs, Tables) or verifying interactive state coverage (`default`, `hover`, `focus-visible`, `active`, `disabled`, `loading`, `error`, `empty`).

> **Rule #1:** Always inspect the existing project's design tokens (`index.css`, `globals.css`, `tailwind.config.*`) and shared components (`src/components/`, `components/ui/`) first. Use the specifications below as defaults when building new primitives or auditing existing ones.

---

## 1. Interactive States & Priority

When multiple states apply to a React component, resolve them in this priority order (highest to lowest):

| Priority | State | Trigger / Prop | Visual & Behavioral Treatment |
|----------|-------|----------------|-------------------------------|
| 1 | `disabled` | `disabled={true}` | `opacity-50 pointer-events-none cursor-not-allowed`; prevent click/submit handlers |
| 2 | `loading` | `isLoading={true}` | Spinner or skeleton, `aria-busy="true"`, `disabled={true}` on buttons to prevent double-submit |
| 3 | `active` / `pressed` | `:active` / pointer down | Subtle surface darken or `active:scale-[0.98]`, immediate feedback (<100ms) |
| 4 | `focus-visible` | Keyboard `Tab` focus | `focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2` (`>=3:1` contrast) |
| 5 | `hover` | `:hover` (pointer devices) | Subtle background/border shift (`transition-colors duration-150`); never shift layout bounds |
| 6 | `default` | Resting state | Base semantic tokens (`bg-primary`, `bg-card`, `text-foreground`, `border-border`) |

### Essential JSX & ARIA State Patterns

```tsx
// 1. Loading & Disabled Submit Button
<button
  type="submit"
  disabled={isSubmitting || isDisabled}
  aria-busy={isSubmitting}
  className="inline-flex items-center justify-center gap-2 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground transition-colors hover:bg-primary/90 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50"
>
  {isSubmitting && <SpinnerIcon aria-hidden="true" className="h-4 w-4 animate-spin" />}
  <span>{isSubmitting ? 'Saving...' : 'Save Changes'}</span>
</button>

// 2. Icon-Only Button (MUST have aria-label)
<button
  type="button"
  aria-label="Close modal"
  onClick={onClose}
  className="inline-flex h-9 w-9 items-center justify-center rounded-md text-muted-foreground transition-colors hover:bg-muted hover:text-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
>
  <XIcon aria-hidden="true" className="h-4 w-4" />
</button>

// 3. Accessible Input with Linked Error Message
<div className="space-y-1.5">
  <label htmlFor="email" className="text-sm font-medium text-foreground">
    Email address
  </label>
  <input
    id="email"
    type="email"
    autoComplete="email"
    aria-invalid={Boolean(error)}
    aria-describedby={error ? 'email-error' : 'email-hint'}
    className={`flex h-10 w-full rounded-md border bg-background px-3 py-2 text-base md:text-sm transition-colors focus-visible:outline-none focus-visible:ring-2 ${
      error
        ? 'border-destructive focus-visible:ring-destructive'
        : 'border-input focus-visible:ring-ring'
    }`}
  />
  {error ? (
    <p id="email-error" role="alert" className="text-xs font-medium text-destructive">
      {error}
    </p>
  ) : (
    <p id="email-hint" className="text-xs text-muted-foreground">
      We will never share your email.
    </p>
  )}
</div>
```

---

## 2. Core Component Specifications

### Button
- **Variants:**
  - `primary` / `default`: Primary CTA (at most 1 primary CTA per screen/section). High-contrast fill.
  - `secondary`: Secondary actions. Muted surface fill with readable foreground.
  - `outline`: Tertiary/cancel actions. Transparent background with `1px` border (`border-input hover:bg-accent`).
  - `ghost`: Low-emphasis toolbar or inline row actions. Transparent until hover.
  - `destructive`: Dangerous actions (Delete, Remove, Revoke). Semantic danger fill (`bg-destructive text-destructive-foreground`), visually separated from primary actions.
- **Sizes:**
  - `sm`: `h-8 px-3 text-xs` (compact tables/toolbars)
  - `default`: `h-10 px-4 py-2 text-sm`
  - `lg`: `h-11 px-6 text-base`
  - `icon`: `h-10 w-10` (expand hit target to `min-h-[44px] min-w-[44px]` on mobile touch views)

### Input, Select & Form Controls
- Always pair with a visible `<label htmlFor="...">` (never rely on `placeholder` as the only label).
- Use `text-base` (`16px`) on mobile viewports (`md:text-sm` on desktop) so iOS Safari does not auto-zoom on focus.
- Validate on `blur` or `submit` (avoid showing red error states on the first keystroke while the user is still typing).
- Distinguish `disabled` (non-focusable, muted `opacity-50`) from `readOnly` (focusable so users can copy the value).

### Card (Composition Pattern)
Prefer composable sub-components over mega-props:
```tsx
export function Card({ className = '', children }: React.ComponentProps<'div'>) {
  return (
    <div className={`rounded-xl border border-border bg-card text-card-foreground shadow-sm ${className}`}>
      {children}
    </div>
  );
}
```
- **Spacing:** `p-4` (`16px`) on mobile / dense views, `p-6` (`24px`) on standard desktop cards, with `space-y-1.5` in headers.

### Badge & Filter Chips
- Keep compact status badges single-line (` inline-flex items-center gap-1 rounded-full px-2.5 py-0.5 text-xs font-medium whitespace-nowrap`).
- Never rely on color alone for status: include clear text (`Active`, `Pending`, `Failed`) or a status icon alongside semantic color.
- When rendering a list of filter chips, allow the container to wrap (`flex flex-wrap gap-2`) rather than truncating individual chip labels.

### Dialog / Modal
- **Sizes:** `max-w-sm` (`384px` confirm dialogs), `max-w-lg` (`512px` standard forms), `max-w-2xl` (`672px` complex views).
- **Accessibility & UX:**
  - Use Radix Dialog (`@/components/ui/dialog`) or native `<dialog>` so focus is trapped inside while open, `Escape` closes the modal, and focus returns to the trigger button when closed.
  - Confirm before closing a modal if the user has unsaved form edits.

### Data Table & List Views
- **Column Alignment:**
  - Left-align text, user names, and descriptions.
  - Right-align numbers, currency, counts, and timestamps (`text-right tabular-nums`).
  - Center-align compact badges; right-align row action buttons.
- **Responsive Wrapping:** Wrap tables in `<div className="w-full overflow-x-auto rounded-lg border border-border">` or render mobile card rows below `md` (`768px`).
- **Required Data States (Loading, Empty, Error):**
  - **Loading:** Render 3–5 skeleton rows matching the table/list height (`animate-pulse bg-muted rounded`) with `role="status" aria-busy="true"`.
  - **Empty:** Distinguish between *"No items created yet"* (show a primary **Create** button) and *"No results match your search/filters"* (show a **Clear filters** button).
  - **Error:** Show the error reason and a **Retry** button (`onClick={refetch}`) instead of a blank screen or silent console error.

---

## 3. Semantic CSS Variable Theming (Light & Dark Mode)

Define semantic color tokens in `globals.css` / `index.css` so React components stay theme-agnostic:

```css
:root {
  --background: #ffffff;
  --foreground: #0f172a;
  --card: #ffffff;
  --card-foreground: #0f172a;
  --muted: #f1f5f9;
  --muted-foreground: #475569;
  --primary: #2563eb;
  --primary-foreground: #ffffff;
  --destructive: #dc2626;
  --destructive-foreground: #ffffff;
  --border: #e2e8f0;
  --ring: #2563eb;
}

.dark {
  --background: #0f172a;
  --foreground: #f8fafc;
  --card: #1e293b;
  --card-foreground: #f8fafc;
  --muted: #1e293b;
  --muted-foreground: #94a3b8;
  --primary: #3b82f6;
  --primary-foreground: #ffffff;
  --destructive: #ef4444;
  --destructive-foreground: #ffffff;
  --border: #334155;
  --ring: #60a5fa;
}
```
- Verify normal text contrast `>=4.5:1` and UI control/focus ring contrast `>=3:1` in **both** light and dark themes independently.
