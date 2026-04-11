const KEY = 'drakkar_provision_kickoff'

/** Выставить перед navigate('/provision'), чтобы страница один раз вызвала startProvision. */
export function markProvisionKickoff(): void {
  if (typeof sessionStorage === 'undefined') return
  try {
    sessionStorage.setItem(KEY, '1')
  } catch {
    /* */
  }
}

/** Прочитать и сбросить флаг (один вызов startProvision на один тап). */
export function consumeProvisionKickoff(): boolean {
  if (typeof sessionStorage === 'undefined') return false
  try {
    if (sessionStorage.getItem(KEY) !== '1') return false
    sessionStorage.removeItem(KEY)
    return true
  } catch {
    return false
  }
}
