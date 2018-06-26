using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.Models;

namespace turkcell_web_app.ViewModels
{
    public class ReceivedFRDs
    {
        private List<ActiveClosedFRDItem> list;

        public List<ActiveClosedFRDItem> List
        {
            get { return list; }
            set { list = value; }
        }

    }
}