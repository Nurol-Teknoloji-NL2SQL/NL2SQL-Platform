const API_BASE = import.meta.env.VITE_API_BASE_URL || "/api";

/**
 * @typedef {Object} ApiError
 * @property {number} status
 * @property {string} message
 * @property {Record<string, string>|null} fieldErrors
 */

class ApiErrorImpl extends Error {
  /**
   * @param {number} status
   * @param {string} message
   * @param {Record<string, string>|null} fieldErrors
   */
  constructor(status, message, fieldErrors = null) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.fieldErrors = fieldErrors;
  }
}

/**
 * Parse error body from various API response formats.
 * @param {number} status
 * @param {string} bodyText
 * @returns {ApiErrorImpl}
 */
function parseError(status, bodyText) {
  let message = `İstek başarısız: ${status}`;
  let fieldErrors = null;

  try {
    const json = JSON.parse(bodyText);
    message =
      json.message ||
      json.error ||
      json.error_message ||
      json.detail ||
      (typeof json.title === "string" ? json.title : null) ||
      message;

    // .NET ValidationProblemDetails style
    if (json.errors && typeof json.errors === "object") {
      fieldErrors = {};
      for (const [field, msgs] of Object.entries(json.errors)) {
        fieldErrors[field] = Array.isArray(msgs) ? msgs.join(", ") : String(msgs);
      }
    }
  } catch (_) {
    if (bodyText && bodyText.length < 200) {
      message = bodyText;
    }
  }

  return new ApiErrorImpl(status, message, fieldErrors);
}

/**
 * Build headers with JWT token if available.
 * @returns {Record<string, string>}
 */
function buildHeaders() {
  const headers = { "Content-Type": "application/json" };
  const token = localStorage.getItem("token");
  if (token) headers["Authorization"] = `Bearer ${token}`;
  return headers;
}

/**
 * @param {string} path - API path (e.g. '/query/generate-sql')
 * @param {Record<string, unknown>} body
 * @returns {Promise<any>}
 * @throws {ApiErrorImpl}
 */
async function post(path, body) {
  const response = await fetch(`${API_BASE}${path}`, {
    method: "POST",
    headers: buildHeaders(),
    body: JSON.stringify(body),
  });

  if (!response.ok) {
    const text = await response.text();
    throw parseError(response.status, text);
  }

  return response.json();
}

/**
 * @param {string} path - API path
 * @returns {Promise<any>}
 * @throws {ApiErrorImpl}
 */
async function get(path) {
  const response = await fetch(`${API_BASE}${path}`, {
    method: "GET",
    headers: buildHeaders(),
  });

  if (!response.ok) {
    const text = await response.text();
    throw parseError(response.status, text);
  }

  return response.json();
}

export const api = { get, post };
export { ApiErrorImpl as ApiError };
