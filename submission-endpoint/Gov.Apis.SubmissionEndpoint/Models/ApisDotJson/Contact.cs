using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Gov.Apis.SubmissionEndpoint.Models.ApisDotJson
{
    /// <summary>A contact record for a person or organization.</summary>
    /// <remarks>Values are all taken from the vCard specification.</remarks>
    public class Contact
    {
        /// <summary>String Value corresponding to the Full Nname name of the individual / organization.</summary>
        [JsonPropertyName("FN")]
        public string? FullName { get; set; }

        /// <summary>String Value corresponding to the email address of the individual / organization</summary>
        public string? Email { get; set; }

        /// <summary>String Value corresponding to a web page about the individual individual / organization</summary>
        [Url]
        public string? Url { get; set; }

        /// <summary>String Value representing the name of the organization associated with the cCard.</summary>
        [JsonPropertyName("org")]
        public string? Organization { get; set; }

        /// <summary>String Value corresponding to the physical address of the individual / organization.</summary>
        [JsonPropertyName("Adr")]
        public string? Address { get; set; }

        /// <summary>String Value corresponding to the phone number including country code of the
        /// individual / organization.</summary>
        [JsonPropertyName("Tel")]
        public string? TelephoneNumber { get; set; }

        /// <summary>String Value corresponding to the twitter username of the individual / organization.</summary>
        /// <remarks>convention do not use the "@" symbol</remarks>
        [JsonPropertyName("X-Twitter")]
        public string? TwitterHandler { get; set; }

        /// <summary>String Value corresponding to the github username of the individual / organization.</summary>
        [JsonPropertyName("X-Github")]
        public string? GitHubUserName { get; set; }

        /// <summary>URL corresponding to an image which could be used to represent the
        /// individual / organization.</summary>
        [Url]
        [JsonPropertyName("photo")]
        public string? PhotoUrl { get; set; }

        /// <summary>URL pointing to a vCard Objective RFC6350</summary>
        [Url]
        [JsonPropertyName("vCard")]
        public string? VCardUrl { get; set; }
    }
}
