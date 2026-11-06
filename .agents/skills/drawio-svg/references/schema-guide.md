# Schema Extraction & JSON Specification

This reference defines the input-driven rules for extracting database schema information, controlling task scope, preserving diagram state, and structuring JSON for [generate_drawio_svg.js](../scripts/generate_drawio_svg.js).

---

## 1. Input-Driven Source Selection & Scope Control

Follow an **input-driven workflow**: identify the requested source → analyze only that source → normalize into JSON → generate or update the diagram → verify the result.

### 1.1 Supported Input Sources
Use the source explicitly specified by the user:
- **Direct User Specification**: Schema definitions, SQL snippets, or modification instructions provided directly in the prompt.
- **Codebase**: SQL migrations, entity models, or ORM configurations when the user asks to analyze the codebase.
- **Documentation**: Specific documentation files when explicitly referenced by the user.
- **Existing `.drawio.svg` Diagram**: Used as the baseline diagram when updating, refining, or extracting from an existing `.drawio.svg` file.
- **Static Images (`.png` / `.jpg`)**: Used to reconstruct the provided diagram when the user supplies an image to convert.

### 1.2 Strict Scope Control
- **Inspect only requested inputs**: Only read files and resources directly specified for the current task.
- **No default documentation assumptions**: Never assume any repository documentation file is a default source of truth.
- **No automatic repository discovery**: Do not scan unrelated directories, documentation files, or codebase modules that were not requested.
- **No implicit source switching**: Do not switch to another source simply because it exists in the workspace. Only consult an additional source when explicitly instructed by the user or when required to resolve an ambiguity/conflict within the requested scope.

### 1.3 Separating Schema Sources from Diagram References
- **Schema Information**: Extract tables, columns, types, nullability, constraints, and relationships from the user's selected input source.
- **Existing `.drawio.svg` Diagrams**: When updating an existing diagram, use it to preserve element IDs (`id`, `keyId`, `colId`, `typeId`), layout coordinates (`x`, `y`), dimensions (`width`, `keyWidth`, `nameWidth`, `typeWidth`), and unchanged elements.
- **Static Images (`.png` / `.jpg`)**: Reconstruct the diagram shown in the image (tables, columns, types, keys, visual grouping, and connectors). Do not invent hidden metadata not shown in the image, and do not independently search the repository for another schema unless the user explicitly asks to combine the image with codebase/documentation sources. If required details are illegible or missing from the image, ask the user for clarification.
- **Multi-Source & Conflict Resolution**: When the user explicitly provides multiple sources, assign each source its intended role (e.g., schema definition vs. layout preservation). Never silently merge contradictory definitions (such as mismatched column types, nullability, or keys)—report the conflict and ask the user to confirm before generating.

---

## 2. Preserving Existing Diagrams & Element IDs

When updating an existing `.drawio.svg` diagram, **never rebuild the diagram from scratch**:
1. Extract the current diagram state into JSON first:
   ```powershell
   node .agents/skills/drawio-svg/scripts/generate_drawio_svg.js --extract-json <existing.drawio.svg> --output <temp.json>
   ```
2. Keep all unchanged `x`/`y` coordinates, widths (`width`, `keyWidth`, `nameWidth`, `typeWidth`), and **element IDs** intact:
   | Element Type | JSON Fields | Preservation & Deterministic Generation Rule |
   | :--- | :--- | :--- |
   | **Table** | `table.id` | Keep existing `id` (even when renaming `table.name`); for new tables, default to `<TableName>`. |
   | **Column Row & Cells** | `col.id`, `col.keyId`, `col.colId`, `col.typeId` | Keep existing cell IDs for unchanged columns; for new columns, generate `<tableId>_row_<colName>`, `<tableId>_key_<colName>`, `<tableId>_col_<colName>`, and `<tableId>_type_<colName>`. |
   | **Relationship** | `rel.id` | Keep existing edge `id`; for new relationships, generate `rel_<fromTable>_<fromCol>_to_<toTable>_<toCol>`. |
3. Modify only the tables, columns, or relationships affected by the update request.

---

## 3. JSON Schema Specification

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
        { "name": "CreatedAt", "type": "DATETIME(6)" }
      ]
    }
  ],
  "relationships": [
    { "id": "rel_Users_Id_to_Relationships_UserId", "from": "Users.Id", "to": "Relationships.UserId", "cardinality": "1..1:0..N" },
    { "id": "rel_Users_Id_to_Relationships_TargetUserId", "from": "Users.Id", "to": "Relationships.TargetUserId", "cardinality": "1..1:0..N" }
  ]
}
```

### 3.1 Tables & 3-Cell Row Architecture (`tables`)
- **3-Cell Row Layout**: Each column row in Draw.io consists of 3 aligned cells (`Key` | `Column Name` | `Data Type`) with a vertical divider after `Key` (`right=1`) and clean column alignment for `Name` and `Type` (`columnLines=0` on table, `right=0` on name/type cells).
- **Full vs. Stub Tables**: Include all columns for tables belonging to the target scope. For external tables referenced only by foreign keys, include only their primary key row (`{"name": "Id", "type": "BIGINT", "key": "PK"}`).
- **Layout Grouping (`column`)**: For new diagrams, assign `"column": 0` (left), `"column": 1` (center), or `"column": 2` (right) so related tables sit in adjacent columns. For existing diagrams, preserve extracted `"x"` and `"y"` coordinates.

### 3.2 Columns & Constraint Badges (`columns`)
- **Nullability (`?`)**: Append `?` to nullable (`NULL`) column names (e.g., `"AvatarMediaId?"`, `"UpdatedAt?"`). Omit `?` for `NOT NULL` columns.
- **Data Types (`type`)**: Always use uppercase SQL types (`"BIGINT"`, `"INT"`, `"TINYINT(1)"`, `"VARCHAR(255)"`, `"CHAR(36)"`, `"TEXT"`, `"LONGTEXT"`, `"DECIMAL(10,7)"`, `"FLOAT"`, `"DATETIME(6)"`).
- **Constraint Badges (`key`)** — numbered within each table (omit `"key"` for regular columns):
  | Badge | Meaning & Numbering Rule |
  | :--- | :--- |
  | `PK` | Primary Key (`PK` on each column of a composite PK; bottom border line is drawn under the last PK row). |
  | `FK1`, `FK2`, ... | Foreign Keys numbered sequentially top-to-bottom (`FK1` = 1st FK column, `FK2` = 2nd FK column). |
  | `UK1`, `UK2`, ... | Unique constraints. Columns forming a **composite unique constraint** share the **same** number (e.g., `UK1` on `UserId`, `TargetUserId`, and `Type`). |
  | `IX1`, `IX2`, ... | Non-unique indexes. Columns forming a **composite index** share the **same** number. |
  | `PK, FK1` / `FK1, UK1` | Combine multiple badges on the same column with `, `. |

### 3.3 Relationships & Cardinality (`relationships`)
- Reference both endpoints as `"Table.Column"` (`"from"` and `"to"`).
- Format `"cardinality"` as `"<fromEnd>:<toEnd>"`:
  - **Left side (`<fromEnd>`)**: Cardinality at the **source (`from`)** table.
  - **Right side (`<toEnd>`)**: Cardinality at the **target (`to`)** table.
- Cardinality tokens:
  - **`1..1`**: Mandatory One (`ERmandOne`, `||`)
  - **`0..1`**: Optional One (`ERzeroToOne`, `o|`)
  - **`1..N`**: Mandatory Many (`ERoneToMany`, `|<`)
  - **`0..N`**: Optional Many (`ERzeroToMany`, `o<`)
- Common pairs (when `"from"` is Parent `PK` and `"to"` is Child `FK`):
  - `"1..1:0..N"` — Standard `NOT NULL` FK (`1..1` at Parent `from`, `0..N` at Child `to`).
  - `"0..1:0..N"` — Nullable `FK?` (`0..1` at Parent `from`, `0..N` at Child `to`).
  - `"1..1:0..1"` — Mandatory 1 at `from` to Optional 1 at `to`.
  - `"0..1:0..1"` — Optional 1 to Optional 1 (e.g., `Users.AvatarMediaId?` to `Medias.Id`).
- **Auto-Routing**: Edges use `edgeStyle=orthogonalEdgeStyle;...libavoidRouting=1;` so Draw.io automatically routes orthogonal connectors around tables without manual waypoint calculation.
