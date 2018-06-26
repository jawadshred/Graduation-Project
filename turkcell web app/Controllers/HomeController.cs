using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using turkcell_web_app.Models;
using turkcell_web_app.Models.Session;
using turkcell_web_app.ViewModels;
using turkcell_web_app.Models.DB_Adapters;
using static turkcell_web_app.ViewModels.LogedInEmployee;

namespace turkcell_web_app.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }
            String ID = Functions.GetID();
            LogedInEmployee user = Models.DB_Adapters.DB_Functions.GetUserData();



            //assign objects obtained above from database in this viewmodel
            LogedInEmployee viewModel = new LogedInEmployee()
            {
                UserInfo = user.UserInfo
            };

            //what do we need api result we inherited for?
            ViewBag.helpMessage = "This Homepage contains your dashboard with general information providing" +
                " an overview of your data & information.\n" +
                "In addition, an up to date number of the FRDs available in each list is shown.";
            return View(user);
        }




    }
}