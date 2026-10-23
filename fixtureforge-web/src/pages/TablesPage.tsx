import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { tablesApi } from '../api/schema';
import type { SchemaTable, CreateTableRequest } from '../types/schema';

export function TablesPage() {
  const { projectId } = useParams<{ projectId: string }>();
  const navigate = useNavigate();
  const [tables, setTables] = useState<SchemaTable[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState<CreateTableRequest>({ name: '', description: '' });
  const [submitting, setSubmitting] = useState(false);

  const load = async () => {
    if (!projectId) return;
    setError(null);
    try {
      setTables(await tablesApi.getAll(projectId));
    } catch {
      setError('Tablolar yüklenemedi.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, [projectId]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!projectId || !form.name.trim()) return;
    setSubmitting(true);
    try {
      await tablesApi.create(projectId, form);
      setForm({ name: '', description: '' });
      await load();
    } catch {
      setError('Tablo oluşturulamadı.');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Bu tabloyu silmek istediğinize emin misiniz?')) return;
    try {
      await tablesApi.delete(id);
      await load();
    } catch {
      setError('Tablo silinemedi.');
    }
  };

  return (
    <div style={{ maxWidth: 900, margin: '0 auto', padding: 24, fontFamily: 'system-ui, sans-serif' }}>
      <button onClick={() => navigate('/')}>← Projelere dön</button>
      <h1>Tablolar</h1>

      <form onSubmit={handleSubmit} style={{ marginBottom: 24, padding: 16, border: '1px solid #ddd', borderRadius: 8 }}>
        <h2 style={{ marginTop: 0 }}>Yeni Tablo</h2>
        <div style={{ display: 'flex', gap: 8, flexDirection: 'column' }}>
          <input placeholder="Tablo adı" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required style={{ padding: 8 }} />
          <input placeholder="Açıklama" value={form.description ?? ''} onChange={(e) => setForm({ ...form, description: e.target.value })} style={{ padding: 8 }} />
          <button type="submit" disabled={submitting}>{submitting ? 'Kaydediliyor...' : 'Oluştur'}</button>
        </div>
      </form>

      {error && <div style={{ color: 'red', marginBottom: 16 }}>{error}</div>}

      {loading ? <p>Yükleniyor...</p> : tables.length === 0 ? <p>Henüz tablo yok.</p> : (
        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ borderBottom: '1px solid #ccc', textAlign: 'left' }}>
              <th style={{ padding: 8 }}>Ad</th>
              <th style={{ padding: 8 }}>Açıklama</th>
              <th style={{ padding: 8 }}></th>
            </tr>
          </thead>
          <tbody>
            {tables.map((t) => (
              <tr key={t.id} style={{ borderBottom: '1px solid #eee' }}>
                <td style={{ padding: 8 }}>{t.name}</td>
                <td style={{ padding: 8 }}>{t.description ?? '-'}</td>
                <td style={{ padding: 8 }}>
                  <button onClick={() => navigate(`/tables/${t.id}/columns`)} style={{ marginRight: 8 }}>Sütunlar</button>
                  <button onClick={() => handleDelete(t.id)} style={{ color: 'red' }}>Sil</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}