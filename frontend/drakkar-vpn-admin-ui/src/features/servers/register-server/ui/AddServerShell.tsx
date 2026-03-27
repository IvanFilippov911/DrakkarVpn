import { useMemo } from 'react'
import { useForm, type UseFormRegisterReturn } from 'react-hook-form'
import { z } from 'zod'
import { isAxiosError } from 'axios'
import { InlineAlert, Overlay } from '../../../../shared/ui'
import { useRegisterServerMutation } from '../../useRegisterServerMutation'
import type { RegisterServerApiRequest } from '../types'

type FormValues = {
  name: string
  region: string
  publicHost: string
  publicPort: string
  realityPublicKey: string
  realityShortId: string
  realitySni: string
  agentBaseUrl: string
  agentTokenEncrypted: string
  maxPeers: string
}

const schema = z.object({
  name: z.string().trim().min(1, 'Name is required'),
  region: z.string().trim().min(1, 'Region is required'),
  publicHost: z.string().trim().min(1, 'Public host is required'),
  publicPort: z
    .string()
    .trim()
    .min(1, 'Public port is required')
    .refine((v) => /^\d+$/.test(v), 'Public port must be a number')
    .transform((v) => Number(v))
    .refine((v) => Number.isInteger(v) && v >= 1 && v <= 65535, 'Public port must be 1..65535'),
  realityPublicKey: z.string().trim().min(1, 'Reality public key is required'),
  realityShortId: z.string().trim().min(1, 'Reality short id is required'),
  realitySni: z.string().trim().min(1, 'Reality SNI is required'),
  agentBaseUrl: z.string().trim().url('Agent base URL must be a valid URL'),
  agentTokenEncrypted: z.string().trim().min(1, 'Agent token is required'),
  maxPeers: z
    .string()
    .trim()
    .optional()
    .transform((v) => v ?? '')
    .refine(
      (v) => v === '' || (/^\d+$/.test(v) && Number(v) > 0),
      'Max peers must be a positive integer',
    ),
})

export function AddServerShell({
  open,
  onClose,
}: {
  open: boolean
  onClose: () => void
}) {
  const registerMutation = useRegisterServerMutation()
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
      publicHost: '',
      publicPort: '',
      realityPublicKey: '',
      realityShortId: '',
      realitySni: '',
      agentBaseUrl: '',
      agentTokenEncrypted: '',
      maxPeers: '',
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

    return err instanceof Error ? err.message : 'Failed to register server.'
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
    const payload: RegisterServerApiRequest = {
      name: v.name.trim(),
      region: v.region.trim(),
      publicHost: v.publicHost.trim(),
      publicPort: v.publicPort,
      realityPublicKey: v.realityPublicKey.trim(),
      realityShortId: v.realityShortId.trim(),
      realitySni: v.realitySni.trim(),
      agentBaseUrl: v.agentBaseUrl.trim(),
      agentTokenEncrypted: v.agentTokenEncrypted.trim(),
      maxPeers: v.maxPeers === '' ? null : Number(v.maxPeers),
    }

    await registerMutation.mutateAsync(payload)

    reset()
    onClose()
  }

  return (
    <Overlay open={open} onClose={onClose}>
      <div className="p-4 border-b flex items-center justify-between">
        <div>
          <div className="text-lg font-semibold">Add Server</div>
          <div className="text-sm text-muted-foreground mt-0.5">
            Register a new VPN node.
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
            <InlineAlert title="Failed to register server" message={submitErrorMessage} />
          ) : null}

          <FormField
            label="Name"
            placeholder="e.g. Drakkar FRA-1"
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
              label="Max peers (optional)"
              placeholder="e.g. 120"
              error={errors.maxPeers?.message}
              inputProps={register('maxPeers')}
            />
          </div>

          <FormField
            label="Public host"
            placeholder="e.g. fra1.drakkar.net"
            error={errors.publicHost?.message}
            inputProps={register('publicHost')}
          />

          <FormField
            label="Public port"
            placeholder="e.g. 443"
            error={errors.publicPort?.message}
            inputProps={register('publicPort')}
          />

          <div className="grid grid-cols-2 gap-3">
            <FormField
              label="Reality public key"
              placeholder="e.g. base64..."
              error={errors.realityPublicKey?.message}
              inputProps={register('realityPublicKey')}
            />
            <FormField
              label="Reality short id"
              placeholder="e.g. 1a2b3c"
              error={errors.realityShortId?.message}
              inputProps={register('realityShortId')}
            />
          </div>

          <FormField
            label="Reality SNI"
            placeholder="e.g. www.cloudflare.com"
            error={errors.realitySni?.message}
            inputProps={register('realitySni')}
          />

          <FormField
            label="Agent base URL"
            placeholder="e.g. https://fra1-agent.drakkar.net"
            error={errors.agentBaseUrl?.message}
            inputProps={register('agentBaseUrl')}
          />

          <FormField
            label="Agent token (encrypted)"
            placeholder="Encrypted token"
            error={errors.agentTokenEncrypted?.message}
            inputProps={register('agentTokenEncrypted')}
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

