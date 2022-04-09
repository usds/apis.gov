using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Gov.Apis.SubmissionEndpoint.Models.ApisDotJson
{
    /// <summary>Metadata describing an individual API.</summary>
    public class Api
    {
        /// <summary>Name of the API.</summary>
        [Required]
        public string? Name { get; set; }

        /// Human readable description of the API.
        [Required]
        public string? Description { get; set; }

        /// <summary>URL of an image which can be used as an icon for the API if displayed by a search engine.</summary>
        [Url]
        [JsonPropertyName("Image")]
        public string? ImageUrl { get; set; }

        /// <summary>Web URL corresponding to human readable information about the API.</summary>
        [Url]
        [JsonPropertyName("humanUrl")]
        public string? HumanUrl { get; set; }

        /// <summary>Web URL corresponding to the root URL of the API or primary endpoint.</summary>
        [Required]
        [Url]
        [JsonPropertyName("baseUrl")]
        public string? BaseUrl { get; set; }

        /// <summary>String representing the version number of the API this description refers to.</summary>
        public string? Version { get; set; }

        /// <summary>A list of descriptive strings which identify the API.</summary>
        public IEnumerable<String>? Tags { get; set; }

        [JsonPropertyName("properties")]
        public IEnumerable<Property>? Properties { get; set; }

        [JsonPropertyName("contact")]
        public IEnumerable<Contact>? Contacts { get; set; }

        public class Property
        {
            [Required]
            [JsonPropertyName("type")]
            public string? Type { get; set; }

            [Required]
            [JsonPropertyName("url")]
            public string? Url { get; set; }
        }
    }
}
