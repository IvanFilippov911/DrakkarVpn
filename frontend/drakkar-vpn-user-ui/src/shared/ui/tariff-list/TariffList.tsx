import type { ReactNode } from 'react'

type TariffListProps = {
  children: ReactNode
}

export function TariffList({ children }: TariffListProps) {
  return <div className="flex flex-col space-y-3">{children}</div>
}
