import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { relationshipsApi, tablesApi, columnsApi } from '../api/schema';
import type { Relationship, SchemaTable, SchemaColumn } from '../types/schema';

export function RelationshipsPage() {
  const { projectId } = useParams<{ projectId: string }>();
  const navigate = useNavigate();
  const [relationships, setRelationships] = useState<Relationship[]>([]);
  const [tables, setTables] = useState<SchemaTable[]>([]);
  const [sourceTableId, setSourceTableId] = useState('');
  const [targetTableId, setTargetTableId] = useState('');
  const [sourceColumns, setSourceColumns] = useState<SchemaColumn[]>([]);
  const [targetColumns, setTargetColumns] = useState<SchemaColumn[]>([]);
  const [sourceColumnId, setSourceColumnId] = useState('');
  const [targetColumnId, setTargetColumnId] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!projectId) return;
    (async () => {
      try {
        const [rels, tbls] = await Promise.all([
          relationshipsApi.getAll(projectId),
          tablesApi.getAll(projectId),
        ]);
        setRelationships(rels);
        setTables(tbls);
      } catch {
        setError('Veriler yüklenemedi.');
      } finally {
        setLoading(false);
      }
    })();
  }, [projectId]);

  useEffect(() => {
    if (!sourceTableId) { setSourceColumns([]); setSourceColumnId(''); return; }
    columnsApi.getAll(sourceTableId).then(setSourceColumns).catch(() => setSourceColumns([]));
  }, [sourceTableId]);

  useEffect(() => {
    if (!targetTableId) { setTargetColumns([]); setTargetColumnId(''); return; }
    columnsApi.getAll(targetTableId).then(setTargetColumns).catch(() => setTargetColumns([]));
  }, [targetTableId]);

 const handleSubmit = async (e: React.FormEvent) => {
  e.preventDefault();
  if (!projectId || !sourceTableId || !sourceColumnId || !targetTableId || !targetColumnId) {
    setError('Lütfen tüm alanları doldurun.');
    return;
  }
  setError(null);
  try {
    const created = await relationshipsApi.create(projectId, {
      sourceTableId, sourceColumnId, targetTableId, targetColumnId, relationshipType: 'FK',
    });
    console.log('İlişki oluşturuldu:', created);
    setSourceTableId(''); setSourceColumnId(''); setTargetTableId(''); setTargetColumnId('');
    const rels = await relationshipsApi.getAll(projectId);
    setRelationships(rels);
  } catch (err: any) {
    console.error('İlişki hatası:', err?.response?.status, err?.response?.data);
    const data = err?.response?.data;
    const msg = data?.error
      ?? (Array.isArray(data) ? data.map((e: any) => e.errorMessage).join('; ') : null)
      ?? (data?.errors ? Object.values(data.errors).flat().join('; ') : null)
      ?? `İlişki oluşturulamadı (${err?.response?.status ?? '?'})`;
    setError(msg);
  }
};

  const handleDelete = async (id: string) => {
    if (!projectId || !confirm('Silmek istediğinize emin misiniz?')) return;
    try {
      await relationshipsApi.delete(id);
      setRelationships(await relationshipsApi.getAll(projectId));
    } catch {
      setError('İlişki silinemedi.');
    }
  };

  return (
    <div style={{ maxWidth: 1100, margin: '0 auto', padding: 24, fontFamily: 'system-ui, sans-serif' }}>
      <button onClick={() => navigate(`/projects/${projectId}`)}>← Projeye dön</button>
      <h1>İlişkiler</h1>

      <form onSubmit={handleSubmit} style={{ marginBottom: 24, padding: 16, border: '1px solid #ddd', borderRadius: 8 }}>
        <h2 style={{ marginTop: 0 }}>Yeni İlişki (FK)</h2>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
          <div>
            <h4>Kaynak</h4>
            <select value={sourceTableId} onChange={(e) => setSourceTableId(e.target.value)} style={{ width: '100%', padding: 6 }}>
              <option value="">Tablo seçin</option>
              {tables.map((t) => <option key={t.id} value={t.id}>{t.name}</option>)}
            </select>
            <select value={sourceColumnId} onChange={(e) => setSourceColumnId(e.target.value)} style={{ width: '100%', padding: 6, marginTop: 6 }}>
              <option value="">Sütun seçin</option>
              {sourceColumns.map((c) => <option key={c.id} value={c.id}>{c.name} ({c.dataType})</option>)}
            </select>
          </div>
          <div>
            <h4>Hedef (PK)</h4>
            <select value={targetTableId} onChange={(e) => setTargetTableId(e.target.value)} style={{ width: '100%', padding: 6 }}>
              <option value="">Tablo seçin</option>
              {tables.map((t) => <option key={t.id} value={t.id}>{t.name}</option>)}
            </select>
            <select value={targetColumnId} onChange={(e) => setTargetColumnId(e.target.value)} style={{ width: '100%', padding: 6, marginTop: 6 }}>
              <option value="">Sütun seçin</option>
              {targetColumns.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name} ({c.dataType}) {c.isPrimaryKey ? '🔑' : ''}
                </option>
              ))}
            </select>
          </div>
        </div>
        <button type="submit" style={{ marginTop: 12 }}>İlişki Oluştur</button>
      </form>

      {error && <div style={{ color: 'red', marginBottom: 16 }}>{error}</div>}

      {loading ? <p>Yükleniyor...</p> : relationships.length === 0 ? <p>Henüz ilişki yok.</p> : (
        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ borderBottom: '1px solid #ccc', textAlign: 'left' }}>
              <th style={{ padding: 8 }}>Kaynak</th>
              <th style={{ padding: 8 }}>Hedef</th>
              <th style={{ padding: 8 }}>Tip</th>
              <th style={{ padding: 8 }}></th>
            </tr>
          </thead>
          <tbody>
            {relationships.map((r) => (
              <tr key={r.id} style={{ borderBottom: '1px solid #eee' }}>
                <td style={{ padding: 8 }}>{r.sourceTableName}.{r.sourceColumnName}</td>
                <td style={{ padding: 8 }}>{r.targetTableName}.{r.targetColumnName}</td>
                <td style={{ padding: 8 }}>{r.relationshipType}</td>
                <td style={{ padding: 8 }}><button onClick={() => handleDelete(r.id)} style={{ color: 'red' }}>Sil</button></td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}