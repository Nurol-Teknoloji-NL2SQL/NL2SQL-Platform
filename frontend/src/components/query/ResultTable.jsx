import { useState, useMemo, useEffect } from "react";
import { ChevronsLeft, ChevronLeft, ChevronRight, ChevronsRight } from "lucide-react";

const PAGE_SIZES = [5, 10, 25, 50];

export default function ResultTable({ dataRows, tableColumns, isArrayOfObjects }) {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);

  // Reset page when data changes
  useEffect(() => {
    setPage(0);
  }, [dataRows]);

  if (dataRows == null || (Array.isArray(dataRows) && dataRows.length === 0)) {
    return (
      <p className="m-0 text-slate-500 text-sm">
        Henüz veri yok. Backend sonuç döndürdüğünde burada tablo görünecek.
      </p>
    );
  }

  if (!isArrayOfObjects) {
    return (
      <pre className="m-0 whitespace-pre-wrap font-mono break-all text-sm text-slate-600">
        {JSON.stringify(dataRows, null, 2)}
      </pre>
    );
  }

  const totalRows = dataRows.length;
  const totalPages = Math.max(1, Math.ceil(totalRows / pageSize));
  const safePage = Math.min(page, totalPages - 1);
  const startIdx = safePage * pageSize;
  const endIdx = Math.min(startIdx + pageSize, totalRows);
  const paginatedRows = dataRows.slice(startIdx, endIdx);
  const showPagination = totalRows > 5;

  return (
    <div>
      <div className="overflow-x-auto">
        <table className="w-full border-collapse font-mono text-xs sm:text-sm">
          <thead>
            <tr>
              {tableColumns.map((col) => (
                <th
                  key={col}
                  className="text-left px-3 py-2 border-b-2 border-slate-200 font-semibold text-slate-700 bg-slate-50 sticky top-0"
                >
                  {col}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {paginatedRows.map((row, i) => (
              <tr
                key={startIdx + i}
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
      </div>

      {showPagination && (
        <PaginationBar
          page={safePage}
          totalPages={totalPages}
          pageSize={pageSize}
          totalRows={totalRows}
          startIdx={startIdx}
          endIdx={endIdx}
          onPageChange={setPage}
          onPageSizeChange={(size) => {
            setPageSize(size);
            setPage(0);
          }}
        />
      )}
    </div>
  );
}

function PaginationBar({
  page,
  totalPages,
  pageSize,
  totalRows,
  startIdx,
  endIdx,
  onPageChange,
  onPageSizeChange,
}) {
  // Generate visible page numbers (max 5 with ellipsis)
  const pageNumbers = useMemo(() => {
    const pages = [];
    const maxVisible = 5;

    if (totalPages <= maxVisible) {
      for (let i = 0; i < totalPages; i++) pages.push(i);
    } else {
      pages.push(0);
      let start = Math.max(1, page - 1);
      let end = Math.min(totalPages - 2, page + 1);

      if (page <= 2) {
        start = 1;
        end = 3;
      } else if (page >= totalPages - 3) {
        start = totalPages - 4;
        end = totalPages - 2;
      }

      if (start > 1) pages.push("...");
      for (let i = start; i <= end; i++) pages.push(i);
      if (end < totalPages - 2) pages.push("...");
      pages.push(totalPages - 1);
    }

    return pages;
  }, [page, totalPages]);

  const iconBtn =
    "w-7 h-7 flex items-center justify-center rounded transition-colors disabled:opacity-30 disabled:cursor-not-allowed hover:bg-slate-100 cursor-pointer";

  return (
    <div className="flex flex-wrap items-center justify-between gap-2 px-3 py-2 border-t border-slate-200 bg-slate-50/80 text-xs text-slate-500">
      <div className="flex items-center gap-2">
        <span>Sayfa başına:</span>
        <select
          value={pageSize}
          onChange={(e) => onPageSizeChange(Number(e.target.value))}
          className="border border-slate-200 rounded px-1.5 py-0.5 text-xs bg-white focus:outline-none focus:ring-1 focus:ring-blue-400 cursor-pointer"
        >
          {PAGE_SIZES.map((s) => (
            <option key={s} value={s}>
              {s}
            </option>
          ))}
        </select>
        <span className="text-slate-400">
          {startIdx + 1}–{endIdx} / {totalRows} satır
        </span>
      </div>

      <div className="flex items-center gap-0.5">
        <button
          onClick={() => onPageChange(0)}
          disabled={page === 0}
          className={iconBtn}
          aria-label="İlk sayfa"
        >
          <ChevronsLeft className="w-3.5 h-3.5" />
        </button>
        <button
          onClick={() => onPageChange(page - 1)}
          disabled={page === 0}
          className={iconBtn}
          aria-label="Önceki sayfa"
        >
          <ChevronLeft className="w-3.5 h-3.5" />
        </button>

        {pageNumbers.map((p, i) =>
          p === "..." ? (
            <span key={`e${i}`} className="px-1 text-slate-300">
              ...
            </span>
          ) : (
            <button
              key={p}
              onClick={() => onPageChange(p)}
              className={`w-7 h-7 flex items-center justify-center rounded text-xs transition-colors cursor-pointer ${
                p === page
                  ? "bg-blue-600 text-white font-semibold"
                  : "hover:bg-slate-100 text-slate-600"
              }`}
            >
              {p + 1}
            </button>
          )
        )}

        <button
          onClick={() => onPageChange(page + 1)}
          disabled={page >= totalPages - 1}
          className={iconBtn}
          aria-label="Sonraki sayfa"
        >
          <ChevronRight className="w-3.5 h-3.5" />
        </button>
        <button
          onClick={() => onPageChange(totalPages - 1)}
          disabled={page >= totalPages - 1}
          className={iconBtn}
          aria-label="Son sayfa"
        >
          <ChevronsRight className="w-3.5 h-3.5" />
        </button>
      </div>
    </div>
  );
}
