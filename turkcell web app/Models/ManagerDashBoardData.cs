using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.ViewModels;
namespace turkcell_web_app.Models
{
    public class ManagerDashBoardData
    {

        private List<MyEmployee> employees;

        public List<MyEmployee> Employees
        {
            get { return employees; }
            set { employees = value; }
        }


        public class MyEmployee : LogedInEmployee
        {
            
        }

    }
}