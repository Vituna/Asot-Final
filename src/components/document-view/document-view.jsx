import React from "react";

export function isPdf(source = "") {
  return source.toLowerCase().includes(".pdf");
}

export function DocumentView({ item, className }) {
  const title = item.caption || "Документ";

  if (isPdf(item.image)) {
    return (
      <iframe
        className={className}
        src={item.image}
        title={title}
        loading="lazy"
      />
    );
  }

  return <img className={className} src={item.image} alt={title} />;
}
