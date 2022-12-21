using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web;
using demo_proj_backend.Models;

namespace demo_proj_backend.common
{
    public static class Global
    {
        public static string ConnectionString { get; set; }

        public static HttpResponseMessage CreateResponse(HttpRequestMessage pRequest, HttpStatusCode pStatusCode, string pStrMessage)
        {
            Models.Error pObjResponse = new Models.Error
            {
                Status = (int)pStatusCode,
                Message = pStrMessage
            };
            return pRequest.CreateResponse((HttpStatusCode)pObjResponse.Status, pObjResponse);
        }

       
    }
}