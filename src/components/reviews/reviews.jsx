import React, { useEffect, useState } from "react";
import { ReviewCard } from "../review-card/review-card.jsx";

export function Reviews({ reviews }) {
  const [active, setActive] = useState(0);
  const [expanded, setExpanded] = useState(false);
  const [slideClass, setSlideClass] = useState("");
  const [animating, setAnimating] = useState(false);
  const [touchStart, setTouchStart] = useState(null);
  const slideDuration = 650;

  const getIndex = (index) => (index + reviews.length) % reviews.length;
  const farPrevIndex = getIndex(active - 2);
  const prevIndex = getIndex(active - 1);
  const nextIndex = getIndex(active + 1);
  const farNextIndex = getIndex(active + 2);

  function slide(direction, targetIndex) {
    if (animating) return;
    const nextSlideClass = direction === "next" ? "is-sliding-left" : "is-sliding-right";

    setExpanded(false);
    setAnimating(true);
    setSlideClass(nextSlideClass);

    window.setTimeout(() => {
      setActive(getIndex(targetIndex));
      setSlideClass("is-resetting");
      window.requestAnimationFrame(() => {
        setSlideClass("");
        setAnimating(false);
      });
    }, slideDuration);
  }

  useEffect(() => {
    const onKeyDown = (event) => {
      if (event.key === "Escape") setExpanded(false);
    };
    const onClick = (event) => {
      if (!expanded) return;
      if (event.target.closest(".review-card--active")) return;
      setExpanded(false);
    };

    document.addEventListener("keydown", onKeyDown);
    document.addEventListener("click", onClick);
    return () => {
      document.removeEventListener("keydown", onKeyDown);
      document.removeEventListener("click", onClick);
    };
  }, [expanded]);

  return (
    <section className={`reviews reviews-block ${slideClass}`} id="reviews" data-reviews>
      <div className="container reviews-block__container">
        <h2>Что говорят о нас</h2>
        <div className="review-stage reviews-block__stage">
          <button className="review-arrow review-arrow--prev" type="button" aria-label="Предыдущий отзыв" onClick={() => slide("prev", active - 1)}>
            <span aria-hidden="true" />
          </button>

          <div
            className="review-cards reviews-block__cards"
            onTouchStart={(event) => setTouchStart(event.touches[0].clientX)}
            onTouchEnd={(event) => {
              if (touchStart === null) return;
              const distance = event.changedTouches[0].clientX - touchStart;
              setTouchStart(null);
              if (Math.abs(distance) < 45) return;
              slide(distance < 0 ? "next" : "prev", distance < 0 ? active + 1 : active - 1);
            }}
          >
            <ReviewCard key={`far-prev-${farPrevIndex}`} review={reviews[farPrevIndex]} position="far-prev" interactive={false} expanded={false} />
            <ReviewCard key={`prev-${prevIndex}`} review={reviews[prevIndex]} position="prev" interactive={false} expanded={false} />
            <ReviewCard key={`active-${active}`} review={reviews[active]} position="active" interactive expanded={expanded} onToggle={() => setExpanded((value) => !value)} />
            <ReviewCard key={`next-${nextIndex}`} review={reviews[nextIndex]} position="next" interactive={false} expanded={false} />
            <ReviewCard key={`far-next-${farNextIndex}`} review={reviews[farNextIndex]} position="far-next" interactive={false} expanded={false} />
          </div>

          <button className="review-arrow review-arrow--next" type="button" aria-label="Следующий отзыв" onClick={() => slide("next", active + 1)}>
            <span aria-hidden="true" />
          </button>
        </div>

        <div className="review-dots" aria-label="Переключение отзывов">
          {reviews.map((review, index) => (
            <button
              className={`review-dot ${index === active ? "is-active" : ""}`}
              type="button"
              aria-label={`Показать отзыв: ${review.author}`}
              aria-current={index === active ? "true" : "false"}
              onClick={() => {
                if (index === active) return;
                const forward = (index - active + reviews.length) % reviews.length;
                const backward = (active - index + reviews.length) % reviews.length;
                slide(forward <= backward ? "next" : "prev", index);
              }}
              key={review.author}
            />
          ))}
        </div>
      </div>
    </section>
  );
}
