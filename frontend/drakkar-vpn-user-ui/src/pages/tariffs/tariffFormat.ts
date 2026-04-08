import type { TariffDto } from '../../entities/user'

export function formatTariffPrice(price: number): string {
  return new Intl.NumberFormat('ru-RU', {
    style: 'currency',
    currency: 'RUB',
    maximumFractionDigits: 0,
  }).format(price)
}

export function formatTariffPriceShort(price: number): string {
  return new Intl.NumberFormat('ru-RU', {
    maximumFractionDigits: 0,
  }).format(price)
}

function pluralizeDaysRu(n: number): string {
  const abs = Math.abs(n)
  const mod10 = abs % 10
  const mod100 = abs % 100
  if (mod100 >= 11 && mod100 <= 14) return 'дней'
  if (mod10 === 1) return 'день'
  if (mod10 >= 2 && mod10 <= 4) return 'дня'
  return 'дней'
}

export function timeSpanToDaysLabel(duration: string): string {
  // Backend serializes .NET TimeSpan as "d.hh:mm:ss" (days part is optional)
  const raw = String(duration).trim()
  const daysPart = raw.includes('.') ? raw.split('.', 1)[0] : '0'
  const days = Number.parseInt(daysPart, 10)
  if (!Number.isFinite(days) || days < 0) return raw
  return `${days} ${pluralizeDaysRu(days)}`
}

export function tariffToCardProps(t: TariffDto) {
  const features = [`✔ До ${t.defaultMaxDevices} устройств`, '✔ Без ограничений скорости']

  return {
    id: t.id,
    name: t.name,
    price: formatTariffPrice(t.price),
    durationLabel: timeSpanToDaysLabel(t.duration),
    features,
  }
}
