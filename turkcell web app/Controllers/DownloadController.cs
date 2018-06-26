using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using turkcell_web_app.Models.DB_Adapters;
using turkcell_web_app.Models.Panels.Classes;

namespace turkcell_web_app.Controllers
{
    public class DownloadController : Controller
    {
        // GET: Download
        //[Route("Download/{frd}/{file_id}")]
        //public FileStreamResult Index(string frd,string file_id)
        //{
        //    Attatchment file = DB_Functions.GetFile(frd, file_id);
        //    if (file == null)
        //        return null;
        //    string File_Name = "/" + file.Id + file.Owner.Replace("application/", ".");
        //    return new FileStreamResult(new FileStream(Server.MapPath("~/Files") + File_Name, FileMode.Open), file.Owner);
        //}
    }
}