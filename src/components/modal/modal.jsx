import React, { useEffect, useRef } from "react";
import { DocumentView } from "../document-view/document-view.jsx";

export function Modal({ item, items = [], type, onClose, onChange }) {
  const touchStartRef = useRef(null);
  const currentIndex = items.findIndex((entry) => entry.image === item?.image);
  const canNavigate = items.length > 1 && currentIndex >= 0;
  const isAward = type === "award";

  const goTo = (direction) => {
    if (!canNavigate) return;
    const nextIndex = (currentIndex + direction + items.length) % items.length;
    onChange(items[nextIndex]);
  };

  useEffect(() => {
    if (!item) return undefined;

    document.body.classList.add("certificate-modal-open");
    const onKeyDown = (event) => {
      if (event.key === "Escape") onClose();
      if (event.key === "ArrowLeft") goTo(-1);
      if (event.key === "ArrowRight") goTo(1);
    };

    document.addEventListener("keydown", onKeyDown);
    return () => {
      document.body.classList.remove("certificate-modal-open");
      document.removeEventListener("keydown", onKeyDown);
    };
  }, [item, onClose, currentIndex, items.length]);

  if (!item) return null;

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
      {canNavigate ? (
        <>
          <button className="certificate-modal__nav certificate-modal__nav--prev" type="button" aria-label="Предыдущий документ" onClick={() => goTo(-1)} />
          <button className="certificate-modal__nav certificate-modal__nav--next" type="button" aria-label="Следующий документ" onClick={() => goTo(1)} />
        </>
      ) : null}
      <div
        className="certificate-modal__content"
        onTouchStart={(event) => {
          touchStartRef.current = event.touches[0].clientX;
        }}
        onTouchEnd={(event) => {
          if (touchStartRef.current === null) return;
          const distance = event.changedTouches[0].clientX - touchStartRef.current;
          touchStartRef.current = null;
          if (Math.abs(distance) < 45) return;
          goTo(distance < 0 ? 1 : -1);
        }}
      >
        <div className={isAward ? "award" : `doc ${item.color || ""}`}>
          <DocumentView item={item} />
        </div>
        {item.caption ? <p>{item.caption}</p> : null}
      </div>
    </div>
  );
}