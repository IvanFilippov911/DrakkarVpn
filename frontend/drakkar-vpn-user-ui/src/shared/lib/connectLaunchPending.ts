const KEY = 'drakkar_connect_launch_pending'

/** После успешного provisioning — перед navigate на главную. */
export function markConnectLaunchPending(): void {
  if (typeof sessionStorage === 'undefined') return
  try {
    sessionStorage.setItem(KEY, '1')
  } catch {
    /* */
  }
}

/** Флаг ещё не снят (ожидаем config / Ready на Home). */
export function peekConnectLaunchPending(): boolean {
  if (typeof sessionStorage === 'undefined') return false
  try {
    return sessionStorage.getItem(KEY) === '1'
  } catch {
    return false
  }
}

/**
 * Снять флаг и вернуть true один раз — только непосредственно перед runConnectLaunch,
 * когда данные для запуска уже доступны (иначе ранний consume ломает повтор).
 */
export function consumeConnectLaunchPending(): boolean {
  if (typeof sessionStorage === 'undefined') return false
  try {
    if (sessionStorage.getItem(KEY) !== '1') return false
    sessionStorage.removeItem(KEY)
    return true
  } catch {
    return false
  }
}
