using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace turkcell_web_app.Models.Panels.Classes
{
    [Serializable]
    public class Sender
    {
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

    }
}