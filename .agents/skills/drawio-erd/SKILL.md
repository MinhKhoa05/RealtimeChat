---
name: drawio-erd
description: >-
  Guides creation and maintenance of editable Draw.io SVG (.drawio.svg)
  database ERD diagrams. Use when creating a new .drawio.svg diagram,
  converting static ERD images (PNG/JPG) or SQL/database schemas into editable
  Draw.io SVG, or updating existing .drawio.svg diagrams.
---

# Draw.io SVG ERD Engineering

## Overview

Generates, updates, and verifies editable Draw.io SVG (`.drawio.svg`) ERD diagrams via [`scripts/drawio-erd.js`](./scripts/drawio-erd.js) and [`scripts/template.js`](./scripts/template.js).

## When to Use

- Create new `.drawio.svg` ERDs from SQL, codebase models, or images.
- Update tables, columns, or relationships in existing `.drawio.svg` files.
- Audit `.drawio.svg` files for editability and layout overlaps.

## Workflow

### 1. Check Environment
- Verify **Node.js**, a local **Chrome/Edge** browser executable, and the **`Draw.io Integration`** (`hediet.vscode-drawio`) webapp directory (`hediet.vscode-drawio*/drawio/src/main/webapp`) are available on the machine.
- If any prerequisite is missing, stop immediately and explain what is required. Do not install dependencies automatically.

### 2. Collect & Analyze Schema Input
- **Inspect Database Schema**: Extract tables, columns, types, `PK`, `FK`, `UNIQUE`, nullability (`?`), and indexes from the user-specified source.
- **Analyze Domain & Application Context (when needed)**: Inspect entity definitions, domain models, or validation rules when schema constraints alone are insufficient to determine relationship participation.

### 3. Prepare Schema JSON
- **New Diagram**: Create `temp_schema.json` following the Schema Guide below.
- **Update Existing Diagram**: Extract the current schema from the `.drawio.svg` file first:
  ```powershell
  node .agents/skills/drawio-erd/scripts/drawio-erd.js --extract-json path/to/diagram.drawio.svg --output temp_schema.json
  ```
  Then apply the requested edits to `temp_schema.json`.

### 4. Generate Diagram
Run the generator script with the detected browser and webapp paths:
```powershell
node .agents/skills/drawio-erd/scripts/drawio-erd.js --input temp_schema.json --output path/to/diagram.drawio.svg --browser "<browserPath>" --webapp "<webappPath>"
```

### 5. Clean Up & Report in Chat
Delete `temp_schema.json` after generation succeeds, then reply directly in chat with a concise summary (do not create extra `.md` report files and do not list unchanged elements):
- **Output**: Generated or updated `.drawio.svg` diagram file.
- **Changes**: Added, modified, or removed tables, columns, and relationships.
- **Cardinality**: Identified relationships and brief reasoning for non-obvious decisions.
- **Verification**: Generation status, editability, and visual validation results.
- **Limitations**: Unresolved ambiguities or checks that could not be completed.

---

## Schema Guide

### 1. Sample JSON Template

```json
{
  "tables": [
    {
      "name": "Users",
      "columns": [
        { "name": "Id", "type": "BIGINT", "key": "PK" },
        { "name": "Email", "type": "VARCHAR(255)", "key": "UK1" },
        { "name": "AvatarMediaId?", "type": "BIGINT", "key": "FK1" },
        { "name": "CreatedAt", "type": "DATETIME" },
        { "name": "UpdatedAt?", "type": "DATETIME" }
      ]
    },
    {
      "name": "Relationships",
      "columns": [
        { "name": "Id", "type": "BIGINT", "key": "PK" },
        { "name": "UserId", "type": "BIGINT", "key": "FK1, UK1" },
        { "name": "TargetUserId", "type": "BIGINT", "key": "FK2, UK1" },
        { "name": "Introduction?", "type": "VARCHAR(500)" }
      ]
    }
  ],
  "relationships": [
    { "from": "Relationships.UserId", "to": "Users.Id", "cardinality": "0..N:1..1" },
    { "from": "Relationships.TargetUserId", "to": "Users.Id", "cardinality": "0..N:1..1" }
  ]
}
```

### 2. Table & Column Normalization
- **Table `name`**: Exact entity/table name (e.g., `"Users"`, `"Orders"`).
- **Nullability (`?`)**:
  - `NOT NULL`: Regular column name (e.g., `"Id"`, `"UserId"`).
  - `NULLABLE`: Append `?` to column name (e.g., `"AvatarMediaId?"`, `"UpdatedAt?"`).
- **Data Types (`type`)**: Always uppercase SQL data types (e.g., `"BIGINT"`, `"INT"`, `"VARCHAR(255)"`, `"DATETIME"`, `"DECIMAL(10,2)"`).
- **Constraint Badges (`key`)** *(omit field for regular columns)*:
  - `PK`: Primary Key.
  - `FK{num}`: Foreign Key.
  - `UK{num}`: Unique Key.
  - `IX{num}`: Non-unique Index.
  - **Numbering (`{num}`)**: Number `FK`, `UK`, and `IX` sequentially and independently within each table (`UK1`, `UK2`, ...). Assign the **same number** to all columns sharing a composite constraint (e.g., composite unique on `(UserId, TargetUserId)` $\rightarrow$ both get `UK1`).
  - **Multiple badges**: Separate with a comma and space (e.g., `"FK1, UK1"`, `"PK, FK1"`).

### 3. Relationship & Cardinality
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
    - `0..1`: Child may reference zero or one parent *(`NULLABLE ?` FK)*.
  - **Composite Constraints**: A composite `PK`/`UK` (e.g., two columns sharing `PK` or `UK1`) does not make its individual columns unique — each FK in a composite key still allows multiple child rows per parent (`0..N` / `1..N`). Only treat an FK as unique (`0..1` / `1..1`) when that single FK column is independently constrained as unique, and evaluate any filtered/partial unique index predicates before deciding.

---

## Red Flags

- **Never hand-craft raw SVG or XML**: Do not write or edit `<svg>`, `<mxCell>`, or `<path>` elements by hand. Always follow the script workflow (`extract -> edit JSON -> generate`).
- **Never rebuild existing diagrams from scratch**: Always run `--extract-json` first when updating an existing `.drawio.svg` to preserve unrelated tables, column ordering, and custom positions.
- **Never manually calculate coordinates or widths for new tables**: Leave layout calculation to `template.js`; only adjust existing `x`, `y` coordinates if needed to resolve a table overlap error.
- **Never guess schema or over-infer cardinality**: Extract strictly what is defined. Do not infer mandatory participation (`1..N` / `1..1`) from entity names, collection properties (`ICollection<T>`), or conventions alone — when evidence is insufficient, default to DB-enforced minimum participation (`0..N` / `0..1`) and do not turn application rules into fake DB constraints.
- **Never mix visual styling into Schema JSON**: Visual changes (colors, fonts, spacing) belong in [`scripts/template.js`](./scripts/template.js), not in the JSON file.
- **Never auto-install or bypass missing dependencies**: Never install Node.js, browsers, or extensions automatically, and never fall back to writing SVG/XML by hand when prerequisites are missing.
- **Never leave temporary `.json` files** in the workspace after completion.

---

## Verification Checklist

- [ ] **Script verification**: Confirm `Missing SVG cells: 0`, `CLEAN (0 overlaps)`, and `STATUS: VALID & EDITABLE`. These results validate the checks implemented by the generator, not the semantic correctness of the source schema.
- [ ] **Schema verification**: Compare tables, columns, data types, nullability, keys, and relationships against the original source.
- [ ] **Visual verification**: Inspect the rendered diagram for truncated labels, unreadable connectors, and layout issues.
- [ ] **Clean workspace**: Confirm temporary JSON and other generated intermediate files have been deleted.
