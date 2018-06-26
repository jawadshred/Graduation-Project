using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
// using AttributeRouting.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using turkcell_web_app.Models;
using turkcell_web_app.Models.DB_Adapters;
using turkcell_web_app.Models.Session;
// using SimplestAuth.Models.Authentication;
using turkcell_web_app.ViewModels;

namespace SimplestAuth.Controllers
{
    public class AuthenticationController : Controller
    {

        IAuthenticationManager Authentication
        {

            get { return HttpContext.GetOwinContext().Authentication; }
        }

        [HttpGet]
        public ActionResult Login(string error="")
        {
            if (!Functions.NoSession())
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.error = error;


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Credentials input)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Session.Clear();
                }
                catch (System.Exception)
                {
                }
                Login R = DB_Functions.LogIn(input.Email, input.Pass);

                    var identity = new ClaimsIdentity(new[] {
                            new Claim(ClaimTypes.Name, input.Email),
                        },
                        DefaultAuthenticationTypes.ApplicationCookie,
                        ClaimTypes.Name, ClaimTypes.Role);

                    // if you want roles, just add as many as you want here (for loop maybe?)
                    identity.AddClaim(new Claim(ClaimTypes.Role, "guest"));
                    // tell OWIN the identity provider, optional
                    // identity.AddClaim(new Claim(IdentityProvider, "Simplest Auth"));

                    Authentication.SignIn(new AuthenticationProperties
                    {
                      IsPersistent = true
                        // input.RememberMe
                    }, identity);

                if (Functions.NoSession())
                {
                    return RedirectToAction("Login", "Authentication", new { error = "E-mail address or password incorrect" });

                }

                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login", "Authentication", new { error = "Error" });
        }


        [HttpGet]
        public ActionResult Logout()
        {
            Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Abandon();
            return RedirectToAction("login");
        }
    }
}