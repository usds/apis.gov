using Gov.Apis.SubmissionEndpoint.Models;
using Gov.Apis.SubmissionEndpoint.Models.ApisDotJson;
using System.Threading.Tasks;

namespace Gov.Apis.SubmissionEndpoint.Services
{
    public interface ISubmissionService
    {
        Task<SubmissionResponse> Submit(Api api);
    }
}
