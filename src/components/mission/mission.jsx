import React from "react";
import { RepeatLines } from "../../shared/repeat-lines.jsx";

export function Mission({ data }) {
  return (
    <section className="section mission mission-block">
      <div className="container mission-grid mission-block__grid">
        <div className="note mission-block__note">
          <h3>
            <RepeatLines text={data.noteTitle} />
          </h3>
          <p>{data.noteText}</p>
        </div>
        <div className="mission-text mission-block__text">
          <h2>{data.title}</h2>
          <p>{data.text}</p>
        </div>
      </div>
    </section>
  );
}
