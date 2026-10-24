import axios from 'axios';
import type {
  DatabaseConnection,
  CreateConnectionRequest,
  UpdateConnectionRequest,
  ConnectionTestResult,
} from '../types/connection';

const api = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
});

export const connectionsApi = {
  getAll: async (projectId: string): Promise<DatabaseConnection[]> => {
    const { data } = await api.get<DatabaseConnection[]>(`/projects/${projectId}/connections`);
    return data;
  },
  getById: async (id: string): Promise<DatabaseConnection> => {
    const { data } = await api.get<DatabaseConnection>(`/connections/${id}`);
    return data;
  },
  create: async (projectId: string, request: CreateConnectionRequest): Promise<DatabaseConnection> => {
    const { data } = await api.post<DatabaseConnection>(`/projects/${projectId}/connections`, request);
    return data;
  },
  update: async (id: string, request: UpdateConnectionRequest): Promise<DatabaseConnection> => {
    const { data } = await api.put<DatabaseConnection>(`/connections/${id}`, request);
    return data;
  },
  delete: async (id: string): Promise<void> => {
    await api.delete(`/connections/${id}`);
  },
  test: async (id: string): Promise<ConnectionTestResult> => {
    const { data } = await api.post<ConnectionTestResult>(`/connections/${id}/test`);
    return data;
  },
};