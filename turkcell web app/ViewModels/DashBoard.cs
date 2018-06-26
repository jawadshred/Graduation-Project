using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.Models;
namespace turkcell_web_app.ViewModels
{
    public class DashBoard
    {
        private List<ManagerDashBoardData.MyEmployee> list;

        public List<ManagerDashBoardData.MyEmployee> List
        {
            get { return list; }
            set { list = value; }
        }

    }
}