export const userQueryKeys = {
  homeContext: ['user', 'home-context'] as const,
  summary: ['user', 'summary'] as const,
  tariffs: ['user', 'tariffs'] as const,
  currentConfig: ['user', 'vpn-config', 'current'] as const,
  provisionPoll: (jobId: string) => ['user', 'provision-poll', jobId] as const,
}
