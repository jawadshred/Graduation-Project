using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace turkcell_web_app.Models.Panels.Classes
{
    [Serializable]
    public class Confirmation : Employee
    {
        public enum Status_Enum  { Approved , Waiting , Rejected};
        private Status_Enum status;

        public Status_Enum Status
        {
            get { return status; }
            set { status = value; }
        }
        private DateTime lastUpdate;

        public DateTime LastUpdate
        {
            get { return lastUpdate; }
            set { lastUpdate = value; }
        }



    }

}