import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { projectsApi } from '../api/projects';
import type { Project, CreateProjectRequest } from '../types/project';

export function ProjectsPage() {
  const navigate = useNavigate();
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [form, setForm] = useState<CreateProjectRequest>({ name: '', description: '' });
  const [submitting, setSubmitting] = useState(false);

  const loadProjects = async () => {
    setError(null);
    try {
      const data = await projectsApi.getAll();
      setProjects(data);
    } catch {
      setError('Projeler yüklenemedi.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadProjects();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.name.trim()) return;

    setSubmitting(true);
    try {
      await projectsApi.create(form);
      setForm({ name: '', description: '' });
      await loadProjects();
    } catch {
      setError('Proje oluşturulamadı.');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Bu projeyi silmek istediğinize emin misiniz?')) return;
    try {
      await projectsApi.delete(id);
      await loadProjects();
    } catch {
      setError('Proje silinemedi.');
    }
  };

  return (
    <div style={{ maxWidth: 900, margin: '0 auto', padding: 24, fontFamily: 'system-ui, sans-serif' }}>
      <h1>FixtureForge — Projeler</h1>

      <form onSubmit={handleSubmit} style={{ marginBottom: 24, padding: 16, border: '1px solid #ddd', borderRadius: 8 }}>
        <h2 style={{ marginTop: 0 }}>Yeni Proje</h2>
        <div style={{ display: 'flex', gap: 8, flexDirection: 'column' }}>
          <input
            placeholder="Proje adı"
            value={form.name}
            onChange={(e) => setForm({ ...form, name: e.target.value })}
            required
            style={{ padding: 8 }}
          />
          <input
            placeholder="Açıklama (opsiyonel)"
            value={form.description ?? ''}
            onChange={(e) => setForm({ ...form, description: e.target.value })}
            style={{ padding: 8 }}
          />
          <button type="submit" disabled={submitting} style={{ padding: 8 }}>
            {submitting ? 'Kaydediliyor...' : 'Oluştur'}
          </button>
        </div>
      </form>

      {error && <div style={{ color: 'red', marginBottom: 16 }}>{error}</div>}

      {loading ? (
        <p>Yükleniyor...</p>
      ) : projects.length === 0 ? (
        <p>Henüz proje yok. Yukarıdan bir tane oluşturun.</p>
      ) : (
        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ borderBottom: '1px solid #ccc', textAlign: 'left' }}>
              <th style={{ padding: 8 }}>Ad</th>
              <th style={{ padding: 8 }}>Açıklama</th>
              <th style={{ padding: 8 }}>Oluşturma</th>
              <th style={{ padding: 8 }}></th>
            </tr>
          </thead>
          <tbody>
            {projects.map((p) => (
              <tr key={p.id} style={{ borderBottom: '1px solid #eee' }}>
                <td style={{ padding: 8 }}>{p.name}</td>
                <td style={{ padding: 8 }}>{p.description ?? '-'}</td>
                <td style={{ padding: 8 }}>{new Date(p.createdAt).toLocaleString('tr-TR')}</td>
                <td style={{ padding: 8 }}>
                   <button onClick={() => navigate(`/projects/${p.id}`)} style={{ marginRight: 8 }}>
                      Detay
                        </button>
                    <button onClick={() => handleDelete(p.id)} style={{ color: 'red' }}>
                      Sil
                      </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}