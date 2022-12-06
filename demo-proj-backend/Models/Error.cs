using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace demo_proj_backend.Models
{
    [DataContract(Name = "Response")]
    public class Error
    {
        [DataMember(Name = "information", EmitDefaultValue = false)]
        public string Information { get; set; }
        [DataMember(Name = "status", EmitDefaultValue = false)]
        public int Status { get; set; }
        [DataMember(Name = "message", EmitDefaultValue = false)]
        public string Message { get; set; }
    }
}

