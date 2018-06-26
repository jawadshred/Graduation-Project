using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using turkcell_web_app.Models.DB_Adapters;
using turkcell_web_app.Models.Panels.Classes;
using turkcell_web_app.Models.Session;

namespace turkcell_web_app.Controllers.Api
{
    public class ContentController : ApiController
    {
        //GET   /Api/content
        // need to check whtehr we are going to return only list of string or list of some object
        //for jaw: also NEED to implement DTO's later
        [HttpGet]
        public Content GetContent(string sender,string code)
        {

            if (Functions.NoSession())
            {
                return new Content();
            }
            //get from database the list of codes based on the selected sender using its id and assign them to codesList
            return DB_Functions.DefaultSmsContent(sender, code);

            //send back to react the list of codes


        }




    }
}
