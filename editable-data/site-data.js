import { about } from "./about.js";
import { awards } from "./awards.js";
import { certificates } from "./certificates.js";
import { clients } from "./clients.js";
import { company } from "./company.js";
import { hero } from "./hero.js";
import { mission } from "./mission.js";
import { navigation } from "./navigation.js";
import { reviews } from "./reviews.js";
import { services } from "./services.js";
import { trust } from "./trust.js";

// Главный файл только собирает данные сайта из файлов по блокам.
// Редактировать текст и картинки удобнее в соседних файлах: services.js, reviews.js, awards.js и т.д.
export const siteData = {
  company,
  navigation,
  hero,
  services,
  trust,
  about,
  mission,
  clients,
  certificates,
  reviews,
  awards
};
