import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import type { TariffDto } from '../../entities/user'
import { useTariffs } from '../../entities/user'
import { usePurchase } from '../../features/user'
import { getTelegramUserId } from '../../shared/lib/telegramContext'
import { AppContainer, Header, PrimaryButton, TariffCard, TariffList } from '../../shared/ui'
import { formatTariffPriceShort, tariffToCardProps } from './tariffFormat'

function sortForChoice(items: TariffDto[]) {
  if (items.length <= 1) return { ordered: items, recommendedId: null as string | null }

  const byPrice = [...items].sort((a, b) => a.price - b.price)
  const recommended = byPrice[Math.floor((byPrice.length - 1) / 2)]
  const cheap = byPrice[0]
  const expensive = byPrice[byPrice.length - 1]

  const seen = new Set<string>()
  const ordered: TariffDto[] = []
  for (const t of [recommended, cheap, expensive, ...byPrice]) {
    if (!t) continue
    if (seen.has(t.id)) continue
    seen.add(t.id)
    ordered.push(t)
  }

  return { ordered, recommendedId: recommended?.id ?? null }
}

export function TariffsPage() {
  const navigate = useNavigate()
  const [selectedId, setSelectedId] = useState<string | null>(null)
  const [freeNotice, setFreeNotice] = useState(false)
  const tariffsQuery = useTariffs()
  const purchase = usePurchase()

  const telegramId = getTelegramUserId()

  const list = tariffsQuery.data ?? []
  const { ordered: orderedList, recommendedId } = sortForChoice(list)
  const isEmpty = !tariffsQuery.isPending && !tariffsQuery.isError && list.length === 0
  const selectedTariff = selectedId ? list.find((x) => x.id === selectedId) : null

  const handlePurchase = () => {
    if (selectedId == null) return
    setFreeNotice(true)
    window.setTimeout(() => setFreeNotice(false), 2500)
  }

  const buyDisabled =
    !selectedId ||
    purchase.isPending ||
    telegramId == null ||
    tariffsQuery.isPending ||
    tariffsQuery.isError ||
    isEmpty

  return (
    <AppContainer>
      <Header title="Выберите тариф" onBack={() => navigate('/')} />
      <div className="min-h-0 flex-1 overflow-y-auto overscroll-y-contain px-6 pb-2">
        {tariffsQuery.isPending || tariffsQuery.isError || isEmpty ? (
          <div className="min-h-0 flex-1" aria-hidden />
        ) : (
          <TariffList>
            {orderedList.map((t) => {
              const card = tariffToCardProps(t)
              return (
                <TariffCard
                  key={card.id}
                  name={card.name}
                  price={card.price}
                  durationLabel={card.durationLabel}
                  features={card.features}
                  recommended={recommendedId === card.id}
                  selected={selectedId === card.id}
                  onSelect={() => setSelectedId(card.id)}
                />
              )
            })}
          </TariffList>
        )}
      </div>
      <footer className="mx-auto w-full max-w-sm shrink-0 px-6 pb-1 pt-3">
        {tariffsQuery.isError ? (
          <PrimaryButton type="button" onClick={() => void tariffsQuery.refetch()}>
            Повторить
          </PrimaryButton>
        ) : (
          <div className="space-y-2">
            <PrimaryButton type="button" disabled={buyDisabled} onClick={handlePurchase}>
              {selectedTariff ? `Подключить за ${formatTariffPriceShort(selectedTariff.price)} ₽` : 'Подключить'}
            </PrimaryButton>
            {freeNotice ? (
              <p className="text-center text-[13px] font-normal text-[#8b8f94]">
                На данный момент сервис бесплатный
              </p>
            ) : null}
          </div>
        )}
      </footer>
    </AppContainer>
  )
}
