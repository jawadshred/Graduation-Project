using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.Models;

namespace turkcell_web_app.ViewModels
{
    public class NotificationPage
    {
        private List<Notification> notifications;

        public List<Notification> Notifications
        {
            get { return notifications; }
            set { notifications = value; }
        }

    }
}