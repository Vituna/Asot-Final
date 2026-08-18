import React, { Fragment } from "react";

export function RepeatLines({ text }) {
  const lines = String(text).split("\n");

  return lines.map((line, index) => (
    <Fragment key={`${line}-${index}`}>
      {line}
      {index < lines.length - 1 ? <br /> : null}
    </Fragment>
  ));
}
