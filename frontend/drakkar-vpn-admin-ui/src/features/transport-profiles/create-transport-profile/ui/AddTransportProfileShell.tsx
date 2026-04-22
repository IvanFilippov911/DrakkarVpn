import { useMemo } from 'react'
import { isAxiosError } from 'axios'
import { useForm, type UseFormRegisterReturn } from 'react-hook-form'
import { z } from 'zod'
import { InlineAlert, Overlay } from '../../../../shared/ui'
import type { CreateTransportProfileApiRequest, TransportType } from '../../../../entities/transport-profiles'
import { useCreateTransportProfileMutation } from '../../useCreateTransportProfileMutation'

const schema = z
  .object({
    name: z.string().trim().min(1, 'Name is required'),
    transportType: z.enum(['Tcp', 'Grpc']),
    realitySni: z.string().trim().min(1, 'Reality SNI is required'),
    realityShortId: z.string().trim().min(1, 'Reality short id is required'),
    realityFingerprint: z.string().trim().min(1, 'Reality fingerprint is required'),
    realityDest: z.string().trim().min(1, 'Reality destination is required'),
    grpcServiceName: z.string().optional(),
    grpcAuthority: z.string().optional(),
    globalPriority: z.coerce.number().int().min(0, 'Priority must be a non-negative integer'),
  })
  .superRefine((value, ctx) => {
    if (value.transportType === 'Grpc') {
      const service = (value.grpcServiceName ?? '').trim()
      if (!service) {
        ctx.addIssue({
          path: ['grpcServiceName'],
          code: z.ZodIssueCode.custom,
          message: 'gRPC service name is required for gRPC transport.',
        })
      }
    }
  })

type FormValues = {
  name: string
  transportType: TransportType
  realitySni: string
  realityShortId: string
  realityFingerprint: string
  realityDest: string
  grpcServiceName: string
  grpcAuthority: string
  globalPriority: string
}

export function AddTransportProfileShell({
  open,
  onClose,
}: {
  open: boolean
  onClose: () => void
}) {
  const createMutation = useCreateTransportProfileMutation()
  const {
    register,
    watch,
    handleSubmit,
    reset,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({
    defaultValues: {
      name: '',
      transportType: 'Tcp',
      realitySni: '',
      realityShortId: '',
      realityFingerprint: 'chrome',
      realityDest: '',
      grpcServiceName: '',
      grpcAuthority: '',
      globalPriority: '0',
    },
  })

  const transportType = watch('transportType')
  const isGrpc = transportType === 'Grpc'

  const submitErrorMessage = useMemo(() => {
    const err = createMutation.error
    if (!err) return null

    if (isAxiosError(err)) {
      const maybeMessage = getMessageFromUnknown(err.response?.data)
      if (typeof maybeMessage === 'string' && maybeMessage.trim().length > 0) {
        return maybeMessage
      }

      return err.response?.status ? `Request failed (${err.response.status}).` : 'Request failed.'
    }

    return err instanceof Error ? err.message : 'Failed to create transport profile.'
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

    const payload: CreateTransportProfileApiRequest = {
      name: parsed.data.name.trim(),
      transportType: parsed.data.transportType,
      securityType: 'Reality',
      realitySni: parsed.data.realitySni.trim(),
      realityShortId: parsed.data.realityShortId.trim(),
      realityFingerprint: parsed.data.realityFingerprint.trim(),
      realityDest: parsed.data.realityDest.trim(),
      grpcServiceName: isGrpc ? (parsed.data.grpcServiceName ?? '').trim() || null : null,
      grpcAuthority: (parsed.data.grpcAuthority ?? '').trim() || null,
      globalPriority: parsed.data.globalPriority,
    }

    try {
      await createMutation.mutateAsync(payload)
    } catch {
      return
    }

    reset()
    onClose()
  }

  return (
    <Overlay open={open} onClose={onClose}>
      <div className="p-4 border-b flex items-center justify-between">
        <div>
          <div className="text-lg font-semibold">Create Transport Profile</div>
          <div className="text-sm text-muted-foreground mt-0.5">
            Define a reusable global transport template.
          </div>
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
          {submitErrorMessage ? (
            <InlineAlert title="Failed to create transport profile" message={submitErrorMessage} />
          ) : null}

          <FormField label="Name" placeholder="e.g. Reality gRPC main" error={errors.name?.message} inputProps={register('name')} />

          <div className="grid grid-cols-2 gap-3">
            <div>
              <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
                Transport type
              </div>
              <select
                {...register('transportType')}
                className="h-9 w-full rounded-md border bg-background px-3 text-sm"
              >
                <option value="Tcp">TCP</option>
                <option value="Grpc">gRPC</option>
              </select>
            </div>
            <div>
              <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
                Security type
              </div>
              <input
                value="Reality"
                disabled
                readOnly
                className="h-9 w-full rounded-md border bg-muted px-3 text-sm text-muted-foreground"
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-3">
            <FormField
              label="Reality SNI"
              placeholder="e.g. maps.yandex.ru"
              error={errors.realitySni?.message}
              inputProps={register('realitySni')}
            />
            <FormField
              label="Reality Short ID"
              placeholder="e.g. abcd1234"
              error={errors.realityShortId?.message}
              inputProps={register('realityShortId')}
            />
          </div>

          <div className="grid grid-cols-2 gap-3">
            <FormField
              label="Reality Fingerprint"
              placeholder="e.g. chrome"
              error={errors.realityFingerprint?.message}
              inputProps={register('realityFingerprint')}
            />
            <FormField
              label="Reality Destination"
              placeholder="e.g. maps.yandex.ru:443"
              error={errors.realityDest?.message}
              inputProps={register('realityDest')}
            />
          </div>

          {isGrpc ? (
            <div className="grid grid-cols-2 gap-3">
              <FormField
                label="gRPC Service Name"
                placeholder="e.g. gun"
                error={errors.grpcServiceName?.message}
                inputProps={register('grpcServiceName')}
              />
              <FormField
                label="gRPC Authority"
                placeholder="e.g. maps.yandex.ru"
                error={errors.grpcAuthority?.message}
                inputProps={register('grpcAuthority')}
              />
            </div>
          ) : null}

          <FormField
            label="Global Priority"
            placeholder="e.g. 10"
            error={errors.globalPriority?.message}
            inputProps={register('globalPriority')}
            type="number"
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
}: {
  label: string
  placeholder: string
  error?: string
  inputProps: UseFormRegisterReturn
  type?: string
}) {
  const props = inputProps as unknown as Record<string, unknown>

  return (
    <div>
      <div className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">{label}</div>
      <input
        {...props}
        type={type}
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
