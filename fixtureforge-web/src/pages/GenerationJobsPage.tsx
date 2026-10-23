import { useCallback, useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { generationApi } from '../api/generation';
import { tablesApi } from '../api/schema';
import type { GenerationJob, CreateGenerationJobRequest, GenerationJobTableTarget } from '../types/generation';
import type { SchemaTable } from '../types/schema';

export function GenerationJobsPage() {
  const { projectId } = useParams<{ projectId: string }>();
  const navigate = useNavigate();
  const [jobs, setJobs] = useState<GenerationJob[]>([]);
  const [tables, setTables] = useState<SchemaTable[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const [randomSeed, setRandomSeed] = useState<number>(Math.floor(Math.random() * 100000));
  const [targets, setTargets] = useState<Record<string, number>>({});

  const load = useCallback(async () => {
    if (!projectId) return;
    try {
      const [jobsData, tablesData] = await Promise.all([
        generationApi.getAll(projectId),
        tablesApi.getAll(projectId),
      ]);
      setJobs(jobsData);
      setTables(tablesData);
    } catch {
      setError('Veriler yüklenemedi.');
    } finally {
      setLoading(false);
    }
  }, [projectId]);

  useEffect(() => {
    load();
  }, [load]);

  const handleTargetChange = (tableId: string, value: string) => {
    const num = value === '' ? 0 : parseInt(value, 10);
    setTargets({ ...targets, [tableId]: isNaN(num) ? 0 : num });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!projectId) return;

    const tableTargets: GenerationJobTableTarget[] = Object.entries(targets)
      .filter(([, count]) => count > 0)
      .map(([schemaTableId, targetRowCount]) => ({ schemaTableId, targetRowCount }));

    if (tableTargets.length === 0) {
      setError('En az bir tablo için hedef kayıt sayısı girin.');
      return;
    }

    setError(null);
    setSubmitting(true);
    try {
      const request: CreateGenerationJobRequest = { randomSeed, tables: tableTargets };
      const created = await generationApi.create(projectId, request);
      navigate(`/generation-jobs/${created.id}`);
    } catch (err: any) {
      const data = err?.response?.data;
      const msg = data?.error
        ?? (Array.isArray(data) ? data.map((e: any) => e.errorMessage).join('; ') : null)
        ?? `İş oluşturulamadı (${err?.response?.status ?? '?'})`;
      setError(msg);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div style={{ maxWidth: 1000, margin: '0 auto', padding: 24, fontFamily: 'system-ui, sans-serif' }}>
      <button onClick={() => navigate(`/projects/${projectId}`)}>← Projeye dön</button>
      <h1>Veri Üretimi</h1>

      <form onSubmit={handleSubmit} style={{ marginBottom: 24, padding: 16, border: '1px solid #ddd', borderRadius: 8 }}>
        <h2 style={{ marginTop: 0 }}>Yeni Üretim İşi</h2>

        <div style={{ marginBottom: 12 }}>
          <label>
            <strong>Random Seed:</strong>{' '}
            <input
              type="number"
              value={randomSeed}
              onChange={(e) => setRandomSeed(parseInt(e.target.value, 10) || 0)}
              style={{ padding: 4, width: 120 }}
            />
          </label>
          <button
            type="button"
            onClick={() => setRandomSeed(Math.floor(Math.random() * 100000))}
            style={{ marginLeft: 8 }}
          >
            🎲 Rastgele
          </button>
        </div>

        <h3>Tablo Hedefleri</h3>
        {tables.length === 0 ? (
          <p>Henüz tablo yok. Önce tablo oluşturun.</p>
        ) : (
          <table style={{ width: '100%', borderCollapse: 'collapse', marginBottom: 12 }}>
            <thead>
              <tr style={{ borderBottom: '1px solid #ccc', textAlign: 'left' }}>
                <th style={{ padding: 8 }}>Tablo</th>
                <th style={{ padding: 8 }}>Hedef Kayıt Sayısı</th>
              </tr>
            </thead>
            <tbody>
              {tables.map((t) => (
                <tr key={t.id} style={{ borderBottom: '1px solid #eee' }}>
                  <td style={{ padding: 8 }}>{t.name}</td>
                  <td style={{ padding: 8 }}>
                    <input
                      type="number"
                      min={0}
                      value={targets[t.id] ?? ''}
                      onChange={(e) => handleTargetChange(t.id, e.target.value)}
                      placeholder="0"
                      style={{ padding: 4, width: 120 }}
                    />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}

        <button type="submit" disabled={submitting || tables.length === 0}>
          {submitting ? 'Oluşturuluyor...' : 'İşi Oluştur'}
        </button>
      </form>

      {error && <div style={{ color: 'red', marginBottom: 16 }}>{error}</div>}

      <h2>Geçmiş İşler</h2>
      {loading ? <p>Yükleniyor...</p> : jobs.length === 0 ? <p>Henüz üretim işi yok.</p> : (
        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ borderBottom: '1px solid #ccc', textAlign: 'left' }}>
              <th style={{ padding: 8 }}>Oluşturma</th>
              <th style={{ padding: 8 }}>Durum</th>
              <th style={{ padding: 8 }}>Seed</th>
              <th style={{ padding: 8 }}>Kayıt</th>
              <th style={{ padding: 8 }}>Şema</th>
              <th style={{ padding: 8 }}></th>
            </tr>
          </thead>
          <tbody>
            {jobs.map((j) => (
              <tr key={j.id} style={{ borderBottom: '1px solid #eee' }}>
                <td style={{ padding: 8 }}>{new Date(j.createdAt).toLocaleString('tr-TR')}</td>
                <td style={{ padding: 8 }}>{j.status}</td>
                <td style={{ padding: 8 }}>{j.randomSeed}</td>
                <td style={{ padding: 8 }}>{j.generatedRowCount}</td>
                <td style={{ padding: 8 }}>{j.schemaVersionTag}</td>
                <td style={{ padding: 8 }}>
                  <button onClick={() => navigate(`/generation-jobs/${j.id}`)}>Detay</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}