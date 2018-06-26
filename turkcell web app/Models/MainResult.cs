using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace turkcell_web_app.Models
{
    public abstract class MainResult
    {
        private int result;
        public int Result
        {
            get { return result; }
            set { result = value; }
        }

        private string error;
        public string Error
        {
            get { return error; }
            set { error = value; }
        }
    }
}