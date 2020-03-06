const {STATIC_SITE_ORIGIN} = require('./constants');

exports.path = '/';
exports.method = 'options';
exports.handlerFunc = (req, res) => {
  res.writeHead(200, {
    'Access-Control-Allow-Origin': STATIC_SITE_ORIGIN,
    'Access-Control-Allow-Methods': 'POST',
    'Access-Control-Allow-Headers': 'Content-Type',
    'Access-Control-Max-Age': 86400,
  });
  res.end();
};
