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

Orchestrates the creation and maintenance of editable Draw.io SVG (`.drawio.svg`) Entity-Relationship Diagrams through an input-driven, JSON-first workflow:
- **Specification & Schema Rules**: [references/schema-guide.md](./references/schema-guide.md) (mandatory reading before building/updating JSON)
- **JSON Template Example**: [references/sample-schema.json](./references/sample-schema.json)
- **Generator & Verifier CLI**: [scripts/generate_drawio_svg.js](./scripts/generate_drawio_svg.js)

## When to Use

- Creating a new `.drawio.svg` database ERD from user-specified schemas, codebase files, or documentation
- Converting a static `.png` / `.jpg` ERD into an editable `.drawio.svg` diagram
- Adding, modifying, or removing tables, columns, or relationships in an existing `.drawio.svg` diagram
- Auditing `.drawio.svg` editability, layout cleanliness, and schema fidelity

## Core Orchestration Principles

1. **Always Read `schema-guide.md` First**: Before extracting or writing any schema JSON, read [references/schema-guide.md](./references/schema-guide.md) and [references/sample-schema.json](./references/sample-schema.json) to follow the exact JSON contract, ID preservation rules, constraint badge numbering, and `from:to` cardinality conventions.
2. **Input-Driven & Strictly Scoped**: Identify the user-requested source (direct prompt, codebase, specific doc, existing `.drawio.svg`, or static image) and inspect **only** that source. Never assume a default documentation file or scan unrelated repository files.
3. **Preserve Existing Diagram State**: When updating an existing `.drawio.svg`, always extract its current JSON via `--extract-json` first to retain existing IDs, coordinates, and dimensions for unchanged elements.
4. **Script Generation Always**: Never hand-write raw Draw.io `<mxCell>` XML or SVG `<path>` elements; always generate and verify via `scripts/generate_drawio_svg.js`.

## Execution Workflow

Follow these 5 steps in order:

1. **Load Specification**:
   Read [references/schema-guide.md](./references/schema-guide.md) (and [references/sample-schema.json](./references/sample-schema.json) when creating a new diagram).
2. **Extract Baseline (Updates Only)**:
   If updating an existing `.drawio.svg` diagram, extract its current state to a temporary JSON file:
   ```powershell
   node .agents/skills/drawio-svg/scripts/generate_drawio_svg.js --extract-json <existing.drawio.svg> --output <temp.json>
   ```
3. **Analyze Requested Source & Build JSON**:
   Inspect only the input source specified by the user (following the source-handling and conflict-resolution rules in [references/schema-guide.md](./references/schema-guide.md#1-input-driven-source-selection--scope-control)) and normalize the schema into JSON according to [references/schema-guide.md](./references/schema-guide.md#3-json-schema-specification).
4. **Generate & Verify `.drawio.svg`**:
   Run the generator script (which automatically exports via Draw.io and runs layout/cell verification):
   ```powershell
   node .agents/skills/drawio-svg/scripts/generate_drawio_svg.js --input <temp.json> --output <target.drawio.svg>
   ```
   *(To audit an existing file without regenerating, run `node .agents/skills/drawio-svg/scripts/generate_drawio_svg.js --verify <target.drawio.svg>`.)*
5. **Refine & Clean Up**:
   If verification reports overlaps, adjust table `column` groupings or `x`/`y` coordinates in the JSON and re-run Step 4. Delete temporary `.json` working files once finished.

## Red Flags

- Building or editing schema JSON without first reading [references/schema-guide.md](./references/schema-guide.md)
- Exploring unrelated repository documentation/codebase files not requested by the user, or assuming any hardcoded documentation file is the default source of truth
- Hand-editing raw `<mxCell>` XML or SVG `<g>` / `<path>` tags instead of running `generate_drawio_svg.js`
- Rebuilding an existing `.drawio.svg` from scratch instead of running `--extract-json` to preserve unchanged element IDs and layout
- Inventing hidden metadata (types, `?` nullability, `PK`/`FK` badges) not shown in the selected source without asking the user
- Leaving temporary `.json` working files in the workspace after delivering the `.drawio.svg`

## Verification Checklist

Before delivering any `.drawio.svg` diagram:

- [ ] JSON schema strictly complies with all rules in [references/schema-guide.md](./references/schema-guide.md) (IDs, uppercase types, `?` nullability, numbered badges, `from:to` cardinalities)
- [ ] Generator output confirms `Missing SVG cells: 0`, `Layout Check: CLEAN (0 overlaps)`, and `STATUS: VALID & EDITABLE`
- [ ] Unchanged elements in updated diagrams retain their original IDs, coordinates, and widths
- [ ] Temporary `.json` working files have been removed
