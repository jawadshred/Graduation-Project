using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace turkcell_web_app.Models.Panels.Classes
{
    [Serializable]
    public class SMS
    {
        private string sender;

        public string Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        private string code;

        public string Code
        {
            get { return code; }
            set { code = value; }
        }


        private string explanation;

        public string Explanation
        {
            get { return explanation; }
            set { explanation = value; }
        }

        private string en_content;

        public string EnContent
        {
            get { return en_content; }
            set { en_content = value; }
        }
        private string tr_content;

        public string TrContent
        {
            get { return tr_content; }
            set { tr_content = value; }
        }

        private double version;

        public double Version
        {
            get { return version; }
            set { version = value; }
        }

        private Reviser reviser;

        public Reviser AddedBy
        {
            get { return reviser; }
            set { reviser = value; }
        }

        private DateTime time;

        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }

    }
}