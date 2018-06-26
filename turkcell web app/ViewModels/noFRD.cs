using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace turkcell_web_app.ViewModels
{
    public class NoFRD
    {
        private string pagetitle;

        public string PageTitle
        {
            get { return pagetitle; }
            set { pagetitle = value; }
        }


        private string heading;

        public string Heading
        {
            get { return heading; }
            set { heading = value; }
        }
        private string primaryParagraph;

        public string PrimaryParagraph
        {
            get { return primaryParagraph; }
            set { primaryParagraph = value; }
        }

        private string secondaryParagraph;

        public string SecondaryParagraph
        {
            get { return secondaryParagraph; }
            set { secondaryParagraph = value; }
        }
        private string lastParagraph;

        public string LastParagraph
        {
            get { return lastParagraph; }
            set { lastParagraph = value; }
        }

        private string buttonText;

        public string ButtonText
        {
            get { return buttonText; }
            set { buttonText = value; }
        }

        private string buttonLink;

        public string ButtonLink
        {
            get { return buttonLink; }
            set { buttonLink = value; }
        }

        public enum menuitem { Home,Active,Pending,ClosedAll,ClosedMe,ClosedOthers,ReceivedU,ReceivedM,ManagerBoard}

        private menuitem page;

        public menuitem Page
        {
            get { return page; }
            set { page = value; }
        }

    }
}