export type RegisterServerApiRequest = {
  name: string
  region: string
  publicHost: string
  publicPort: number
  agentBaseUrl: string
  agentTokenEncrypted: string
  maxPeers: number | null
}

export type RegisterServerApiResponse = {
  id: string
}

