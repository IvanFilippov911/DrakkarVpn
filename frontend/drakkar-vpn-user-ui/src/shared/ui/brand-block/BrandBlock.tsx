import type { ReactNode } from 'react'

type BrandBlockProps = {
  title?: string
  subtitle?: string
  icon?: ReactNode
}

export function BrandBlock({
  title = 'Drakkar Network',
  subtitle = 'Приватный доступ к сети',
  icon,
}: BrandBlockProps) {
  return (
    <div className="flex w-full min-w-0 flex-col items-center text-center">
      <div className="mb-3 flex w-full min-h-[8.5rem] items-center justify-center sm:min-h-[10rem] md:min-h-[11rem]" aria-hidden>
        {icon ?? (
          <img
            src="/drakkar-mark.png"
            alt=""
            className="h-[8.5rem] w-auto max-h-[min(32vh,11.5rem)] max-w-[min(96vw,14rem)] object-contain select-none sm:h-[10.5rem] sm:max-w-[17rem] md:h-[11.5rem] md:max-w-[19rem]"
            draggable={false}
          />
        )}
      </div>
      <h1 className="w-full min-w-0 overflow-hidden text-ellipsis whitespace-nowrap font-sans text-[clamp(2.25rem,5vw,2.5rem)] font-bold leading-none tracking-[-0.02em] text-[#e8e2d6]">
        {title}
      </h1>
      {subtitle ? (
        <p className="mt-2 max-w-sm text-base font-normal leading-normal text-[#8b8f94]">{subtitle}</p>
      ) : null}
    </div>
  )
}
