const API_BASE =
  typeof import.meta.env?.VITE_API_BASE === "string"
    ? import.meta.env.VITE_API_BASE
    : "http://localhost:5102";

export const API_BASE_URL = API_BASE.replace(/\/$/, "");

/** Один тариф для MVP (подставь свой guid с бэкенда) */
export const TARIFF_ID = "00000000-0000-0000-0000-000000000001";
