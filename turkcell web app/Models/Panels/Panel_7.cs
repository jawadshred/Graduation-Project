using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.Models.Panels.Classes;

namespace turkcell_web_app.Models.Panels
{
    [Serializable]
    public class Panel_7
    {

        //do we need these???
        //private List<Confirmation> confirmations;

        //public List<Confirmation> Confirmations
        //{
        //    get { return confirmations; }
        //    set { confirmations = value; }
        //}

        private List<Distribution_Groups> distributionGroups;
        public List<Distribution_Groups> DistributionGroups
        {
            get { return distributionGroups; }
            set { distributionGroups = value; }
        }

    }
}