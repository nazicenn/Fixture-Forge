import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { ProjectsPage } from './pages/ProjectsPage';
import { ProjectDetailPage } from './pages/ProjectDetailPage';
import { TablesPage } from './pages/TablesPage';
import { TableColumnsPage } from './pages/TableColumnsPage';
import { RelationshipsPage } from './pages/RelationshipsPage';
import { GenerationJobsPage } from './pages/GenerationJobsPage';
import { GenerationJobDetailPage } from './pages/GenerationJobDetailPage';
import { ConnectionsPage } from './pages/ConnectionsPage';
import { QueryWorkspacePage } from './pages/QueryWorkspacePage';

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<ProjectsPage />} />
        <Route path="/projects/:projectId" element={<ProjectDetailPage />} />
        <Route path="/projects/:projectId/tables" element={<TablesPage />} />
        <Route path="/projects/:projectId/relationships" element={<RelationshipsPage />} />
        <Route path="/projects/:projectId/generation-jobs" element={<GenerationJobsPage />} />
        <Route path="/projects/:projectId/connections" element={<ConnectionsPage />} />
        <Route path="/projects/:projectId/query" element={<QueryWorkspacePage />} />
        <Route path="/tables/:tableId/columns" element={<TableColumnsPage />} />
        <Route path="/generation-jobs/:jobId" element={<GenerationJobDetailPage />} />
      </Routes>
    </BrowserRouter>
  );
}