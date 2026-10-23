export interface Project {
  id: string;
  name: string;
  description: string | null;
  schemaVersion: number;  
  createdAt: string;
  updatedAt: string;
}
export interface CreateProjectRequest {
  name: string;
  description?: string | null;
}

export interface UpdateProjectRequest {
  name: string;
  description?: string | null;
}