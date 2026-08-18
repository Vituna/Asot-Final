import React from "react";

export function About({ data }) {
  return (
    <section className="section about about-block" id="about">
      <div className="container about-grid about-block__grid">
        <div className="about-text about-block__text">
          <h2>{data.title}</h2>
          <p>
            <strong>{data.lead}</strong>
          </p>
          <p>{data.text}</p>
        </div>
        <div className="about-photo about-block__photo">
          <img src={data.image} alt="Сотрудники компании за рабочим столом" />
        </div>
      </div>
    </section>
  );
}
