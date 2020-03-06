const https = require('https');
const {Buffer} = require('buffer');
const {randomBytes} = require('crypto');

const REPO_OWNER = 'usds';
const REPO_NAME = 'apis.gov';

const BASE_OPTIONS = {
  hostname: 'api.github.com',
  auth: `${process.env.GITHUB_BOT_USERNAME}:${process.env.GITHUB_BOT_API_TOKEN}`,
  headers: {
    Accept: 'application/json',
    // GitHub recommends using a username as the useragent string for any
    // requests sent to their API for traceability
    'User-Agent': process.env.GITHUB_BOT_USERNAME,
  },
};

async function createPullRequestToAddDocumentToApisJson(document) {
  // Create a new submission branch name with a collision-resistant name
  const branchName = `submission/${randomBytes(16).toString('hex')}`;

  // Determine the base against which to build the new branch
  const baseSha = await fetchPullRequestBaseSha();

  // Create the new branch
  const newBranchMetadata = await createNewBranch(branchName, baseSha);

  const newApisJsonBlob = await uploadDocumentAsBlob(document);
  // Tree stuff

};

function uploadDocumentAsBlob(document) {
  return new Promise((resolve, reject) => {
    const request = https.request(
      {
        ...BASE_OPTIONS,
        method: 'POST',
        path: `/repos/${REPO_OWNER}/${REPO_NAME}/git/blobs`,
        headers: {
          ...BASE_OPTIONS.headers,
          'Content-Type': 'application/json',
        },
      },
      response => {
        digestApiResponseIntoJson(response)
          .then(resolve)
          .catch(reject);
      }
    );

    request.on('error', reject);

    request.write(JSON.stringify({
      encoding: 'utf-8',
      content: JSON.stringify(document),
    }));
    request.end();
  });
}

function createNewBranch(name, baseSha) {
  return new Promise((resolve, reject) => {
    const request = https.request(
      {
        ...BASE_OPTIONS,
        method: 'POST',
        path: `/repos/${REPO_OWNER}/${REPO_NAME}/git/refs`,
        headers: {
          ...BASE_OPTIONS.headers,
          'Content-Type': 'application/json',
        },
      },
      response => {
        digestApiResponseIntoJson(response)
          .then(resolve)
          .catch(reject);
      }
    );

    request.on('error', reject);

    request.write(JSON.stringify({
      sha: baseSha,
      ref: `refs/heads/${name}`,
    }));

    request.end();
  });
}

function fetchApisJson(commitSha) {
  return new Promise((resolve, reject) => {
    const request = https.request(
      {
        ...BASE_OPTIONS,
        method: 'GET',
        path: `/repos/${REPO_OWNER}/${REPO_NAME}/contents/docs/apis.json?ref=${commitSha}`
      },
      response => {
        digestApiResponseIntoJson(response)
          .then(fileData => {
            resolve(JSON.parse(
              Buffer.from(fileData.contents, fileData.encoding)
                .toString()
                .trim()
            ));
          })
          .catch(reject);
      }
    );

    request.on('error', reject);

    request.end();
  });
}

async function fetchPullRequestBaseSha() {
  const metadata = await fetchRepositoryMetadata();
  if (!metadata.default_branch) {
    throw new Error(`No default branch defined for ${REPO_OWNER}/${REPO_NAME}`);
  }

  const branchMetadata = await fetchBranchMetadata(metadata.default_branch);
  if ((branchMetadata.commit || {}).sha) {
    return branchMetadata.commit.sha;
  } else {
    throw new Error(`Unintelligible response received from GitHub branch API: ${
      JSON.stringify(branchMetadata)} does not contain a HEAD sha.`);
  }
}

function fetchRepositoryMetadata() {
  return new Promise((resolve, reject) => {
    const request = https.request(
      {
        ...BASE_OPTIONS,
        path: `/repos/${REPO_OWNER}/${REPO_NAME}`,
        method: 'GET'
      },
      response => {
        digestApiResponseIntoJson(response)
          .then(resolve)
          .catch(reject);
      }
    );

    request.on('error', reject);

    request.end();
  });
}

function fetchBranchMetadata(branchName) {
  return new Promise((resolve, reject) => {
    const request = https.request(
      {
        ...BASE_OPTIONS,
        method: 'GET',
        path: `/repos/${REPO_OWNER}/${REPO_NAME}/branches/${branchName}`
      },
      response => {
        digestApiResponseIntoJson(response)
          .then(resolve)
          .catch(reject);
      }
    );

    request.on('error', reject);

    request.end();
  });
}

function digestApiResponseIntoJson(response) {
  return new Promise((resolve, reject) => {
    if (response.statusCode < 200 || request.statusCode > 299) {
      reject(new Error(`Received unexpected status code from GitHub API: ${
        response.statusCode} received but expected 200`));
      return;
    }

    if (response.headers['content-type'] !== 'application/json') {
      reject(new Error(`Received unexpected content type from GitHub API: ${
        response.headers['content-type']
      } received but expected 'application/json.'`));
      return;
    }

    const chunks = [];
    readable.on('data', chunk => {
      chunks.push(Buffer.from(chunk).toString());
    });

    readable.on('end', () => {
      resolve(JSON.parse(chunks.join()));
    });

    readable.on('error', reject);
  });
}
