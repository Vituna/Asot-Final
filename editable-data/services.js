// Блок "Проектирование и монтаж".
// title и items - текст карточки, image - картинка карточки.
// area и device отвечают за раскладку/размеры в CSS, их лучше не менять без правки стилей.
export const services = [
  {
    area: "fire",
    device: "smoke",
    image: "images/service-smoke-optimized.png",
    title: "Средств противопожарной защиты",
    items: [
      "Автоматическая установка пожарной сигнализации",
      "Система оповещения и управления эвакуацией людей при пожаре",
      "Автоматическая установка пожаротушения",
      "Автоматика противодымной вентиляции",
      "Автоматика внутреннего пожарного водопровода"
    ]
  },
  {
    area: "cable",
    device: "wifi",
    image: "images/service-wifi-optimized.png",
    title: "Структурированной кабельной системы",
    items: [
      "Локально-вычислительные сети",
      "Системы беспроводной передачи данных"
    ]
  },
  {
    area: "auto",
    device: "display",
    image: "images/service-monitor-optimized.png",
    title: "Систем автоматизации и диспетчеризации инженерных систем здания",
    items: []
  },
  {
    area: "security",
    device: "sensor",
    image: "images/service-sensor.svg",
    title: "Технических средств защиты",
    items: [
      "Автоматическая установка охранной сигнализации",
      "Автоматическая установка тревожной сигнализации",
      "Автоматическая установка защиты периметра",
      "Средства передачи тревожных извещений"
    ]
  },
  {
    area: "access",
    device: "gate",
    image: "images/service-gate-optimized.png",
    title: "Систем контроля и управления доступом",
    items: []
  },
  {
    area: "video",
    device: "camera",
    image: "images/service-camera-optimized.png",
    title: "Телевизионных систем видеоконтроля",
    items: [
      "Система охранного видеонаблюдения",
      "Система технологического видеонаблюдения"
    ]
  },
  {
    area: "entry",
    device: "barrier",
    image: "images/service-barrier-optimized.png",
    title: "Систем организации въезда на охраняемую территорию",
    items: []
  }
];
