using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqliteTest.Models
{
    public class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("county")]
        public string County { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("zipCode")]
        public string ZipCode { get; set; }
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("bs")]
        public string Bs { get; set; }
        [JsonProperty("catchPhrase")]
        public string CatchPhrase { get; set; }
        [JsonProperty("companyName")]
        public string CompanyName { get; set; }
        [JsonProperty("words")]
        public string Words { get; set; }
        [JsonProperty("sentence")]
        public string Sentence { get; set; }
    }
}
