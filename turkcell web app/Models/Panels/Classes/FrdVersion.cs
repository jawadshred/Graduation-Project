using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace turkcell_web_app.Models.Panels.Classes
{
    [Serializable]
    public class FrdVersion
    {
        private int number;

        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        private DateTime date;

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        private Reviser reviserinfo;

        public Reviser ReviserInfo
        {
            get { return reviserinfo; }
            
            set { reviserinfo = value; }
        }

        private string details;

        public string Details
        {
            get { return details; }
            set { details = value; }
        }

        private string explanation;

        public string Explanation
        {
            get { return explanation; }
            set { explanation = value; }
        }


    }
}