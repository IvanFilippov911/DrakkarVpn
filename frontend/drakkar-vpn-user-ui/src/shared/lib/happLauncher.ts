const HAPP_LAUNCH_TIMEOUT_MS = 1500
const DEFAULT_HIDDIFY_ANDROID_URL = 'https://play.google.com/store/apps/details?id=app.hiddify.com'
const DEFAULT_V2RAYTUN_IOS_URL = 'https://apps.apple.com/ru/app/v2raytun/id6476628951'
const DEFAULT_HIDDIFY_DESKTOP_URL = 'https://hiddify.com/'

/**
 * Tries to open a HApp deep-link and detects whether the app switch likely happened.
 * Returns false when browser stayed visible past timeout (likely no handler installed).
 */
export async function openHappLinkWithFallback(
  happLink: string,
  timeoutMs: number = HAPP_LAUNCH_TIMEOUT_MS,
): Promise<boolean> {
  if (!happLink) return false

  let didHide = false
  const onVisibilityChange = () => {
    if (document.hidden) didHide = true
  }

  document.addEventListener('visibilitychange', onVisibilityChange)

  try {
    window.location.href = happLink
    await new Promise((resolve) => window.setTimeout(resolve, timeoutMs))
    return didHide
  } finally {
    document.removeEventListener('visibilitychange', onVisibilityChange)
  }
}

export function getHappInstallUrl(platform?: string): string {
  const p = platform?.toLowerCase() ?? ''

  if (p.includes('android')) {
    return import.meta.env.VITE_HIDDIFY_ANDROID_URL || DEFAULT_HIDDIFY_ANDROID_URL
  }

  if (p.includes('ios') || p.includes('iphone') || p.includes('ipad')) {
    return import.meta.env.VITE_V2RAYTUN_IOS_URL || DEFAULT_V2RAYTUN_IOS_URL
  }

  return import.meta.env.VITE_HIDDIFY_DESKTOP_URL || DEFAULT_HIDDIFY_DESKTOP_URL
}
