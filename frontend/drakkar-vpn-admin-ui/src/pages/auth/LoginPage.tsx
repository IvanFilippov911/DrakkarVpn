import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { useNavigate } from 'react-router-dom'
import { useLoginMutation } from '../../features/auth/useLoginMutation'
import { isAxiosError } from 'axios'
import styles from './LoginPage.module.css'

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
    <div className={styles.container}>
      <div className={styles.card}>
        <header className={styles.header}>
          <h1 className={styles.title}>Drakkar Network</h1>
          <p className={styles.subtitle}>Admin Panel</p>
        </header>

        <form onSubmit={handleSubmit(onSubmit)} className={styles.form}>
          {submitError && (
            <div className={styles.errorAlert} role="alert">
              {submitError}
            </div>
          )}

          <div className={styles.field}>
            <label htmlFor="login-email" className={styles.label}>
              Email
            </label>
            <input
              id="login-email"
              type="email"
              autoComplete="email"
              className={styles.input}
              {...register('email')}
            />
            {errors.email && (
              <span className={styles.fieldError}>{errors.email.message}</span>
            )}
          </div>

          <div className={styles.field}>
            <label htmlFor="login-password" className={styles.label}>
              Password
            </label>
            <input
              id="login-password"
              type="password"
              autoComplete="current-password"
              className={styles.input}
              {...register('password')}
            />
            {errors.password && (
              <span className={styles.fieldError}>
                {errors.password.message}
              </span>
            )}
          </div>

          <div className={styles.remember}>
            <input
              id="login-remember"
              type="checkbox"
              className={styles.checkbox}
              {...register('rememberMe')}
            />
            <label htmlFor="login-remember" className={styles.rememberLabel}>
              Remember me
            </label>
          </div>

          <button
            type="submit"
            className={styles.submit}
            disabled={loginMutation.isPending}
          >
            {loginMutation.isPending ? 'Вход…' : 'Login'}
          </button>
        </form>
      </div>
    </div>
  )
}
