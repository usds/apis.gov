const https = require('https');
const {Buffer} = require('buffer');
const {randomBytes} = require('crypto');

const REPO_OWNER = 'usds';
const REPO_NAME = 'apis.gov';

const BASE_OPTIONS = {
  hostname: 'api.github.com',
  auth: `${process.env.GITHUB_API_USERNAME}:${process.env.GITHUB_API_TOKEN}`,
  headers: {
    Accept: 'application/json',
    // GitHub recommends using a username as the useragent string for any
    // requests sent to their API for traceability
    'User-Agent': process.env.GITHUB_API_USERNAME,
  },
};

exports.createPullRequestToAddDocumentToApisJson = createPullRequestToAddDocumentToApisJson;

async function createPullRequestToAddDocumentToApisJson(document) {
  // Create a new submission branch name with a collision-resistant name
  const branchName = `submission/${randomBytes(16).toString('hex')}`;

  // Determine the default branch and its current commit and tree SHAs
  const {commitSha, defaultBranch, treeSha} = await fetchPullRequestBaseShas();

  // Create the new branch
  const newBranchMetadata = await createNewBranch(branchName, commitSha);

  const apisJson = await fetchApisJson(commitSha);
  apisJson.apis.push(document);

  const newTree = await uploadTree(treeSha, apisJson);

  const newCommit = await createNewCommit(commitSha, newTree.sha);

  const newBranchHead = await updateBranchHead(branchName, newCommit.sha);

  const newPullRequest = await createNewPullRequest(branchName, defaultBranch);

  return newPullRequest.url;
};

function updateBranchHead(branchName, commitSha) {
  console.log(`Updating branch [${branchName}] to use commit [${
    commitSha}] as its head.`);

  return new Promise((resolve, reject) => {
    const request = https.request(
      {
        ...BASE_OPTIONS,
        headers: {
          ...BASE_OPTIONS.headers,
          'Content-Type': 'application/json',
        },
        method: 'PATCH',
        path: `/repos/${REPO_OWNER}/${REPO_NAME}/git/refs/heads/${branchName}`
      },
      response => {
        digestApiResponseIntoJson(response)
          .then(resolve)
          .catch(reject);
      }
    );

    request.on('error', reject);

    request.write(JSON.stringify({sha: commitSha}));

    request.end();
  });
}

function createNewPullRequest(fromBranch, intoBranch) {
  console.log(`Creating a new pull request to merge [${fromBranch}] into [${
    intoBranch}]`);

  return new Promise((resolve, reject) => {
    const request = https.request(
      {
        ...BASE_OPTIONS,
        headers: {
          ...BASE_OPTIONS.headers,
          'Content-Type': 'application/json',
          'Accept': 'application/vnd.github.shadow-cat-preview+json'
        },
        method: 'POST',
        path: `/repos/${REPO_OWNER}/${REPO_NAME}/pulls`
      },
      response => {
        digestApiResponseIntoJson(response)
          .then(resolve)
          .catch(reject);
      }
    );

    request.on('error', reject);

    request.write(JSON.stringify({
      title: "New API for you, gov'nor!",
      head: fromBranch,
      base: intoBranch,
      body: `Hello! A website user submitted this API and would like it to be featured on our website!`,
    }));

    request.end();
  });
}

function createNewCommit(parentCommitSha, treeSha) {
  console.log(`Creating a new commit from tree [${treeSha}] with parent [${
    parentCommitSha}]`);

  return new Promise((resolve, reject) => {
    const request = https.request(
      {
        ...BASE_OPTIONS,
        headers: {
          ...BASE_OPTIONS.headers,
          'Content-Type': 'application/json',
        },
        method: 'POST',
        path: `/repos/${REPO_OWNER}/${REPO_NAME}/git/commits`
      },
      response => {
        digestApiResponseIntoJson(response)
          .then(resolve)
          .catch(reject);
      }
    );

    request.on('error', reject);

    request.write(JSON.stringify({
      message: 'Update apis.json with new API submission',
      tree: treeSha,
      parents: [parentCommitSha],
    }));

    request.end();
  });
}

function uploadTree(baseSha, newApisJson) {
  console.log(`Creating a new tree with the updated /docs/apis.json `
    + `from tree [${baseSha}]`);

  return new Promise((resolve, reject) => {
    const request = https.request(
      {
        ...BASE_OPTIONS,
        method: 'POST',
        path: `/repos/${REPO_OWNER}/${REPO_NAME}/git/trees`,
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
    )
    request.on('error', reject);
    request.write(JSON.stringify({
      base_tree: baseSha,
      tree: [
        {
          mode: '100644',
          type: 'blob',
          content: JSON.stringify(newApisJson, undefined, '  '),
          path: 'docs/api.json',
        }
      ]
    }));
    request.end();
  });
}

function createNewBranch(name, baseSha) {
  console.log(`Creating a new branch named [${name}] at commit [${baseSha}]`);
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
  console.log(`Fetching content of /docs/apis.json at commit [${commitSha}]`);
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
              Buffer.from(fileData.content, fileData.encoding)
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

async function fetchPullRequestBaseShas() {
  const metadata = await fetchRepositoryMetadata();
  if (!metadata.default_branch) {
    throw new Error(`No default branch defined for ${REPO_OWNER}/${REPO_NAME}`);
  }

  const branchMetadata = await fetchBranchMetadata(metadata.default_branch);
  const {commit = {}} = branchMetadata;
  const commitSha = commit.sha;
  const treeSha = ((commit.commit || {}).tree || {}).sha;
  if (commitSha && treeSha) {
    return {commitSha, treeSha, defaultBranch: metadata.default_branch};
  } else {
    throw new Error(`Unintelligible response received from GitHub branch API: ${
      JSON.stringify(branchMetadata)
    } does not contain a HEAD commit and/or tree SHA.`);
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
    if (!response.headers['content-type'] || response.headers['content-type'].indexOf('application/json') !== 0) {
      reject(new Error(`Received unexpected content type from GitHub API: ${
        response.headers['content-type']
      } received but expected 'application/json.'`));
      return;
    }

    const chunks = [];
    response.on('data', chunk => {
      chunks.push(Buffer.from(chunk).toString());
    });

    response.on('end', () => {
      let parsed;
      try {
        parsed = JSON.parse(chunks.join(''));
      } catch (e) {
        console.error(e);
        reject(e);
        return;
      }

      if (response.statusCode < 200 || response.statusCode > 299) {
        console.error(`Error response received from Github: ${response.statusCode}`);
        console.error(parsed);
        reject(new Error(`Received unexpected status code from GitHub API: ${
          response.statusCode} received but expected 200`));
      } else {
        resolve(parsed);
      }
    });

    response.on('error', reject);
  });
}
