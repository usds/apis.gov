using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Apis.SubmissionEndpoint.Models
{
    public class SubmissionResponse
    {
        public string Id { get; }
        public string? PublicUrl { get; set; }
        public string? InternalUrl { get; set; }

        public SubmissionResponse(string id)
        {
            Id = id;
        }
    }
}
