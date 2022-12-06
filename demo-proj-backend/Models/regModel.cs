using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace demo_proj_backend.Models
{

    [DataContract(Name = "regModel")]
    public  class regModel
    {
        /*  [id] INT IDENTITY(1, 1) NOT NULL,
            [firstName] VARCHAR(50) NULL,
            [lastName] VARCHAR(50) NULL,
            [email] VARCHAR(50) NULL,
            [mobile] NUMERIC(13) NULL,
            [password] CHAR(8)     NULL,
            [dob] DATE NULL,
            [createdAt] DATETIME NULL,
            [updatedAt] NCHAR(10)   NULL,
            [isActive] INT NULL,*/

        //[DataMember(Name = "id", EmitDefaultValue = false)]
        //public int id { get; set; }
        //public string firstName { get; set; }
        //public string lastName { get; set; }
        //public string email { get; set; }
        //public int mobile { get; set; }
        //public string password { get; set; }
        //public Date dob { get; set; }
        //public DateTime createdAt { get; set; }
        //public DateTime updatedAt { get; set; }
        //public int isActive { get; set; }

        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int? Id { get; set; }

        [DataMember(Name = "firstName", EmitDefaultValue = false)]
        public string FirstName { get; set; }

        [DataMember(Name = "lastName", EmitDefaultValue = false)]
        public string LastName { get; set; }

        [DataMember(Name = "dateOfBirth", EmitDefaultValue = false)]
        public DateTime DateOfBirth { get; set; }

        [DataMember(Name = "email", EmitDefaultValue = false)]
        public string Email { get; set; }

        [DataMember(Name = "mobile", EmitDefaultValue = false)]
        public string Mobile { get; set; }

        [DataMember(Name = "password", EmitDefaultValue = false)]
        public string Password { get; set; }

    }
}