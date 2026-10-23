import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { ProjectsPage } from './pages/ProjectsPage';
import { ProjectDetailPage } from './pages/ProjectDetailPage';
import { TablesPage } from './pages/TablesPage';
import { TableColumnsPage } from './pages/TableColumnsPage';
import { RelationshipsPage } from './pages/RelationshipsPage';

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<ProjectsPage />} />
        <Route path="/projects/:projectId" element={<ProjectDetailPage />} />
        <Route path="/projects/:projectId/tables" element={<TablesPage />} />
        <Route path="/projects/:projectId/relationships" element={<RelationshipsPage />} />
        <Route path="/tables/:tableId/columns" element={<TableColumnsPage />} />
      </Routes>
    </BrowserRouter>
  );
}