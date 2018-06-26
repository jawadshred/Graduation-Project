using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using turkcell_web_app.Models.Session;
using turkcell_web_app.Models.DB_Adapters;
using System.Net.Http.Headers;
using turkcell_web_app.Models.Panels.Classes;

namespace turkcell_web_app.Controllers
{
    public class FileController : Controller  {
        [System.Web.Mvc.HttpPost]
        public async Task<JsonResult> Upload(string Token)
        {

            if (Functions.NoSession())
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }
            if (!DB_Functions.CheckToken(Token))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }
            string User_ID = Functions.GetID();
            try
            {
                foreach (string x in Request.Files)
                {
                    var thefile = Request.Files[x];
                    if (thefile != null && thefile.ContentLength > 0)
                    {
                        string FileId = Guid.NewGuid().ToString();
                        // get a stream
                        string extension = Path.GetExtension(thefile.FileName);
                        var stream = thefile.InputStream;
                        var fileName = FileId + extension;
                        var path = Path.Combine(Server.MapPath("~/Files"), fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            stream.CopyTo(fileStream);
                        }
                        DB_Functions.InsertTempFile(FileId, "application/" + extension.Replace(".",""), Token, thefile.FileName, (new FileInfo(path)).Length);
                    }
                }

                //here send to db 
                return Json("File uploaded successfully");
            }catch(Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }



        }





        //private string Download_Folder = System.Web.Hosting.HostingEnvironment.MapPath("~/Files/");
        Stream stream;

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("Download/{frdID}/{FID}")]
        public FileStreamResult TestDownload(string frdID, string FID)
        {
            Attatchment file = DB_Functions.GetFile(frdID, FID);
            if (file == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);


            //HttpResponseMessage gg = new HttpResponseMessage();
            string File_Name = "/" + file.Id + file.Owner.Replace("application/", ".");
            try
            {
                stream = new FileStream(Server.MapPath("~/Files") + File_Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            FileStreamResult fileStream = new FileStreamResult(stream, file.Owner);
            fileStream.FileDownloadName = file.Name;
            return fileStream;
        }
        [System.Web.Mvc.HttpGet]

        public void DeleteFile(string U_Token,string FileName)
        {
            DB_Functions.DeleteTempFile(U_Token, FileName);

        }


    }
}
