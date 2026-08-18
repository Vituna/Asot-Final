import React from "react";
import { RepeatLines } from "../../shared/repeat-lines.jsx";

export function Trust({ data }) {
  return (
    <section className="section trust trust-block">
      <div className="container trust-block__container">
        <h2 className="trust-block__title">
          <RepeatLines text={data.title} />
        </h2>
        <div className="trust-row trust-block__list">
          {data.items.map((item) => (
            <div className={`trust-item trust-${item.mod} trust-block__item`} key={item.mod}>
              <span className="trust-block__icon" aria-hidden="true">
                <img src={item.icon} alt="" />
              </span>
              <p>{item.text}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
