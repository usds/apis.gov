using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Gov.Apis.SubmissionEndpoint.Models.ApisDotJson
{
    /// <summary>A single inventory of all API resources available within a domain</summary>
    public class Manifest
    {
        private static readonly string SPECIFICATION_VERSION = "0.15";

        /// <summary>text string of human readable name for the collection of APIs</summary>
        public string? Name { get; set; }

        /// <summary>text human readable description of the collection of APIs.</summary>
        public string? Description { get; set; }

        /// <summary>Web URL leading to an image to be used to represent the collection of
        /// APIs defined in this file.</summary>
        public string? Image { get; set; }

        /// <summary>Web URL indicating the location of the latest version of this file</summary>
        public string? Url { get; set; }

        /// <summary>a list of descriptive strings which identify the contents of the APIs.json file</summary>
        public IEnumerable<string>? Tags { get; set; }

        /// <summary>date of creation of the file</summary>
        public string? Created { get; set; }

        /// <summary>date of last modification of the file</summary>
        public string? Modified { get; set; }

        /// <summary>version of the APIs.json specification in use.</summary>
        [JsonPropertyName("specificationVersion")]
        public string SpecificationVersion { get { return SPECIFICATION_VERSION; } }

        /// <summary>list of APIs identified in the file</summary>
        public IEnumerable<Api>? Apis { get; set; }

        /// <summary></summary>
        public IEnumerable<Inclusion>? Include { get; set; }

        /// <summary></summary>
        public IEnumerable<Contact>? Maintainers { get; set; }

        public class Inclusion
        {
            /// <summary>name of the APIs.json file referenced.</summary>
            public string? Name { get; set; }

            /// <summary>Web URL of the file.</summary>
            public string? Url { get; set; }
        }
    }
}
