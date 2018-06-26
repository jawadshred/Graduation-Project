using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.ViewModels;

namespace turkcell_web_app.Models.Session
{
    public class Functions
    {
        public static bool NoSession()
        {
            if (HttpContext.Current.Session["User_Info"] == null)
                return true;
            return false;
        }

        public static string GetID()
        {
            return ((LogedInEmployee)HttpContext.Current.Session["User_Info"]).UserInfo.Id;
        }
        public static string GetName()
        {
            LogedInEmployee user= (LogedInEmployee)HttpContext.Current.Session["User_Info"];
            return user.UserInfo.Name +" "+ user.UserInfo.Surname;
        }
        public static string GetEmail()
        {
            return ((LogedInEmployee)HttpContext.Current.Session["User_Info"]).UserInfo.E_Mail;
        }
        public static LogedInEmployee GetInfo()
        {
            return ((LogedInEmployee)HttpContext.Current.Session["User_Info"]);
        }

        public static List<Notification> getNotifications()
        {
            if (NoSession())
                return new List<Notification>();
            return DB_Adapters.DB_Functions.Read5Notifications(GetID());
        }
        public static int getNotificationsCount()
        {
            if (NoSession())
                return 0;
            return DB_Adapters.DB_Functions.UnseenNotificationsCount();
        }
        public static bool HasManager()
        {
            return GetInfo().UserInfo.ManagerInfo.Id == "0000" ? false : true;
        }
        public static bool IsManager()
        {
            return GetInfo().IsManager;
        }
    }
}