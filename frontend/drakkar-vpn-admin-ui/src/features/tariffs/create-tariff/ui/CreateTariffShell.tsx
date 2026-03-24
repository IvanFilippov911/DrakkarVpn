import { useMemo } from 'react'
import { isAxiosError } from 'axios'
import { useForm, type UseFormRegisterReturn } from 'react-hook-form'
import { z } from 'zod'
import { InlineAlert, Overlay } from '../../../../shared/ui'
import type { CreateTariffApiRequest } from '../../../../entities/tariffs'
import { useCreateTariffMutation } from '../../useCreateTariffMutation'

const schema = z.object({
  name: z.string().trim().min(1, 'Name is required'),
  durationDays: z.coerce.number().int().positive('Duration must be a positive integer'),
  price: z.coerce.number().positive('Price must be a positive number'),
  defaultMaxDevices: z.coerce.number().int().positive('Devices must be a positive integer'),
})

type FormValues = {
  name: string
  durationDays: string
  price: string
  defaultMaxDevices: string
}

export function CreateTariffShell({
  open,
  onClose,
}: {
  open: boolean
  onClose: () => void
}) {
  const createMutation = useCreateTariffMutation()
  const {
    register,
    handleSubmit,
    reset,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({
    defaultValues: {
      name: '',
      durationDays: '30',
      price: '9.99',
      defaultMaxDevices: '5',
    },
  })

  const submitErrorMessage = useMemo(() => {
    const err = createMutation.error
    if (!err) return null

    if (isAxiosError(err)) {
      const maybeMessage = getMessageFromUnknown(err.response?.data)
      if (typeof maybeMessage === 'string' && maybeMessage.trim().length > 0) {
        return maybeMessage
      }

      return err.response?.status
        ? `Request failed (${err.response.status}).`
        : 'Request failed.'
    }

    return err instanceof Error ? err.message : 'Failed to create tariff.'
  }, [createMutation.error])

  async function onSubmit(values: FormValues) {
    const parsed = schema.safeParse(values)
    if (!parsed.success) {
      for (const issue of parsed.error.issues) {
        const field = issue.path[0]
        if (typeof field === 'string') {
          setError(field as keyof FormValues, { message: issue.message })
        }
      }
      return
    }

    const payload: CreateTariffApiRequest = parsed.data
    try {
      await createMutation.mutateAsync(payload)
    } catch {
      // Error is displayed via `submitErrorMessage`.
      return
    }

    reset()
    onClose()
  }

  return (
    <Overlay open={open} onClose={onClose}>
      <div className="p-4 border-b flex items-center justify-between">
        <div>
          <div className="text-lg font-semibold">Create Tariff</div>
          <div className="text-sm text-muted-foreground mt-0.5">Define pricing and subscription duration.</div>
        </div>
        <button
          type="button"
          className="h-8 w-8 inline-flex items-center justify-center rounded-md hover:bg-accent"
          onClick={onClose}
          disabled={isSubmitting}
        >
          ×
        </button>
      </div>

      <form onSubmit={handleSubmit(onSubmit)}>
        <div className="p-4 space-y-4">
          {submitErrorMessage ? <InlineAlert title="Failed to create tariff" message={submitErrorMessage} /> : null}

          <FormField
            label="Name"
            placeholder="e.g. Basic"
            error={errors.name?.message}
            inputProps={register('name')}
          />

          <div className="grid grid-cols-2 gap-3">
            <FormField
              label="Duration (days)"
              placeholder="e.g. 30"
              type="number"
              error={errors.durationDays?.message}
              inputProps={register('durationDays')}
            />
            <FormField
              label="Default devices"
              placeholder="e.g. 5"
              type="number"
              error={errors.defaultMaxDevices?.message}
              inputProps={register('defaultMaxDevices')}
            />
          </div>

          <FormField
            label="Price"
            placeholder="e.g. 9.99"
            type="number"
            step="0.01"
            error={errors.price?.message}
            inputProps={register('price')}
          />
        </div>

        <div className="p-4 border-t flex items-center justify-end gap-2">
          <button
            type="button"
            className="h-8 px-3 rounded-md text-sm border bg-background hover:bg-accent disabled:opacity-50"
            onClick={() => {
              reset()
              onClose()
            }}
            disabled={isSubmitting || createMutation.isPending}
          >
            Cancel
          </button>
          <button
            type="submit"
            className="h-8 px-3 rounded-md text-sm bg-primary text-primary-foreground hover:bg-primary/90 disabled:opacity-50"
            disabled={isSubmitting || createMutation.isPending}
          >
            {createMutation.isPending || isSubmitting ? 'Saving…' : 'Save'}
          </button>
        </div>
      </form>
    </Overlay>
  )
}

function FormField({
  label,
  placeholder,
  error,
  inputProps,
  type = 'text',
  step,
}: {
  label: string
  placeholder: string
  error?: string
  inputProps: UseFormRegisterReturn
  type?: string
  step?: string
}) {
  const props = inputProps as unknown as Record<string, unknown>

  return (
    <div>
      <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">{label}</div>
      <input
        {...props}
        type={type}
        step={step}
        placeholder={placeholder}
        className="h-9 w-full rounded-md border bg-background px-3 text-sm"
      />
      {error ? <div className="text-xs text-destructive mt-1">{error}</div> : null}
    </div>
  )
}

function getMessageFromUnknown(value: unknown): string | undefined {
  if (!value || typeof value !== 'object') return undefined
  if (!('message' in value)) return undefined
  const msg = (value as { message?: unknown }).message
  return typeof msg === 'string' ? msg : undefined
}

