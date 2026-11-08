// Kích thước, khoảng cách bố cục & màu sắc giao diện Draw.io
const HEADER_H = 30, ROW_H = 30, START_X = 40, START_Y = 40, ROW_GAP = 60, COL_GAP = 120;
const HEADER_BG = 'light-dark(#00ffff, #ededed)', CANVAS_BG = 'light-dark(#ffffff, #121212)';

const TABLE_STYLE = `shape=table;startSize=${HEADER_H};container=1;collapsible=1;childLayout=tableLayout;fixedRows=1;rowLines=1;columnLines=1;fontStyle=1;align=center;resizeLast=1;html=1;rounded=0;fillColor=${HEADER_BG};`;
const ROW_STYLE = 'shape=tableRow;horizontal=0;startSize=0;swimlaneHead=0;swimlaneBody=0;fillColor=none;collapsible=0;dropTarget=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;top=0;left=0;right=0;bottom=1;rounded=0;';
const CELL_STYLE = 'shape=partialRectangle;connectable=0;fillColor=none;top=0;left=0;bottom=0;overflow=hidden;whiteSpace=wrap;html=1;rounded=0;';

// Ánh xạ 2 chiều giữa Crow's Foot và kiểu mũi tên Draw.io
const CARD_TO_ARROW = { '1..1': 'ERmandOne', '0..1': 'ERzeroToOne', '1..N': 'ERoneToMany', '0..N': 'ERzeroToMany' };
const ARROW_TO_CARD = Object.fromEntries(Object.entries(CARD_TO_ARROW).map(([k, v]) => [v, k]));

const esc = (s) => String(s || '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');

// Tính kích thước cột (auto-fit) và tọa độ (x, y) tránh chồng chéo các bảng
function calculateLayout(tables = []) {
  let curX = START_X, curY = START_Y, rowH = 0;
  const fit = (len, charW = 6.8, pad = 12, min = 40) => Math.max(min, Math.ceil((len * charW + pad) / 10) * 10);
  return tables.map((t, i) => {
    const cols = t.columns || [];
    const keyWidth = Math.max(1, ...cols.map(c => c.key ? c.key.split(',').length : 0)) * 30;
    const typeWidth = fit(Math.max(6, ...cols.map(c => String(c.type || 'BIGINT').length)), 6.8, 10, 60);
    const colWidth = fit(Math.max(2, ...cols.map(c => String(c.name || '').length + (c.nullable ? 1 : 0))), 6.8, 12, 40);
    const nameWidth = Math.max(colWidth, fit(t.name.length, 7.5, 16, 120) - keyWidth - typeWidth);
    const width = keyWidth + nameWidth + typeWidth, height = HEADER_H + cols.length * ROW_H;

    if (i > 0 && i % 3 === 0) { curX = START_X; curY += rowH + ROW_GAP; rowH = 0; }
    const x = t.x ?? curX, y = t.y ?? curY;
    curX += width + COL_GAP;
    rowH = Math.max(rowH, height);

    return { ...t, x, y, width, keyWidth, nameWidth, typeWidth, height, columns: cols };
  });
}

// Dựng XML cho bảng (ID dòng đặt trực tiếp là "TableName.ColumnName")
function renderTable(t) {
  const headerCell = `<mxCell id="${esc(t.name)}" parent="1" style="${TABLE_STYLE}" value="${esc(t.name)}" vertex="1"><mxGeometry height="${t.height}" width="${t.width}" x="${t.x}" y="${t.y}" as="geometry"><mxRectangle height="${HEADER_H}" width="${t.width}" as="alternateBounds"/></mxGeometry></mxCell>`;

  const rowCells = t.columns.map((c, idx) => {
    const rowId = `${t.name}.${c.name}`;
    const isPk = c.key === 'PK', y = HEADER_H + idx * ROW_H;
    const keyVal = c.key ? (isPk ? 'PK' : `&lt;b&gt;${esc(c.key)}&lt;/b&gt;`) : '';
    const colVal = `${esc(c.name)}${c.nullable ? '?' : ''}`;

    return `<mxCell id="${esc(rowId)}" parent="${esc(t.name)}" style="${ROW_STYLE}" value="" vertex="1"><mxGeometry height="${ROW_H}" width="${t.width}" y="${y}" as="geometry"/></mxCell>` +
      `<mxCell id="${esc(rowId)}.key" parent="${esc(rowId)}" style="${CELL_STYLE}right=1;${isPk ? 'fontStyle=1;' : 'editable=1;'}" value="${keyVal}" vertex="1"><mxGeometry height="${ROW_H}" width="${t.keyWidth}" as="geometry"><mxRectangle height="${ROW_H}" width="${t.keyWidth}" as="alternateBounds"/></mxGeometry></mxCell>` +
      `<mxCell id="${esc(rowId)}.col" parent="${esc(rowId)}" style="${CELL_STYLE}right=1;align=left;spacingLeft=6;" value="${colVal}" vertex="1"><mxGeometry height="${ROW_H}" width="${t.nameWidth}" x="${t.keyWidth}" as="geometry"><mxRectangle height="${ROW_H}" width="${t.nameWidth}" as="alternateBounds"/></mxGeometry></mxCell>` +
      `<mxCell id="${esc(rowId)}.type" parent="${esc(rowId)}" style="${CELL_STYLE}right=0;align=left;spacingLeft=4;" value="${esc(c.type || 'BIGINT')}" vertex="1"><mxGeometry height="${ROW_H}" width="${t.typeWidth}" x="${t.keyWidth + t.nameWidth}" as="geometry"><mxRectangle height="${ROW_H}" width="${t.typeWidth}" as="alternateBounds"/></mxGeometry></mxCell>`;
  }).join('');

  return headerCell + rowCells;
}

// Dựng XML cho đường nối quan hệ (nối thẳng từ r.from đến r.to)
function renderRelationship(r, idx) {
  const { from, to } = r;
  const [fromCard, toCard] = String(r.cardinality || '').toUpperCase().split(':');
  const sArrow = CARD_TO_ARROW[fromCard] || 'none', eArrow = CARD_TO_ARROW[toCard] || 'none';
  const edgeId = r.id || `rel_${idx + 1}_${from}_to_${to}`;

  return `<mxCell id="${esc(edgeId)}" edge="1" parent="1" source="${esc(from)}" target="${esc(to)}" style="edgeStyle=orthogonalEdgeStyle;fontSize=12;html=1;startArrow=${sArrow};startFill=0;endArrow=${eArrow};endFill=0;libavoidRouting=1;" value=""><mxGeometry height="100" relative="1" width="100" as="geometry"/></mxCell>`;
}

// Sinh toàn bộ tài liệu mxGraphModel XML từ Schema
function renderDiagram(schema) {
  const tablesXml = calculateLayout(schema.tables).map(renderTable).join('');
  const edgesXml = (schema.relationships || []).map(renderRelationship).join('');
  return `<mxGraphModel dx="1200" dy="800" grid="1" gridSize="10" guides="1" tooltips="1" connect="1" arrows="1" fold="1" page="1" pageScale="1" pageWidth="1200" pageHeight="800" background="${CANVAS_BG}" math="0" shadow="0"><root><mxCell id="0"/><mxCell id="1" parent="0"/>${tablesXml}${edgesXml}</root></mxGraphModel>`;
}

module.exports = { ARROW_TO_CARD, renderDiagram };
