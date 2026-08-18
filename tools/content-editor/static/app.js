const tabs = document.querySelectorAll("[data-tab]");
const panels = document.querySelectorAll("[data-panel]");
const forms = document.querySelectorAll("[data-form]");
const status = document.querySelector("[data-status]");
const reload = document.querySelector("[data-reload]");

let content = {};

function setStatus(text, type = "ok") {
  status.textContent = text;
  status.className = `status is-${type}`;
}

function escapeHtml(text = "") {
  return text
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll("\"", "&quot;");
}

function fileToDataUrl(file) {
  return new Promise((resolve, reject) => {
    if (!file) {
      resolve(null);
      return;
    }

    const reader = new FileReader();
    reader.onload = () => resolve({
      name: file.name,
      type: file.type,
      dataUrl: reader.result
    });
    reader.onerror = () => reject(new Error("Не получилось прочитать картинку."));
    reader.readAsDataURL(file);
  });
}

function renderList(name) {
  const list = document.querySelector(`[data-list="${name}"]`);
  const items = content[name] || [];

  if (!items.length) {
    list.innerHTML = "<p>Пока пусто.</p>";
    return;
  }

  list.innerHTML = items.map((item) => {
    if (name === "reviews") {
      return `
        <article class="item item--review">
          <div>
            <h3>${escapeHtml(item.author)}</h3>
            <p>${escapeHtml(item.text)}</p>
          </div>
        </article>
      `;
    }

    return `
      <article class="item">
        <img src="/${escapeHtml(item.image)}" alt="">
        <div>
          <h3>${escapeHtml(item.caption)}</h3>
          <p>${escapeHtml(item.image)}</p>
        </div>
      </article>
    `;
  }).join("");
}

function renderAll() {
  renderList("certificates");
  renderList("awards");
  renderList("reviews");
}

async function loadContent() {
  const response = await fetch("/api/content");
  const data = await response.json();
  content = data.content || {};
  renderAll();
}

tabs.forEach((tab) => {
  tab.addEventListener("click", () => {
    const name = tab.dataset.tab;
    tabs.forEach((button) => button.classList.toggle("is-active", button === tab));
    panels.forEach((panel) => panel.classList.toggle("is-active", panel.dataset.panel === name));
  });
});

forms.forEach((form) => {
  form.addEventListener("submit", async (event) => {
    event.preventDefault();
    setStatus("Загружаю данные на сайт...");

    const formData = new FormData(form);
    const block = form.dataset.form;
    const fileInput = form.querySelector('input[type="file"]');
    const file = await fileToDataUrl(fileInput?.files[0]);

    const payload = {
      block,
      caption: formData.get("caption") || "",
      color: formData.get("color") || "",
      author: formData.get("author") || "",
      text: formData.get("text") || "",
      file
    };

    const response = await fetch("/api/add", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });
    const data = await response.json();

    if (!data.ok) {
      setStatus(data.message || "Не получилось сохранить.", "error");
      return;
    }

    content = data.content;
    form.reset();
    renderAll();
    setStatus("Готово. Данные добавлены, сайт на npm start обновится после перезагрузки вкладки.");
  });
});

reload.addEventListener("click", async () => {
  await loadContent();
  setStatus("Данные обновлены.");
});

loadContent().catch((error) => setStatus(error.message, "error"));
