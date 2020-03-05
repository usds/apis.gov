const {httpError} = require('./httpError');
const {isValidApisJsonDocument} = require('../validator');

const MAX_UPLOAD_SECONDS = 120;
const MAX_DOCUMENT_SIZE = 2 * 1024 * 1024;

exports.path = '/';
exports.method = 'POST';
exports.handlerFunc = (req, res) => {
  try {
    validateRequestHeaders(req);
  } catch (e) {
    res.writeHead(e.statusCode || 400, {'Content-Type': 'application/json'});
    res.end(JSON.stringify({message: e.message}));
    return;
  }

  readRequestBody(req)
    .then(Buffer.from)
    .then(buf => buf.toString())
    .then(JSON.parse)
    .then(parsed => {
      if (isValidApisJsonDocument(parsed)) {
        return parsed;
      }

      throw httpError('Submitted API JSON did not match the schema', 422);
    }).then(parsedAndValidated => {
      res.writeHead(200, {'Content-Type': 'application/json'});
      res.end(JSON.stringify(parsedAndValidated));
    }).catch(err => {
      console.error(err);
      res.writeHead(err.statusCode || 500, {'Content-Type': 'application/json'});
      res.end(JSON.stringify({message: err.message}));
    });
};

/**
 * @param {http.IncomingMessage} req
 */
function validateRequestHeaders(req) {
  // Only allow JSON bodies
  if (req.headers["content-type"].toLowerCase() !== 'application/json') {
    throw httpError('Only JSON payloads are accepted.', 415);
  }

  // Do some pre-emptive max size enforcement
  if (!req.headers["content-length"]) {
    throw httpError(
      'Requests must include JSON payloads with a reported content length.',
      411
    );
  } else if (req.headers["content-length"] > MAX_DOCUMENT_SIZE) {
    throw httpError(
      `The submitted document is ${
        req.headers["content-length"]
      }; documents may not exceed ${MAX_DOCUMENT_SIZE} bytes.`,
      413
    );
  }
}

/**
 * @param {http.IncomingMessage} req
 * @returns {Promise<Uint8Array>}
 */
function readRequestBody(req) {
  const readStartedAt = process.hrtime();
  const expectedLength = Number.parseInt(req.headers["content-length"], 10) || 0;
  const buffer = new Uint8Array(expectedLength);
  let bytesRead = 0;

  return new Promise((resolve, reject) => {
    req.setTimeout(MAX_UPLOAD_SECONDS * 1000, () => {
      reject(httpError(`Socket timed out after ${MAX_UPLOAD_SECONDS} seconds.`,
        408));
    });

    const chunks = [];
    req.on('data', (chunk) => {
      const timeElapsed = process.hrtime(readStartedAt);

      if (timeElapsed[0] >= MAX_UPLOAD_SECONDS) {
        reject(httpError(`Request body not fully read within ${
          MAX_UPLOAD_SECONDS} seconds`, 408));
        return;
      }

      for (const byte of chunk) {
        buffer[bytesRead++] = byte;
        if (bytesRead > expectedLength) {
          reject(httpError(`Request body has exceeded its stated length of ${
            expectedLength} bytes`, 413));
          return;
        }
      }
    });

    req.on('end', () => {
      if (bytesRead != expectedLength) {
        reject(httpError(`Request body did not match its reported length of ${
          expectedLength}. Only ${bytesRead} were received.`, 400));
        return;
      }

      resolve(buffer);
    });

    req.on('error', err => {
      err.statusCode = 500;
      reject(err);
    });
  });
}
