import React, { useState } from "react";
import { About } from "../about/about.jsx";
import { Clients } from "../clients/clients.jsx";
import { Contacts } from "../contacts/contacts.jsx";
import { DocumentsCarousel } from "../documents-carousel/documents-carousel.jsx";
import { Footer } from "../footer/footer.jsx";
import { Header } from "../header/header.jsx";
import { Mission } from "../mission/mission.jsx";
import { Modal } from "../modal/modal.jsx";
import { RequestModal } from "../request-modal/request-modal.jsx";
import { Reviews } from "../reviews/reviews.jsx";
import { Services } from "../services/services.jsx";
import { Trust } from "../trust/trust.jsx";

export function App({ data }) {
  const [modal, setModal] = useState(null);
  const [isRequestOpen, setIsRequestOpen] = useState(false);

  const openModal = (item, type) => setModal({ item, type });
  const modalItems = modal?.type === "award" ? data.awards : data.certificates;
  const closeModal = () => {
    window.dispatchEvent(new CustomEvent("asot-modal-close"));
    setModal(null);
  };

  return (
    <>
      <Header
        data={data.hero}
        navigation={data.navigation}
        company={data.company}
        onRequestOpen={() => setIsRequestOpen(true)}
      />
      <main className="page__main">
        <Services services={data.services} />
        <Trust data={data.trust} />
        <About data={data.about} />
        <Mission data={data.mission} />
        <Clients clients={data.clients} />
        <DocumentsCarousel
          block="section certificates certificates-block"
          title="Наши свидетельства"
          items={data.certificates}
          type="certificate"
          onOpen={openModal}
        />
        <Reviews reviews={data.reviews} />
        <DocumentsCarousel
          block="section awards awards-block"
          title="Грамоты и благодарности"
          items={data.awards}
          type="award"
          onOpen={openModal}
        />
        <Contacts company={data.company} />
      </main>
      <Footer />
      <Modal
        item={modal?.item}
        items={modalItems}
        type={modal?.type}
        onClose={closeModal}
        onChange={(item) => setModal((current) => current ? { ...current, item } : current)}
      />
      <RequestModal isOpen={isRequestOpen} onClose={() => setIsRequestOpen(false)} />
    </>
  );
}
