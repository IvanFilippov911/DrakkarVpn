import { isAxiosError } from 'axios'
import { z } from 'zod'

export const loginSchema = z.object({
  email: z.string().min(1, 'Введите email').email('Некорректный email'),
  password: z.string().min(1, 'Введите пароль'),
  rememberMe: z.boolean().default(true),
})

export type LoginFormValues = z.infer<typeof loginSchema>

export function getLoginErrorMessage(error: unknown): string {
  if (!isAxiosError(error)) {
    return 'Произошла ошибка. Попробуйте ещё раз.'
  }

  const status = error.response?.status
  const code = (error.response?.data as { code?: string } | undefined)?.code

  if (status === 401) {
    return 'Неверный email или пароль'
  }
  if (status === 403 || code === 'ADMIN_INACTIVE') {
    return 'Учётная запись администратора отключена'
  }

  return 'Произошла ошибка. Попробуйте ещё раз.'
}

