using Microsoft.AspNetCore.Http;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Runtime.Serialization;
using System.Web;
using System.IO;

namespace demo_proj_backend.Models
{
    [DataContract(Name = "file")]
    public class Uploadmodel
    {

        [DataMember(Name = "files", EmitDefaultValue = false)]
        public string files { get; set; }

        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string name { get; set; }
    }

}