using demo_proj_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNetCore;
using System.Threading;
using System.Configuration;
using HttpContext = System.Web.HttpContext;
using Amazon.DynamoDBv2;

namespace demo_proj_backend.Controllers
{
    public class uploadfilesController : ApiController
    {

        // POST: api/uploadfiles
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("uploadImage")]

        public void UploadImage(Uploadmodel uploadmodel)
            {
            bool Result = false;
            var req = System.Web.HttpContext.Current.Request;
            try
            {
                var _uploadedfile = req.Form;
              
            }
            catch(Exception ex)
            {

            }
           
        }
       
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("upload")]

        public string Post([System.Web.Http.FromBody] Uploadmodel fileUpload)
        {

            var req =HttpContext.Current.Request;
            var file=req.Form;
          
            try
            {
                //  Congig congig=new Congig();
                /* if (fileUpload.files.Length > 0)
                 {*/
                // string confi=ConfigurationBinder.appseting
                //   string path = _webHostEnvironment.WebRootPath + "\\uploads\\";

                // string path = ConfigurationManager.AppSettings["UploadDirectory"];
                string path = "D:/jaydip/dotnetbackend/demo-proj-backend/demo-proj-backend/wwwroot/uploads/";
                


                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
       
                using (FileStream fileStream = System.IO.File.Create(path + fileUpload.files))
                {
  
                   // fileUpload.files.CopyTo(fileStream);
                    fileStream.Flush();
                    return "Upload Succesfully.";
                }

                //using FileStream fileStream = System.IO.File.Create(path + fileUpload.files);

                // fileUpload.files.CopyTo(fileStream);
                // fileStream.Flush();
                // return "Upload Succesfully.";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //[System.Web.Http.HttpGet]

        //public async Task<IActionResult> Get([FromRoute] string fileName)
        //{
        //    string path = "\\uploads\\";
        //    var filePath = path + fileName + ".png";
        //    if (System.IO.File.Exists(filePath))
        //    {
        //        byte[] b = System.IO.File.ReadAllBytes(filePath);
        //        return b;
        //        return File(b, "image/png");
        //    }
        //    return null;
        //}
    }
}
