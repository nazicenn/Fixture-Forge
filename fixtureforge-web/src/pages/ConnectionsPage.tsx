import { useCallback, useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { connectionsApi } from '../api/connection';
import type { DatabaseConnection, CreateConnectionRequest, ConnectionTestResult } from '../types/connection';

export function ConnectionsPage() {
  const { projectId } = useParams<{ projectId: string }>();
  const navigate = useNavigate();
  const [connections, setConnections] = useState<DatabaseConnection[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [testing, setTesting] = useState<string | null>(null);
  const [testResults, setTestResults] = useState<Record<string, ConnectionTestResult>>({});

  const [form, setForm] = useState<CreateConnectionRequest>({
    name: '',
    serverReference: 'localhost,1433',
    databaseName: '',
    username: 'sa',
    password: '',
    provider: 'SqlServer',
    authenticationType: 'SqlAuth',
  });

  const load = useCallback(async () => {
    if (!projectId) return;
    try {
      setConnections(await connectionsApi.getAll(projectId));
    } catch {
      setError('Bağlantılar yüklenemedi.');
    } finally {
      setLoading(false);
    }
  }, [projectId]);

  useEffect(() => {
    load();
  }, [load]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!projectId || !form.name.trim() || !form.databaseName.trim()) return;

    setError(null);
    setSubmitting(true);
    try {
      await connectionsApi.create(projectId, form);
      setForm({ ...form, name: '', databaseName: '', password: '' });
      await load();
    } catch (err: any) {
      const data = err?.response?.data;
      const msg = data?.error
        ?? (Array.isArray(data) ? data.map((e: any) => e.errorMessage).join('; ') : null)
        ?? `Bağlantı oluşturulamadı (${err?.response?.status ?? '?'})`;
      setError(msg);
    } finally {
      setSubmitting(false);
    }
  };

  const handleTest = async (id: string) => {
    setError(null);
    setTesting(id);
    try {
      const result = await connectionsApi.test(id);
      setTestResults({ ...testResults, [id]: result });
    } catch (err: any) {
      setError(err?.response?.data?.error ?? 'Bağlantı test edilemedi.');
    } finally {
      setTesting(null);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Bu bağlantıyı silmek istediğinize emin misiniz?')) return;
    setError(null);
    try {
      await connectionsApi.delete(id);
      const { [id]: _, ...rest } = testResults;
      setTestResults(rest);
      await load();
    } catch {
      setError('Bağlantı silinemedi.');
    }
  };

  return (
    <div style={{ maxWidth: 1000, margin: '0 auto', padding: 24, fontFamily: 'system-ui, sans-serif' }}>
      <button onClick={() => navigate(`/projects/${projectId}`)}>← Projeye dön</button>
      <h1>SQL Bağlantıları</h1>

      <form onSubmit={handleSubmit} style={{ marginBottom: 24, padding: 16, border: '1px solid #ddd', borderRadius: 8 }}>
        <h2 style={{ marginTop: 0 }}>Yeni Bağlantı</h2>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 8 }}>
          <input placeholder="Ad (örn. Local SQL)" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required style={{ padding: 8 }} />
          <input placeholder="Sunucu (örn. localhost,1433)" value={form.serverReference} onChange={(e) => setForm({ ...form, serverReference: e.target.value })} required style={{ padding: 8 }} />
          <input placeholder="Veritabanı" value={form.databaseName} onChange={(e) => setForm({ ...form, databaseName: e.target.value })} required style={{ padding: 8 }} />
          <input placeholder="Kullanıcı (örn. sa)" value={form.username ?? ''} onChange={(e) => setForm({ ...form, username: e.target.value })} style={{ padding: 8 }} />
          <input type="password" placeholder="Şifre" value={form.password ?? ''} onChange={(e) => setForm({ ...form, password: e.target.value })} style={{ padding: 8 }} />
        </div>
        <button type="submit" disabled={submitting} style={{ marginTop: 8 }}>
          {submitting ? 'Kaydediliyor...' : 'Bağlantı Ekle'}
        </button>
      </form>

      {error && <div style={{ color: 'red', marginBottom: 16 }}>{error}</div>}

      {loading ? <p>Yükleniyor...</p> : connections.length === 0 ? <p>Henüz bağlantı yok.</p> : (
        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ borderBottom: '1px solid #ccc', textAlign: 'left' }}>
              <th style={{ padding: 8 }}>Ad</th>
              <th style={{ padding: 8 }}>Sunucu</th>
              <th style={{ padding: 8 }}>Veritabanı</th>
              <th style={{ padding: 8 }}>Test</th>
              <th style={{ padding: 8 }}></th>
            </tr>
          </thead>
          <tbody>
            {connections.map((c) => {
              const result = testResults[c.id];
              return (
                <tr key={c.id} style={{ borderBottom: '1px solid #eee' }}>
                  <td style={{ padding: 8 }}>{c.name}</td>
                  <td style={{ padding: 8 }}>{c.serverReference}</td>
                  <td style={{ padding: 8 }}>{c.databaseName}</td>
                  <td style={{ padding: 8 }}>
                    {result ? (
                      <span style={{ color: result.success ? 'green' : 'red' }}>
                        {result.success ? `✓ ${result.durationMs}ms` : `✗ ${result.error?.slice(0, 50)}`}
                      </span>
                    ) : (
                      <button onClick={() => handleTest(c.id)} disabled={testing === c.id}>
                        {testing === c.id ? 'Test ediliyor...' : 'Test Et'}
                      </button>
                    )}
                  </td>
                  <td style={{ padding: 8 }}>
                    <button onClick={() => handleDelete(c.id)} style={{ color: 'red' }}>Sil</button>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      )}
    </div>
  );
}