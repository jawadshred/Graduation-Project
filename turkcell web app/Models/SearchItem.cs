using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace turkcell_web_app.Models
{
    public class SearchItem
    {
        private string frdId;

        public string FrdId
        {
            get { return frdId; }
            set { frdId = value; }
        }
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private List<Version_Result> v_list;

        public List<Version_Result> V_List
        {
            get { return v_list; }
            set { v_list = value; }
        }

        private List<Item_Result> i_list;

        public List<Item_Result> I_List
        {
            get { return i_list; }
            set { i_list = value; }
        }




        public class Version_Result
        {
            private int version;

            public int Version
            {
                get { return version; }
                set { version = value; }
            }
            private string note;

            public string Note
            {
                get { return note; }
                set { note = value; }
            }


        }
        public class Item_Result
        {
            private int version;

            public int Version
            {
                get { return version; }
                set { version = value; }
            }

            private string itemText;

            public string ItemText
            {
                get { return itemText; }
                set { itemText = value; }
            }

        }


    }
}