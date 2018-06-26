using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.Models.Panels.Classes;

namespace turkcell_web_app.Models.Panels
{
    [Serializable]
    public class Panel_3
    {
        private List<Channel> channels;

        public List<Channel> Channels
        {
            get { return channels; }
            set { channels = value; }
        }

    }
}