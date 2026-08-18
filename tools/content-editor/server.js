import { createServer } from "node:http";
import { copyFile, mkdir, readFile, writeFile } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath, pathToFileURL } from "node:url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const rootDir = path.resolve(__dirname, "../..");
const editorDir = path.join(__dirname, "static");
const editableDir = path.join(rootDir, "editable-data");
const imagesDir = path.join(rootDir, "src", "images");
const port = 3100;

const blocks = {
  certificates: {
    label: "Сертификаты",
    exportName: "certificates",
    file: "certificates.js",
    comment: "// Блок \"Наши свидетельства\". Чтобы добавить документ, используй редактор сайта или добавь объект image + caption.",
    fields: ["image", "color", "caption"]
  },
  awards: {
    label: "Грамоты",
    exportName: "awards",
    file: "awards.js",
    comment: "// Блок \"Грамоты и благодарности\". Чтобы добавить грамоту, используй редактор сайта или добавь объект image + caption.",
    fields: ["image", "caption"]
  },
  reviews: {
    label: "Отзывы",
    exportName: "reviews",
    file: "reviews.js",
    comment: "// Блок \"Отзывы\". Чтобы добавить отзыв, используй редактор сайта или добавь объект author + text.",
    fields: ["author", "text"]
  }
};

function sendJson(response, status, data) {
  response.writeHead(status, { "Content-Type": "application/json; charset=utf-8" });
  response.end(JSON.stringify(data));
}

function safeName(name) {
  const ext = path.extname(name).toLowerCase();
  const base = path.basename(name, ext)
    .toLowerCase()
    .replace(/[^a-zа-яё0-9]+/giu, "-")
    .replace(/^-+|-+$/g, "")
    .slice(0, 70);

  return `${base || "image"}-${Date.now()}${ext || ".jpg"}`;
}

function stringifyExport(block, list) {
  return `${block.comment}\nexport const ${block.exportName} = ${JSON.stringify(list, null, 2)};\n`;
}

async function refreshDevServer() {
  const entryFile = path.join(rootDir, "src", "index.jsx");
  const content = await readFile(entryFile, "utf8");
  await writeFile(entryFile, content, "utf8");
}

async function readBlock(name) {
  const block = blocks[name];
  const file = path.join(editableDir, block.file);
  const moduleUrl = `${pathToFileURL(file).href}?updated=${Date.now()}`;
  const module = await import(moduleUrl);
  return module[block.exportName];
}

async function readAllContent() {
  const entries = await Promise.all(
    Object.keys(blocks).map(async (name) => [name, await readBlock(name)])
  );

  return Object.fromEntries(entries);
}

async function saveUploadedImage(file) {
  if (!file?.dataUrl || !file?.name) return null;

  const matches = file.dataUrl.match(/^data:(.+);base64,(.+)$/);
  if (!matches) {
    throw new Error("Картинка пришла в неверном формате.");
  }

  await mkdir(imagesDir, { recursive: true });
  const filename = safeName(file.name);
  const target = path.join(imagesDir, filename);
  await writeFile(target, Buffer.from(matches[2], "base64"));

  return `images/${filename}`;
}

async function copyExistingImage(sourcePath) {
  if (!sourcePath) return null;

  const resolved = path.resolve(sourcePath);
  const filename = safeName(path.basename(resolved));
  const target = path.join(imagesDir, filename);

  await mkdir(imagesDir, { recursive: true });
  await copyFile(resolved, target);

  return `images/${filename}`;
}

function pickItem(blockName, body, imagePath) {
  if (blockName === "reviews") {
    return {
      author: body.author.trim(),
      text: body.text.trim()
    };
  }

  if (blockName === "certificates") {
    return {
      image: imagePath,
      color: body.color || "blue",
      caption: body.caption.trim()
    };
  }

  return {
    image: imagePath,
    caption: body.caption.trim()
  };
}

function validateItem(blockName, body, imagePath) {
  if (blockName === "reviews") {
    if (!body.author?.trim() || !body.text?.trim()) {
      throw new Error("Заполни автора и текст отзыва.");
    }
    return;
  }

  if (!body.caption?.trim()) {
    throw new Error("Заполни подпись.");
  }

  if (!imagePath) {
    throw new Error("Добавь картинку.");
  }
}

async function handleAdd(response, body) {
  const block = blocks[body.block];
  if (!block) {
    sendJson(response, 400, { ok: false, message: "Неизвестный блок." });
    return;
  }

  try {
    const current = await readBlock(body.block);
    const imagePath = await saveUploadedImage(body.file) || await copyExistingImage(body.imageSourcePath);

    validateItem(body.block, body, imagePath);

    const next = [...current, pickItem(body.block, body, imagePath)];
    await writeFile(path.join(editableDir, block.file), stringifyExport(block, next), "utf8");
    await refreshDevServer();

    sendJson(response, 200, { ok: true, refreshed: true, content: await readAllContent() });
  } catch (error) {
    sendJson(response, 400, { ok: false, message: error.message });
  }
}

async function readRequestBody(request) {
  const chunks = [];
  for await (const chunk of request) chunks.push(chunk);
  return JSON.parse(Buffer.concat(chunks).toString("utf8") || "{}");
}

async function serveStatic(response, requestPath) {
  if (requestPath.startsWith("/images/")) {
    await serveImage(response, requestPath);
    return;
  }

  const urlPath = requestPath === "/" ? "/index.html" : requestPath;
  const file = path.resolve(editorDir, `.${urlPath}`);

  if (!file.startsWith(editorDir)) {
    response.writeHead(403);
    response.end();
    return;
  }

  try {
    const body = await readFile(file);
    const ext = path.extname(file);
    const type = ext === ".css" ? "text/css" : ext === ".js" ? "text/javascript" : "text/html";
    response.writeHead(200, { "Content-Type": `${type}; charset=utf-8` });
    response.end(body);
  } catch {
    response.writeHead(404);
    response.end("Not found");
  }
}

async function serveImage(response, requestPath) {
  const file = path.resolve(rootDir, "src", `.${requestPath}`);

  if (!file.startsWith(imagesDir)) {
    response.writeHead(403);
    response.end();
    return;
  }

  try {
    const body = await readFile(file);
    const ext = path.extname(file).toLowerCase();
    const type = ext === ".svg" ? "image/svg+xml" : ext === ".png" ? "image/png" : ext === ".webp" ? "image/webp" : "image/jpeg";
    response.writeHead(200, { "Content-Type": type });
    response.end(body);
  } catch {
    response.writeHead(404);
    response.end("Not found");
  }
}

const server = createServer(async (request, response) => {
  const url = new URL(request.url, `http://127.0.0.1:${port}`);

  if (request.method === "GET" && url.pathname === "/api/content") {
    sendJson(response, 200, { ok: true, content: await readAllContent() });
    return;
  }

  if (request.method === "POST" && url.pathname === "/api/add") {
    await handleAdd(response, await readRequestBody(request));
    return;
  }

  await serveStatic(response, url.pathname);
});

server.listen(port, "127.0.0.1", () => {
  console.log(`Редактор сайта открыт: http://127.0.0.1:${port}`);
});
