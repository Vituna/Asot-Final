# АСОТ React + Webpack

## Запуск в VS Code

1. Открой папку проекта в Visual Studio Code:
   `C:\Users\ivanov\Documents\Codex\2026-06-03\files-mentioned-by-the-user-v2`

2. Установи зависимости, если папка `node_modules` отсутствует:
   ```bash
   npm install
   ```

3. Запусти локальный сервер:
   ```bash
   npm run dev
   ```

4. Открой сайт:
   `http://127.0.0.1:3000`

## Production-сборка

```bash
npm run build
```

Готовая сборка появится в папке `dist`.

## Где менять информацию

Основной редактируемый файл:

`editable-data/site-data.js`

В нём можно добавлять клиентов, отзывы, свидетельства, грамоты, услуги и контакты. React автоматически отрисует новые элементы из массивов.

## Структура React

Точка входа:

`src/index.jsx`

Главный компонент приложения:

`src/components/app/app.jsx`

В `App` подключаются остальные компоненты:

`src/components/header/header.jsx`
`src/components/services/services.jsx`
`src/components/clients/clients.jsx`
`src/components/reviews/reviews.jsx`

## Где менять стили

Главный вход:

`src/css/styles.css`

Стили разбиты по блокам:

`src/css/blocks/`
