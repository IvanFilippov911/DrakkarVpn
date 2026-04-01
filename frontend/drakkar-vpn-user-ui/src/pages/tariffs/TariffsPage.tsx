import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useTariffs } from '../../entities/user'
import { usePurchase } from '../../features/user'
import { getTelegramUserId } from '../../shared/lib/telegramContext'
import {
  AppContainer,
  Header,
  PrimaryButton,
  StateCard,
  TariffCard,
  TariffList,
} from '../../shared/ui'
import {
  IconBlocked,
  IconNoSubscription,
  IconPending,
} from '../../shared/ui/icons/homeStateIcons'
import { tariffToCardProps } from './tariffFormat'

export function TariffsPage() {
  const navigate = useNavigate()
  const [selectedId, setSelectedId] = useState<string | null>(null)
  const tariffsQuery = useTariffs()
  const purchase = usePurchase()

  const telegramId = getTelegramUserId()

  const list = tariffsQuery.data ?? []
  const isEmpty = !tariffsQuery.isPending && !tariffsQuery.isError && list.length === 0

  const handlePurchase = async () => {
    if (telegramId == null || selectedId == null) return
    const t = list.find((x) => x.id === selectedId)
    if (!t) return

    try {
      await purchase.mutateAsync({
        telegramId,
        tariffId: selectedId,
        devicesCount: t.defaultMaxDevices,
        requestId: crypto.randomUUID(),
      })
      navigate('/')
    } catch {
      /* см. purchase.isError при необходимости */
    }
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
      <Header title="Тарифы" onBack={() => navigate('/')} />
      <div className="flex flex-1 flex-col px-6 pb-4">
        {tariffsQuery.isPending ? (
          <div className="flex flex-1 flex-col items-center justify-center">
            <div className="flex w-full flex-col items-center">
              <StateCard
                variant="default"
                title="Загрузка"
                subtitle="Получаем список тарифов…"
                icon={<IconPending />}
              />
            </div>
          </div>
        ) : tariffsQuery.isError ? (
          <div className="flex flex-1 flex-col items-center justify-center">
            <div className="flex w-full flex-col items-center">
              <StateCard
                variant="error"
                title="Не удалось загрузить"
                subtitle="Проверьте подключение и попробуйте снова."
                icon={<IconBlocked />}
              />
            </div>
          </div>
        ) : isEmpty ? (
          <div className="flex flex-1 flex-col items-center justify-center">
            <div className="flex w-full flex-col items-center">
              <StateCard
                variant="default"
                title="Нет доступных тарифов"
                subtitle="Попробуйте зайти позже или вернитесь на главный экран."
                icon={<IconNoSubscription />}
              />
            </div>
          </div>
        ) : (
          <TariffList>
            {list.map((t) => {
              const card = tariffToCardProps(t)
              return (
                <TariffCard
                  key={card.id}
                  name={card.name}
                  price={card.price}
                  period={card.period}
                  features={card.features}
                  selected={selectedId === card.id}
                  onSelect={() => setSelectedId(card.id)}
                />
              )
            })}
          </TariffList>
        )}
      </div>
      <footer className="ui-safe-bottom px-6 pt-4">
        {tariffsQuery.isError ? (
          <PrimaryButton type="button" onClick={() => void tariffsQuery.refetch()}>
            Повторить
          </PrimaryButton>
        ) : (
          <PrimaryButton
            type="button"
            disabled={buyDisabled}
            onClick={handlePurchase}
          >
            Купить
          </PrimaryButton>
        )}
      </footer>
    </AppContainer>
  )
}
