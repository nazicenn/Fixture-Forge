import { useCallback, useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { generationApi } from '../api/generation';
import type { GenerationJob, GenerationJobTable } from '../types/generation';

export function GenerationJobDetailPage() {
  const { jobId } = useParams<{ jobId: string }>();
  const navigate = useNavigate();
  const [job, setJob] = useState<GenerationJob | null>(null);
  const [tables, setTables] = useState<GenerationJobTable[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  const load = useCallback(async () => {
    if (!jobId) return;
    try {
      const [jobData, tablesData] = await Promise.all([
        generationApi.getById(jobId),
        generationApi.getTables(jobId),
      ]);
      setJob(jobData);
      setTables(tablesData);
    } catch {
      setError('İş yüklenemedi.');
    } finally {
      setLoading(false);
    }
  }, [jobId]);

  useEffect(() => {
    load();
  }, [load]);

  const handleCancel = async () => {
    if (!jobId || !confirm('İşi iptal etmek istediğinize emin misiniz?')) return;
    setError(null);
    setBusy(true);
    try {
      await generationApi.cancel(jobId);
      await load();
    } catch (err: any) {
      setError(err?.response?.data?.error ?? 'İş iptal edilemedi.');
    } finally {
      setBusy(false);
    }
  };

  const handleRun = async () => {
    if (!jobId || !confirm('İşi şimdi çalıştırmak istediğinize emin misiniz?')) return;
    setError(null);
    setBusy(true);
    try {
      await generationApi.run(jobId);
      await load();
    } catch (err: any) {
      setError(err?.response?.data?.error ?? 'İş çalıştırılamadı.');
    } finally {
      setBusy(false);
    }
  };

  const handleDownloadSql = async () => {
    if (!jobId) return;
    setError(null);
    setBusy(true);
    try {
      const blob = await generationApi.downloadSql(jobId);
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `fixtureforge-${jobId}.sql`;
      document.body.appendChild(a);
      a.click();
      a.remove();
      window.URL.revokeObjectURL(url);
    } catch (err: any) {
      setError(err?.response?.data?.error ?? 'SQL indirilemedi.');
    } finally {
      setBusy(false);
    }
  };

  if (loading) return <p style={{ padding: 24 }}>Yükleniyor...</p>;
  if (error) return <p style={{ padding: 24, color: 'red' }}>{error}</p>;
  if (!job) return <p style={{ padding: 24 }}>İş bulunamadı.</p>;

  return (
    <div style={{ maxWidth: 1000, margin: '0 auto', padding: 24, fontFamily: 'system-ui, sans-serif' }}>
      <button onClick={() => navigate(`/projects/${job.projectId}/generation-jobs`)}>← Üretim İşlerine dön</button>
      <h1>Üretim İşi</h1>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: 12, marginBottom: 24 }}>
        <div><strong>Durum:</strong> {job.status}</div>
        <div><strong>Seed:</strong> {job.randomSeed}</div>
        <div><strong>Oluşturma:</strong> {new Date(job.createdAt).toLocaleString('tr-TR')}</div>
        <div><strong>Başlangıç:</strong> {job.startedAt ? new Date(job.startedAt).toLocaleString('tr-TR') : '-'}</div>
        <div><strong>Bitiş:</strong> {job.completedAt ? new Date(job.completedAt).toLocaleString('tr-TR') : '-'}</div>
        <div><strong>Üretilen Kayıt:</strong> {job.generatedRowCount}</div>
        <div><strong>Şema Sürümü:</strong> {job.schemaVersionTag}</div>
        <div><strong>Kural Sürümü:</strong> {job.ruleVersionTag}</div>
        <div><strong>Algoritma Sürümü:</strong> {job.generatorVersionTag}</div>
      </div>

      {job.errorSummary && (
        <div style={{ color: 'red', marginBottom: 16, padding: 8, border: '1px solid red', borderRadius: 4 }}>
          <strong>Hata:</strong> {job.errorSummary}
        </div>
      )}

      <div style={{ display: 'flex', gap: 8, marginBottom: 16 }}>
        {job.status === 'Pending' && (
          <button onClick={handleRun} disabled={busy} style={{ background: '#4CAF50', color: 'white', padding: '8px 16px' }}>
            {busy ? 'Çalıştırılıyor...' : '▶ Çalıştır'}
          </button>
        )}
        {job.status === 'Succeeded' && (
          <button onClick={handleDownloadSql} disabled={busy} style={{ background: '#2196F3', color: 'white', padding: '8px 16px' }}>
            {busy ? 'İndiriliyor...' : '⬇ SQL İndir'}
          </button>
        )}
        {(job.status === 'Pending' || job.status === 'Running') && (
          <button onClick={handleCancel} disabled={busy} style={{ color: 'red', padding: '8px 16px' }}>
            İşi İptal Et
          </button>
        )}
      </div>

      <h2>Tablo Hedefleri</h2>
      {tables.length === 0 ? <p>Tablo hedefi yok.</p> : (
        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ borderBottom: '1px solid #ccc', textAlign: 'left' }}>
              <th style={{ padding: 8 }}>Tablo</th>
              <th style={{ padding: 8 }}>Hedef</th>
              <th style={{ padding: 8 }}>Üretilen</th>
              <th style={{ padding: 8 }}>Durum</th>
              <th style={{ padding: 8 }}>Hata</th>
            </tr>
          </thead>
          <tbody>
            {tables.map((t) => (
              <tr key={t.id} style={{ borderBottom: '1px solid #eee' }}>
                <td style={{ padding: 8 }}>{t.schemaTableName}</td>
                <td style={{ padding: 8 }}>{t.targetRowCount}</td>
                <td style={{ padding: 8 }}>{t.generatedRowCount}</td>
                <td style={{ padding: 8 }}>{t.status}</td>
                <td style={{ padding: 8 }}>{t.errorSummary ?? '-'}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}