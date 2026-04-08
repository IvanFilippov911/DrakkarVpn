import { isAxiosError } from 'axios'

export function getErrorMessage(err: unknown, fallback: string): string {
  if (!err) return fallback

  if (isAxiosError(err)) {
    const maybeMessage = getMessageFromUnknown(err.response?.data)
    if (typeof maybeMessage === 'string' && maybeMessage.trim().length > 0) {
      return maybeMessage
    }

    return err.response?.status ? `Request failed (${err.response.status}).` : 'Request failed.'
  }

  return err instanceof Error ? err.message : fallback
}

function getMessageFromUnknown(value: unknown): string | undefined {
  if (!value || typeof value !== 'object') return undefined
  if (!('message' in value)) return undefined
  const msg = (value as { message?: unknown }).message
  return typeof msg === 'string' ? msg : undefined
}

