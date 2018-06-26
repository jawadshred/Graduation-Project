using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace turkcell_web_app.Models
{
    public class ActiveClosedFRDItem
    {
        private string id;

        public string ID
        {
            get { return id; }
            set { id = value; }
        }
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private int version;

        public int Version
        {
            get { return version; }
            set { version = value; }
        }
        private DateTime created;

        public DateTime Created
        {
            get { return created; }
            set { created = value; }
        }

        private DateTime lastUpdate;

        public DateTime LastUpdate
        {
            get { return lastUpdate; }
            set { lastUpdate = value; }
        }

        private int noti;
        public int Notification
        {
            get { return noti; }
            set { noti = value; }
        }

        private Employee employee;

        public Employee Owner
        {
            get { return employee; }
            set { employee = value; }
        }


        private string hashed_id;

        public string Hashed_ID
        {
            get { return hashed_id; }
            set { hashed_id = value; }
        }
        private int type;

        public int Type
        {
            get { return type; }
            set { type = value; }
        }



    }
}