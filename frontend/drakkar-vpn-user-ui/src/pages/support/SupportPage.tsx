import { useState } from 'react'
import { BrandBlock, HomeBrandShell, HomeFooterStatusCardSpace, PrimaryButton } from '../../shared/ui'

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
    <HomeBrandShell
      brand={<BrandBlock logoOnly />}
      statusCard={<HomeFooterStatusCardSpace />}
      footerMiddle={
        <button
          type="button"
          onClick={() => void copyUsername()}
          className="w-full text-center text-[14px] font-normal leading-normal text-[#8b8f94] transition-colors hover:text-[var(--foreground)]"
        >
          {copied ? 'Скопировано' : username}
        </button>
      }
      footer={
        <PrimaryButton
          type="button"
          onClick={() => window.open('https://t.me/drakkar_network_help', '_blank', 'noopener,noreferrer')}
        >
          Написать в поддержку
        </PrimaryButton>
      }
    />
  )
}
