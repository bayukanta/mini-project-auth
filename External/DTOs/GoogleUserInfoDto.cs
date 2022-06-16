using System;
using System.Collections.Generic;
using System.Text;

namespace External.DTOs
{
    public class GoogleUserInfoDto
    {
        public string family_name { get; set; }
        public string name { get; set; }
        public string picture { get; set; }
        public string locale { get; set; }
        public string email { get; set; }
        public string given_name { get; set; }
        public string id { get; set; }
        public bool verified_email { get; set; }
    }
}
