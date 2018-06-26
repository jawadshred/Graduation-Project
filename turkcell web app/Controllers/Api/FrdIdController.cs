using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using turkcell_web_app.Models.DB_Adapters;
using turkcell_web_app.Models.Session;

namespace turkcell_web_app.Controllers.Api
{
    public class FrdIdController : ApiController
    {
        [HttpGet]
        public bool GetFrdId(string id)
        {
            if (Functions.NoSession())
            {
                return false;
            }
            return DB_Functions.CheckFrdId(id);
        }
    }
}
