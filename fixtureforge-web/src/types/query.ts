export interface QueryExecutionResult {
  id: string;
  queryType: string;
  queryFingerprint: string;
  fingerprintPolicyVersion: string;
  maskedQueryText: string;
  columnNames: string[];
  rows: (string | null)[][];
  rowCount: number;
  rowsAffected: number;
  durationMs: number;
  status: string;
  errorMessage: string | null;
}

export interface QueryHistoryDto {
  id: string;
  connectionId: string;
  queryType: string;
  queryFingerprint: string;
  queryText: string;
  durationMs: number;
  rowCount: number;
  rowsAffected: number;
  status: string;
  startedAt: string;
  completedAt: string | null;
}

export interface ExecuteQueryRequest {
  connectionId: string;
  queryText: string;
  timeoutSeconds?: number | null;
}