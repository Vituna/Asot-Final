import React, { useEffect } from "react";
import { DocumentView } from "../document-view/document-view.jsx";

export function Modal({ item, type, onClose }) {
  useEffect(() => {
    if (!item) return undefined;

    document.body.classList.add("certificate-modal-open");
    const onKeyDown = (event) => {
      if (event.key === "Escape") onClose();
    };

    document.addEventListener("keydown", onKeyDown);
    return () => {
      document.body.classList.remove("certificate-modal-open");
      document.removeEventListener("keydown", onKeyDown);
    };
  }, [item, onClose]);

  if (!item) return null;

  const isAward = type === "award";

  return (
    <div
      className={`certificate-modal ${isAward ? "award-modal" : ""} is-open`}
      role="dialog"
      aria-modal="true"
      onClick={(event) => {
        if (event.target === event.currentTarget) onClose();
      }}
    >
      <button className="certificate-modal__close" type="button" aria-label="Закрыть" onClick={onClose}>
        ×
      </button>
      <div className="certificate-modal__content">
        <div className={isAward ? "award" : `doc ${item.color || ""}`}>
          <DocumentView item={item} />
        </div>
        {item.caption ? <p>{item.caption}</p> : null}
      </div>
    </div>
  );
}
