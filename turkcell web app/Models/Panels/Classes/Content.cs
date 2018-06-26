using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace turkcell_web_app.Models.Panels.Classes
{
    [JsonObject]
    [Serializable]
    public class Content
    {
        private string english;

        public string English
        {
            get { return english; }
            set { english = value; }
        }
        private string turkish;

        public string Turkish
        {
            get { return turkish; }
            set { turkish = value; }
        }

    }
}