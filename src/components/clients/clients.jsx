import React, { useEffect, useRef, useState } from "react";

export function Clients({ clients }) {
  const [active, setActive] = useState(0);
  const [pendingActive, setPendingActive] = useState(null);
  const [transitionPhase, setTransitionPhase] = useState("idle");
  const transitionRef = useRef(null);
  const resetRef = useRef(null);
  const current = clients[active];
  const pending = pendingActive === null ? null : clients[pendingActive];
  const orderedClients = clients.map((_, index) => clients[(active + index) % clients.length]);
  const bottomClients = transitionPhase === "switching"
    ? [...orderedClients.slice(3), orderedClients[0]]
    : orderedClients.slice(3);

  const getIndex = (index) => (index + clients.length) % clients.length;

  const switchTo = (index) => {
    const nextIndex = getIndex(index);

    if (nextIndex === active || transitionPhase !== "idle") return;

    window.clearTimeout(transitionRef.current);
    window.clearTimeout(resetRef.current);
    setPendingActive(nextIndex);
    setTransitionPhase("switching");
    transitionRef.current = window.setTimeout(() => {
      setActive(nextIndex);
      setTransitionPhase("resetting");
      resetRef.current = window.setTimeout(() => {
        setPendingActive(null);
        setTransitionPhase("idle");
      }, 120);
    }, 780);
  };

  useEffect(() => {
    const timer = window.setTimeout(() => {
      switchTo(active + 1);
    }, 6000);

    return () => window.clearTimeout(timer);
  }, [active, transitionPhase, clients.length]);

  useEffect(() => () => {
    window.clearTimeout(transitionRef.current);
    window.clearTimeout(resetRef.current);
  }, []);

  const renderLogo = (client, index, extraClass = "", style) => {
    const originalIndex = clients.indexOf(client);
    const isActive = originalIndex === active;
    const isText = !client.logoMono;

    return (
      <button
        className={`client-logo clients-block__logo ${isText ? "client-logo--text" : ""} ${isActive ? "is-active" : ""} ${extraClass}`}
        type="button"
        role="tab"
        aria-selected={isActive}
        onClick={() => switchTo(originalIndex)}
        style={style}
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
  };

  const renderDetailsContent = (client) => (
    <>
      <h3>{client.title}</h3>
      <p>{client.description}</p>
      <p>Проектирование и монтаж систем:</p>
      <ul>
        {client.services.map((service) => (
          <li key={service}>{service}</li>
        ))}
      </ul>
    </>
  );

  const renderDetails = (extraClass = "") => (
    <article className={`client-details clients-block__details ${extraClass}`} aria-live="polite">
      {renderDetailsContent(current)}
    </article>
  );

  return (
    <section className="section clients clients-block" id="clients" data-clients>
      <div className="container clients-block__container">
        <h2 className="clients-block__title">Наши клиенты</h2>
        <div className="clients-layout clients-block__layout">
          <div className="clients-logo-grid clients-block__logos" role="tablist" aria-label="Клиенты АСОТ">
            {clients.map((client, index) => renderLogo(client, index))}
          </div>

          {renderDetails()}
        </div>

        <div className={`clients-mobile-flow is-${transitionPhase}`} role="tablist" aria-label="Клиенты АСОТ">
          <div className="clients-mobile-logos-window">
            <div className="clients-mobile-logos clients-mobile-logos--top">
              {orderedClients.slice(0, 4).map((client, index) => renderLogo(client, index, "clients-mobile-logo"))}
            </div>
          </div>
          <div className="client-details clients-block__details clients-mobile-details" aria-live="polite">
            <article className="clients-mobile-details__pane clients-mobile-details__pane--current">
              {renderDetailsContent(current)}
            </article>
            {pending ? (
              <article className="clients-mobile-details__pane clients-mobile-details__pane--next">
                {renderDetailsContent(pending)}
              </article>
            ) : null}
          </div>
          <div className="clients-mobile-logos clients-mobile-logos--bottom">
            {bottomClients.map((client, index) => {
              const isFirst = index === 0;
              const movesAcrossRow = index % 3 === 0;
              const moveX = isFirst
                ? "calc(-1 * (((min(100vw - 32px, 328px) - 14px) / 3) + 7px))"
                : movesAcrossRow
                  ? "calc(2 * (((min(100vw - 32px, 328px) - 14px) / 3) + 7px))"
                  : "calc(-1 * (((min(100vw - 32px, 328px) - 14px) / 3) + 7px))";
              const moveY = isFirst ? "-10px" : movesAcrossRow ? "-62px" : "0px";

              return renderLogo(
                client,
                index + 3,
                `clients-mobile-logo clients-mobile-logo--bottom ${isFirst ? "clients-mobile-logo--bottom-first" : ""}`,
                {
                  "--client-bottom-move-x": moveX,
                  "--client-bottom-move-y": moveY
                }
              );
            })}
          </div>
        </div>
      </div>
    </section>
  );
}
