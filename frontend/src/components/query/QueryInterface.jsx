import { useState, useRef, useEffect } from "react";
import { SendHorizontal, Database, User, Loader2, Sparkles } from "lucide-react";
import SectionWrapper from "../shared/SectionWrapper";
import ScrollReveal from "../shared/ScrollReveal";

const API_BASE = import.meta.env.VITE_API_BASE_URL || "/api";

export default function QueryInterface() {
  const [question, setQuestion] = useState("");
  const [messages, setMessages] = useState([]);
  const [loading, setLoading] = useState(false);
  const messagesEndRef = useRef(null);
  const textareaRef = useRef(null);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages, loading]);

  // Auto-resize textarea
  const handleInput = (e) => {
    setQuestion(e.target.value);
    const ta = textareaRef.current;
    if (ta) {
      ta.style.height = "auto";
      ta.style.height = Math.min(ta.scrollHeight, 160) + "px";
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const trimmed = question.trim();
    if (!trimmed || loading) return;

    // Add user message
    const userMsg = { role: "user", content: trimmed };
    setMessages((prev) => [...prev, userMsg]);
    setQuestion("");
    if (textareaRef.current) textareaRef.current.style.height = "auto";

    setLoading(true);
    try {
      const headers = { "Content-Type": "application/json" };
      const token = localStorage.getItem("token");
      if (token) headers["Authorization"] = `Bearer ${token}`;

      const response = await fetch(`${API_BASE}/query/generate-sql`, {
        method: "POST",
        headers,
        body: JSON.stringify({ query: trimmed }),
      });

      if (!response.ok) {
        const text = await response.text();
        let msg = `İstek başarısız: ${response.status}`;
        try {
          const j = JSON.parse(text);
          msg = j.message || j.error || j.error_message || j.detail || msg;
        } catch (_) {}
        setMessages((prev) => [...prev, { role: "error", content: msg }]);
        setLoading(false);
        return;
      }

      const data = await response.json();
      setMessages((prev) => [...prev, { role: "assistant", data }]);
    } catch (err) {
      setMessages((prev) => [
        ...prev,
        { role: "error", content: "Sunucuya ulaşırken bir hata oluştu." },
      ]);
    } finally {
      setLoading(false);
    }
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSubmit(e);
    }
  };

  return (
    <div className="bg-slate-50/50">
      <SectionWrapper id="query" className="!py-12 sm:!py-16">
        <ScrollReveal>
          <div className="max-w-3xl mx-auto">
            {/* Header */}
            <div className="text-center mb-8">
              <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-blue-100 text-blue-700 text-xs font-medium mb-3">
                <Sparkles className="w-3 h-3" />
                AI Destekli
              </div>
              <h2 className="text-2xl sm:text-3xl font-bold text-slate-900">
                Veritabanınıza sorun
              </h2>
            </div>

            {/* Chat container */}
            <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
              {/* Messages area */}
              <div
                className="min-h-[120px] max-h-[480px] overflow-y-auto p-4 sm:p-6 space-y-4"
                data-lenis-prevent
              >
                {messages.length === 0 && !loading && (
                  <div className="flex flex-col items-center justify-center py-10 text-slate-400">
                    <Database className="w-10 h-10 mb-3 text-slate-300" />
                    <p className="text-sm text-center max-w-xs">
                      Doğal dilde bir soru yazın, yapay zeka sizin için SQL üretsin.
                    </p>
                    <div className="flex flex-wrap gap-2 mt-4 justify-center">
                      {["Son 10 kullanıcıyı getir", "Toplam sipariş sayısı", "En çok satan ürün"].map(
                        (s) => (
                          <button
                            key={s}
                            onClick={() => {
                              setQuestion(s);
                              textareaRef.current?.focus();
                            }}
                            className="px-3 py-1.5 text-xs rounded-lg border border-slate-200 text-slate-500 hover:border-blue-300 hover:text-blue-600 hover:bg-blue-50 transition-all cursor-pointer"
                          >
                            {s}
                          </button>
                        )
                      )}
                    </div>
                  </div>
                )}

                {messages.map((msg, i) => (
                  <MessageBubble key={i} message={msg} />
                ))}

                {loading && (
                  <div className="flex items-start gap-3">
                    <div className="w-7 h-7 rounded-full bg-blue-600 flex items-center justify-center shrink-0">
                      <Database className="w-3.5 h-3.5 text-white" />
                    </div>
                    <div className="bg-slate-50 rounded-2xl rounded-tl-sm px-4 py-3 flex items-center gap-2 text-sm text-slate-500">
                      <Loader2 className="w-4 h-4 animate-spin" />
                      Düşünüyor...
                    </div>
                  </div>
                )}

                <div ref={messagesEndRef} />
              </div>

              {/* Input area */}
              <div className="border-t border-slate-100 p-3 sm:p-4">
                <form
                  onSubmit={handleSubmit}
                  className="flex items-end gap-2 bg-slate-50 rounded-xl px-4 py-2 border border-slate-200 focus-within:border-blue-400 focus-within:ring-2 focus-within:ring-blue-100 transition-all"
                >
                  <textarea
                    ref={textareaRef}
                    value={question}
                    onChange={handleInput}
                    onKeyDown={handleKeyDown}
                    rows={1}
                    placeholder="Veritabanınıza bir soru sorun..."
                    className="flex-1 bg-transparent resize-none text-sm sm:text-base text-slate-800 placeholder-slate-400 focus:outline-none py-1.5 leading-relaxed max-h-[160px]"
                  />
                  <button
                    type="submit"
                    disabled={loading || !question.trim()}
                    className={`shrink-0 w-9 h-9 rounded-lg flex items-center justify-center transition-all ${
                      loading || !question.trim()
                        ? "bg-slate-200 text-slate-400 cursor-not-allowed"
                        : "bg-blue-600 text-white hover:bg-blue-700 cursor-pointer"
                    }`}
                  >
                    <SendHorizontal className="w-4 h-4" />
                  </button>
                </form>
                <p className="text-[11px] text-slate-400 text-center mt-2">
                  Enter ile gönder &middot; Shift+Enter ile yeni satır
                </p>
              </div>
            </div>
          </div>
        </ScrollReveal>
      </SectionWrapper>
    </div>
  );
}

/* --- Message Bubble --- */

function MessageBubble({ message }) {
  const { role, content, data } = message;

  if (role === "user") {
    return (
      <div className="flex items-start gap-3 justify-end">
        <div className="bg-blue-600 text-white rounded-2xl rounded-tr-sm px-4 py-2.5 max-w-[80%] text-sm leading-relaxed">
          {content}
        </div>
        <div className="w-7 h-7 rounded-full bg-slate-200 flex items-center justify-center shrink-0">
          <User className="w-3.5 h-3.5 text-slate-600" />
        </div>
      </div>
    );
  }

  if (role === "error") {
    return (
      <div className="flex items-start gap-3">
        <div className="w-7 h-7 rounded-full bg-red-100 flex items-center justify-center shrink-0">
          <Database className="w-3.5 h-3.5 text-red-500" />
        </div>
        <div className="bg-red-50 border border-red-200 text-red-700 rounded-2xl rounded-tl-sm px-4 py-2.5 max-w-[80%] text-sm">
          {content}
        </div>
      </div>
    );
  }

  // Assistant message with data
  const sqlText = data?.sql_query || data?.sql || "";
  const explanationText = data?.explanation || data?.message || data?.details || "";
  const dataRows = data?.data ?? data?.rows ?? data?.result ?? null;
  const isValidated =
    data && Object.prototype.hasOwnProperty.call(data, "is_validated")
      ? data.is_validated
      : undefined;

  const isArrayOfObjects =
    Array.isArray(dataRows) &&
    dataRows.length > 0 &&
    typeof dataRows[0] === "object" &&
    dataRows[0] !== null &&
    !Array.isArray(dataRows[0]);
  const tableColumns = isArrayOfObjects ? Object.keys(dataRows[0]) : [];

  return (
    <div className="flex items-start gap-3">
      <div className="w-7 h-7 rounded-full bg-blue-600 flex items-center justify-center shrink-0 mt-0.5">
        <Database className="w-3.5 h-3.5 text-white" />
      </div>
      <div className="flex-1 min-w-0 space-y-3 max-w-[90%]">
        {/* SQL block */}
        {sqlText && (
          <div className="bg-slate-900 text-slate-200 rounded-xl px-4 py-3 font-mono text-xs sm:text-sm whitespace-pre-wrap overflow-x-auto">
            <span className="text-blue-400 text-[10px] uppercase tracking-wider font-semibold block mb-1.5">
              SQL
            </span>
            {sqlText}
          </div>
        )}

        {/* Explanation */}
        {explanationText && (
          <div className="bg-slate-50 rounded-xl px-4 py-3 text-sm text-slate-600 leading-relaxed">
            {explanationText}
          </div>
        )}

        {/* Validation badge */}
        {isValidated !== undefined && (
          <span
            className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-[11px] font-semibold ${
              isValidated
                ? "bg-green-100 text-green-700"
                : "bg-amber-100 text-amber-700"
            }`}
          >
            {isValidated ? "Doğrulandı" : "Doğrulanmadı"}
          </span>
        )}

        {/* Data table */}
        {dataRows != null &&
          !(Array.isArray(dataRows) && dataRows.length === 0) && (
            <div className="bg-white border border-slate-200 rounded-xl overflow-hidden">
              <div className="px-4 py-2 bg-slate-50 border-b border-slate-200 text-xs font-semibold text-slate-500 uppercase tracking-wider">
                Sonuçlar
              </div>
              <div className="overflow-x-auto max-h-[280px] overflow-y-auto">
                {isArrayOfObjects ? (
                  <table className="w-full text-xs sm:text-sm">
                    <thead>
                      <tr className="border-b border-slate-200">
                        {tableColumns.map((col) => (
                          <th
                            key={col}
                            className="text-left px-3 py-2 font-semibold text-slate-700 bg-slate-50 sticky top-0"
                          >
                            {col}
                          </th>
                        ))}
                      </tr>
                    </thead>
                    <tbody>
                      {dataRows.map((row, i) => (
                        <tr
                          key={i}
                          className="border-b border-slate-100 last:border-0 hover:bg-slate-50 transition-colors"
                        >
                          {tableColumns.map((col) => (
                            <td key={col} className="px-3 py-2 text-slate-600 whitespace-nowrap">
                              {row[col] == null ? "—" : String(row[col])}
                            </td>
                          ))}
                        </tr>
                      ))}
                    </tbody>
                  </table>
                ) : (
                  <pre className="p-3 text-xs font-mono whitespace-pre-wrap text-slate-600">
                    {JSON.stringify(dataRows, null, 2)}
                  </pre>
                )}
              </div>
            </div>
          )}
      </div>
    </div>
  );
}
