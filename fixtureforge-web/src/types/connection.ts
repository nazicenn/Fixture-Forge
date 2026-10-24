export interface DatabaseConnection {
  id: string;
  projectId: string;
  name: string;
  provider: string;
  serverReference: string;
  databaseName: string;
  authenticationType: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateConnectionRequest {
  name: string;
  serverReference: string;
  databaseName: string;
  username?: string | null;
  password?: string | null;
  provider?: string;
  authenticationType?: string;
}

export interface UpdateConnectionRequest {
  name: string;
  serverReference: string;
  databaseName: string;
  username?: string | null;
  password?: string | null;
}

export interface ConnectionTestResult {
  success: boolean;
  durationMs: number;
  error: string | null;
}