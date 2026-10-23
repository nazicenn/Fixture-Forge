import { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { projectsApi } from '../api/projects';
import type { Project } from '../types/project';

export function ProjectDetailPage() {
  const { projectId } = useParams<{ projectId: string }>();
  const navigate = useNavigate();
  const [project, setProject] = useState<Project | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!projectId) return;
    (async () => {
      try {
        setProject(await projectsApi.getById(projectId));
      } catch {
        setError('Proje yüklenemedi.');
      } finally {
        setLoading(false);
      }
    })();
  }, [projectId]);

  if (loading) return <p style={{ padding: 24 }}>Yükleniyor...</p>;
  if (error) return <p style={{ padding: 24, color: 'red' }}>{error}</p>;
  if (!project) return <p style={{ padding: 24 }}>Proje bulunamadı.</p>;

  return (
    <div style={{ maxWidth: 900, margin: '0 auto', padding: 24, fontFamily: 'system-ui, sans-serif' }}>
      <button onClick={() => navigate('/')}>← Projelere dön</button>

      <h1>{project.name}</h1>
      <p style={{ color: '#666' }}>{project.description ?? 'Açıklama yok'}</p>

      <div style={{ display: 'flex', gap: 16, margin: '24px 0', color: '#555' }}>
        <div><strong>SchemaVersion:</strong> {project.schemaVersion}</div>
        <div><strong>Oluşturma:</strong> {new Date(project.createdAt).toLocaleString('tr-TR')}</div>
        <div><strong>Güncelleme:</strong> {new Date(project.updatedAt).toLocaleString('tr-TR')}</div>
      </div>

      <h2>Modüller</h2>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: 12 }}>
        <Link to={`/projects/${project.id}/tables`} style={cardStyle}>
          <strong>Tablolar</strong>
          <span style={{ color: '#666' }}>Tablo ve sütun tanımları</span>
        </Link>

        <Link to={`/projects/${project.id}/relationships`} style={cardStyle}>
          <strong>İlişkiler</strong>
          <span style={{ color: '#666' }}>Tablo ilişkileri (FK)</span>
        </Link>

        <div style={{ ...cardStyle, opacity: 0.5, cursor: 'not-allowed' }}>
          <strong>Veri Üretimi</strong>
          <span style={{ color: '#666' }}>Yakında (3. Hafta)</span>
        </div>

        <div style={{ ...cardStyle, opacity: 0.5, cursor: 'not-allowed' }}>
          <strong>Performans</strong>
          <span style={{ color: '#666' }}>Yakında (6. Hafta)</span>
        </div>
      </div>
    </div>
  );
}

const cardStyle: React.CSSProperties = {
  display: 'flex',
  flexDirection: 'column',
  gap: 4,
  padding: 16,
  border: '1px solid #ddd',
  borderRadius: 8,
  textDecoration: 'none',
  color: '#000',
};