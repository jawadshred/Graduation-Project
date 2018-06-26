using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.Models.Panels;
using turkcell_web_app.Models.Session;

namespace turkcell_web_app.ViewModels
{
    [Serializable]
    public class Frd
    {
        public enum TypesEnum
        {
            Pending, Active, Closed, ReceivedManagerPending,ReceivedManagerActive, ReceivedUser, Saved,New
        }

        public enum ButtonsEnum
        {
            Submit, Save, Reset, Approve, Reject, Close
        }

        private string ownerId;

        public string OwnerId
        {
            get { return ownerId; }
            set { ownerId = value; }
        }

        private ButtonsEnum submitType;

        public ButtonsEnum SubmitType
        {
            get { return submitType; }
            set { submitType = value; }
        }



        public Frd()
        {
            currentUser = Functions.GetName();
        }
        private string versionNotes;

        public string VersionNotes
        {
            get { return versionNotes; }
            set { versionNotes = value; }
        }


        private string currentUser;

        public string CurrentUser
        {
            get { return currentUser; }
        }

        private int latestVersion;

        public int LatestVersion
        {
            get { return latestVersion; }
            set { latestVersion = value; }
        }

        private TypesEnum type;

        public TypesEnum Type
        {
            get { return type; }
            set { type = value; }
        }



        //private int type;
        //// type of frd ex: 0: pending / 1: active / 2: closed / 3: received as manager /4: received as others
        //public int Type
        //{
        //    get { return type; }
        //    set { type = value; }
        //}

        private string upload_token;

        public string Upload_Token
        {
            get { return upload_token; }
            set { upload_token = value; }
        }

        private Panel_0 panel0;//information

        public Panel_0 Panel0    
        {
            get { return panel0; }
            set { panel0 = value; }
        }

        private Panel_1 panel1;//Items and their comments

        public Panel_1 Panel1
        {
            get { return panel1; }
            set { panel1 = value; }
        }

        private Panel_2 panel2; //Targets

        public Panel_2 Panel2
        {
            get { return panel2; }
            set { panel2 = value; }
        }

        private Panel_3 panel3;//Channels

        public Panel_3 Panel3
        {
            get { return panel3; }
            set { panel3 = value; }
        }

        private Panel_4 panel4;//SMS Panel

        public Panel_4 Panel4
        {
            get { return panel4; }
            set { panel4 = value; }
        }

        private Panel_5 panel5;//Files

        public Panel_5 Panel5
        {
            get { return panel5; }
            set { panel5 = value; }
        }

        private Panel_6 panel6;//Discounts

        public Panel_6 Panel6
        {
            get { return panel6; }
            set { panel6 = value; }
        }

        private Panel_7 panel7;//Confirmations

        public Panel_7 Panel7
        {
            get { return panel7; }
            set { panel7 = value; }
        }




    }
}