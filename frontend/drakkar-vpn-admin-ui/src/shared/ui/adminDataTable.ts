/**
 * Layout-only helpers for admin list tables.
 * Avoid `table-layout: fixed` + forced narrow columns: overflowing cell paint defaults to visible
 * and header/body text can overlap adjacent columns (looks like merged labels / shifted rows).
 */
export const ADMIN_TABLE = 'w-full border-collapse text-sm'

export const ADMIN_TH = 'border-b px-2 py-2 font-medium align-middle whitespace-nowrap'

export const ADMIN_TD = 'border-b px-2 py-2 align-middle'
