using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.Models.Panels.Classes;

namespace turkcell_web_app.Models.Panels
{
    [Serializable]
    public class Panel_1
    {
        private List<Item> items;

        public List<Item> Items
        {
            get { return items; }
            set { items = value; }
        }

        


    }
}