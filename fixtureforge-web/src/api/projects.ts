import axios from 'axios';
import type { Project, CreateProjectRequest, UpdateProjectRequest } from '../types/project';

const api = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
});

export const projectsApi = {
  getAll: async (): Promise<Project[]> => {
    const { data } = await api.get<Project[]>('/projects');
    return data;
  },

  getById: async (id: string): Promise<Project> => {
    const { data } = await api.get<Project>(`/projects/${id}`);
    return data;
  },

  create: async (request: CreateProjectRequest): Promise<Project> => {
    const { data } = await api.post<Project>('/projects', request);
    return data;
  },

  update: async (id: string, request: UpdateProjectRequest): Promise<Project> => {
    const { data } = await api.put<Project>(`/projects/${id}`, request);
    return data;
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/projects/${id}`);
  },
};