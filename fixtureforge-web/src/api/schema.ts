import axios from 'axios';
import type {
  SchemaTable,
  CreateTableRequest,
  SchemaColumn,
  CreateColumnRequest,
  Relationship,
  CreateRelationshipRequest,
} from '../types/schema';

const api = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
});

export const tablesApi = {
  getAll: async (projectId: string): Promise<SchemaTable[]> => {
    const { data } = await api.get<SchemaTable[]>(`/projects/${projectId}/tables`);
    return data;
  },
  getById: async (id: string): Promise<SchemaTable> => {
    const { data } = await api.get<SchemaTable>(`/tables/${id}`);
    return data;
  },
  create: async (projectId: string, request: CreateTableRequest): Promise<SchemaTable> => {
    const { data } = await api.post<SchemaTable>(`/projects/${projectId}/tables`, request);
    return data;
  },
  delete: async (id: string): Promise<void> => {
    await api.delete(`/tables/${id}`);
  },
};

export const columnsApi = {
  getAll: async (tableId: string): Promise<SchemaColumn[]> => {
    const { data } = await api.get<SchemaColumn[]>(`/tables/${tableId}/columns`);
    return data;
  },
  create: async (tableId: string, request: CreateColumnRequest): Promise<SchemaColumn> => {
    const { data } = await api.post<SchemaColumn>(`/tables/${tableId}/columns`, request);
    return data;
  },
  delete: async (id: string): Promise<void> => {
    await api.delete(`/columns/${id}`);
  },
};

export const relationshipsApi = {
  getAll: async (projectId: string): Promise<Relationship[]> => {
    const { data } = await api.get<Relationship[]>(`/projects/${projectId}/relationships`);
    return data;
  },
  create: async (projectId: string, request: CreateRelationshipRequest): Promise<Relationship> => {
    const { data } = await api.post<Relationship>(`/projects/${projectId}/relationships`, request);
    return data;
  },
  delete: async (id: string): Promise<void> => {
    await api.delete(`/relationships/${id}`);
  },
};