import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { useNavigate } from 'react-router-dom'
import { useLoginMutation } from '../../features/auth/useLoginMutation'
import { isAxiosError } from 'axios'

const loginSchema = z.object({
  email: z.string().min(1, 'Введите email').email('Некорректный email'),
  password: z.string().min(1, 'Введите пароль'),
  rememberMe: z.boolean().default(true),
})

type LoginFormValues = z.infer<typeof loginSchema>

function getLoginErrorMessage(error: unknown): string {
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

export function LoginPage() {
  const navigate = useNavigate()
  const loginMutation = useLoginMutation()

  const [submitError, setSubmitError] = useState<string | null>(null)

  const {
    register,
    handleSubmit,
    setError,
    formState: { errors },
  } = useForm<LoginFormValues>({
    defaultValues: { email: '', password: '', rememberMe: true },
  })

  const onSubmit = (values: LoginFormValues) => {
    const parsed = loginSchema.safeParse(values)
    if (!parsed.success) {
      const issues = parsed.error.flatten().fieldErrors
      if (issues.email?.[0]) setError('email', { message: issues.email[0] })
      if (issues.password?.[0])
        setError('password', { message: issues.password[0] })
      return
    }

    setSubmitError(null)
    loginMutation.mutate(
      {
        email: parsed.data.email,
        password: parsed.data.password,
        rememberMe: parsed.data.rememberMe,
      },
      {
        onSuccess: () => {
          navigate('/dashboard', { replace: true })
        },
        onError: (error) => {
          setSubmitError(getLoginErrorMessage(error))
        },
      },
    )
  }

  return (
    <div
      className="min-h-screen bg-background flex items-center justify-center p-6"
      data-auth-page
    >
      <div className="w-full max-w-sm rounded-lg border bg-card p-6 text-card-foreground">
        <header className="text-center mb-6">
          <h1 className="text-lg font-semibold">Drakkar Admin</h1>
          <p className="text-sm text-muted-foreground mt-0.5">
            Sign in to access the admin panel.
          </p>
        </header>

        <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4">
          {submitError && (
            <div
              className="rounded-lg border px-4 py-3 text-sm bg-destructive/10 text-destructive border-destructive/30"
              role="alert"
            >
              {submitError}
            </div>
          )}

          <div className="flex flex-col gap-1">
            <label
              htmlFor="login-email"
              className="text-sm font-medium text-foreground"
            >
              Email
            </label>
            <input
              id="login-email"
              type="email"
              autoComplete="email"
              className="h-9 rounded-md border border-input bg-background px-3 text-sm text-foreground outline-none focus:border-primary/50 focus:ring-2 focus:ring-primary/20"
              {...register('email')}
            />
            {errors.email && (
              <span className="text-xs text-destructive">
                {errors.email.message}
              </span>
            )}
          </div>

          <div className="flex flex-col gap-1">
            <label
              htmlFor="login-password"
              className="text-sm font-medium text-foreground"
            >
              Password
            </label>
            <input
              id="login-password"
              type="password"
              autoComplete="current-password"
              className="h-9 rounded-md border border-input bg-background px-3 text-sm text-foreground outline-none focus:border-primary/50 focus:ring-2 focus:ring-primary/20"
              {...register('password')}
            />
            {errors.password && (
              <span className="text-xs text-destructive">
                {errors.password.message}
              </span>
            )}
          </div>

          <div className="flex items-center gap-2">
            <input
              id="login-remember"
              type="checkbox"
              className="h-4 w-4 accent-primary"
              {...register('rememberMe')}
            />
            <label
              htmlFor="login-remember"
              className="text-sm text-muted-foreground cursor-pointer"
            >
              Remember me
            </label>
          </div>

          <button
            type="submit"
            className="w-full h-9 rounded-md bg-primary text-primary-foreground text-sm font-medium inline-flex items-center justify-center gap-2 disabled:opacity-60"
            disabled={loginMutation.isPending}
          >
            {loginMutation.isPending ? (
              <>
                <span
                  className="h-4 w-4 animate-spin rounded-full border-2 border-primary-foreground/30 border-t-primary-foreground"
                  aria-hidden
                />
                <span>Signing in…</span>
              </>
            ) : (
              'Sign In'
            )}
          </button>
        </form>
      </div>
    </div>
  )
}
