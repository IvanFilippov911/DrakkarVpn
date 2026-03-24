import type { TariffDto } from '../../entities/user'

export function formatTariffPrice(price: number): string {
  return new Intl.NumberFormat('ru-RU', {
    style: 'currency',
    currency: 'RUB',
    maximumFractionDigits: 0,
  }).format(price)
}

export function tariffToCardProps(t: TariffDto) {
  return {
    id: t.id,
    name: t.name,
    price: formatTariffPrice(t.price),
    period: `Период: ${t.duration} · ${t.status}`,
    features: [`До ${t.defaultMaxDevices} устройств`],
  }
}
