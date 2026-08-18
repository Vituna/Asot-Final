import React, { useEffect, useState } from "react";

export function Header({ data, navigation, company, onRequestOpen }) {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const menuItems = navigation.slice(0, -1);

  useEffect(() => {
    if (!isMenuOpen) return undefined;

    const onKeyDown = (event) => {
      if (event.key === "Escape") setIsMenuOpen(false);
    };

    document.body.classList.add("mobile-menu-open");
    window.addEventListener("keydown", onKeyDown);

    return () => {
      document.body.classList.remove("mobile-menu-open");
      window.removeEventListener("keydown", onKeyDown);
    };
  }, [isMenuOpen]);

  const closeMenu = () => setIsMenuOpen(false);

  return (
    <header className="hero header" id="top">
      <img className="header__background" src={data.backgroundImage} alt="" aria-hidden="true" />
      <nav className="nav header__nav">
        <a className="logo header__logo" href="#top">
          <img src="images/logo.svg" alt="АСОТ системы безопасности" />
        </a>
        <div className="nav-links header__menu">
          {navigation.map((item) =>
            item.label === "Заявки" ? (
              <button className="header__link header__link--button" type="button" key={item.label} onClick={onRequestOpen}>
                {item.label}
              </button>
            ) : (
              <a className="header__link" href={item.href} key={item.label}>
                {item.label}
              </a>
            )
          )}
        </div>
        <a className="phone header__phone" href={company.phoneHref}>
          {company.phone}
        </a>
        <button
          className="header__menu-toggle"
          type="button"
          aria-label="Открыть меню"
          aria-expanded={isMenuOpen}
          onClick={() => setIsMenuOpen(true)}
        >
          <span />
          <span />
          <span />
        </button>
      </nav>

      <div className={`mobile-menu ${isMenuOpen ? "is-open" : ""}`} aria-hidden={!isMenuOpen}>
        <div className="mobile-menu__bar">
          <a className="mobile-menu__logo" href="#top" onClick={closeMenu}>
            <img src="images/logo-mark.png" alt="" aria-hidden="true" />
            <span>
              <strong>АСОТ</strong>
              <small>системы безопасности</small>
            </span>
          </a>
          <a className="mobile-menu__phone" href={company.phoneHref}>
            {company.phone}
          </a>
          <button className="mobile-menu__close" type="button" aria-label="Закрыть меню" onClick={closeMenu}>
            <span />
            <span />
          </button>
        </div>
        <div className="mobile-menu__links">
          {menuItems.map((item) => (
            <a href={item.href} key={item.label} onClick={closeMenu}>
              {item.label}
            </a>
          ))}
        </div>
        <div className="mobile-menu__contacts">
          <h2>Связаться с нами</h2>
          <div className="mobile-menu__contacts-grid">
            <div>
              <a href={company.phoneHref}>{company.phone}</a>
              <p>{company.addressText}</p>
              <a href={company.emailHref}>{company.email}</a>
            </div>
            <div>
              <p>{company.director}</p>
              <a href={company.phoneHref}>{company.phone}</a>
              <a href={company.emailHref}>{company.email}</a>
            </div>
          </div>
        </div>
      </div>

      <div className="hero-content header__content">
        <h1 className="header__title">{data.title}</h1>
        <p className="header__text">{data.text}</p>
      </div>
      <a className="scroll header__scroll" href="#services" aria-label="Перейти к услугам">
        ⌄
      </a>
    </header>
  );
}
