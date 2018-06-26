using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.Models;

namespace turkcell_web_app.ViewModels
{
    public class LogedInEmployee 
    {
        private Employee userInfo;

        public Employee UserInfo
        {
            get { return userInfo; }
            set { userInfo = value; }
        }

      


        private int activeFrd;

        public int ActiveFrd
        {
            get { return activeFrd; }
            set { activeFrd = value; }
        }

        private bool activeNoti;

        public bool ActiveNoti
        {
            get { return activeNoti; }
            set { activeNoti = value; }
        }


        private int closedFrd;

        public int ClosedFrd
        {
            get { return closedFrd; }
            set { closedFrd = value; }
        }

        private bool closedNoti;

        public bool ClosedNoti
        {
            get { return closedNoti; }
            set { closedNoti = value; }
        }


        private int receivedFrds;

        public int RecievedFrds
        {
            get { return receivedFrds; }
            set { receivedFrds = value; }
        }

        private bool receivedNoti;

        public bool ReceivedNoti
        {
            get { return receivedNoti; }
            set { receivedNoti = value; }
        }

        private int pendingFrds;

        public int PendingFRDs
        {
            get { return pendingFrds; }
            set { pendingFrds = value; }
        }

        private bool isManager;

        public bool IsManager
        {
            get { return isManager; }
            set { isManager = value; }
        }

        private int managerReceivedFrds;

        public int ManagerReceivedFRDs
        {
            get { return managerReceivedFrds; }
            set { managerReceivedFrds = value; }
        }
        private bool mRnoti;

        public bool MRNoti
        {
            get { return mRnoti; }
            set { mRnoti = value; }
        }



    }
}