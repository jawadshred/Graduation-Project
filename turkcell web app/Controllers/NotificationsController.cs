using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using turkcell_web_app.Models;
using turkcell_web_app.Models.DB_Adapters;
using turkcell_web_app.Models.Session;
using turkcell_web_app.ViewModels;

namespace turkcell_web_app.Controllers
{
    public class NotificationsController : Controller
    {
        // GET: Notifications
        [System.Web.Mvc.Route("Noti/{noti_id}")]
        public ActionResult Check(string noti_id)
        {
            if (Models.Session.Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }
            Notification N = DB_Functions.GetNotiInfo(noti_id);
            if (N == null)
                return new HttpNotFoundResult();

            DB_Functions.NotiSeen(noti_id);
            Frd frd = DB_Functions.CheckForFrd(N.FrdId);
            if (frd == null)
                return new HttpNotFoundResult();


            if (frd.LatestVersion == N.Version)
            {
                switch (frd.Type)
                {
                    case Frd.TypesEnum.Active:
                        return RedirectToAction("ActiveFrd", "Request", new { Id = G_Functions.GetHash(N.FrdId) });
                    case Frd.TypesEnum.Closed:
                        return RedirectToAction("ClosedFrd", "Request", new { Id = G_Functions.GetHash(N.FrdId) });
                    case Frd.TypesEnum.Pending:
                        return RedirectToAction("PendingFrd", "Request", new { Id = G_Functions.GetHash(N.FrdId) });
                    case Frd.TypesEnum.ReceivedManagerActive:
                    case Frd.TypesEnum.ReceivedManagerPending:
                    case Frd.TypesEnum.ReceivedUser:
                        return RedirectToAction("ReceivedFrd", "Request", new { Id = G_Functions.GetHash(N.FrdId) });
                }
            }
            else
            {
                return RedirectToAction("ViewFrd", "Request", new { Id = G_Functions.GetHash(N.FrdId), V = N.Version });
            }

            return View();
        }



        // GET: NotificationPage

        public ActionResult NotificationPage(string op="all",string frd=null)
        {
            if (Models.Session.Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }

            NotificationPage viewModel = new NotificationPage();
            switch (op) {
                case "all":
                    viewModel.Notifications = DB_Functions.ReadNotifications(Functions.GetID());
                    break;
                case "R":
                    viewModel.Notifications = DB_Functions.ReadNotifications(Functions.GetID(),DB_Functions.NotificationsType.Received);
                    break;
                case "C":
                    viewModel.Notifications = DB_Functions.ReadNotifications(Functions.GetID(), DB_Functions.NotificationsType.Closed);
                    break;
                case "P":
                    viewModel.Notifications = DB_Functions.ReadNotifications(Functions.GetID(), DB_Functions.NotificationsType.Pending);
                    break;
                case "A":
                    viewModel.Notifications = DB_Functions.ReadNotifications(Functions.GetID(), DB_Functions.NotificationsType.Active);
                    break;
                case "FRD":
                    viewModel.Notifications = DB_Functions.ReadNotifications(Functions.GetID(), DB_Functions.NotificationsType.FRD,G_Functions.GetIdFromHash(frd));
                    break;
            }

                
            return View("NotificationPage", viewModel);
        }

    }
}