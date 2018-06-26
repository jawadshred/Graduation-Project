using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.Models.Panels.Classes;

namespace turkcell_web_app.Models.Panels
{
    [Serializable]
    public class Panel_2
    {
        private List<TargetAudience> targets;

        public List<TargetAudience> Targets
        {
            get { return targets; }
            set { targets = value; }
        }

        



    }
}