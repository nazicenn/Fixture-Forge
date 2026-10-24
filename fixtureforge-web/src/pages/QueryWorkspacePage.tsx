import { useCallback, useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import Editor from '@monaco-editor/react';
import { queriesApi } from '../api/query';
import { connectionsApi } from '../api/connection';
import type { QueryExecutionResult, QueryHistoryDto } from '../types/query';
import type { DatabaseConnection } from '../types/connection';

const DEFAULT_QUERY = 'SELECT TOP 10 * FROM Projects';

export function QueryWorkspacePage() {
  const { projectId } = useParams<{ projectId: string }>();
  const navigate = useNavigate();
  const [connections, setConnections] = useState<DatabaseConnection[]>([]);
  const [selectedConnectionId, setSelectedConnectionId] = useState<string>('');
  const [queryText, setQueryText] = useState<string>(DEFAULT_QUERY);
  const [result, setResult] = useState<QueryExecutionResult | null>(null);
  const [history, setHistory] = useState<QueryHistoryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [running, setRunning] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadConnections = useCallback(async () => {
    if (!projectId) return;
    try {
      const list = await connectionsApi.getAll(projectId);
      setConnections(list);
      if (list.length > 0) setSelectedConnectionId(list[0].id);
    } catch {
      setError('Bağlantılar yüklenemedi.');
    } finally {
      setLoading(false);
    }
  }, [projectId]);

  useEffect(() => {
    loadConnections();
  }, [loadConnections]);

  const loadHistory = useCallback(async () => {
    if (!selectedConnectionId) return;
    try {
      setHistory(await queriesApi.getHistory(selectedConnectionId));
    } catch {
      // sessizce geç
    }
  }, [selectedConnectionId]);

  useEffect(() => {
    loadHistory();
  }, [loadHistory]);

  const handleRun = async () => {
    if (!selectedConnectionId) {
      setError('Bir bağlantı seçin.');
      return;
    }
    if (!queryText.trim()) {
      setError('Sorgu boş olamaz.');
      return;
    }

    setError(null);
    setRunning(true);
    setResult(null);
    try {
      const res = await queriesApi.execute({
        connectionId: selectedConnectionId,
        queryText,
      });
      setResult(res);
      await loadHistory();
    } catch (err: any) {
      setError(err?.response?.data?.error ?? 'Sorgu çalıştırılamadı.');
    } finally {
      setRunning(false);
    }
  };

  if (loading) return <p style={{ padding: 24 }}>Yükleniyor...</p>;

  return (
    <div style={{ maxWidth: 1300, margin: '0 auto', padding: 24, fontFamily: 'system-ui, sans-serif' }}>
      <button onClick={() => navigate(`/projects/${projectId}`)}>← Projeye dön</button>
      <h1>SQL Workspace</h1>

      {connections.length === 0 ? (
        <div>
          <p>Henüz bağlantı yok. Önce bir bağlantı ekleyin.</p>
          <button onClick={() => navigate(`/projects/${projectId}/connections`)}>
            Bağlantı Ekle
          </button>
        </div>
      ) : (
        <>
          <div style={{ display: 'flex', gap: 8, alignItems: 'center', marginBottom: 12 }}>
            <label>
              <strong>Bağlantı:</strong>{' '}
              <select
                value={selectedConnectionId}
                onChange={(e) => setSelectedConnectionId(e.target.value)}
                style={{ padding: 6 }}
              >
                {connections.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.name} ({c.databaseName})
                  </option>
                ))}
              </select>
            </label>
            <button
              onClick={handleRun}
              disabled={running}
              style={{ background: '#4CAF50', color: 'white', padding: '8px 16px' }}
            >
              {running ? 'Çalışıyor...' : '▶ Çalıştır (F5)'}
            </button>
            <button onClick={() => navigate(`/projects/${projectId}/connections`)}>
              Bağlantıları Yönet
            </button>
          </div>

          <div style={{ border: '1px solid #ddd', borderRadius: 8, overflow: 'hidden', marginBottom: 16 }}>
            <Editor
              height="300px"
              defaultLanguage="sql"
              value={queryText}
              onChange={(value) => setQueryText(value ?? '')}
              options={{
                minimap: { enabled: false },
                fontSize: 14,
                scrollBeyondLastLine: false,
                wordWrap: 'on',
              }}
            />
          </div>

          {error && <div style={{ color: 'red', marginBottom: 16 }}>{error}</div>}

          {result && (
            <div style={{ marginBottom: 24 }}>
              <div style={{ display: 'flex', gap: 24, marginBottom: 8, fontSize: 14 }}>
                <span><strong>Tip:</strong> {result.queryType}</span>
                <span><strong>Süre:</strong> {result.durationMs} ms</span>
                <span><strong>Satır:</strong> {result.rowCount}</span>
                <span style={{ color: result.status === 'Succeeded' ? 'green' : 'red' }}>
                  <strong>Durum:</strong> {result.status}
                </span>
                <span style={{ fontSize: 12, color: '#888' }}>
                  Fingerprint: {result.queryFingerprint.substring(0, 12)}...
                </span>
              </div>

              {result.errorMessage && (
                <div style={{ color: 'red', padding: 8, border: '1px solid red', borderRadius: 4, marginBottom: 8 }}>
                  {result.errorMessage}
                </div>
              )}

              {result.columnNames.length > 0 && (
                <div style={{ overflow: 'auto', maxHeight: 400, border: '1px solid #ddd', borderRadius: 8 }}>
                  <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13 }}>
                    <thead style={{ background: '#f5f5f5', position: 'sticky', top: 0 }}>
                      <tr>
                        {result.columnNames.map((col) => (
                          <th key={col} style={{ padding: 8, borderBottom: '1px solid #ccc', textAlign: 'left' }}>
                            {col}
                          </th>
                        ))}
                      </tr>
                    </thead>
                    <tbody>
                      {result.rows.map((row, i) => (
                        <tr key={i} style={{ borderBottom: '1px solid #eee' }}>
                          {row.map((cell, j) => (
                            <td key={j} style={{ padding: 8 }}>
                              {cell ?? <span style={{ color: '#aaa' }}>NULL</span>}
                            </td>
                          ))}
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          )}

          <h2>Geçmiş Sorgular</h2>
          {history.length === 0 ? <p>Henüz sorgu yok.</p> : (
            <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13 }}>
              <thead>
                <tr style={{ borderBottom: '1px solid #ccc', textAlign: 'left' }}>
                  <th style={{ padding: 8 }}>Zaman</th>
                  <th style={{ padding: 8 }}>Tip</th>
                  <th style={{ padding: 8 }}>Sorgu (maskelenmiş)</th>
                  <th style={{ padding: 8 }}>Süre</th>
                  <th style={{ padding: 8 }}>Satır</th>
                  <th style={{ padding: 8 }}>Durum</th>
                </tr>
              </thead>
              <tbody>
                {history.map((h) => (
                  <tr key={h.id} style={{ borderBottom: '1px solid #eee' }}>
                    <td style={{ padding: 8 }}>{new Date(h.startedAt).toLocaleString('tr-TR')}</td>
                    <td style={{ padding: 8 }}>{h.queryType}</td>
                    <td style={{ padding: 8, fontFamily: 'monospace', fontSize: 12, color: '#666' }}>
                      {h.queryText.substring(0, 60)}{h.queryText.length > 60 ? '...' : ''}
                    </td>
                    <td style={{ padding: 8 }}>{h.durationMs} ms</td>
                    <td style={{ padding: 8 }}>{h.rowCount}</td>
                    <td style={{ padding: 8, color: h.status === 'Succeeded' ? 'green' : 'red' }}>
                      {h.status}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </>
      )}
    </div>
  );
}