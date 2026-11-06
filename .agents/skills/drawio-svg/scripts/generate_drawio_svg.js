#!/usr/bin/env node
const fs = require('fs');
const path = require('path');
const http = require('http');
const zlib = require('zlib');
const { execFile } = require('child_process');

const stylePath = path.join(__dirname, '..', 'references', 'visual-style.json');
const styleSpec = fs.existsSync(stylePath) ? JSON.parse(fs.readFileSync(stylePath, 'utf8')) : {};
const HEADER_H = (styleSpec.table && styleSpec.table.headerHeight) || 30;
const ROW_H = (styleSpec.table && styleSpec.table.rowHeight) || 30;
const HEADER_BG = (styleSpec.table && styleSpec.table.headerBackground) || 'light-dark(#00ffff, #ededed)';
const CANVAS_BG = (styleSpec.canvas && styleSpec.canvas.background) || 'light-dark(#ffffff, #121212)';

const esc = (s) => String(s || '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
const unesc = (s) => String(s || '').replace(/&amp;/g, '&').replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace(/&quot;/g, '"').replace(/<[^>]+>/g, '').trim();

const CARD_MAP = {
  '1..1': { arrow: 'ERmandOne', fill: '0' },
  '0..1': { arrow: 'ERzeroToOne', fill: '0' },
  '1..N': { arrow: 'ERoneToMany', fill: '1' },
  '0..N': { arrow: 'ERzeroToMany', fill: '1' }
};
const ARROW_TO_CARD = { ERmandOne: '1..1', ERzeroToOne: '0..1', ERoneToMany: '1..N', ERzeroToMany: '0..N' };

function prepareLayout(schema) {
  const tables = (schema.tables || []).map((t, idx) => {
    const tableId = t.id || t.name;
    const cols = (t.columns || []).map(c => {
      const raw = String(c.name || '');
      const cleanName = raw.endsWith('?') ? raw.slice(0, -1) : raw;
      const nullable = c.nullable !== undefined ? Boolean(c.nullable) : raw.endsWith('?');
      return {
        id: c.id || `${tableId}_row_${cleanName}`,
        keyId: c.keyId || `${tableId}_key_${cleanName}`,
        colId: c.colId || `${tableId}_col_${cleanName}`,
        name: cleanName,
        type: String(c.type || 'BIGINT'),
        key: String(c.key || ''),
        nullable
      };
    });
    const keyWidth = t.keyWidth || (cols.some(c => c.key.includes(',')) ? 60 : 30);
    let width = t.width;
    if (!width) {
      const maxLen = Math.max(t.name.length + 6, ...cols.map(c => c.name.length + (c.nullable ? 1 : 0) + c.type.length + 4));
      const rawW = keyWidth + Math.ceil((maxLen * 8.2) / 20) * 20;
      width = rawW <= 220 ? 220 : rawW <= 240 ? 240 : rawW <= 260 ? 260 : 280;
    }
    return {
      id: tableId, name: t.name,
      colIdx: t.column !== undefined ? t.column : null, order: idx,
      x: t.x !== undefined ? t.x : null, y: t.y !== undefined ? t.y : null,
      width, keyWidth, colWidth: width - keyWidth, height: HEADER_H + cols.length * ROW_H, columns: cols
    };
  });

  if (tables.some(t => t.x === null || t.y === null)) {
    const groups = new Map();
    tables.forEach((t, i) => {
      const c = t.colIdx !== null ? t.colIdx : (i % 3);
      if (!groups.has(c)) groups.set(c, []);
      groups.get(c).push(t);
    });
    let curX = 40;
    for (const c of [...groups.keys()].sort((a, b) => a - b)) {
      let curY = 40, maxW = 0;
      for (const t of groups.get(c)) {
        if (t.x === null) t.x = curX;
        if (t.y === null) t.y = curY;
        curY = t.y + t.height + 60;
        if (t.width > maxW) maxW = t.width;
      }
      curX += maxW + 120;
    }
  }
  return tables;
}

function routeRelationships(tables, relationships) {
  const rowMap = new Map();
  for (const t of tables) {
    t.columns.forEach((c, idx) => {
      const info = { rowId: c.id, table: t, centerY: t.y + HEADER_H + idx * ROW_H + ROW_H / 2 };
      rowMap.set(`${t.name}.${c.name}`, info);
      rowMap.set(`${t.id}.${c.name}`, info);
    });
  }

  // Track vertical segments { x, minY, maxY } so only overlapping Y-ranges need different X channels
  const usedSegments = [];
  const hasConflict = (x, minY, maxY) =>
    usedSegments.some(s => Math.abs(s.x - x) < 8 && !(maxY < s.minY - 5 || minY > s.maxY + 5)) ||
    tables.some(t => x > t.x - 8 && x < t.x + t.width + 8 && !(maxY < t.y - 5 || minY > t.y + t.height + 5));

  const allocX = (idealX, y1, y2, dir = 0, minBound = -Infinity, maxBound = Infinity) => {
    const minY = Math.min(y1, y2), maxY = Math.max(y1, y2);
    let base = Math.round(idealX / 10) * 10;
    let x = base, step = 1;
    while (hasConflict(x, minY, maxY) && step < 40) {
      const off = dir < 0 ? -step * 10 : dir > 0 ? step * 10 : (step % 2 === 1 ? Math.ceil(step / 2) * 10 : -(step / 2) * 10);
      const cand = Math.round((base + off) / 10) * 10;
      if (cand >= minBound && cand <= maxBound) x = cand;
      step++;
    }
    usedSegments.push({ x, minY, maxY });
    return x;
  };

  const gapGroups = new Map();
  const seenEdgeIds = new Set();
  const rels = (relationships || []).map((r, i) => {
    const [fromT, fromC] = (r.from || `${r.fromTable}.${r.fromColumn}`).split('.');
    const [toT, toC] = (r.to || `${r.toTable}.${r.toColumn}`).split('.');
    const src = rowMap.get(`${fromT}.${fromC}`), dst = rowMap.get(`${toT}.${toC}`);
    if (!src || !dst) throw new Error(`Unknown relationship endpoint: ${fromT}.${fromC} -> ${toT}.${toC}`);
    const [fromCard, toCard] = String(r.cardinality || '').toUpperCase().split(':');
    let edgeId = r.id || `rel_${fromT}_${fromC}_to_${toT}_${toC}`;
    if (seenEdgeIds.has(edgeId)) edgeId = `${edgeId}_${i + 1}`;
    seenEdgeIds.add(edgeId);
    const rel = { id: edgeId, src, dst, fromCard, toCard, points: r.points || null };

    if (!rel.points && src.table.id !== dst.table.id && Math.abs(src.table.x - dst.table.x) >= Math.min(src.table.width, dst.table.width)) {
      const key = src.table.x < dst.table.x ? `${src.table.x + src.table.width}_${dst.table.x}` : `${dst.table.x + dst.table.width}_${src.table.x}`;
      if (!gapGroups.has(key)) gapGroups.set(key, []);
      gapGroups.get(key).push(rel);
    }
    return rel;
  });

  for (const rel of rels) {
    if (rel.points) continue;
    const sT = rel.src.table, dT = rel.dst.table;
    const y1 = rel.src.centerY, y2 = rel.dst.centerY;
    if (sT.id === dT.id) {
      const x = allocX(sT.x - 20, y1, y2, -1);
      rel.points = [{ x, y: y1 }, { x, y: y2 }];
    } else if (Math.abs(sT.x - dT.x) < Math.min(sT.width, dT.width)) {
      const leftEdge = Math.min(sT.x, dT.x);
      const x = allocX(leftEdge - 20, y1, y2, -1);
      rel.points = [{ x, y: y1 }, { x, y: y2 }];
    } else if (y1 !== y2) {
      const leftEdge = sT.x < dT.x ? sT.x + sT.width : dT.x + dT.width;
      const rightEdge = sT.x < dT.x ? dT.x : sT.x;
      const midX = (leftEdge + rightEdge) / 2;
      const x = allocX(midX, y1, y2, 0, leftEdge + 15, rightEdge - 15);
      rel.points = [{ x, y: y1 }, { x, y: y2 }];
    }
  }
  return rels;
}

function buildMxGraphModelXml(schema) {
  const cells = ['<mxCell id="0"/>', '<mxCell id="1" parent="0"/>'];
  const tables = prepareLayout(schema);
  const rels = routeRelationships(tables, schema.relationships);

  for (const t of tables) {
    const lastPkIdx = t.columns.reduce((acc, c, i) => (c.key === 'PK' || c.key.startsWith('PK,')) ? i : acc, 0);
    cells.push(
      `<mxCell id="${esc(t.id)}" parent="1" style="shape=table;startSize=${HEADER_H};container=1;collapsible=1;childLayout=tableLayout;fixedRows=1;rowLines=0;fontStyle=1;align=center;resizeLast=1;html=1;rounded=0;fillColor=${HEADER_BG};" value="${esc(t.name)}" vertex="1">` +
      `<mxGeometry height="${t.height}" width="${t.width}" x="${t.x}" y="${t.y}" as="geometry"><mxRectangle height="${HEADER_H}" width="${t.width}" as="alternateBounds"/></mxGeometry></mxCell>`
    );
    t.columns.forEach((c, idx) => {
      const rowId = c.id;
      const isPk = c.key === 'PK' || c.key.startsWith('PK,');
      const isSinglePk = c.key === 'PK';
      const colLabel = c.name + (c.nullable ? '?' : '');
      const targetChars = Math.max(22, Math.floor(t.colWidth / 8.2));
      const pad = '&amp;nbsp;'.repeat(Math.max(2, targetChars - colLabel.length - c.type.length));
      const bottomBorder = idx === lastPkIdx ? 1 : 0;
      const keyStyle = isSinglePk
        ? 'shape=partialRectangle;connectable=0;fillColor=none;top=0;left=0;bottom=0;right=0;fontStyle=1;overflow=hidden;whiteSpace=wrap;html=1;rounded=0;'
        : 'shape=partialRectangle;connectable=0;fillColor=none;top=0;left=0;bottom=0;right=0;editable=1;overflow=hidden;whiteSpace=wrap;html=1;rounded=0;';
      const colStyle = isPk
        ? 'shape=partialRectangle;connectable=0;fillColor=none;top=0;left=0;bottom=0;right=0;align=left;spacingLeft=6;fontStyle=5;overflow=hidden;whiteSpace=wrap;html=1;rounded=0;'
        : 'shape=partialRectangle;connectable=0;fillColor=none;top=0;left=0;bottom=0;right=0;align=left;spacingLeft=6;overflow=hidden;whiteSpace=wrap;html=1;rounded=0;';
      cells.push(
        `<mxCell id="${esc(rowId)}" parent="${esc(t.id)}" style="shape=tableRow;horizontal=0;startSize=0;swimlaneHead=0;swimlaneBody=0;fillColor=none;collapsible=0;dropTarget=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;top=0;left=0;right=0;bottom=${bottomBorder};rounded=0;" value="" vertex="1"><mxGeometry height="${ROW_H}" width="${t.width}" y="${HEADER_H + idx * ROW_H}" as="geometry"/></mxCell>`,
        `<mxCell id="${esc(c.keyId)}" parent="${esc(rowId)}" style="${keyStyle}" value="${c.key ? (isSinglePk ? 'PK' : `&lt;b&gt;${esc(c.key)}&lt;/b&gt;`) : ''}" vertex="1"><mxGeometry height="${ROW_H}" width="${t.keyWidth}" as="geometry"><mxRectangle height="${ROW_H}" width="${t.keyWidth}" as="alternateBounds"/></mxGeometry></mxCell>`,
        `<mxCell id="${esc(c.colId)}" parent="${esc(rowId)}" style="${colStyle}" value="${esc(colLabel)}${pad}${esc(c.type)}" vertex="1"><mxGeometry height="${ROW_H}" width="${t.colWidth}" x="${t.keyWidth}" as="geometry"><mxRectangle height="${ROW_H}" width="${t.colWidth}" as="alternateBounds"/></mxGeometry></mxCell>`
      );
    });
  }

  for (const r of rels) {
    const s = CARD_MAP[r.fromCard] || { arrow: 'none', fill: '0' };
    const t = CARD_MAP[r.toCard] || { arrow: 'none', fill: '0' };
    const pts = r.points ? `<Array as="points">${r.points.map(p => `<mxPoint x="${p.x}" y="${p.y}"/>`).join('')}</Array>` : '';
    cells.push(
      `<mxCell id="${esc(r.id)}" edge="1" parent="1" source="${esc(r.src.rowId)}" target="${esc(r.dst.rowId)}" style="edgeStyle=orthogonalEdgeStyle;fontSize=12;html=1;endArrow=${t.arrow};endFill=${t.fill};startArrow=${s.arrow};startFill=${s.fill};libavoidRouting=1;" value=""><mxGeometry height="100" relative="1" width="100" as="geometry">${pts}</mxGeometry></mxCell>`
    );
  }
  return `<mxGraphModel dx="1200" dy="800" grid="1" gridSize="10" guides="1" tooltips="1" connect="1" arrows="1" fold="1" page="1" pageScale="1" pageWidth="1200" pageHeight="800" background="${CANVAS_BG}" math="0" shadow="0"><root>${cells.join('')}</root></mxGraphModel>`;
}

function decodeDrawioSvg(svgPath) {
  const svg = fs.readFileSync(svgPath, 'utf8');
  const m = svg.match(/content="([^"]+)"/);
  if (!m) throw new Error(`No Draw.io content in ${svgPath}`);
  const mxfile = m[1].replace(/&quot;/g, '"').replace(/&amp;/g, '&').replace(/&lt;/g, '<').replace(/&gt;/g, '>');
  const diag = mxfile.match(/<diagram[^>]*>([\s\S]*?)<\/diagram>/);
  const payload = diag[1].trim();
  return payload.startsWith('<mxGraphModel') ? payload : decodeURIComponent(zlib.inflateRawSync(Buffer.from(payload, 'base64')).toString('utf8'));
}

function extractJsonFromDrawioSvg(svgPath) {
  const xml = decodeDrawioSvg(svgPath);
  const getA = (s, k) => { const m = s.match(new RegExp(`\\b${k}="([^"]*)"`)); return m ? m[1] : null; };
  const tablesById = new Map(), rowsById = new Map(), cellsByRow = new Map(), edges = [];
  const re = /<mxCell\b([^>]*?)(?:\/>|>([\s\S]*?)<\/mxCell>)/g;
  let m;
  while ((m = re.exec(xml)) !== null) {
    const a = m[1], inner = m[2] || '', id = getA(a, 'id'), parent = getA(a, 'parent'), style = getA(a, 'style') || '';
    const gm = inner.match(/<mxGeometry\b([^>]*?)(?:\/>|>([\s\S]*?)<\/mxGeometry>)/);
    const ga = gm ? gm[1] : '', gi = gm ? (gm[2] || '') : '';
    const gx = parseFloat(getA(ga, 'x') || 0), gy = parseFloat(getA(ga, 'y') || 0), gw = parseFloat(getA(ga, 'width') || 0);

    if (style.includes('shape=table;')) tablesById.set(id, { id, name: unesc(getA(a, 'value')), x: gx, y: gy, width: gw, rows: [] });
    else if (style.includes('shape=tableRow;')) rowsById.set(id, { id, parent, y: gy });
    else if (style.includes('shape=partialRectangle;')) {
      if (!cellsByRow.has(parent)) cellsByRow.set(parent, []);
      cellsByRow.get(parent).push({ id, x: gx, width: gw, val: getA(a, 'value') });
    } else if (getA(a, 'edge') === '1') {
      const pts = [...gi.matchAll(/<mxPoint\b([^>]*)\/>/g)].filter(p => !getA(p[1], 'as')).map(p => ({ x: parseFloat(getA(p[1], 'x')), y: parseFloat(getA(p[1], 'y')) }));
      edges.push({ id, src: getA(a, 'source'), dst: getA(a, 'target'), style, pts });
    }
  }
  rowsById.forEach(r => { if (tablesById.has(r.parent)) tablesById.get(r.parent).rows.push(r); });

  const rowToCol = new Map(), tables = [];
  for (const t of tablesById.values()) {
    t.rows.sort((a, b) => a.y - b.y);
    let keyWidth = 30;
    const columns = t.rows.map(r => {
      const [kCell, cCell] = (cellsByRow.get(r.id) || []).sort((a, b) => a.x - b.x);
      keyWidth = kCell.width || keyWidth;
      const parts = unesc(cCell.val).replace(/(?:&nbsp;|\u00a0)+/g, ' ').trim().split(/\s+/);
      const colName = parts[0], key = unesc(kCell.val);
      const cleanName = colName.replace(/\?$/, '');
      rowToCol.set(r.id, `${t.name}.${cleanName}`);
      const colObj = { id: r.id, keyId: kCell.id, colId: cCell.id, name: colName, type: parts.slice(1).join(' ') };
      if (key) colObj.key = key;
      return colObj;
    });
    tables.push({ id: t.id, name: t.name, x: t.x, y: t.y, width: t.width, keyWidth, columns });
  }
  const relationships = edges.map(e => {
    const sA = (e.style.match(/startArrow=([^;]+)/) || [])[1], eA = (e.style.match(/endArrow=([^;]+)/) || [])[1];
    const rel = { id: e.id, from: rowToCol.get(e.src), to: rowToCol.get(e.dst) };
    if (ARROW_TO_CARD[sA] && ARROW_TO_CARD[eA]) rel.cardinality = `${ARROW_TO_CARD[sA]}:${ARROW_TO_CARD[eA]}`;
    if (e.pts.length && !e.style.includes('libavoidRouting=1')) rel.points = e.pts;
    return rel;
  });
  return { tables, relationships };
}

function verifyDrawioSvg(svgPath) {
  const svg = fs.readFileSync(svgPath, 'utf8'), xml = decodeDrawioSvg(svgPath);
  const mxIds = [...xml.matchAll(/<mxCell\s+id="([^"]+)"/g)].map(m => m[1]);
  const svgIds = new Set([...svg.matchAll(/data-cell-id="([^"]+)"/g)].map(m => m[1]));
  const missing = mxIds.filter(id => !svgIds.has(id));
  const { tables, relationships } = extractJsonFromDrawioSvg(svgPath);
  const boxes = tables.map(t => ({ ...t, h: HEADER_H + t.columns.length * ROW_H }));
  const overlaps = [];
  for (let i = 0; i < boxes.length; i++) {
    for (let j = i + 1; j < boxes.length; j++) {
      const a = boxes[i], b = boxes[j];
      if (a.x < b.x + b.width && a.x + a.width > b.x && a.y < b.y + b.h && a.y + a.h > b.y)
        overlaps.push(`${a.name} overlaps ${b.name}`);
    }
  }
  for (const r of relationships) {
    for (const p of (r.points || [])) {
      for (const b of boxes) {
        if (p.x > b.x && p.x < b.x + b.width && p.y > b.y && p.y < b.y + b.h)
          overlaps.push(`Edge ${r.from}->${r.to} waypoint (${p.x},${p.y}) inside ${b.name}`);
      }
    }
  }
  console.log(`=== Verification: ${svgPath} ===\n- Tables (${tables.length}): ${tables.map(t => t.name).join(', ')}\n- Relationships: ${relationships.length}\n- Missing SVG cells: ${missing.length}\n- Layout Check: ${overlaps.length ? overlaps.join('; ') : 'CLEAN (0 overlaps)'}\nSTATUS: ${missing.length || overlaps.length ? 'INVALID' : 'VALID & EDITABLE'}`);
  if (missing.length || overlaps.length) throw new Error('Verification failed');
}

function exportViaDrawioWebapp(xml, outPath) {
  const extRoot = path.join(process.env.USERPROFILE || process.env.HOME || '', '.vscode', 'extensions');
  const extDir = fs.existsSync(extRoot) ? fs.readdirSync(extRoot).filter(d => d.startsWith('hediet.vscode-drawio')).sort().reverse()[0] : null;
  const webapp = extDir ? path.join(extRoot, extDir, 'drawio', 'src', 'main', 'webapp') : null;
  const browser = [
    'C:/Program Files/Google/Chrome/Application/chrome.exe',
    'C:/Program Files (x86)/Google/Chrome/Application/chrome.exe',
    path.join(process.env.LOCALAPPDATA || '', 'Google/Chrome/Application/chrome.exe'),
    'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe',
    'C:/Program Files/Microsoft/Edge/Application/msedge.exe'
  ].find(p => fs.existsSync(p));
  if (!webapp || !fs.existsSync(webapp) || !browser) {
    throw new Error('Local VS Code Draw.io extension or Chrome/Edge browser not found');
  }

  const mimes = { '.html': 'text/html', '.js': 'application/javascript', '.css': 'text/css', '.svg': 'image/svg+xml', '.png': 'image/png', '.json': 'application/json', '.xml': 'application/xml', '.gif': 'image/gif', '.ico': 'image/x-icon' };
  return new Promise((resolve, reject) => {
    let childProc = null;
    const cleanup = () => {
      clearTimeout(timer);
      try { if (childProc) childProc.kill(); } catch (e) {}
      try { server.close(); } catch (e) {}
    };
    const timer = setTimeout(() => { cleanup(); reject(new Error('Draw.io export timed out after 20s')); }, 20000);
    const server = http.createServer((req, res) => {
      const u = req.url.split('?')[0];
      if (u === '/harness.html') {
        res.writeHead(200, { 'Content-Type': 'text/html' });
        return res.end(`<!DOCTYPE html><html><body><iframe id="f" src="/index.html?embed=1&proto=json&spin=0&libraries=0&saveAndExit=0&noSaveBtn=1&noExitBtn=1" style="width:1200px;height:900px;border:none;"></iframe><script>window.addEventListener('message',function(e){if(!e.data||typeof e.data!=='string')return;try{var m=JSON.parse(e.data),w=document.getElementById('f').contentWindow;if(m.event==='init'){fetch('/xml').then(function(r){return r.text()}).then(function(x){w.postMessage(JSON.stringify({action:'load',xml:x}),'*')})}else if(m.event==='load'){w.postMessage(JSON.stringify({action:'export',format:'xmlsvg',theme:'light'}),'*')}else if(m.event==='export'){fetch('/save',{method:'POST',body:m.data}).then(function(){window.close()})}}catch(err){}});</script></body></html>`);
      }
      if (u === '/xml') { res.writeHead(200, { 'Content-Type': 'application/xml' }); return res.end(xml); }
      if (u === '/save' && req.method === 'POST') {
        let body = '';
        req.on('data', c => body += c);
        return req.on('end', () => {
          const raw = Buffer.from(body.replace(/^data:image\/svg\+xml;base64,/, ''), 'base64').toString('utf8');
          const svg = raw.replace(/^<svg (?!host=)/, '<svg host="65bd71144e" ').replace(/content="&lt;mxfile[^&]*&gt;/, 'content="&lt;mxfile&gt;');
          fs.mkdirSync(path.dirname(path.resolve(outPath)), { recursive: true });
          fs.writeFileSync(outPath, svg, 'utf8');
          res.writeHead(200);
          res.end('ok');
          setTimeout(() => { cleanup(); resolve(); }, 100);
        });
      }
      const fp = path.join(webapp, u === '/' ? 'index.html' : u);
      if (!fs.existsSync(fp)) { res.writeHead(404); return res.end(); }
      res.writeHead(200, { 'Content-Type': mimes[path.extname(fp)] || 'application/octet-stream' });
      fs.createReadStream(fp).pipe(res);
    });
    server.listen(0, '127.0.0.1', () => {
      childProc = execFile(browser, ['--headless=new', '--disable-gpu', '--no-first-run', '--no-default-browser-check', `http://127.0.0.1:${server.address().port}/harness.html`], err => {
        if (err && !fs.existsSync(outPath)) { cleanup(); reject(err); }
      });
    });
  });
}

async function main() {
  const args = process.argv.slice(2);
  const arg = (k) => { const i = args.indexOf(k); return i !== -1 ? args[i + 1] : null; };
  if (arg('--verify')) return verifyDrawioSvg(arg('--verify'));
  if (arg('--extract-json')) {
    const json = JSON.stringify(extractJsonFromDrawioSvg(arg('--extract-json')), null, 2);
    return arg('--output') ? fs.writeFileSync(arg('--output'), json, 'utf8') : console.log(json);
  }
  if (arg('--input') && arg('--output')) {
    const xml = buildMxGraphModelXml(JSON.parse(fs.readFileSync(arg('--input'), 'utf8')));
    await exportViaDrawioWebapp(xml, arg('--output'));
    return verifyDrawioSvg(arg('--output'));
  }
  console.log('Usage:\n  node generate_drawio_svg.js --input <schema.json> --output <out.drawio.svg>\n  node generate_drawio_svg.js --extract-json <in.drawio.svg> [--output <schema.json>]\n  node generate_drawio_svg.js --verify <in.drawio.svg>');
}

if (require.main === module) main().catch(e => { console.error('Error:', e.message); process.exit(1); });
