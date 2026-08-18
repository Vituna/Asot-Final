export function createYandexMapIframe({ address, coordinates }) {
  const [latitude, longitude] = coordinates;
  const point = `${longitude},${latitude}`;
  const iframe = document.createElement("iframe");

  iframe.src = `https://yandex.ru/map-widget/v1/?ll=${point}&pt=${point},pm2rdm&z=16`;
  iframe.title = `Карта: ${address}`;
  iframe.loading = "lazy";
  iframe.referrerPolicy = "no-referrer-when-downgrade";

  return iframe;
}
