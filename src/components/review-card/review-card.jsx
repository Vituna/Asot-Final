import React, { useLayoutEffect, useRef, useState } from "react";

export function ReviewCard({ review, position, interactive, onToggle, expanded }) {
  const textRef = useRef(null);
  const [hasOverflow, setHasOverflow] = useState(false);

  useLayoutEffect(() => {
    const node = textRef.current;
    if (!node) return;
    setHasOverflow(node.scrollHeight > node.clientHeight + 8);
  }, [review, position]);

  const classes = [
    "review-card",
    `review-card--${position}`,
    hasOverflow ? "has-overflow" : "",
    expanded ? "is-expanded" : ""
  ].filter(Boolean).join(" ");

  return (
    <article className={classes} aria-hidden={position === "active" ? undefined : "true"}>
      <span className="review-card__quote" aria-hidden="true">“</span>
      <h3>{review.author}</h3>
      <p ref={textRef}>{review.text}</p>
      <span className={`review-card__line ${hasOverflow ? "is-visible" : ""}`} aria-hidden="true" />
      <button
        className={`review-more ${hasOverflow ? "is-visible" : ""}`}
        type="button"
        onClick={(event) => {
          event.stopPropagation();
          if (interactive) onToggle();
        }}
      >
        {expanded ? "Свернуть отзыв" : "Смотреть полный отзыв"}
      </button>
    </article>
  );
}
