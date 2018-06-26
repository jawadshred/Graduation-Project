using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace turkcell_web_app.Models.Panels.Classes
{
    [Serializable]
    public class Comment
    {
        private int versionNo;

        public int VersionNo
        {
            get { return versionNo; }
            set { versionNo = value; }
        }
        private Reviser commentor;

        public Reviser Commentor
        {
            get { return commentor; }
            set { commentor = value; }
        }
        private DateTime date;

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }


        private string commentId;

        public string CommentId
        {
            get { return commentId; }
            set { commentId = value; }
        }


        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        








    }
}