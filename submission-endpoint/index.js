"use strict";

const {handlers} = require('./handlers');
const http = require('http');
const {URL} = require('url');
const port = process.env.PORT || 2014;

const handlerMap = {};
for (const handler of handlers) {
  const pathEntry = handlerMap[handler.path] || {};
  if (pathEntry[handler.method.toLowerCase()]) {
    throw new Error(`Conflicting routes defined for ${
      handler.method.toUpperCase()} ${handler.path}`);
  }

  pathEntry[handler.method.toLowerCase()] = handler.handlerFunc;
  handlerMap[handler.path] = pathEntry;
}

const server = http.createServer((req, res) => {
  const parsedUrl = new URL(req.url, `http://${req.headers.host}`);
  const handler = (handlerMap[parsedUrl.pathname] || {})[req.method.toLowerCase()];
  if (!handler) {
    res.writeHead(404);
    res.end();
    return;
  }

  handler(req, res);
});

server.listen(port);
