using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace turkcell_web_app.Models.Panels.Classes
{
    public class SmsCode
    {
        private string code;

        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        private string explaination;

        public string Explaination
        {
            get { return explaination; }
            set { explaination = value; }
        }

    }
}