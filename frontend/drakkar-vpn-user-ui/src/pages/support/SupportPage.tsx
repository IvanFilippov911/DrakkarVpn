import { useState } from 'react'
import { AppContainer, PrimaryButton } from '../../shared/ui'

export function SupportPage() {
  const username = '@drakkar_network_help'
  const [copied, setCopied] = useState(false)

  const copyUsername = async () => {
    try {
      await navigator.clipboard.writeText(username)
      setCopied(true)
      window.setTimeout(() => setCopied(false), 1500)
    } catch {
      // ignore
    }
  }

  return (
    <AppContainer>
      <main className="flex min-h-0 flex-1 flex-col items-center justify-center overflow-hidden px-6 pb-4 pt-[max(1.25rem,env(safe-area-inset-top))]">
        <h1 className="font-sans text-xl font-bold tracking-[-0.02em] text-[#e8e2d6]">Поддержка</h1>

        <div className="mt-6 w-full max-w-sm">
          <PrimaryButton
            type="button"
            onClick={() => window.open('https://t.me/drakkar_network_help', '_blank', 'noopener,noreferrer')}
          >
            Написать
          </PrimaryButton>
          <button
            type="button"
            onClick={() => void copyUsername()}
            className="mt-3 block w-full text-center text-sm font-normal text-[#8b8f94] transition-colors hover:text-[var(--foreground)]"
          >
            {copied ? 'Скопировано' : username}
          </button>
        </div>
      </main>
    </AppContainer>
  )
}
