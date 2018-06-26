using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace turkcell_web_app.Models.Panels.Classes
{
    [Serializable]
    public class Item
    {
        private string itemId;

        public string ItemId
        {
            get { return itemId; }
            set { itemId = value; }
        }

        private List<Comment> comments;

        public List<Comment> Comments
        {
            get { return comments; }
            set { comments = value; }
        }

        private double versionNo;

        public double VersionNo
        {
            get { return versionNo; }
            set { versionNo = value; }
        }

        private DateTime date;

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

    }
}