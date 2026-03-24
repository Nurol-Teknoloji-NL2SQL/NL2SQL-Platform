const SQL_KEYWORDS = new Set([
  "SELECT", "FROM", "WHERE", "JOIN", "ON", "GROUP", "BY", "ORDER",
  "HAVING", "INSERT", "UPDATE", "DELETE", "CREATE", "ALTER", "DROP",
  "AS", "AND", "OR", "NOT", "IN", "BETWEEN", "LIKE", "LIMIT", "OFFSET",
  "DISTINCT", "UNION", "EXISTS", "NULL", "IS", "SET", "VALUES", "INTO",
  "TABLE", "INDEX", "PRIMARY", "KEY", "FOREIGN", "REFERENCES", "CASCADE",
  "LEFT", "RIGHT", "INNER", "OUTER", "CROSS", "FULL", "ASC", "DESC",
  "CASE", "WHEN", "THEN", "ELSE", "END", "WITH", "RECURSIVE", "ALL",
  "ANY", "SOME", "TRUE", "FALSE", "DEFAULT", "CHECK", "CONSTRAINT",
  "ADD", "COLUMN", "IF", "REPLACE", "VIEW", "GRANT", "REVOKE",
  "BEGIN", "COMMIT", "ROLLBACK", "TRANSACTION", "RETURNING",
]);

const SQL_FUNCTIONS = new Set([
  "COUNT", "SUM", "AVG", "MAX", "MIN", "COALESCE", "NULLIF",
  "CAST", "CONVERT", "UPPER", "LOWER", "TRIM", "LENGTH", "SUBSTR",
  "SUBSTRING", "CONCAT", "REPLACE", "NOW", "DATE", "EXTRACT",
  "ROW_NUMBER", "RANK", "DENSE_RANK", "OVER", "PARTITION",
]);

/**
 * @typedef {'keyword'|'string'|'number'|'comment'|'function'|'operator'|'default'} TokenType
 * @typedef {{ type: TokenType, value: string }} Token
 */

/**
 * Tokenize a SQL string into colored tokens.
 * Single-pass character scanner, no regex.
 * @param {string} sql
 * @returns {Token[]}
 */
export function tokenizeSql(sql) {
  if (!sql) return [];

  const tokens = [];
  const len = sql.length;
  let i = 0;

  while (i < len) {
    const ch = sql[i];

    // -- line comment
    if (ch === "-" && sql[i + 1] === "-") {
      const start = i;
      i += 2;
      while (i < len && sql[i] !== "\n") i++;
      tokens.push({ type: "comment", value: sql.slice(start, i) });
      continue;
    }

    // /* block comment */
    if (ch === "/" && sql[i + 1] === "*") {
      const start = i;
      i += 2;
      while (i < len - 1 && !(sql[i] === "*" && sql[i + 1] === "/")) i++;
      i += 2; // skip */
      tokens.push({ type: "comment", value: sql.slice(start, i) });
      continue;
    }

    // 'string literal'
    if (ch === "'") {
      const start = i;
      i++;
      while (i < len) {
        if (sql[i] === "'" && sql[i + 1] === "'") {
          i += 2; // escaped quote
        } else if (sql[i] === "'") {
          i++;
          break;
        } else {
          i++;
        }
      }
      tokens.push({ type: "string", value: sql.slice(start, i) });
      continue;
    }

    // Numbers
    if (/[0-9]/.test(ch) || (ch === "." && i + 1 < len && /[0-9]/.test(sql[i + 1]))) {
      const start = i;
      i++;
      while (i < len && /[0-9.]/.test(sql[i])) i++;
      tokens.push({ type: "number", value: sql.slice(start, i) });
      continue;
    }

    // Words (identifiers / keywords / functions)
    if (/[a-zA-Z_]/.test(ch)) {
      const start = i;
      i++;
      while (i < len && /[a-zA-Z0-9_]/.test(sql[i])) i++;
      const word = sql.slice(start, i);
      const upper = word.toUpperCase();

      // Check if next non-space is '(' => function
      let nextNonSpace = i;
      while (nextNonSpace < len && sql[nextNonSpace] === " ") nextNonSpace++;

      if (SQL_FUNCTIONS.has(upper) && nextNonSpace < len && sql[nextNonSpace] === "(") {
        tokens.push({ type: "function", value: word });
      } else if (SQL_KEYWORDS.has(upper)) {
        tokens.push({ type: "keyword", value: word });
      } else {
        tokens.push({ type: "default", value: word });
      }
      continue;
    }

    // Operators
    if ("=<>!+-%*/".includes(ch)) {
      tokens.push({ type: "operator", value: ch });
      i++;
      continue;
    }

    // Everything else (whitespace, parens, commas, etc.)
    tokens.push({ type: "default", value: ch });
    i++;
  }

  return tokens;
}
