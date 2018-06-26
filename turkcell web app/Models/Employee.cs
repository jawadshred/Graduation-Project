using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace turkcell_web_app.Models
{
    [Serializable]
    public class Employee
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string surname;

        public string Surname
        {
            get { return surname; }
            set { surname = value; }
        }


        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        private string email;

        public string E_Mail
        {
            get { return email; }
            set { email = value; }
        }
        private string department;

        public string Department
        {
            get { return department; }
            set { department = value; }
        }

        private bool selected;

        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        private Manager manager;

        public Manager ManagerInfo
        {
            get { return manager; }
            set { manager = value; }
        }

    }
}