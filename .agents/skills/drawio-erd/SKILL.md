---
name: drawio-erd
description: Guides creation and maintenance of editable Draw.io SVG (.drawio.svg) database ERD diagrams. Use when creating a new .drawio.svg diagram, converting static ERD images (PNG/JPG) or SQL/database schemas into editable Draw.io SVG, or updating existing .drawio.svg diagrams.
metadata:
  source: self-developed (MinhKhoa05)
  repository: https://github.com/MinhKhoa05/software-diagram-skills
---

# Draw.io SVG ERD Engineering

## Overview

Create and maintain accurate, well-structured, and fully editable Draw.io SVG ERD diagrams based on database schemas and structural definitions. The goal is diagrams that clearly represent database structures and relationships while remaining consistent, readable, and easy to maintain.

## When to Use

- Create new `.drawio.svg` ERDs from SQL, codebase models, or images:
  - **Type 1 (Architecture / Logical Overview)**: All tables (PK & FK only) with all relationships.
  - **Type 2 (Data Dictionary / Table Overview)**: All tables (full columns) with no relationships.
  - **Type 3 (Module / Physical ERD)**: Full columns for target module tables, PK-only stubs for referenced external tables, and all relationships among them.
- Update tables, columns, or relationships in existing `.drawio.svg` files.
- Audit `.drawio.svg` files for editability and layout overlaps.

## Task Scope & Source Selection

- **Start with the minimum**: Identify and inspect only the sources directly required for the requested task.
- **Execute directly**: Use the relevant existing files for edits or transfers; use the specified source materials or related code for creating diagrams. Do not inspect additional sources upfront.
- **Expand only when necessary**: If the task cannot be completed reliably or potential issues arise, inspect only the additional information directly needed to resolve them.
- **Ask before expanding scope** when further investigation goes beyond the user's request.
- **Preserve unrelated content**: Modify only what the user requested.

**Core rule: Inspect the minimum required, execute the task directly, and gather additional information only when necessary. Do not investigate broadly upfront.**

## Workflow

### 1. Check Environment
- Verify **Node.js**, a local **Chrome/Edge** browser executable, and the **`Draw.io Integration`** (`hediet.vscode-drawio`) webapp directory (`hediet.vscode-drawio*/drawio/src/main/webapp`) are available on the machine.
- If any prerequisite is missing, stop immediately and explain what is required. Do not install dependencies automatically.

### 2. Collect & Analyze Schema Input *(Skip for visual/template updates)*
- **Classify Diagram Type**: Identify the requested diagram type (Type 1, Type 2, or Type 3; default to Type 1 for full-system requests or Type 3 for specific modules/tables).
- **Inspect Specified Schema Source**: Extract tables, columns, types, `PK`, `FK`, `UNIQUE`, nullability (`nullable`), and indexes from the user-specified source.
- **Analyze Domain & Application Context (when authorized & needed)**: Inspect entity definitions, domain models, or validation rules only when schema constraints alone are insufficient to determine relationship participation.

### 3. Prepare Schema JSON
- **New Diagram**: Create `temp_schema.json` following the Schema Guide below.
- **Update Existing Diagram**: Extract the current schema from the `.drawio.svg` file first:
  ```powershell
  node scripts/drawio-erd.js --extract-json path/to/diagram.drawio.svg --output temp_schema.json
  ```
  Apply only the changes required by the user's request to `temp_schema.json` (or leave `temp_schema.json` untouched if only applying visual/template changes from `template.js`).

### 4. Generate Diagram & Clean Up
Run the generator script with the detected browser and webapp paths:
```powershell
node scripts/drawio-erd.js --input temp_schema.json --output path/to/diagram.drawio.svg --browser "<browserPath>" --webapp "<webappPath>"
```
Delete `temp_schema.json` immediately after generation succeeds.

---

## Schema Guide

### 1. Sample JSON Template

```json
{
  "tables": [
    {
      "name": "Users",
      "metadata": {
        "x": 70,
        "y": 150,
        "color": "light-dark(#00ffff, #ededed)"
      },
      "columns": [
        { "name": "Id", "type": "BIGINT", "key": "PK" },
        { "name": "Email", "type": "VARCHAR(255)", "key": "UK1" },
        { "name": "AvatarMediaId", "type": "BIGINT", "key": "FK1", "nullable": true },
        { "name": "CreatedAt", "type": "DATETIME" },
        { "name": "UpdatedAt", "type": "DATETIME", "nullable": true }
      ]
    },
    {
      "name": "Relationships",
      "metadata": {
        "x": 300,
        "y": 150
      },
      "columns": [
        { "name": "Id", "type": "BIGINT", "key": "PK" },
        { "name": "UserId", "type": "BIGINT", "key": "FK1, UK1" },
        { "name": "TargetUserId", "type": "BIGINT", "key": "FK2, UK1" },
        { "name": "Introduction", "type": "VARCHAR(500)", "nullable": true }
      ]
    }
  ],
  "relationships": [
    { "from": "Relationships.UserId", "to": "Users.Id", "cardinality": "0..N:1..1" },
    { "from": "Relationships.TargetUserId", "to": "Users.Id", "cardinality": "0..N:1..1" }
  ]
}
```

> **Note on `metadata`**: Avoid manual tweaking of `metadata` (`x`, `y`, `color`) unless necessary; the script automatically handles default layout and styling. Only customize colors to differentiate modules/tables or adjust `x`, `y` coordinates if needed to resolve layout overlap collisions.

### 2. Diagram Types & Schema Construction
- **Type 1: Architecture / Logical Overview**:
  - **`tables`**: Include all tables, but keep **only PK and FK** columns (omit all other business attributes).
  - **`relationships`**: Include 100% foreign key relationships between all tables.
- **Type 2: Data Dictionary / Table Overview**:
  - **`tables`**: Include all tables with **all columns** (full types, nullability, badges).
  - **`relationships`**: Set to empty array (`[]`). Do not generate any relationships.
- **Type 3: Module / Physical ERD**:
  - **`tables`**: Identify all tables belonging to the target module and include their full column definitions. Also include external tables directly referenced by FKs from these core tables as reduced stubs containing **only their PK**.
  - **`relationships`**: Include 100% of FK relationships among the included tables.

### 3. Table & Column Normalization
- **Table & Column `name`**: Exact entity/table and column names (e.g., `"Users"`, `"AvatarMediaId"`).
- **Nullability (`nullable`)**:
  - `NOT NULL`: Omit `nullable` (or set `false`).
  - `NULLABLE`: Set `"nullable": true`.
- **Data Types (`type`)**: Always uppercase SQL data types (e.g., `"BIGINT"`, `"INT"`, `"VARCHAR(255)"`, `"DATETIME"`, `"DECIMAL(10,2)"`).
- **Constraint Badges (`key`)** *(omit field for regular columns)*:
  - `PK`: Primary Key.
  - `FK{num}`: Foreign Key.
  - `UK{num}`: Unique Key.
  - `IX{num}`: Non-unique Index.
  - **Numbering (`{num}`)**: Number `FK`, `UK`, and `IX` sequentially and independently within each table (`UK1`, `UK2`, ...). Assign the **same number** to all columns sharing a composite constraint (e.g., composite unique on `(UserId, TargetUserId)` $\rightarrow$ both get `UK1`).
  - **Multiple badges**: Separate with a comma and space (e.g., `"FK1, UK1"`, `"PK, FK1"`).

### 4. Relationship & Cardinality
- **Endpoints (`from` & `to`)**: Format as `"TableName.ColumnName"`, where **`from` is the child FK** and **`to` is the parent PK** (`FK -> PK`).
- **Cardinality Tokens**: `1..1` (Mandatory One `||`), `0..1` (Optional One `o|`), `0..N` (Optional Many `o<`), `1..N` (Mandatory Many `|<`).
- **How to Determine `<fromEnd>:<toEnd>`**: Use **database constraints** as the primary source, supported by **domain/code logic** (including entity lifecycle states) and **user descriptions** to determine participation independently at each end:
  - **`<fromEnd>` (Child FK side — how many children a parent can have)**:
    - `0..N`: Parent can have zero or many children *(default for non-unique FK when no mandatory rule is established)*.
    - `1..N`: Parent must have at least one child *(non-unique FK + explicit domain/code/user rule requiring $\ge 1$ child)*.
    - `0..1`: Parent can have zero or at most one child *(single-column `UK`/`PK` on the child FK)*.
    - `1..1`: Parent must have exactly one child *(single-column `UK`/`PK` on the child FK + explicit mandatory rule)*.
  - **`<toEnd>` (Parent PK side — how many parents a child can belong to)**:
    - `1..1`: Child must reference exactly one parent *(`NOT NULL` FK)*.
    - `0..1`: Child may reference zero or one parent *(`nullable: true` FK)*.
  - **Composite Constraints**: A composite `PK`/`UK` (e.g., two columns sharing `PK` or `UK1`) does not make its individual columns unique — each FK in a composite key still allows multiple child rows per parent (`0..N` / `1..N`). Only treat an FK as unique (`0..1` / `1..1`) when that single FK column is independently constrained as unique, and evaluate any filtered/partial unique index predicates before deciding.

---

## Red Flags

- **Never expand task scope or scan source code by default**: Do not read application source code (`DbContext`, entities, migrations) unless explicitly requested, and do not modify schema, relationships, or layout beyond the user's specific instruction.
- **Never exceed 1-hop boundary in module diagrams**: When creating Type 3 diagrams, never traverse secondary FKs of external referenced tables (e.g., do not pull in `Medias` just because an external `Users` table points to `Medias`).
- **Never hand-craft raw SVG or XML**: Do not write or edit `<svg>`, `<mxCell>`, or `<path>` elements by hand. Always follow the script workflow (`extract -> edit JSON -> generate`).
- **Never rebuild existing diagrams from scratch**: Always run `--extract-json` first when updating an existing `.drawio.svg` to preserve unrelated tables, column ordering, and custom positions.
- **Never manually calculate coordinates or widths for new tables**: Leave layout calculation to `template.js`; only adjust existing `x`, `y` coordinates if needed to resolve a table overlap error.
- **Never guess schema or over-infer cardinality**: Extract strictly what is defined. Do not infer mandatory participation (`1..N` / `1..1`) from entity names, collection properties (`ICollection<T>`), or conventions alone — when evidence is insufficient, default to DB-enforced minimum participation (`0..N` / `0..1`) and do not turn application rules into fake DB constraints.
- **Never mix visual styling into Schema JSON**: Visual changes (colors, fonts, spacing) belong in [`scripts/template.js`](./scripts/template.js), not in the JSON file.
- **Never auto-install or bypass missing dependencies**: Never install Node.js, browsers, or extensions automatically, and never fall back to writing SVG/XML by hand when prerequisites are missing.
- **Never leave temporary `.json` files** in the workspace after completion.

---

## Verification & Handoff

- **Clean workspace**: Confirm temporary JSON and intermediate files created during execution have been deleted. Never delete user-owned files.
- **Minimal confirmation**: After successful completion, provide a brief confirmation. Do not generate a separate report in chat.
- **Raise issues only**: Explain only errors, missing prerequisites, table overlap collisions, or unresolved ambiguities that require user attention.
