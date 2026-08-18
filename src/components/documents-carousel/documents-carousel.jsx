import React, { useEffect, useMemo, useRef, useState } from "react";
import { DocumentView } from "../document-view/document-view.jsx";

export function DocumentsCarousel({ block, title, items, type, onOpen }) {
  const [paused, setPaused] = useState(false);
  const trackRef = useRef(null);
  const repeated = useMemo(() => [...items, ...items, ...items, ...items], [items]);

  useEffect(() => {
    const track = trackRef.current;
    if (!track || !items.length) return undefined;

    const updateLoopWidth = () => {
      const first = track.children[0];
      const nextGroupFirst = track.children[items.length];
      if (!first || !nextGroupFirst) return;
      const distance = nextGroupFirst.offsetLeft - first.offsetLeft;
      track.style.setProperty(type === "award" ? "--awards-loop-width" : "--certificates-loop-width", `${distance}px`);
    };

    window.requestAnimationFrame(updateLoopWidth);
    window.addEventListener("resize", updateLoopWidth);
    return () => window.removeEventListener("resize", updateLoopWidth);
  }, [items.length, type]);

  useEffect(() => {
    const resume = () => setPaused(false);
    window.addEventListener("asot-modal-close", resume);
    return () => window.removeEventListener("asot-modal-close", resume);
  }, []);

  const rowClass = type === "award" ? "awards-row awards-block__row" : "doc-row certificates-block__row";

  return (
    <section
      className={`${block} ${paused ? "is-paused" : ""}`}
      id={type === "award" ? undefined : "certificates"}
      data-certificates={type === "certificate" || undefined}
      data-awards={type === "award" || undefined}
    >
      <div className="container">
        <h2>{title}</h2>
        <div className={type === "award" ? "awards-window" : "certificates-window"}>
          <div className={rowClass} ref={trackRef}>
            {repeated.map((item, index) => {
              const clone = index >= items.length;
              const original = items[index % items.length];

              return (
                <article
                  className={`${type === "award" ? "award-item awards-block__item" : "certificates-block__item"} ${clone ? "is-clone" : ""}`}
                  role="button"
                  tabIndex={clone ? -1 : 0}
                  aria-hidden={clone ? "true" : undefined}
                  onClick={() => {
                    setPaused(true);
                    onOpen(original, type);
                  }}
                  onMouseEnter={() => setPaused(true)}
                  onMouseLeave={() => setPaused(false)}
                  onKeyDown={(event) => {
                    if (clone) return;
                    if (event.key === "Enter" || event.key === " ") {
                      event.preventDefault();
                      setPaused(true);
                      onOpen(original, type);
                    }
                  }}
                  key={`${original.image}-${index}`}
                >
                  <div className={type === "award" ? "award" : `doc ${original.color || ""}`}>
                    <DocumentView item={original} />
                  </div>
                  {type === "certificate" ? <p>{original.caption}</p> : null}
                </article>
              );
            })}
          </div>
        </div>
      </div>
    </section>
  );
}
