import React, { useEffect, useRef } from "react";
import { createYandexMapIframe } from "./yandex-map.js";

export function Contacts({ company }) {
  const mapRef = useRef(null);

  useEffect(() => {
    const container = mapRef.current;
    if (!container || container.children.length) return;

    const iframe = createYandexMapIframe({
      address: company.address,
      coordinates: company.mapCoordinates
    });

    container.appendChild(iframe);
  }, [company.address, company.mapCoordinates]);

  return (
    <section className="contacts contacts-block" id="contacts">
      <div className="map contacts-block__map" ref={mapRef} aria-label="Карта расположения офиса" />
      <div className="contact-card contacts-block__card">
        <h2>Связаться с нами</h2>
        <p><a href={company.phoneHref}>{company.phone}</a></p>
        <p>{company.addressText}</p>
        <p><a href={company.emailHref}>{company.email}</a></p>
        <p className="director contacts-block__director">{company.director}</p>
      </div>
    </section>
  );
}
