import axios from 'axios';
import type { QueryExecutionResult, QueryHistoryDto, ExecuteQueryRequest } from '../types/query';

const api = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
});

export const queriesApi = {
  execute: async (request: ExecuteQueryRequest): Promise<QueryExecutionResult> => {
    const { data } = await api.post<QueryExecutionResult>('/queries/execute', request);
    return data;
  },
  getHistory: async (connectionId: string): Promise<QueryHistoryDto[]> => {
    const { data } = await api.get<QueryHistoryDto[]>(`/connections/${connectionId}/queries`);
    return data;
  },
};