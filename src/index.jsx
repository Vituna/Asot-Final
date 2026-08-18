import React from "react";
import { createRoot } from "react-dom/client";
import { App } from "./components/app/app.jsx";
import { siteData } from "../editable-data/site-data.js";
import "./css/styles.css";

createRoot(document.getElementById("root")).render(
  <App data={siteData} />
);
