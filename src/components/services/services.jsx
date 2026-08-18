import React from "react";

export function Services({ services }) {
  return (
    <section className="section services services-block" id="services">
      <div className="container services-block__container">
        <h2 className="services-block__title">Проектирование и монтаж</h2>
        <div className="service-grid services-block__grid">
          {services.map((service) => (
            <article
              className={`service-card service-${service.area} services-block__card services-block__card--${service.area}`}
              key={service.area}
            >
              <div className="services-block__content">
                <h3>{service.title}</h3>
                {service.items.length ? (
                  <ul>
                    {service.items.map((item) => (
                      <li key={item}>{item}</li>
                    ))}
                  </ul>
                ) : null}
              </div>
              <div className={`device ${service.device}`} aria-hidden="true">
                <img src={service.image} alt="" />
              </div>
            </article>
          ))}
        </div>
      </div>
    </section>
  );
}
