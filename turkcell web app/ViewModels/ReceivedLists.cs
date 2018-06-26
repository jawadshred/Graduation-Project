using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.Models;

namespace turkcell_web_app.ViewModels
{
    public class ReceivedLists
    {
        private List<ActiveClosedFRDItem> listAsUser;

        public List<ActiveClosedFRDItem> ListAsUser

        {
            get { return listAsUser; }
            set { listAsUser = value; }
        }


        private List<ActiveClosedFRDItem> listAsManager;

        public List<ActiveClosedFRDItem> ListAsManager
        {
            get { return listAsManager; }
            set { listAsManager = value; }
        }


    }
}