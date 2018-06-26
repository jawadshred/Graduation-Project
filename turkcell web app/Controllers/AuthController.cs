using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using turkcell_web_app.ViewModels;
using turkcell_web_app.Models.DB_Adapters;
using turkcell_web_app.Models;

namespace turkcell_web_app.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Login(Credentials emp1)
        {
            {


                //var httpWebRequest = (HttpWebRequest)WebRequest.Create(String.Format("http://127.0.0.1/api/Main/Login?EMP_ID={0}&Pass={1}", emp1.Email, emp1.Pass));
                //httpWebRequest.Method = "GET";
                //var response = httpWebRequest.GetResponse();

                //var serializer = new JsonSerializer();
                //Login result = new Login() ;
                //using (var sr = new StreamReader(response.GetResponseStream()))
                //using (var jsonTextReader = new JsonTextReader(sr))
                //{
                //    result = serializer.Deserialize<Login>(jsonTextReader);
                //}
            }

            

            if (ModelState.IsValid)
            {
               
               
            }
            
            
            //here we connect with database and authenticate user's credentials 
            //if user credentials dont match, redirect....if credentials are correct, continue



            return View("Login");
        }



    }
}