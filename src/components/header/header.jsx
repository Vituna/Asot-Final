import React from "react";

export function Header({ data, navigation, company, onRequestOpen }) {
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
      </nav>

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
