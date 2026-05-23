interface GoogleAccountsId {
  initialize(config: { client_id: string; callback: (response: GoogleCredentialResponse) => void }): void;
  renderButton(element: HTMLElement, config: Record<string, unknown>): void;
  prompt(): void;
}

interface GoogleCredentialResponse {
  credential: string;
  select_by?: string;
}

interface Window {
  google?: {
    accounts?: {
      id?: GoogleAccountsId;
    };
  };
}
