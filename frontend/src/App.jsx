import React, { useState } from "react";

const API_BASE = import.meta.env.VITE_API_BASE_URL || "/api";

export default function App() {
  const [question, setQuestion] = useState("");
  const [result, setResult] = useState(null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setResult(null);

    if (!question.trim()) {
      setError("Lütfen bir doğal dil sorgusu girin.");
      return;
    }

    const requestBody = { query: question };
    console.log("REQUEST BODY:", requestBody);

    const headers = { "Content-Type": "application/json" };
    const token = localStorage.getItem("token");
    if (token) headers["Authorization"] = `Bearer ${token}`;

    setLoading(true);
    try {
      const response = await fetch(`${API_BASE}/query/generate-sql`, {
        method: "POST",
        headers,
        body: JSON.stringify(requestBody),
      });

      if (!response.ok) {
        const text = await response.text();
        let msg = `İstek başarısız: ${response.status}`;
        try {
          const j = JSON.parse(text);
          msg = j.message || j.error || j.error_message || j.detail || msg;
        } catch (_) {}
        setError(msg);
        setLoading(false);
        return;
      }

      const data = await response.json();
      console.log("RESPONSE DATA:", data);
      setResult(data);
    } catch (err) {
      console.error("REQUEST ERROR:", err);
      setError("Sunucuya ulaşırken bir hata oluştu.");
    } finally {
      setLoading(false);
    }
  };

  const sqlText = result?.sql_query || result?.sql || "";
  const explanationText = result?.explanation || result?.message || result?.details || "";
  const dataRows = result?.data ?? result?.rows ?? result?.result ?? null;
  const isValidated = result && Object.prototype.hasOwnProperty.call(result, "is_validated") ? result.is_validated : undefined;

  // Sorgu sonucu: dizi ise tablo, değilse JSON göster
  const isArrayOfObjects = Array.isArray(dataRows) && dataRows.length > 0 && typeof dataRows[0] === "object" && dataRows[0] !== null && !Array.isArray(dataRows[0]);
  const tableColumns = isArrayOfObjects ? Object.keys(dataRows[0]) : [];

  return (
    <main style={{ fontFamily: "system-ui, sans-serif", padding: "2rem", maxWidth: "960px", margin: "0 auto" }}>
      <h1 style={{ fontSize: "2rem", marginBottom: "1rem" }}>NL2SQL Sorgu Arayüzü</h1>

      <form onSubmit={handleSubmit} style={{ marginBottom: "1.5rem" }}>
        <label style={{ display: "block", marginBottom: "0.5rem", fontWeight: 500 }}>Doğal dil sorgusu</label>
        <textarea
          value={question}
          onChange={(e) => setQuestion(e.target.value)}
          rows={4}
          placeholder='Örn: Son 5 kullanıcıyı getir'
          style={{ width: "100%", padding: "0.75rem", borderRadius: "0.5rem", border: "1px solid #d1d5db", resize: "vertical", fontSize: "0.95rem" }}
        />
        <button
          type="submit"
          disabled={loading}
          style={{ marginTop: "0.75rem", padding: "0.6rem 1.4rem", borderRadius: "999px", border: "none", backgroundColor: loading ? "#9ca3af" : "#2563eb", color: "white", fontWeight: 600, cursor: loading ? "default" : "pointer" }}
        >
          {loading ? "Çalıştırılıyor..." : "SQL Üret"}
        </button>
      </form>

      {error && (
        <div style={{ marginBottom: "1rem", padding: "0.75rem 1rem", borderRadius: "0.5rem", backgroundColor: "#fef2f2", color: "#b91c1c", border: "1px solid #fecaca" }}>
          {error}
        </div>
      )}

      {result && (
        <section style={{ display: "grid", gridTemplateColumns: "minmax(0, 1.2fr) minmax(0, 1fr)", gap: "1rem" }}>
          <div>
            {sqlText && (
              <div style={{ marginBottom: "1rem", padding: "0.75rem 1rem", borderRadius: "0.5rem", border: "1px solid #e5e7eb", backgroundColor: "#0b1120", color: "#e5e7eb", fontFamily: "ui-monospace, monospace", fontSize: "0.85rem", whiteSpace: "pre-wrap" }}>
                <div style={{ fontWeight: 600, marginBottom: "0.5rem", color: "#93c5fd" }}>Üretilen SQL</div>
                {sqlText}
              </div>
            )}
            {explanationText && (
              <div style={{ marginBottom: "1rem", padding: "0.75rem 1rem", borderRadius: "0.5rem", border: "1px solid #e5e7eb", backgroundColor: "#f9fafb", fontSize: "0.9rem" }}>
                <div style={{ fontWeight: 600, marginBottom: "0.5rem" }}>Açıklama</div>
                <p style={{ margin: 0, whiteSpace: "pre-wrap" }}>{explanationText}</p>
              </div>
            )}
            {isValidated !== undefined && (
              <div style={{ display: "inline-flex", alignItems: "center", padding: "0.25rem 0.6rem", borderRadius: "999px", fontSize: "0.8rem", fontWeight: 600, backgroundColor: isValidated ? "#dcfce7" : "#fef3c7", color: isValidated ? "#166534" : "#92400e" }}>
                {isValidated ? "Doğrulandı" : "Doğrulanmadı"}
              </div>
            )}
          </div>
          <div>
            <div style={{ padding: "0.75rem 1rem", borderRadius: "0.5rem", border: "1px solid #e5e7eb", backgroundColor: "#f9fafb", maxHeight: "420px", overflow: "auto", fontSize: "0.85rem" }}>
              <div style={{ fontWeight: 600, marginBottom: "0.5rem" }}>Sorgu sonucu</div>
              {dataRows == null || (Array.isArray(dataRows) && dataRows.length === 0) ? (
                <p style={{ margin: 0, color: "#6b7280" }}>Henüz veri yok. Backend sonuç döndürdüğünde burada tablo görünecek.</p>
              ) : isArrayOfObjects ? (
                <table style={{ width: "100%", borderCollapse: "collapse", fontFamily: "ui-monospace, monospace", fontSize: "0.8rem" }}>
                  <thead>
                    <tr>
                      {tableColumns.map((col) => (
                        <th key={col} style={{ textAlign: "left", padding: "0.5rem 0.75rem", borderBottom: "2px solid #e5e7eb", fontWeight: 600 }}>
                          {col}
                        </th>
                      ))}
                    </tr>
                  </thead>
                  <tbody>
                    {dataRows.map((row, i) => (
                      <tr key={i} style={{ borderBottom: "1px solid #e5e7eb" }}>
                        {tableColumns.map((col) => (
                          <td key={col} style={{ padding: "0.5rem 0.75rem" }}>
                            {row[col] == null ? "—" : String(row[col])}
                          </td>
                        ))}
                      </tr>
                    ))}
                  </tbody>
                </table>
              ) : (
                <pre style={{ margin: 0, whiteSpace: "pre-wrap", fontFamily: "ui-monospace, monospace", wordBreak: "break-all" }}>{JSON.stringify(dataRows, null, 2)}</pre>
              )}
            </div>
          </div>
        </section>
      )}
    </main>
  );
}
