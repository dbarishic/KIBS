using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KIBS.Models
{
    public class User
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("profile_pic")]
        public string ProfilePic { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
