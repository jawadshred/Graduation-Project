using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace turkcell_web_app.Models.Panels.Classes
{
    [Serializable]
    public class Discount
    {
        private string discountName;

        public string DiscountName
        {
            get { return discountName; }
            set { discountName = value; }
        }
        private string discountCode;

        public string DiscountCode
        {
            get { return discountCode; }
            set { discountCode = value; }
        }

        private int version;

        public int Version
        {
            get { return version; }
            set { version = value; }
        }


    }
}