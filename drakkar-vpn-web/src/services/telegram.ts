export function isTelegramWebApp(): boolean {
  return typeof window !== "undefined" && Boolean(window.Telegram?.WebApp);
}

export function initTelegramWebApp(): void {
  const twa = window.Telegram?.WebApp;
  if (!twa) return;
  twa.ready();
  twa.expand();
}

export function getInitData(): string {
  return window.Telegram?.WebApp?.initData ?? "";
}

export function getTelegramUserId(): number | null {
  const user = window.Telegram?.WebApp?.initDataUnsafe?.user;
  return user?.id ?? null;
}

export type Platform = "ios" | "android" | "macos" | "windows" | "unknown";

export function getPlatform(): Platform {
  const ua = navigator.userAgent.toLowerCase();
  if (/iphone|ipad|ipod/.test(ua)) return "ios";
  if (/android/.test(ua)) return "android";
  if (/macintosh|mac os x/.test(ua)) return "macos";
  if (/windows/.test(ua)) return "windows";
  return "unknown";
}

export function getDeviceName(): string {
  return "Drakkar Device";
}

/** Подгоняет фон приложения под тему Telegram (убирает белые полосы). */
export function applyTelegramTheme(): void {
  const twa = window.Telegram?.WebApp;
  if (!twa) return;
  const bg = twa.themeParams?.bg_color ?? "#0f172a";
  twa.setBackgroundColor?.(bg);
  twa.setHeaderColor?.(bg);
}
