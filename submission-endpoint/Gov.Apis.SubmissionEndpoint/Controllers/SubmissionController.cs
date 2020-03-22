using Gov.Apis.SubmissionEndpoint.Models;
using Gov.Apis.SubmissionEndpoint.Models.ApisDotJson;
using Gov.Apis.SubmissionEndpoint.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Gov.Apis.SubmissionEndpoint.Controllers
{
    [Route("submissions")]
    [ApiController]
    public class SubmissionController : ControllerBase
    {
        private readonly ILogger<SubmissionController> _logger;
        private readonly ISubmissionService submissionService;

        public SubmissionController(
            ILogger<SubmissionController> logger,
            ISubmissionService submissionService
        ) {
            _logger = logger;
            this.submissionService = submissionService;
        }

        [Produces("application/json")]
        [Consumes("application/json")]
        [RequestSizeLimit(2 * 1024 * 1024)]
        public IActionResult Post(Api api)
        {
            _logger.LogInformation("Received external API submission for [{Name}]", api.Name);

            Task<SubmissionResponse> task = submissionService.Submit(api);

            task.Wait();
            return Ok(task.Result);
        }
    }
}
