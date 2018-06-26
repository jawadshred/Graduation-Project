using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.Models.Panels.Classes;

namespace turkcell_web_app.Models.Panels
{
    [Serializable]
    public class Panel_5
    {
        private List<Attatchment> attatchments;

        public List<Attatchment> Attatchments
        {
            get { return attatchments; }
            set { attatchments = value; }
        }
        public Panel_5()
        {
            attatchments = new List<Attatchment>();
        }
    }
}