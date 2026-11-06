# Schema Extraction & JSON Specification

This reference defines the standardized rules for extracting database schema information, resolving source conflicts, preserving element IDs, and structuring JSON for `scripts/generate_drawio_svg.js`.

---

## 1. Source of Truth & Conflict Resolution

When multiple inputs exist, follow this strict priority order:
1. **Explicit User Instructions** in the current task.
2. **Authoritative Schema Definitions**: SQL migrations / entity configurations (when in scope) and `docs/database_design.md`.
3. **Existing `.drawio.svg` Diagram**: Source of truth for existing element IDs (`id`, `keyId`, `colId`), layout coordinates (`x`, `y`), table widths, and unchanged elements during incremental updates.
4. **Static Images (`.png` / `.jpg`)**: Reference for initial visual grouping and scope only—never copy outdated column names or types from a PNG when authoritative docs disagree.

**Conflict Rule:**
- Do **not** silently merge contradictory definitions (e.g., if SQL and `docs/database_design.md` disagree on a column type, nullability, or foreign key).
- Verify which source is authoritative for the requested scope, or ask the user to clarify before generating.
- Never invent undocumented columns, constraints, or relationships.

### 1.1 Static Image (PNG/JPG) Conversion Rules

When converting a static `.png` or `.jpg` ERD into `.drawio.svg`:
1. **Read & Inventory**: Identify all visible table names, column names, visual groupings, and relationship lines from the image.
2. **Separate Observed vs. Inferred**: Clearly distinguish information explicitly drawn in the image from assumed metadata.
3. **No Blind Inference**: Never guess column nullability (`?`), SQL data types, or `PK`/`FK`/`UK`/`IX` constraints if the image does not display them.
4. **Cross-Reference Codebase**: Look up matching tables in SQL migrations, entity models, or `docs/database_design.md` to supply authoritative types, keys, and nullability.
5. **Mark Unknowns for Confirmation**: If any SQL type, nullability, constraint, or cardinality remains undetermined after checking the codebase, flag the ambiguous items and ask the user for confirmation before finalizing.

---

## 2. Preserving Existing Diagrams & Element IDs

**Core Rule: Preserve existing IDs for unchanged elements; generate deterministic unique IDs for new elements.**

When updating an existing `.drawio.svg` diagram, **do not rebuild the entire diagram from scratch**:
1. Extract the current diagram state into JSON first:
   ```powershell
   node .agents/skills/drawio-svg/scripts/generate_drawio_svg.js --extract-json <existing.drawio.svg> --output <temp.json>
   ```
2. Keep all unchanged `x`/`y` coordinates, `width`, `keyWidth`, relationship `points`, and **element IDs** intact:
   | Element Type | JSON Fields | Preservation & Deterministic Generation Rule |
   | :--- | :--- | :--- |
   | **Table** | `table.id` | Keep existing `id` (even when renaming `table.name`); for new tables, default to `<TableName>`. |
   | **Column Row & Cells** | `col.id`, `col.keyId`, `col.colId` | Keep existing cell IDs for unchanged columns; for new columns, generate `<tableId>_row_<colName>`, `<tableId>_key_<colName>`, and `<tableId>_col_<colName>`. |
   | **Relationship** | `rel.id` | Keep existing edge `id`; for new relationships, generate `rel_<fromTable>_<fromCol>_to_<toTable>_<toCol>`. |
   | **Labels / Auxiliary** | `id` | Preserve existing IDs on any auxiliary cells so XML `<mxCell id>` and SVG `data-cell-id` stay in sync. |
3. Modify only the tables, columns, or relationships affected by the update request.

---

## 3. JSON Schema Rules

See [sample-schema.json](./sample-schema.json) for a complete working example.

```json
{
  "tables": [
    {
      "id": "Users",
      "name": "Users",
      "column": 0,
      "columns": [
        { "name": "Id", "type": "BIGINT", "key": "PK" },
        { "name": "Email", "type": "VARCHAR(255)", "key": "UK1" },
        { "name": "AvatarMediaId?", "type": "BIGINT", "key": "FK1" }
      ]
    },
    {
      "id": "Relationships",
      "name": "Relationships",
      "column": 1,
      "columns": [
        { "name": "Id", "type": "BIGINT", "key": "PK" },
        { "name": "UserId", "type": "BIGINT", "key": "FK1, UK1" },
        { "name": "TargetUserId", "type": "BIGINT", "key": "FK2, UK1" },
        { "name": "Type", "type": "INT", "key": "UK1" },
        { "name": "CreatedAt", "type": "DATETIME" }
      ]
    }
  ],
  "relationships": [
    { "id": "rel_Users_Id_to_Relationships_UserId", "from": "Users.Id", "to": "Relationships.UserId", "cardinality": "1..1:0..N" },
    { "id": "rel_Users_Id_to_Relationships_TargetUserId", "from": "Users.Id", "to": "Relationships.TargetUserId", "cardinality": "1..1:0..N" }
  ]
}
```

### 3.1 Tables & Layout Grouping (`tables`)
- **Full Tables**: Include all columns for tables belonging to the target business module.
- **Referenced Stub Tables**: For external tables referenced only by foreign keys (e.g., `Users`, `Medias`, `Conversations` outside the target module), include only their primary key row (`{"name": "Id", "type": "BIGINT", "key": "PK"}`).
- **Layout Grouping (`column`)**: For new diagrams, assign `"column": 0` (left), `"column": 1` (center), or `"column": 2` (right) so related tables sit in adjacent columns and connectors stay short. (For existing diagrams, keep the extracted `"x"` and `"y"` coordinates.)

### 3.2 Columns & Constraint Badges (`columns`)
- **Nullability (`?`)**: Append `?` to nullable (`NULL`) column names (e.g., `"AvatarMediaId?"`, `"UpdatedAt?"`). Omit `?` for `NOT NULL` columns.
- **Data Types (`type`)**: Always use uppercase SQL types (`"BIGINT"`, `"INT"`, `"TINYINT(1)"`, `"VARCHAR(255)"`, `"CHAR(36)"`, `"TEXT"`, `"LONGTEXT"`, `"DECIMAL(10,7)"`, `"DATETIME"`).
- **Constraint Badges (`key`)** — numbered within each table (omit `"key"` for regular columns):
  | Badge | Meaning & Numbering Rule |
  | :--- | :--- |
  | `PK` | Primary Key (`PK` on each column of a composite PK). |
  | `FK1`, `FK2`, ... | Foreign Keys numbered sequentially top-to-bottom (`FK1` = 1st FK column, `FK2` = 2nd FK column). |
  | `UK1`, `UK2`, ... | Unique constraints. Columns forming a **composite unique constraint** share the **same** number (e.g., `UK1` on `UserId`, `TargetUserId`, and `Type`). |
  | `IX1`, `IX2`, ... | Non-unique indexes. Columns forming a **composite index** share the **same** number (e.g., `IX1` on each column in Composite Index 1). |
  | `PK, FK1` / `FK1, UK1` | Combine multiple badges on the same column with `, `. |

### 3.3 Relationships & Cardinality (`relationships`)
- Reference both endpoints as `"Table.Column"` (`"from"` and `"to"`).
- Format `"cardinality"` as `"<fromEnd>:<toEnd>"`:
  - **Left side (`<fromEnd>`)**: Cardinality at the **source (`from`)** table.
  - **Right side (`<toEnd>`)**: Cardinality at the **target (`to`)** table.
  - Example: `{ "from": "Users.Id", "to": "Posts.UserId", "cardinality": "1..1:0..N" }`
    - Left (`1..1` at `Users`): Each `Posts` record references mandatory one (`1..1`) `Users` record.
    - Right (`0..N` at `Posts`): One `Users` record can have zero-to-many (`0..N`) `Posts` records.
- Cardinality tokens (`<min>..<max>` where `0` = optional, `1` = mandatory; `1` = one, `N` = many):
  - **`1..1`**: Mandatory One (`||`)
  - **`0..1`**: Optional One (`o|`)
  - **`1..N`**: Mandatory Many (`|<`)
  - **`0..N`**: Optional Many (`o<`)
- Common pairs (when `"from"` is Parent `PK` and `"to"` is Child `FK`):
  - `"1..1:0..N"` — Standard `NOT NULL` FK (`1..1` at Parent `from`, `0..N` at Child `to`).
  - `"0..1:0..N"` — Nullable `FK?` (`0..1` at Parent `from`, `0..N` at Child `to`).
  - `"1..1:0..1"` — Mandatory 1 at `from` to Optional 1 at `to`.
  - `"0..1:0..1"` — Optional 1 to Optional 1 (e.g., `Users.AvatarMediaId?` $\leftrightarrow$ `Medias.Id`).
  - Omit `"cardinality"` when it cannot be reliably determined.
- Edge style and auto-waypoints: Edge styling standardizes on `edgeStyle=orthogonalEdgeStyle;...libavoidRouting=1;` for automatic orthogonal avoidance routing (Waypoints auto-layout in Draw.io).
