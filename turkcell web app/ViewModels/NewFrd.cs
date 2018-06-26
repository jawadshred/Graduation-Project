using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.Models.Panels;
using turkcell_web_app.Models.Panels.Classes;

namespace turkcell_web_app.ViewModels
{
    public class NewFrd
    {

        private string frdNo;

        public string FrdNo
        {
            get { return frdNo; }
            set { frdNo = value; }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string owner;

        public string Owner
        {
            get { return owner; }
            set { owner = value; }
        }



        private Panel_1 secondPanel;

        public Panel_1 SecondPanel
        {
            get { return secondPanel; }
            set { secondPanel = value; }
        }

        private Panel_2 thirdPanel;

        public Panel_2 ThirdPanel
        {
            get { return thirdPanel; }
            set { thirdPanel = value; }
        }



        private Panel_3 fourthPanel;

        public Panel_3 FourthPanel
        {
            get { return fourthPanel; }
            set { fourthPanel = value; }
        }

        private Panel_4 fifthPanel;

        public Panel_4 FifthPanel
        {
            get { return fifthPanel; }
            set { fifthPanel = value; }
        }

        private Panel_5 sixthPanel;

        public Panel_5 SixthPanel
        {
            get { return sixthPanel; }
            set { sixthPanel = value; }
        }


        private Panel_6 seventhPanel;

        public Panel_6 SeventhPanel
        {
            get { return seventhPanel; }
            set { seventhPanel = value; }
        }

        private Panel_7 eightPanel;

        public Panel_7 EigthPanel
        {
            get { return eightPanel; }
            set { eightPanel = value; }
        }







        private List<TargetAudience> targets_list;
        public List<TargetAudience> Targets_List
        {
            get { return targets_list; }
            set { targets_list = value; }
        }

        private List<Channel> channels_list;
        public List<Channel> Channels_List
        {
            get { return channels_list; }
            set { channels_list = value; }
        }

        private List<Distribution_Groups> distributionGroups;
        public List<Distribution_Groups> DistributionGroups
        {
            get { return distributionGroups; }
            set { distributionGroups = value; }
        }





    }
}