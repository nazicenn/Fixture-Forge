import axios from 'axios';
import type {
  GenerationJob,
  GenerationJobTable,
  CreateGenerationJobRequest,
} from '../types/generation';

const api = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
});

export const generationApi = {
  getAll: async (projectId: string): Promise<GenerationJob[]> => {
    const { data } = await api.get<GenerationJob[]>(`/projects/${projectId}/generation-jobs`);
    return data;
  },
  getById: async (id: string): Promise<GenerationJob> => {
    const { data } = await api.get<GenerationJob>(`/generation-jobs/${id}`);
    return data;
  },
  getTables: async (id: string): Promise<GenerationJobTable[]> => {
    const { data } = await api.get<GenerationJobTable[]>(`/generation-jobs/${id}/tables`);
    return data;
  },
  create: async (projectId: string, request: CreateGenerationJobRequest): Promise<GenerationJob> => {
    const { data } = await api.post<GenerationJob>(`/projects/${projectId}/generation-jobs`, request);
    return data;
  },
  cancel: async (id: string): Promise<void> => {
    await api.post(`/generation-jobs/${id}/cancel`);
  },
  run: async (id: string): Promise<void> => {
    await api.post(`/generation-jobs/${id}/run`);
  },
  downloadSql: async (id: string): Promise<Blob> => {
    const { data } = await api.get(`/generation-jobs/${id}/export?format=sql`, {
      responseType: 'blob',
    });
    return data;
  },
};