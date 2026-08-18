import React, { useEffect, useState } from "react";

export function Clients({ clients }) {
  const [active, setActive] = useState(0);
  const current = clients[active];

  useEffect(() => {
    const timer = window.setTimeout(() => {
      setActive((index) => (index + 1) % clients.length);
    }, 6000);

    return () => window.clearTimeout(timer);
  }, [active, clients.length]);

  return (
    <section className="section clients clients-block" id="clients" data-clients>
      <div className="container clients-block__container">
        <h2 className="clients-block__title">Наши клиенты</h2>
        <div className="clients-layout clients-block__layout">
          <div className="clients-logo-grid clients-block__logos" role="tablist" aria-label="Клиенты АСОТ">
            {clients.map((client, index) => {
              const isActive = index === active;
              const isText = !client.logoMono;

              return (
                <button
                  className={`client-logo clients-block__logo ${isText ? "client-logo--text" : ""} ${isActive ? "is-active" : ""}`}
                  type="button"
                  role="tab"
                  aria-selected={isActive}
                  onClick={() => setActive(index)}
                  key={`${client.title}-${index}`}
                >
                  {isText ? (
                    <span>{client.textLogo}</span>
                  ) : (
                    <img src={isActive ? client.logoColor : client.logoMono} alt={client.alt || client.title} />
                  )}
                  <i aria-hidden="true" />
                </button>
              );
            })}
          </div>

          <article className="client-details clients-block__details" aria-live="polite">
            <h3>{current.title}</h3>
            <p>{current.description}</p>
            <p>Проектирование и монтаж систем:</p>
            <ul>
              {current.services.map((service) => (
                <li key={service}>{service}</li>
              ))}
            </ul>
          </article>
        </div>
      </div>
    </section>
  );
}
