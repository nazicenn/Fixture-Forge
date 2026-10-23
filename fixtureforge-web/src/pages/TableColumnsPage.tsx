import { useCallback, useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { columnsApi } from '../api/schema';
import type { SchemaColumn, CreateColumnRequest } from '../types/schema';

const DATA_TYPES = [
  'int', 'bigint', 'smallint', 'tinyint',
  'nvarchar', 'varchar', 'char', 'nchar',
  'datetime2', 'datetime', 'date', 'time',
  'bit', 'decimal', 'numeric', 'float', 'real',
  'uniqueidentifier', 'varbinary', 'binary',
];

export function TableColumnsPage() {
  const { tableId } = useParams<{ tableId: string }>();
  const navigate = useNavigate();
  const [columns, setColumns] = useState<SchemaColumn[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const [form, setForm] = useState<CreateColumnRequest>({
    name: '', dataType: 'int', isNullable: true, isPrimaryKey: false, isUnique: false,
    defaultValue: null, maxLength: null, precision: null, scale: null, ordinalPosition: 0,
  });

  const load = useCallback(async () => {
    if (!tableId) return;
    try {
      setColumns(await columnsApi.getAll(tableId));
    } catch {
      setError('Sütunlar yüklenemedi.');
    } finally {
      setLoading(false);
    }
  }, [tableId]);

  useEffect(() => {
    load();
  }, [load]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!tableId || !form.name.trim()) return;
    setError(null);
    setSubmitting(true);
    try {
      await columnsApi.create(tableId, { ...form, ordinalPosition: columns.length });
      setForm({ ...form, name: '', isPrimaryKey: false, isUnique: false });
      await load();
    } catch (err: any) {
      setError(err?.response?.data?.error ?? 'Sütun oluşturulamadı.');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Silmek istediğinize emin misiniz?')) return;
    setError(null);
    try {
      await columnsApi.delete(id);
      await load();
    } catch {
      setError('Sütun silinemedi.');
    }
  };

  return (
    <div style={{ maxWidth: 1100, margin: '0 auto', padding: 24, fontFamily: 'system-ui, sans-serif' }}>
      <button onClick={() => navigate(-1)}>← Tablolara dön</button>
      <h1>Sütunlar</h1>

      <form onSubmit={handleSubmit} style={{ marginBottom: 24, padding: 16, border: '1px solid #ddd', borderRadius: 8 }}>
        <h2 style={{ marginTop: 0 }}>Yeni Sütun</h2>
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: 8 }}>
          <input placeholder="Ad" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
          <select value={form.dataType} onChange={(e) => setForm({ ...form, dataType: e.target.value })}>
            {DATA_TYPES.map((t) => <option key={t} value={t}>{t}</option>)}
          </select>
          <input type="number" placeholder="MaxLength" value={form.maxLength ?? ''} onChange={(e) => setForm({ ...form, maxLength: e.target.value ? +e.target.value : null })} />
          <label><input type="checkbox" checked={form.isNullable} onChange={(e) => setForm({ ...form, isNullable: e.target.checked })} /> Nullable</label>
          <label><input type="checkbox" checked={form.isPrimaryKey} onChange={(e) => setForm({ ...form, isPrimaryKey: e.target.checked })} /> PK</label>
          <label><input type="checkbox" checked={form.isUnique} onChange={(e) => setForm({ ...form, isUnique: e.target.checked })} /> Unique</label>
        </div>
        <button type="submit" disabled={submitting} style={{ marginTop: 8 }}>{submitting ? 'Kaydediliyor...' : 'Oluştur'}</button>
      </form>

      {error && <div style={{ color: 'red', marginBottom: 16 }}>{error}</div>}

      {loading ? <p>Yükleniyor...</p> : columns.length === 0 ? <p>Henüz sütun yok.</p> : (
        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ borderBottom: '1px solid #ccc', textAlign: 'left' }}>
              <th style={{ padding: 8 }}>#</th>
              <th style={{ padding: 8 }}>Ad</th>
              <th style={{ padding: 8 }}>Tip</th>
              <th style={{ padding: 8 }}>MaxLen</th>
              <th style={{ padding: 8 }}>Nullable</th>
              <th style={{ padding: 8 }}>PK</th>
              <th style={{ padding: 8 }}>Unique</th>
              <th style={{ padding: 8 }}></th>
            </tr>
          </thead>
          <tbody>
            {columns.map((c) => (
              <tr key={c.id} style={{ borderBottom: '1px solid #eee' }}>
                <td style={{ padding: 8 }}>{c.ordinalPosition}</td>
                <td style={{ padding: 8 }}>{c.name}</td>
                <td style={{ padding: 8 }}>{c.dataType}</td>
                <td style={{ padding: 8 }}>{c.maxLength ?? '-'}</td>
                <td style={{ padding: 8 }}>{c.isNullable ? '✓' : '✗'}</td>
                <td style={{ padding: 8 }}>{c.isPrimaryKey ? '✓' : ''}</td>
                <td style={{ padding: 8 }}>{c.isUnique ? '✓' : ''}</td>
                <td style={{ padding: 8 }}><button onClick={() => handleDelete(c.id)} style={{ color: 'red' }}>Sil</button></td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}