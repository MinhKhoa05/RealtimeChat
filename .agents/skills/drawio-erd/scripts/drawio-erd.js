#!/usr/bin/env node
const fs = require('fs'), path = require('path'), http = require('http'), zlib = require('zlib');
const { execFile } = require('child_process');
const { ARROW_TO_CARD, renderDiagram } = require('./template');

const HARNESS_HTML = `<!DOCTYPE html><html><body>
<iframe id="f" src="/index.html?embed=1&proto=json&spin=0&libraries=0&saveAndExit=0&noSaveBtn=1&noExitBtn=1" style="width:1200px;height:900px;border:none;"></iframe>
<script>
window.addEventListener('message', function(e) {
  if (!e.data || typeof e.data !== 'string') return;
  try {
    var m = JSON.parse(e.data), w = document.getElementById('f').contentWindow;
    if (m.event === 'init') fetch('/xml').then(r => r.text()).then(xml => w.postMessage(JSON.stringify({ action: 'load', xml }), '*'));
    else if (m.event === 'load') w.postMessage(JSON.stringify({ action: 'export', format: 'xmlsvg', theme: 'light' }), '*');
    else if (m.event === 'export') fetch('/save', { method: 'POST', body: m.data }).then(() => window.close());
  } catch(err) {}
});
</script></body></html>`;

const unesc = (s) => String(s || '').replace(/&amp;/g, '&').replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace(/&quot;/g, '"').replace(/<[^>]+>/g, '').trim();
const getAttr = (s, k) => (s.match(new RegExp(`\\b${k}="([^"]*)"`)) || [])[1] ?? null;

// 1. Kiểm tra dữ liệu Schema JSON
function validateSchema(schema) {
  if (!schema || !Array.isArray(schema.tables)) throw new Error('Invalid schema: "tables" array is required.');
  const validCols = new Set();

  for (const t of schema.tables) {
    if (!t?.name || !Array.isArray(t.columns)) throw new Error(`Invalid table "${t?.name || 'unknown'}": requires name and columns.`);
    for (const c of t.columns) validCols.add(`${t.name}.${c.name}`);
  }
  for (const r of schema.relationships || []) {
    if (!validCols.has(r.from) || !validCols.has(r.to)) throw new Error(`Relationship endpoints not found: from="${r.from}", to="${r.to}"`);
  }
  return schema;
}

// 2. Xuất file .drawio.svg qua headless browser
function exportSvg(xml, outPath, { browserPath, webappPath } = {}) {
  if (!browserPath || !fs.existsSync(browserPath)) throw new Error(`Browser not found at "${browserPath || ''}".`);
  if (!webappPath || !fs.existsSync(webappPath)) throw new Error(`Draw.io webapp not found at "${webappPath || ''}".`);

  const mimes = { '.html': 'text/html', '.js': 'application/javascript', '.css': 'text/css', '.svg': 'image/svg+xml', '.png': 'image/png', '.json': 'application/json', '.xml': 'application/xml', '.gif': 'image/gif', '.ico': 'image/x-icon' };

  return new Promise((resolve, reject) => {
    let childProc = null;
    const cleanup = () => { clearTimeout(timer); try { childProc?.kill(); } catch {} try { server.close(); } catch {} };
    const timer = setTimeout(() => { cleanup(); reject(new Error('Draw.io export timed out after 20s')); }, 20000);

    const server = http.createServer((req, res) => {
      const u = req.url.split('?')[0];
      if (u === '/harness.html') return res.writeHead(200, { 'Content-Type': 'text/html' }).end(HARNESS_HTML);
      if (u === '/xml') return res.writeHead(200, { 'Content-Type': 'application/xml' }).end(xml);
      if (u === '/save' && req.method === 'POST') {
        let body = '';
        req.on('data', c => body += c);
        return req.on('end', () => {
          const raw = Buffer.from(body.replace(/^data:image\/svg\+xml;base64,/, ''), 'base64').toString('utf8');
          const svg = raw.replace(/^<svg (?!host=)/, '<svg host="65bd71144e" ').replace(/content="&lt;mxfile[^&]*&gt;/, 'content="&lt;mxfile&gt;');
          fs.mkdirSync(path.dirname(path.resolve(outPath)), { recursive: true });
          fs.writeFileSync(outPath, svg, 'utf8');
          res.writeHead(200).end('ok');
          setTimeout(() => { cleanup(); resolve(); }, 100);
        });
      }
      const fp = path.join(webappPath, u === '/' ? 'index.html' : u);
      if (!fs.existsSync(fp)) return res.writeHead(404).end();
      res.writeHead(200, { 'Content-Type': mimes[path.extname(fp)] || 'application/octet-stream' });
      fs.createReadStream(fp).pipe(res);
    });

    server.listen(0, '127.0.0.1', () => {
      childProc = execFile(browserPath, ['--headless=new', '--disable-gpu', '--no-first-run', '--no-default-browser-check', `http://127.0.0.1:${server.address().port}/harness.html`], err => {
        if (err && !fs.existsSync(outPath)) { cleanup(); reject(err); }
      });
    });
  });
}

// 3. Giải mã XML và trích xuất cấu trúc Schema JSON từ file .drawio.svg
function extractSchema(svgPath, withMeta = false) {
  const svg = fs.readFileSync(svgPath, 'utf8');
  const contentMatch = svg.match(/content="([^"]+)"/);
  if (!contentMatch) throw new Error(`No Draw.io content in ${svgPath}`);
  const mxfile = contentMatch[1].replace(/&quot;/g, '"').replace(/&amp;/g, '&').replace(/&lt;/g, '<').replace(/&gt;/g, '>');
  const payload = mxfile.match(/<diagram[^>]*>([\s\S]*?)<\/diagram>/)[1].trim();
  const xml = payload.startsWith('<mxGraphModel') ? payload : decodeURIComponent(zlib.inflateRawSync(Buffer.from(payload, 'base64')).toString('utf8'));

  const tablesById = new Map(), rowsById = new Map(), cellsByRow = new Map(), edges = [], mxIds = [];
  const re = /<mxCell\b([^>]*?)(?:\/>|>([\s\S]*?)<\/mxCell>)/g;
  let m;

  while ((m = re.exec(xml)) !== null) {
    const a = m[1], inner = m[2] || '', id = getAttr(a, 'id'), parent = getAttr(a, 'parent'), style = getAttr(a, 'style') || '';
    if (id) mxIds.push(id);
    const ga = (inner.match(/<mxGeometry\b([^>]*)/) || [])[1] || '';
    const gx = +getAttr(ga, 'x') || 0, gy = +getAttr(ga, 'y') || 0, gw = +getAttr(ga, 'width') || 0, gh = +getAttr(ga, 'height') || 0;

    if (style.includes('shape=table;')) tablesById.set(id, { name: unesc(getAttr(a, 'value')), x: gx, y: gy, w: gw, h: gh, rows: [] });
    else if (style.includes('shape=tableRow;')) rowsById.set(id, { id, parent, y: gy });
    else if (style.includes('shape=partialRectangle;')) {
      if (!cellsByRow.has(parent)) cellsByRow.set(parent, []);
      cellsByRow.get(parent).push({ x: gx, val: getAttr(a, 'value') });
    } else if (getAttr(a, 'edge') === '1') {
      edges.push({ src: getAttr(a, 'source'), dst: getAttr(a, 'target'), style });
    }
  }

  rowsById.forEach(r => tablesById.get(r.parent)?.rows.push(r));

  const rawTables = [...tablesById.values()];
  const tables = rawTables.map(t => ({
    name: t.name, x: t.x, y: t.y,
    columns: t.rows.sort((a, b) => a.y - b.y).map(r => {
      const [key, rawName, type] = (cellsByRow.get(r.id) || []).sort((a, b) => a.x - b.x).map(c => unesc(c.val));
      const nullable = rawName.endsWith('?');
      const name = nullable ? rawName.slice(0, -1) : rawName;
      return { name, type, ...(key ? { key } : {}), ...(nullable ? { nullable: true } : {}) };
    })
  }));

  const relationships = edges.map(e => {
    const sA = (e.style.match(/startArrow=([^;]+)/) || [])[1], eA = (e.style.match(/endArrow=([^;]+)/) || [])[1];
    const cardinality = (ARROW_TO_CARD[sA] && ARROW_TO_CARD[eA]) ? `${ARROW_TO_CARD[sA]}:${ARROW_TO_CARD[eA]}` : undefined;
    return { from: e.src.replace('_row_', '.'), to: e.dst.replace('_row_', '.'), ...(cardinality ? { cardinality } : {}) };
  });

  if (!withMeta) return { tables, relationships };
  const svgIds = new Set([...svg.matchAll(/data-cell-id="([^"]+)"/g)].map(x => x[1]));
  return { tables, relationships, boxes: rawTables, missing: mxIds.filter(id => !svgIds.has(id)) };
}

// 4. Kiểm tra tính toàn vẹn cell SVG và chồng đè bảng
function verifyDiagram(svgPath) {
  const { tables, relationships, boxes, missing } = extractSchema(svgPath, true);
  validateSchema({ tables, relationships });

  const overlaps = [];
  for (let i = 0; i < boxes.length; i++) {
    for (let j = i + 1; j < boxes.length; j++) {
      const a = boxes[i], b = boxes[j];
      if (a.x < b.x + b.w && a.x + a.w > b.x && a.y < b.y + b.h && a.y + a.h > b.y) overlaps.push(`${a.name} overlaps ${b.name}`);
    }
  }

  console.log(`=== Verification: ${svgPath} ===\n- Tables (${tables.length}): ${tables.map(t => t.name).join(', ')}\n- Relationships: ${relationships.length}\n- Missing SVG cells: ${missing.length}\n- Layout Check: ${overlaps.length ? overlaps.join('; ') : 'CLEAN (0 overlaps)'}\nSTATUS: ${missing.length || overlaps.length ? 'INVALID' : 'VALID & EDITABLE'}`);
  if (missing.length || overlaps.length) throw new Error('Verification failed');
}

// Điều phối CLI
(async () => {
  try {
    const args = process.argv.slice(2);
    const arg = (k) => { const i = args.indexOf(k); return i !== -1 ? args[i + 1] : null; };

    if (arg('--verify')) return verifyDiagram(arg('--verify'));

    if (arg('--extract-json')) {
      const data = validateSchema(extractSchema(arg('--extract-json')));
      const json = JSON.stringify(data, null, 2);
      return arg('--output') ? fs.writeFileSync(arg('--output'), json, 'utf8') : console.log(json);
    }

    if (arg('--input') && arg('--output')) {
      const schema = validateSchema(JSON.parse(fs.readFileSync(arg('--input'), 'utf8')));
      const xml = renderDiagram(schema);
      await exportSvg(xml, arg('--output'), { browserPath: arg('--browser'), webappPath: arg('--webapp') });
      return verifyDiagram(arg('--output'));
    }
  } catch (err) {
    console.error('Error:', err.message);
    process.exit(1);
  }
})();

