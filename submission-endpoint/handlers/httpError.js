/**
 * @param {string} message
 * @param {number} statusCode
 */
function httpError(message, statusCode) {
  const error = new Error(message);
  error.statusCode = statusCode;
  return error;
}

exports.httpError = httpError;
