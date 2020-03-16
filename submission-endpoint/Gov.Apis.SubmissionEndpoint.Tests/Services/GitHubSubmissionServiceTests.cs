using Gov.Apis.SubmissionEndpoint.Models.ApisDotJson;
using Gov.Apis.SubmissionEndpoint.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Octokit;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

#nullable disable
namespace Gov.Apis.SubmissionEndpoint.Tests.Services
{
    public class GitHubSubmissionServiceTests
    {
        private const string OWNER = "owner";
        private const string REPO = "repo";
        private const string DEFAULT_BRANCH = "defaultBranch";
        private const string MANIFEST_PATH = "path/to/manifest.json";
        private const string ID = "id";
        private const string BASE_COMMIT_ID = "baseCommitId";
        private const string BASE_TREE_ID = "baseTreeId";
        private const string BLOB_ID = "blobId";
        private const string NEW_TREE_ID = "newTreeId";
        private const string NEW_COMMIT_ID = "newCommitId";
        private const string PULL_REQUEST_URL = "pullRequestUrl";

        [Fact]
        public async Task Submit_ShouldCreateTheRequisiteResourcesAndOpenAPullRequest()
        {
            var clientMock = new Mock<IGitHubClient>();

            // stub out the workflow of a successful submission
            clientMock.StubGetRepository(OWNER, REPO, DEFAULT_BRANCH);
            clientMock.StubGetBranch(OWNER, REPO, DEFAULT_BRANCH, BASE_COMMIT_ID);
            clientMock.StubGetCommit(OWNER, REPO, BASE_COMMIT_ID, BASE_TREE_ID);
            clientMock.StubGetFileAtRef(OWNER, REPO, BASE_COMMIT_ID, MANIFEST_PATH, new byte[] { 0x7b, 0x7d });
            clientMock.StubCreateBlob(OWNER, REPO, BLOB_ID);
            clientMock.StubCreateTree(OWNER, REPO, BASE_TREE_ID, MANIFEST_PATH, BLOB_ID, NEW_TREE_ID);
            clientMock.StubCreateCommit(OWNER, REPO, NEW_TREE_ID, BASE_COMMIT_ID, NEW_COMMIT_ID);
            clientMock.StubCreateBranch(OWNER, REPO, NEW_COMMIT_ID, $"submission/{ID}");
            clientMock.StubCreatePullRequest(OWNER, REPO, $"submission/{ID}", DEFAULT_BRANCH, PULL_REQUEST_URL);

            var sut = new GitHubSubmissionService(
                new Mock<ILogger<GitHubSubmissionService>>().Object,
                clientMock.Object,
                new MemoryCache(new MemoryCacheOptions()),
                OWNER,
                REPO,
                MANIFEST_PATH,
                api => ID
            );

            var response = await sut.Submit(new Api());
            Assert.Equal(ID, response.Id);
            Assert.Equal(PULL_REQUEST_URL, response.PublicUrl);
        }

        [Fact]
        public async Task Submit_ShouldDeleteSubmissionBranchWhenPullRequestCreationFails()
        {

            var clientMock = new Mock<IGitHubClient>();

            // stub out most of the workflow of a successful submission
            clientMock.StubGetRepository(OWNER, REPO, DEFAULT_BRANCH);
            clientMock.StubGetBranch(OWNER, REPO, DEFAULT_BRANCH, BASE_COMMIT_ID);
            clientMock.StubGetCommit(OWNER, REPO, BASE_COMMIT_ID, BASE_TREE_ID);
            clientMock.StubGetFileAtRef(OWNER, REPO, BASE_COMMIT_ID, MANIFEST_PATH, new byte[] { 0x7b, 0x7d });
            clientMock.StubCreateBlob(OWNER, REPO, BLOB_ID);
            clientMock.StubCreateTree(OWNER, REPO, BASE_TREE_ID, MANIFEST_PATH, BLOB_ID, NEW_TREE_ID);
            clientMock.StubCreateCommit(OWNER, REPO, NEW_TREE_ID, BASE_COMMIT_ID, NEW_COMMIT_ID);
            clientMock.StubCreateBranch(OWNER, REPO, NEW_COMMIT_ID, $"submission/{ID}");

            // then have it fail on the last step
            clientMock.StubPullRequestCreationFailure(OWNER, REPO, $"submission/{ID}", DEFAULT_BRANCH, new InvalidProgramException());

            var sut = new GitHubSubmissionService(
                new Mock<ILogger<GitHubSubmissionService>>().Object,
                clientMock.Object,
                new MemoryCache(new MemoryCacheOptions()),
                OWNER,
                REPO,
                MANIFEST_PATH,
                api => ID
            );

            // The original exception should be surfaced...
            await Assert.ThrowsAsync<InvalidProgramException>(() => sut.Submit(new Api()));

            // and the branch should be deleted.
            clientMock.Verify(_ => _.Git.Reference.Delete(OWNER, REPO, $"refs/heads/submission/{ID}"));
        }
    }

    public static class ExtensionMethods
    {
        public static void StubGetRepository(
            this Mock<IGitHubClient> mock,
            string owner,
            string repo,
            string defaultBranch
        ) {
            var repository = new Repository(defaultBranch: defaultBranch,
                url: null,
                htmlUrl: null,
                cloneUrl: null,
                gitUrl: null,
                sshUrl: null,
                svnUrl: null,
                mirrorUrl: null,
                id: 0,
                nodeId: null,
                owner: null,
                name: null,
                fullName: null,
                isTemplate: false,
                description: null,
                homepage: null,
                language: null,
                @private: false,
                fork: false,
                forksCount: 0,
                stargazersCount: 0,
                openIssuesCount: 0,
                pushedAt: null,
                createdAt: DateTimeOffset.Now,
                updatedAt: DateTimeOffset.Now,
                permissions: null,
                parent: null,
                source: null,
                license: null,
                hasIssues: false,
                hasWiki: false,
                hasDownloads: false,
                hasPages: false,
                subscribersCount: 0,
                size: 0,
                allowRebaseMerge: null,
                allowSquashMerge: null,
                allowMergeCommit: null,
                archived: false);
            mock
                .Setup(_ => _.Repository.Get(owner, repo))
                .ReturnsAsync(repository);
        }

        public static void StubGetBranch(
            this Mock<IGitHubClient> mock,
            string owner,
            string repo,
            string branchName,
            string baseCommitId
        ) {
            mock
                .Setup(_ => _.Repository.Branch.Get(owner, repo, branchName))
                .ReturnsAsync(new Branch(branchName, RefAtSha(baseCommitId), false));
        }

        public static void StubGetCommit(
            this Mock<IGitHubClient> mock,
            string owner,
            string repo,
            string baseCommitId,
            string baseTreeId
        ) {
            var commit = new Commit(sha: baseCommitId,
                tree: RefAtSha(baseTreeId),
                nodeId: null,
                url: null,
                label: null,
                @ref: null,
                user: null,
                repository: null,
                message: null,
                author: null,
                committer: null,
                parents: new [] { RefAtSha(baseCommitId) },
                commentCount: 0,
                verification: null);

            var ghCommit = new GitHubCommit(sha: baseCommitId,
                commit: commit,
                nodeId: null,
                url: null,
                label: null,
                @ref: null,
                user: null,
                repository: null,
                author: null,
                commentsUrl: null,
                committer: null,
                htmlUrl: null,
                stats: null,
                parents: null,
                files: null);

            mock
                .Setup(_ => _.Repository.Commit.Get(owner, repo, baseCommitId))
                .ReturnsAsync(ghCommit);
        }

        public static void StubGetFileAtRef(
            this Mock<IGitHubClient> mock,
            string owner,
            string repo,
            string commitSha,
            string filePath,
            byte[] content
        ) {
            var repoContent = new RepositoryContent(
                name: null,
                path: filePath,
                sha: null,
                size: 0,
                type: ContentType.File,
                downloadUrl: null,
                url: null,
                gitUrl: null,
                htmlUrl: null,
                encoding: "base64",
                encodedContent: System.Convert.ToBase64String(content),
                target: null,
                submoduleGitUrl: null);

            mock
                .Setup(_ => _.Repository.Content.GetAllContentsByRef(owner, repo, filePath, commitSha))
                .ReturnsAsync(new [] { repoContent });
        }

        public static void StubCreateBlob(
            this Mock<IGitHubClient> mock,
            string owner,
            string repo,
            string blobId
        ) {
            mock
                .Setup(_ => _.Git.Blob.Create(owner,
                    repo,
                    It.IsAny<NewBlob>()))
                .ReturnsAsync(new BlobReference(blobId));
        }

        public static void StubCreateTree(
            this Mock<IGitHubClient> mock,
            string owner,
            string repo,
            string baseTreeId,
            string filePath,
            string blobId,
            string newTreeId
        ) {
            var treeReponse = new TreeResponse(sha: newTreeId,
                url: null,
                tree: null,
                truncated: false);

            mock
                .Setup(_ => _.Git.Tree.Create(owner,
                    repo,
                    It.Is<NewTree>(t => t.BaseTree == baseTreeId
                        && t.Tree.Count == 1
                        && t.Tree.First().Sha == blobId)))
                .ReturnsAsync(treeReponse);
        }

        public static void StubCreateCommit(
            this Mock<IGitHubClient> mock,
            string owner,
            string repo,
            string treeId,
            string parentId,
            string commitId
        ) {
            var commit = new Commit(sha: commitId,
                nodeId: null,
                url: null,
                label: null,
                @ref: null,
                user: null,
                repository: null,
                message: null,
                author: null,
                committer: null,
                tree: null,
                parents: new [] { RefAtSha(parentId) },
                commentCount: 0,
                verification: null);

            mock
                .Setup(_ => _.Git.Commit.Create(owner,
                    repo,
                    It.Is<NewCommit>(c => c.Tree == treeId
                        && c.Parents.Count() == 1
                        && c.Parents.First() == parentId)))
                .ReturnsAsync(commit);
        }

        public static void StubCreateBranch(
            this Mock<IGitHubClient> mock,
            string owner,
            string repo,
            string commitId,
            string branchName
        ) {
            var reference = new Reference(@ref: null,
                nodeId: null,
                url: null,
                @object: null);

            mock
                .Setup(_ => _.Git.Reference.Create(owner,
                    repo,
                    It.Is<NewReference>(r => r.Sha == commitId
                        && r.Ref == $"refs/heads/{branchName}")))
                .ReturnsAsync(reference);
        }

        public static void StubCreatePullRequest(
            this Mock<IGitHubClient> mock,
            string owner,
            string repo,
            string headBranchName,
            string baseBranchName,
            string url
        ) {
            var pullRequest = new PullRequest(url: url,
                id: 0,
                nodeId: null,
                htmlUrl: null,
                diffUrl: null,
                patchUrl: null,
                issueUrl: null,
                statusesUrl: null,
                number: 0,
                state: new ItemState(),
                title: null,
                body: null,
                createdAt: DateTimeOffset.Now,
                updatedAt: DateTimeOffset.Now,
                closedAt: null,
                mergedAt: null,
                head: null,
                @base: null,
                user: null,
                assignee: null,
                assignees: null,
                draft: false,
                mergeable: null,
                mergeableState: null,
                mergedBy: null,
                mergeCommitSha: null,
                comments: 0,
                commits: 0,
                additions: 0,
                deletions: 0,
                changedFiles: 0,
                milestone: null,
                locked: false,
                maintainerCanModify: null,
                requestedReviewers: null,
                labels: null);

            mock
                .Setup(_ => _.Repository.PullRequest.Create(owner,
                    repo,
                    It.Is<NewPullRequest>(pr => pr.Head == headBranchName && pr.Base == baseBranchName)))
                .ReturnsAsync(pullRequest);
        }

        public static void StubPullRequestCreationFailure(
            this Mock<IGitHubClient> mock,
            string owner,
            string repo,
            string headBranchName,
            string baseBranchName,
            Exception toThrow
        ) {
            mock
                .Setup(_ => _.Repository.PullRequest.Create(owner,
                    repo,
                    It.Is<NewPullRequest>(pr => pr.Head == headBranchName && pr.Base == baseBranchName)))
                .ThrowsAsync(toThrow);
        }

        private static GitReference RefAtSha(string sha)
        {
            return new GitReference(sha: sha,
                nodeId: null,
                url: null,
                label: null,
                @ref: null,
                user: null,
                repository: null);
        }
    }
}
#nullable restore
