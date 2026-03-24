import { useMemo } from "react";
import { tokenizeSql } from "../../utils/sqlTokenizer";

const COLOR_MAP = {
  keyword: "text-blue-400 font-semibold",
  string: "text-green-400",
  number: "text-amber-400",
  comment: "text-slate-500 italic",
  function: "text-violet-400",
  operator: "text-sky-300",
  default: "text-slate-200",
};

export default function SqlHighlight({ sql }) {
  const tokens = useMemo(() => tokenizeSql(sql || ""), [sql]);

  return (
    <code className="whitespace-pre-wrap">
      {tokens.map((token, i) => (
        <span key={i} className={COLOR_MAP[token.type] || COLOR_MAP.default}>
          {token.value}
        </span>
      ))}
    </code>
  );
}
