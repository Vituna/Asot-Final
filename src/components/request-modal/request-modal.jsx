import React, { useEffect, useState } from "react";

const REQUEST_EMAIL = "vituna9@bk.ru";

const initialForm = {
  name: "",
  phone: "",
  message: "",
};

export function RequestModal({ isOpen, onClose }) {
  const [form, setForm] = useState(initialForm);

  useEffect(() => {
    if (!isOpen) return undefined;

    const onKeyDown = (event) => {
      if (event.key === "Escape") onClose();
    };

    document.body.classList.add("request-modal-open");
    window.addEventListener("keydown", onKeyDown);

    return () => {
      document.body.classList.remove("request-modal-open");
      window.removeEventListener("keydown", onKeyDown);
    };
  }, [isOpen, onClose]);

  if (!isOpen) return null;

  const updateField = (field) => (event) => {
    setForm((current) => ({ ...current, [field]: event.target.value }));
  };

  const submitRequest = (event) => {
    event.preventDefault();

    const subject = "Заявка с сайта АСОТ";
    const body = [
      `От кого: ${form.name.trim()}`,
      `Телефон: ${form.phone.trim()}`,
      "",
      "Что нужно сделать:",
      form.message.trim(),
    ].join("\n");

    window.location.href = `mailto:${REQUEST_EMAIL}?subject=${encodeURIComponent(subject)}&body=${encodeURIComponent(body)}`;
    setForm(initialForm);
    onClose();
  };

  const closeModal = () => {
    setForm(initialForm);
    onClose();
  };

  return (
    <div className="request-modal is-open" role="dialog" aria-modal="true" aria-labelledby="request-modal-title">
      <div className="request-modal__backdrop" onClick={closeModal} />
      <form className="request-modal__panel" onSubmit={submitRequest}>
        <button className="request-modal__close" type="button" aria-label="Закрыть" onClick={closeModal}>
          x
        </button>
        <h2 className="request-modal__title" id="request-modal-title">
          Заявка
        </h2>
        <label className="request-modal__field">
          <span>От кого</span>
          <input type="text" value={form.name} onChange={updateField("name")} required />
        </label>
        <label className="request-modal__field">
          <span>Номер телефона для связи</span>
          <input type="tel" value={form.phone} onChange={updateField("phone")} required />
        </label>
        <label className="request-modal__field">
          <span>Что вы хотите сделать</span>
          <textarea value={form.message} onChange={updateField("message")} required />
        </label>
        <div className="request-modal__actions">
          <button className="request-modal__button request-modal__button--secondary" type="button" onClick={closeModal}>
            Отмена
          </button>
          <button className="request-modal__button request-modal__button--primary" type="submit">
            Отправить заявку
          </button>
        </div>
      </form>
    </div>
  );
}
