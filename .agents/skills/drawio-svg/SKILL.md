---
name: drawio-svg
description: >-
  Guides creation and maintenance of editable Draw.io SVG (.drawio.svg)
  database ERD diagrams. Use when creating a new .drawio.svg diagram,
  converting static ERD images (PNG/JPG) or SQL/database schemas into editable
  Draw.io SVG, or updating existing .drawio.svg diagrams.
---

# Draw.io SVG ERD Engineering

## Overview

Design and maintain clean, editable Draw.io SVG (`.drawio.svg`) Entity-Relationship Diagrams using a JSON-first workflow, standardized visual styles ([references/visual-style.json](./references/visual-style.json)), and automated generation ([scripts/generate_drawio_svg.js](./scripts/generate_drawio_svg.js)).

## When to Use

- Creating a new `.drawio.svg` database ERD from SQL migrations, entity models, or schema docs
- Converting a static `.png` / `.jpg` ERD into an editable `.drawio.svg` diagram
- Adding, modifying, or removing tables, columns, or relationships in an existing `.drawio.svg` diagram
- Auditing `.drawio.svg` editability, layout cleanliness, and schema fidelity

## Core Principles

### 1. Authoritative Source of Truth

Resolve schema definitions using this strict priority order:
1. **Explicit user instructions** in the current task
2. **Authoritative schema definitions** (SQL migrations / entity configurations in scope and `docs/database_design.md`)
3. **Existing `.drawio.svg` diagram** (source of truth for element IDs, layout coordinates `x`/`y`, widths, and unchanged elements)
4. **Static images (`.png` / `.jpg`)** — use for initial scope and visual grouping only

> **Never silently merge conflicts.** If SQL and `docs/database_design.md` contradict each other on column types, nullability, or keys, verify which source is authoritative or ask the user before generating.

### 2. Preserve Layout & Element IDs on Update

**Preserve existing IDs for unchanged elements; generate deterministic unique IDs for new elements.**
When updating an existing diagram, extract its JSON first and retain all unchanged coordinates (`x`, `y`), widths, routing waypoints (`points`), and element IDs:
- **Table ID** (`table.id`): Keep existing ID even if display `name` is adjusted; use `<TableName>` for new tables.
- **Column Cell IDs** (`id`, `keyId`, `colId`): Keep existing row, key-cell, and column-cell IDs for unchanged columns; generate `<tableId>_row_<colName>`, `<tableId>_key_<colName>`, and `<tableId>_col_<colName>` for new columns.
- **Relationship ID** (`relationship.id`): Keep existing edge IDs; generate `rel_<fromTable>_<fromCol>_to_<toTable>_<toCol>` for new relationships.
- **Auxiliary / Label IDs**: Preserve any existing label or auxiliary cell IDs so XML `<mxCell id>` and SVG `data-cell-id` stay strictly synchronized.

### 3. JSON Contract First, Script Generation Always

Define the schema in JSON following [references/schema-guide.md](./references/schema-guide.md) and [references/sample-schema.json](./references/sample-schema.json), then generate via `scripts/generate_drawio_svg.js`. Never hand-write raw Draw.io `<mxCell>` XML or SVG `<path>` elements.

### 4. Predictable Schema & Cardinality Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| Nullable columns | Append `?` to column name | `"AvatarMediaId?"`, `"UpdatedAt?"` |
| SQL data types | Uppercase SQL type | `"BIGINT"`, `"VARCHAR(255)"`, `"DATETIME"` |
| Constraint badges | Sequential per table (`PK`, `FK#`, `UK#`, `IX#`) | `"PK"`, `"FK1, UK1"`, `"IX1"` |
| Relationship endpoints | `"from": "SourceTable.Col", "to": "TargetTable.Col"` | `"from": "Users.Id", "to": "Posts.UserId"` |
| Cardinality (`from:to`) | **Left side** = at `from` table; **Right side** = at `to` table | `"1..1:0..N"` (`1..1` at `Users`, `0..N` at `Posts`) |
| Waypoint & Connector Routing | Use auto-routing with obstacle avoidance (`libavoidRouting=1`) | `"edgeStyle=orthogonalEdgeStyle;...libavoidRouting=1;"` |
| External stub tables | PK row only for out-of-module referenced tables | `{ "name": "Id", "type": "BIGINT", "key": "PK" }` |

## Workflows

### A. Creating or Updating from Schema / Existing `.drawio.svg`

1. **Read References**: Read [references/schema-guide.md](./references/schema-guide.md) and [references/sample-schema.json](./references/sample-schema.json) before inspecting input files.
2. **Extract Existing State (When Updating)**:
   ```powershell
   node .agents/skills/drawio-svg/scripts/generate_drawio_svg.js --extract-json <existing.drawio.svg> --output <temp.json>
   ```
   Keep unchanged element IDs (`id`, `keyId`, `colId`), coordinates (`x`, `y`), widths, and waypoints (`points`).
3. **Prepare JSON Schema**: Build or update the JSON file with accurate tables, columns, `?` nullability markers, constraint badges, and `from:to` cardinalities.
4. **Generate & Verify**:
   ```powershell
   node .agents/skills/drawio-svg/scripts/generate_drawio_svg.js --input <schema.json> --output <target.drawio.svg>
   ```
5. **Refine & Clean Up**: If overlaps or visual clutter appear, adjust `column` grouping, coordinates (`x`, `y`), or waypoints (`points`) and regenerate. Delete temporary `.json` working files once finalized.

### B. Converting Static Images (PNG/JPG) to `.drawio.svg`

Static images rarely capture full database metadata. Follow these 5 rules:
1. **Inspect & Inventory**: Read the image to identify visible tables, column names, visual groupings, and connector lines.
2. **Separate Observed vs. Inferred**: Distinguish facts explicitly shown in the image from missing schema details.
3. **Never Guess Hidden Metadata**: Do not invent nullability (`?`), SQL data types (`VARCHAR(255)` vs `TEXT`), or `PK`/`FK`/`UK`/`IX` badges if they are not visible in the image.
4. **Cross-Reference Code/Docs**: Match the tables against SQL migrations, entity models, or `docs/database_design.md` in the workspace to fill in authoritative types, nullability, and keys.
5. **Flag Ambiguities**: If no authoritative code/docs exist for missing types, nullability, or cardinalities, explicitly list the unresolved items and ask the user to confirm before finalizing.

## Common Rationalizations

| Rationalization | Reality |
|---|---|
| "It's faster to regenerate the existing `.drawio.svg` from scratch" | Rebuilding from scratch destroys existing element IDs, coordinates (`x`, `y`), and custom routing waypoints. Always `--extract-json` first. |
| "Element IDs don't matter as long as table names match" | Randomly regenerated IDs break `<mxCell>`-to-SVG synchronization and lose edit state. Preserve existing IDs and use deterministic IDs for new items. |
| "I can guess the SQL types and nullability from column names in the PNG" | Guessing produces inaccurate schema diagrams. Cross-check against SQL/entity models or ask the user to confirm missing details. |
| "The script exited with code 0, so the diagram is ready" | Script checks catch cell/bounding-box issues, not reversed `from:to` cardinalities or awkward visual flow. Always review the rendered diagram. |

## Red Flags

- Hand-editing raw `<mxCell>` XML or SVG `<g>` / `<path>` tags instead of running `generate_drawio_svg.js`
- Rebuilding an existing `.drawio.svg` or regenerating element IDs (`id`, `keyId`, `colId`) for unchanged tables, columns, or relationships
- Reversing cardinality sides (putting target cardinality on the left or source cardinality on the right of `from:to`)
- Inventing SQL types, `?` nullability, or `PK`/`FK` badges from a static PNG/JPG without checking SQL/docs or confirming with the user
- Missing `?` suffix on nullable columns or using lowercase data types (`varchar`, `bigint`)
- Unnumbered `FK`, `UK`, or `IX` badges, or giving columns in the same composite index/unique constraint different numbers
- Leaving temporary `.json` working files in the repository after delivering the `.drawio.svg`

## Verification

Before delivering any `.drawio.svg` diagram:

- [ ] Generator output confirms `Missing SVG cells: 0`, `Layout Check: CLEAN (0 overlaps)`, and `STATUS: VALID & EDITABLE`
- [ ] Unchanged tables, columns, and relationships in updated diagrams retain their existing IDs, `x`/`y` coordinates, and widths; new elements use deterministic unique IDs
- [ ] Every table, column name, uppercase SQL type, and `?` nullable suffix matches the authoritative schema (no unverified guesses from PNG/JPG)
- [ ] Primary keys (`PK`), sequential foreign keys (`FK1`, `FK2`), composite unique keys (`UK1`), and indexes (`IX1`) are accurately numbered
- [ ] Every relationship has accurate `"from"` / `"to"` endpoints and `<fromCardinality>:<toCardinality>` order (left = `from` table, right = `to` table)
- [ ] Related tables are logically grouped so connectors do not cut through unrelated tables or obscure labels
- [ ] Temporary `.json` working files have been removed
