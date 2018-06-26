using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.Models.Panels.Classes;

namespace turkcell_web_app.Models.Panels
{
    [Serializable]
    public class Panel_4
    {
       
        private List<SMS> sms;

        public List<SMS> SMS
        {
            get { return sms; }
            set { sms = value; }
        }
        private List<Sender> senders;

        public List<Sender> Senders
        {
            get { return senders; }
            set { senders = value; }
        }


    }
}