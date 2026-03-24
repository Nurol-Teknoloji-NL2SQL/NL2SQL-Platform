export default function ResultTable({ dataRows, tableColumns, isArrayOfObjects }) {
  if (dataRows == null || (Array.isArray(dataRows) && dataRows.length === 0)) {
    return (
      <p className="m-0 text-slate-500">
        Henüz veri yok. Backend sonuç döndürdüğünde burada tablo görünecek.
      </p>
    );
  }

  if (isArrayOfObjects) {
    return (
      <div className="overflow-x-auto">
        <table className="w-full border-collapse font-mono text-sm">
          <thead>
            <tr>
              {tableColumns.map((col) => (
                <th
                  key={col}
                  className="text-left px-3 py-2 border-b-2 border-slate-200 font-semibold text-slate-700"
                >
                  {col}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {dataRows.map((row, i) => (
              <tr key={i} className="border-b border-slate-200 hover:bg-slate-50 transition-colors">
                {tableColumns.map((col) => (
                  <td key={col} className="px-3 py-2 text-slate-600">
                    {row[col] == null ? "—" : String(row[col])}
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    );
  }

  return (
    <pre className="m-0 whitespace-pre-wrap font-mono break-all text-sm text-slate-600">
      {JSON.stringify(dataRows, null, 2)}
    </pre>
  );
}
