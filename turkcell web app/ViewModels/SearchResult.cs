using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.Models;
namespace turkcell_web_app.ViewModels
{
    public class SearchResult
    {

        private string searched;

        public string Searched
        {
            get { return searched; }
            set { searched = value; }
        }

        private List<SearchItem> list;

        public List<SearchItem> List
        {
            get { return list; }
            set { list = value; }
        }

    }
}