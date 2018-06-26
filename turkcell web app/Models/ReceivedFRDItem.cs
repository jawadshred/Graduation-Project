using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace turkcell_web_app.Models
{
    public class ReceivedFRDItem
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
        private DateTime received;

        public DateTime Received
        {
            get { return received; }
            set { received = value; }
        }

        private DateTime lastUpdate;

        public DateTime LastUpdate
        {
            get { return lastUpdate; }
            set { lastUpdate = value; }
        }
        private string ownerName;

        public string OwnerName
        {
            get { return ownerName; }
            set { ownerName = value; }
        }
        private string ownerId;

        public string OwnerID
        {
            get { return ownerId; }
            set { ownerId = value; }
        }


    }
}