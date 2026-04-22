import { useMemo } from 'react'
import { useForm, type UseFormRegisterReturn } from 'react-hook-form'
import { z } from 'zod'
import { isAxiosError } from 'axios'
import { InlineAlert, Overlay } from '../../../../shared/ui'
import { useRegisterProbeNodeMutation } from '../../useRegisterProbeNodeMutation'
import type { RegisterProbeNodeApiRequest } from '../api'

type FormValues = {
  name: string
  region: string
  host: string
}

const schema = z.object({
  name: z.string().trim().min(1, 'Name is required'),
  region: z.string().trim().min(1, 'Region is required'),
  host: z.string().trim().min(1, 'Host is required'),
})

export function AddProbeNodeShell({
  open,
  onClose,
}: {
  open: boolean
  onClose: () => void
}) {
  const registerMutation = useRegisterProbeNodeMutation()
  const {
    register,
    handleSubmit,
    reset,
    setError,
    formState: { errors },
  } = useForm<FormValues>({
    defaultValues: {
      name: '',
      region: '',
      host: '',
    },
  })

  const submitErrorMessage = useMemo(() => {
    const err = registerMutation.error
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

    return err instanceof Error ? err.message : 'Failed to register probe node.'
  }, [registerMutation.error])

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

    const v = parsed.data
    const payload: RegisterProbeNodeApiRequest = {
      name: v.name.trim(),
      region: v.region.trim(),
      host: v.host.trim(),
    }

    await registerMutation.mutateAsync(payload)

    reset()
    onClose()
  }

  return (
    <Overlay open={open} onClose={onClose}>
      <div className="p-4 border-b flex items-center justify-between">
        <div>
          <div className="text-lg font-semibold">Register probe</div>
          <div className="text-sm text-muted-foreground mt-0.5">
            Add a new network monitoring probe node.
          </div>
        </div>
        <button
          type="button"
          className="h-8 w-8 inline-flex items-center justify-center rounded-md hover:bg-accent"
          onClick={onClose}
          disabled={registerMutation.isPending}
        >
          ×
        </button>
      </div>

      <form onSubmit={handleSubmit(onSubmit)}>
        <div className="p-4 space-y-4">
          {submitErrorMessage ? (
            <InlineAlert title="Failed to register probe" message={submitErrorMessage} />
          ) : null}

          <FormField
            label="Name"
            placeholder="e.g. FRA probe #1"
            error={errors.name?.message}
            inputProps={register('name')}
          />

          <div className="grid grid-cols-2 gap-3">
            <FormField
              label="Region"
              placeholder="e.g. eu-central"
              error={errors.region?.message}
              inputProps={register('region')}
            />
            <FormField
              label="Host"
              placeholder="e.g. probe-fra1.internal"
              error={errors.host?.message}
              inputProps={register('host')}
            />
          </div>
        </div>

        <div className="p-4 border-t flex items-center justify-end gap-2">
          <button
            type="button"
            className="h-8 px-3 rounded-md text-sm border bg-background hover:bg-accent disabled:opacity-50"
            onClick={() => {
              reset()
              onClose()
            }}
            disabled={registerMutation.isPending}
          >
            Cancel
          </button>
          <button
            type="submit"
            className="h-8 px-3 rounded-md text-sm bg-primary text-primary-foreground hover:bg-primary/90 disabled:opacity-50"
            disabled={registerMutation.isPending}
          >
            {registerMutation.isPending ? 'Saving…' : 'Save'}
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
}: {
  label: string
  placeholder: string
  error?: string
  inputProps: UseFormRegisterReturn
}) {
  return (
    <div>
      <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
        {label}
      </div>
      <input
        {...inputProps}
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

