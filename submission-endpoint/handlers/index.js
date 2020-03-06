const rootHandler = require('./rootHandler');
const rootCorsHandler = require('./rootCorsHandler');

exports.handlers = [
  rootHandler,
  rootCorsHandler,
];
