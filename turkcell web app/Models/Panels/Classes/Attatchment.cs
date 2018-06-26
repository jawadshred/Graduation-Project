using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace turkcell_web_app.Models.Panels.Classes
{
    [Serializable]
    public class Attatchment
    {
        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private DateTime date;

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        private int version;

        public int Version
        {
            get { return version; }
            set { version = value; }
        }

        private string owner;

        public string Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        private string size;

        public string Size
        {
            get { return size; }
            set { size = value; }
        }


    }
}