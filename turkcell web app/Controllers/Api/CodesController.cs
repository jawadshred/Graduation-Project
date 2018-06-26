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
    public class CodesController : ApiController
    {
        //GET   /Api/Codes
        // need to check whtehr we are going to return only list of string or list of some object
        //for jaw: also NEED to implement DTO's later
        [HttpGet]
        public List<SmsCode> GetCodes(string sender)
        {

            if (Functions.NoSession())
            {
                return new List<SmsCode>();
            }
            return DB_Functions.DefaultSmsCodes(sender);
        }


    }
}
