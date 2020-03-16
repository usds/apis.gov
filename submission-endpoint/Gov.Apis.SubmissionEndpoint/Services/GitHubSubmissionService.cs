using Gov.Apis.SubmissionEndpoint.Models;
using Gov.Apis.SubmissionEndpoint.Models.ApisDotJson;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Gov.Apis.SubmissionEndpoint.Services
{
    public class GitHubSubmissionService : ISubmissionService
    {
        private readonly ILogger<GitHubSubmissionService> logger;
        private readonly IGitHubClient gitHubClient;
        private readonly IMemoryCache cache;
        private readonly string repositoryOwner;
        private readonly string repositoryName;
        private readonly string apisDotJsonPath;
        private readonly Func<Api, string> idProvider;

        public GitHubSubmissionService(
            ILogger<GitHubSubmissionService> logger,
            IGitHubClient gitHubClient,
            IMemoryCache cache,
            string repositoryOwner,
            string repositoryName,
            string apisDotJsonPath,
            [Optional] Func<Api, string> idProvider
        )
        {
            this.logger = logger;
            this.gitHubClient = gitHubClient;
            this.cache = cache;
            this.repositoryOwner = repositoryOwner;
            this.repositoryName = repositoryName;
            this.apisDotJsonPath = apisDotJsonPath;
            this.idProvider = idProvider ?? GitHubSubmissionService.defaultIdProvider;
        }

        public async Task<SubmissionResponse> Submit(Api api)
        {
            var id = idProvider.Invoke(api);
            // Attach submission metadata to the logging scope for the duration of this method
            using (logger.BeginScope(new Dictionary<string, string>() {
                ["ApiName"] = api.Name!,
                ["ApiBaseUrl"] = api.BaseUrl!,
                ["WorkflowId"] = id,
            })) {
                var state = await PrepareBaseState(id);

                try {
                    var manifest = await FetchManifestAtRef(reference: state.BaseCommitId);

                    // Append the submitted API to the end of the Apis list
                    manifest.Apis = Enumerable.Concat(manifest.Apis ?? new Api[0], new Api[] { api });
                    manifest.Modified = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd");

                    state.BlobId = await CreateBlobFromManifest(manifest: manifest);

                    state.TreeId = await CreateTreeFromManifestBlob(baseTreeId: state.BaseTreeId,
                        blobId: state.BlobId!);

                    state.CommitId = await CreateCommitFromTree(treeId: state.TreeId!,
                        commitMessage: $"Add API record for {api.Name!} to docs/apis.json",
                        baseCommitId: state.BaseCommitId);

                    state.BranchName = await CreateBranchFromCommit(branchName: $"submission/{id}",
                        commitId: state.CommitId!);

                    var pullRequestUrl = await CreatePullRequest(baseBranchName: state.BaseBranchName,
                            title: $"Add API record for {api.Name!} to docs/apis.json",
                            body: $"A website user submitted this metadata about {api.Name!} and would like it featured on apis.gov",
                            headBranchName: state.BranchName!);

                    logger.LogInformation("Pull request successfully submitted.");

                    return new SubmissionResponse(id) { PublicUrl = pullRequestUrl };
                } catch (Exception e) {
                    logger.LogError(e, "Error encountered during; attempting to roll back");
                    try {
                        await Rollback(id, state);
                    } catch (Exception err) {
                        logger.LogWarning(err, "Unable to rollback workflow. Manual cleanup may be required.");
                    }
                    throw e;
                }
            }
        }

        private async Task<GitHubPullRequestCreationState> PrepareBaseState(string id)
        {
            var defaultBranchName = cache.GetOrCreate<Lazy<string>>($"{repositoryOwner}/{repositoryName}:defaultBranch",
                entry => {
                    entry.SetAbsoluteExpiration(DateTimeOffset.Now.AddHours(1));

                    return new Lazy<string>(() => {
                        logger.LogInformation("Fetching default branch for {RepositoryOwner}/{RepositoryName}",
                            repositoryOwner,
                            repositoryName);

                        var task = gitHubClient.Repository.Get(owner: repositoryOwner, name: repositoryName);
                        task.Wait();
                        return task.Result.DefaultBranch;
                    });
                });

            logger.LogInformation("Fetching HEAD of [{BaseBranchName}] branch of {RepositoryOwner}/{RepositoryName}",
                defaultBranchName.Value,
                repositoryOwner,
                repositoryName);
            var baseCommitSha = (await gitHubClient.Repository.Branch.Get(owner: repositoryOwner,
                name: repositoryName,
                branch: defaultBranchName.Value))
                    .Commit.Sha;

            var baseTreeSha = cache.GetOrCreate<Lazy<string>>($"{repositoryOwner}/{repositoryName}",
                entry => new Lazy<string>(() => {
                    entry.SetSlidingExpiration(TimeSpan.FromDays(1));

                    logger.LogInformation("Fetching tree ID for {RepositoryOwner}/{RepositoryName}#{BaseCommitId}",
                        repositoryOwner,
                        repositoryName,
                        baseCommitSha);

                    var task = gitHubClient.Repository.Commit.Get(owner: repositoryOwner,
                        name: repositoryName,
                        reference: baseCommitSha);
                    task.Wait();
                    return task.Result.Commit.Tree.Sha;
                }));

            return new GitHubPullRequestCreationState(defaultBranchName.Value, baseCommitSha, baseTreeSha.Value);
        }

        private async Task<Manifest> FetchManifestAtRef(string reference)
        {
            logger.LogInformation("Fetching {ManifestPath} file from {RepositoryOwner}/{RepositoryName}#{BaseCommitId}",
                apisDotJsonPath,
                repositoryOwner,
                repositoryName,
                reference);

            var contents = await gitHubClient.Repository.Content.GetAllContentsByRef(owner: repositoryOwner,
                name: repositoryName,
                path: apisDotJsonPath,
                reference: reference);

            // Because the previous API call gave a path to a specific file, the returned array will have 1 member.
            return JsonSerializer.Deserialize<Manifest>(contents[0].Content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        private async Task<string> CreateBlobFromManifest(Manifest manifest)
        {
            logger.LogInformation("Uploading manifest as a git blob");

            var blob = await gitHubClient.Git.Blob.Create(owner: repositoryOwner,
                name: repositoryName,
                newBlob: new NewBlob() {
                    Encoding = EncodingType.Utf8,
                    Content = JsonSerializer.Serialize<Manifest>(manifest, new JsonSerializerOptions {
                        WriteIndented = true,
                        // Make sure null values are omitted from the output instead of being rendered as `null`
                        IgnoreNullValues = true,
                        // Without the unsafe encoder, ampersands get rendered as a UTF-8 escape sequence (`\u0026`)
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    }),
                });
            return blob.Sha;
        }

        private async Task<string> CreateTreeFromManifestBlob(string baseTreeId, string blobId)
        {
            logger.LogInformation("Creating a new tree replacing manifest with blob [{BlobId}] at [{BaseTreeId}]",
                blobId,
                baseTreeId);

            var newTree = new NewTree() { BaseTree = baseTreeId };
            newTree.Tree.Add(new NewTreeItem() {
                Path = apisDotJsonPath,
                Mode = "100644",
                Type = TreeType.Blob,
                Sha = blobId,
            });
            var tree = await gitHubClient.Git.Tree.Create(owner: repositoryOwner,
                name: repositoryName,
                newTree: newTree);
            return tree.Sha;
        }

        private async Task<string> CreateCommitFromTree(string commitMessage, string baseCommitId, string treeId)
        {
            logger.LogInformation("Creating commit from parent [{BaseCommitId}] and tree [{TreeId}]",
                baseCommitId,
                treeId);

            var commit = await gitHubClient.Git.Commit.Create(owner: repositoryOwner,
                name: repositoryName,
                commit: new NewCommit(message: commitMessage, tree: treeId, parents: new [] { baseCommitId }));
            return commit.Sha;
        }

        private async Task<string> CreateBranchFromCommit(string branchName, string commitId)
        {
            logger.LogInformation("Creating branch [{BranchName}] at commit [{CommitId}]", branchName, commitId);

            var branch = await gitHubClient.Git.Reference.Create(owner: repositoryOwner,
                name: repositoryName,
                reference: new NewReference(reference: $"refs/heads/{branchName}", sha: commitId));
            return branchName;
        }

        private async Task<string> CreatePullRequest(
            string title,
            string body,
            string baseBranchName,
            string headBranchName
        ) {
            logger.LogInformation("Opening pull request to merge [{BranchName}] into [{BaseBranchName}]",
                headBranchName,
                baseBranchName);

            var pullRequest = await gitHubClient.Repository.PullRequest.Create(owner: repositoryOwner,
                name: repositoryName,
                newPullRequest: new NewPullRequest(title: title, head: headBranchName, baseRef: baseBranchName) {
                    Body = body,
                });

            return pullRequest.Url;
        }

        private async Task Rollback(string contextId, GitHubPullRequestCreationState state)
        {
            if (state.BranchName != null)
            {
                logger.LogInformation("Deleting branch [{BranchName}]", state.BranchName);

                await gitHubClient.Git.Reference.Delete(owner: repositoryOwner,
                    name: repositoryName,
                    reference: $"refs/heads/{state.BranchName}");
            }

            // GitHub doesn't allow trees, blobs, or commits to be deleted via the API. Unreferenced objects will
            // eventually be garbage collected (typically after two weeks).
            logger.LogInformation("Workflow successfully rolled back");
        }

        private static string defaultIdProvider(Api api)
        {
            return Guid.NewGuid().ToString();
        }
    }

    internal class GitHubPullRequestCreationState
    {
        internal string BaseBranchName { get; }
        internal string BaseCommitId { get; }
        internal string BaseTreeId { get; }
        internal string? BlobId { get; set; }
        internal string? TreeId { get; set; }
        internal string? CommitId { get; set; }
        internal string? BranchName { get; set; }

        internal GitHubPullRequestCreationState(string baseBranchName, string baseCommitId, string baseTreeId)
        {
            BaseBranchName = baseBranchName;
            BaseCommitId = baseCommitId;
            BaseTreeId = baseTreeId;
        }
    }
}
