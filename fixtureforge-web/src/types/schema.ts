export interface SchemaTable {
  id: string;
  projectId: string;
  name: string;
  description: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTableRequest {
  name: string;
  description?: string | null;
}

export interface SchemaColumn {
  id: string;
  schemaTableId: string;
  name: string;
  dataType: string;
  isNullable: boolean;
  isPrimaryKey: boolean;
  isUnique: boolean;
  defaultValue: string | null;
  maxLength: number | null;
  precision: number | null;
  scale: number | null;
  ordinalPosition: number;
}

export interface CreateColumnRequest {
  name: string;
  dataType: string;
  isNullable: boolean;
  isPrimaryKey: boolean;
  isUnique: boolean;
  defaultValue?: string | null;
  maxLength?: number | null;
  precision?: number | null;
  scale?: number | null;
  ordinalPosition: number;
}

export interface Relationship {
  id: string;
  projectId: string;
  sourceTableId: string;
  sourceTableName: string;
  sourceColumnId: string;
  sourceColumnName: string;
  targetTableId: string;
  targetTableName: string;
  targetColumnId: string;
  targetColumnName: string;
  relationshipType: string;
  createdAt: string;
}

export interface CreateRelationshipRequest {
  sourceTableId: string;
  sourceColumnId: string;
  targetTableId: string;
  targetColumnId: string;
  relationshipType: string;
}