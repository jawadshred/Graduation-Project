using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.Models.Panels.Classes;

namespace turkcell_web_app.Models.Panels
{
    [Serializable]
    public class Panel_6
    {
        private List<Discount> discountItems;

        public List<Discount> DiscountItems
        {
            get { return discountItems; }
            set { discountItems = value; }
        }

        public Panel_6()
        {
            discountItems = new List<Discount>();
        }
    }
}