export interface GenerationJob {
  id: string;
  projectId: string;
  status: 'Pending' | 'Running' | 'Succeeded' | 'Failed' | 'Cancelled';
  startedAt: string | null;
  completedAt: string | null;
  generatedRowCount: number;
  errorSummary: string | null;
  randomSeed: number;
  schemaVersionTag: string;
  ruleVersionTag: string;
  generatorVersionTag: string;
  createdAt: string;
}

export interface GenerationJobTable {
  id: string;
  generationJobId: string;
  schemaTableId: string;
  schemaTableName: string;
  targetRowCount: number;
  generatedRowCount: number;
  status: 'Pending' | 'Succeeded' | 'Failed' | 'Skipped';
  errorSummary: string | null;
}

export interface CreateGenerationJobRequest {
  randomSeed: number;
  tables: GenerationJobTableTarget[];
}

export interface GenerationJobTableTarget {
  schemaTableId: string;
  targetRowCount: number;
}