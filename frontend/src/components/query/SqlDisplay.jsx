export default function SqlDisplay({ sqlText, explanationText, isValidated }) {
  return (
    <div className="space-y-4">
      {sqlText && (
        <div className="p-4 rounded-lg border border-slate-700 bg-slate-900 text-slate-200 font-mono text-sm whitespace-pre-wrap">
          <div className="font-semibold mb-2 text-blue-300">Üretilen SQL</div>
          {sqlText}
        </div>
      )}

      {explanationText && (
        <div className="p-4 rounded-lg border border-slate-200 bg-slate-50 text-sm">
          <div className="font-semibold mb-2 text-slate-800">Açıklama</div>
          <p className="m-0 whitespace-pre-wrap text-slate-600">{explanationText}</p>
        </div>
      )}

      {isValidated !== undefined && (
        <span
          className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold ${
            isValidated
              ? "bg-green-100 text-green-800"
              : "bg-amber-100 text-amber-800"
          }`}
        >
          {isValidated ? "Doğrulandı" : "Doğrulanmadı"}
        </span>
      )}
    </div>
  );
}
