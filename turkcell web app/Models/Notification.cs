using System;

namespace turkcell_web_app.Models
{
    public class Notification 
    {
        public enum FrdType { Active=1,Received,Closed,Pending}
        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; }
        }


        private string id;

        public string ID
        {
            get { return id; }
            set { id = value; }
        }
        private int version;

        public int Version
        {
            get { return version; }
            set { version = value; }
        }

        private string frdTitle;

        public string FrdTitle
        {
            get { return frdTitle; }
            set { frdTitle = value; }
        }
        private DateTime time;

        public DateTime NotiTime
        {
            get { return time; }
            set { time = value; }
        }


        private bool seen;

        public bool Seen
        {
            get { return seen; }
            set { seen = value; }
        }


        private FrdType frdType;

        public FrdType FRDType
        {
            get { return frdType; }
            set { frdType = value; }
        }

        private String frdId;

        public String FrdId
        {
            get { return frdId; }
            set { frdId = value; }
        }

    }
}