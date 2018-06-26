using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using turkcell_web_app.Models.Panels.Classes;

namespace turkcell_web_app.Models.Panels
{
    [Serializable]
    public class Panel_0
    {
        private string id;
        [Required]
        public string Id
        {
            get { return id; }
            set { id = value;
                if (id!=null) //in case RESET is pressed and no ID was entered
                {
                Hashed_Id = Convert.ToBase64String(ASCIIEncoding.UTF8.GetBytes(value));
                }
            }
        }

        private string title;
        [Required]
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

        private List<FrdVersion> versions;

        public List<FrdVersion> Versions
        {
            get { return versions; }
            set { versions = value; }
        }

        private int latestVersion;

        public int LatestVersion
        {
            get { return latestVersion; }
            set { latestVersion = value; }
        }

        public string Hashed_Id;
    }
}