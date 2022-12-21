using demo_proj_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Security.Cryptography.Pkcs;
using demo_proj_backend.common;
using Microsoft.Graph;
using System.Reflection;
using demo_proj_backend.DataObject;
using System.Web.Http.Cors;
using Newtonsoft.Json;

namespace demo_proj_backend.Controllers
{

    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
    [RoutePrefix("api")]
    public class ValuesController : ApiController
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["webapi_conn"].ConnectionString);
        regModel resObj =new  regModel();

        // GET api/values
        [Route("getAllData")]
        [HttpGet]

        public List<regModel> Get()
        {
            SqlDataAdapter da=new SqlDataAdapter("[dbo].[GetAllEmployeeData]", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataTable dt= new DataTable();
            da.Fill(dt);
            List<regModel> lastRecord=new List<regModel>();
            if(dt.Rows.Count>0)
            {
                for(int i=0;i< dt.Rows.Count;i++)
                {
                    regModel data=new regModel();
   
                    data.FirstName = dt.Rows[i]["firstName"].ToString();
                    data.LastName=dt.Rows[i]["lastName"].ToString();
                    lastRecord.Add(data);
                }
            }
            if(lastRecord.Count>0)
            {
                return lastRecord;
            }
            else
            {
                return null;
            }

        }

        // GET api/values/
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values


        [Route("registration")]
        [HttpPost]

        public HttpResponseMessage Post([FromBody]regModel objData)
         {
                 
            SqlCommand cmd = new SqlCommand("[dbo].[NewEmployeeRegistration]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@firstName", objData.FirstName);
            cmd.Parameters.AddWithValue("@lastName", objData.LastName);
            cmd.Parameters.AddWithValue("@email", objData.Email);
            cmd.Parameters.AddWithValue("@mobile", objData.Mobile);
            cmd.Parameters.AddWithValue("@password", objData.Password);
            cmd.Parameters.AddWithValue("@dob", objData.DateOfBirth);
         


            con.Open();
            var i = cmd.ExecuteNonQuery();
           // Global.Application_BeginRequest();
            con.Close();
            if (i <= 0)
            {
                
                return Global.CreateResponse(Request, HttpStatusCode.OK, "Registered has been successfully.");
            }
            else
            {
                return Global.CreateResponse(Request, HttpStatusCode.ServiceUnavailable, "unexpected Error");
            }
          
           
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
