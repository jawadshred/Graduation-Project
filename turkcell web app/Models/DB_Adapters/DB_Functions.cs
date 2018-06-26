using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using turkcell_web_app.Models.Panels.Classes;
using turkcell_web_app.ViewModels;
using System.Text;
using System.IO;
using turkcell_web_app.Models.Panels;
using turkcell_web_app.Models.FRD_Process;

namespace turkcell_web_app.Models.DB_Adapters
{
    public class DB_Functions
    {
        static OracleConnection MyConnection = new OracleConnection("Data Source=127.0.0.1:1521/xe;Persist Security Info=True;User ID=JAWAD;Password=123456");
        static OracleCommand MyCommand;
        static OracleDataAdapter MyAdapter;
        static OracleTransaction MyTransaction;
        public enum UploadTokenType { New, Received, Active }



        //Login Function
        public static Login LogIn(String EMAIL, String PASS)
        {

            Login R = new Login();
            MyCommand = new OracleCommand(
                "SELECT U.ID,U.E_MAIL,U.NAME,U.SURNAME,D.NAME AS D_NAME,M.ID AS M_ID,M.NAME AS M_NAME, M.SURNAME AS M_SURNAME " +
                "FROM USERS U JOIN DEPARTMENTS D ON U.DEPARTMENT=D.ID  LEFT OUTER JOIN USERS M ON U.MANAGER_ID= M.ID" +
                " WHERE U.E_MAIL=:e_mail AND U.STATUS='E' AND U.PASSWORD = :pass", MyConnection);
            MyCommand.Parameters.Add(":e_mail", EMAIL.ToLower());
            MyCommand.Parameters.Add(":pass", G_Functions.GenerateSHA256String(PASS));
            string password = G_Functions.GenerateSHA256String(PASS);
            MyCommand.BindByName = true;
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.LogIn_InfoDataTable table = new Main_DB.LogIn_InfoDataTable();
            MyAdapter.Fill(table);
            if (table.Count() == 1)
            {
                var firstrow = table.First();
                LogedInEmployee user = new LogedInEmployee
                {
                    UserInfo = new Employee
                    {
                        Id = firstrow.ID,
                        Name = firstrow.NAME,
                        Surname = firstrow.SURNAME,
                        Department = firstrow.D_NAME,
                        E_Mail = firstrow.E_MAIL,
                        ManagerInfo = new Manager
                        {
                            Id = firstrow.M_ID ?? "0000",
                            Name = firstrow.M_NAME ?? "No",
                            Surname = firstrow.M_SURNAME ?? "Manager"
                        }
                    },

                };
                HttpContext.Current.Session.Add("User_Info", user);
                return R;
            }
            else
            {
                HttpContext.Current.Session.Clear();
                return R;
            }
        }




        //HomePage & User Info
        public static LogedInEmployee GetUserData()
        {
            LogedInEmployee user = (LogedInEmployee)HttpContext.Current.Session["User_Info"];

            MyCommand = new OracleCommand(
                "SELECT ACOUNT.ACO A_COUNT,ANOTI.ANCO AS A_NOTI,CCOUNT.CCO C_COUNT,CNOTI.CNCO AS C_NOTI,RCOUNT.RCO R_COUNT,RNOTI.RNCO AS R_NOTI,PCOUNT.PCO  P_COUNT,RECEIVED_MANAGER_COUNT.M_R_COUNT , MC.MANAGER_COUNT " +
                "FROM (SELECT COUNT(ID) AS ACO FROM FRDOCUMENT WHERE STATUS='A' AND USER_ID=:id) ACOUNT," +
                "(SELECT COUNT(N.ID) ANCO FROM NOTIFICATION N JOIN FRDOCUMENT F ON F.USER_ID=:id AND F.ID=N.FRD_ID AND N.USER_ID = F.USER_ID WHERE N.USER_ID=:id AND N.SEEN!='S' AND F.STATUS='A') ANOTI," +
                " (SELECT COUNT(FRD.ID) AS CCO FROM FRDOCUMENT FRD LEFT OUTER JOIN FRD_GROUP_MEMBERS FGM ON FRD.ID=FGM.FRD_ID AND FGM.MEMBER_ID=:id  WHERE FRD.STATUS='C' AND (FRD.USER_ID=:id OR FGM.MEMBER_ID=:id)) CCOUNT," +
                "(SELECT COUNT(N.ID) CNCO FROM NOTIFICATION N JOIN FRDOCUMENT F ON F.USER_ID=:id AND F.ID=N.FRD_ID WHERE N.USER_ID=:id AND N.SEEN!='S' AND F.STATUS='C') CNOTI," +
                "(SELECT COUNT(R.FRD_ID) AS RCO FROM CONFIRMATIONS R,FRDOCUMENT FRD WHERE FRD.ID=R.FRD_ID AND R.VERSION=FRD.LATEST_V AND FRD.STATUS<>'C'  AND R.EMPLOYEE_ID=:id AND R.GROUP_ID!='0000') RCOUNT," +
                "(SELECT COUNT(N.ID) RNCO FROM NOTIFICATION N INNER JOIN FRDOCUMENT F ON N.USER_ID=:id AND F.ID=N.FRD_ID INNER JOIN FRD_GROUP_MEMBERS FGM ON FGM.FRD_ID=F.ID AND FGM.GROUP_ID!='0000' AND FGM.MEMBER_ID=:id WHERE  N.SEEN!='S' AND F.STATUS='A' AND F.USER_ID!=N.USER_ID) RNOTI, " +
                "(SELECT COUNT(N.ID) RNCO FROM NOTIFICATION N INNER JOIN FRDOCUMENT F ON N.USER_ID=:id AND F.ID=N.FRD_ID INNER JOIN FRD_GROUP_MEMBERS FGM ON FGM.FRD_ID=F.ID AND FGM.GROUP_ID='0000' AND FGM.MEMBER_ID=:id WHERE  N.SEEN!='S' AND F.STATUS!='C' AND F.USER_ID!=N.USER_ID) MRNOTI, " +
                "(SELECT COUNT(ID) AS PCO FROM FRDOCUMENT WHERE  STATUS='N' AND USER_ID=:id) PCOUNT," +
                "(SELECT COUNT(N.ID) PNCO FROM NOTIFICATION N JOIN FRDOCUMENT F ON F.USER_ID=:id AND F.ID=N.FRD_ID WHERE N.USER_ID=:id AND N.SEEN!='S' AND F.STATUS='N') PNOTI," +
                "(SELECT COUNT(FRD.ID) M_R_COUNT FROM FRDOCUMENT FRD JOIN FRD_GROUP_MEMBERS FGM ON FRD.ID=FGM.FRD_ID  AND FGM.GROUP_ID='0000' AND FGM.MEMBER_ID=:id AND FRD.STATUS!='C') RECEIVED_MANAGER_COUNT," +
                "(SELECT COUNT(U.ID) AS MANAGER_COUNT FROM USERS U WHERE U.MANAGER_ID=:id) MC", MyConnection);
            MyCommand.Parameters.Add("id", user.UserInfo.Id);
            MyCommand.BindByName = true;
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.FRD_COUNTSDataTable COUNTS = new Main_DB.FRD_COUNTSDataTable();
            try
            {
                MyAdapter.Fill(COUNTS);
            }
            catch (Exception)
            {
                user.ActiveFrd = 0;
                user.ActiveNoti = false;
                user.ClosedFrd = 0;
                user.RecievedFrds = 0;
                user.PendingFRDs = 0;
                return user;
            }
            if (COUNTS.Count == 1)
            {
                user.ActiveFrd = COUNTS[0].A_COUNT;
                user.ActiveNoti = COUNTS[0].A_NOTI != 0;
                user.ClosedFrd = COUNTS[0].C_COUNT;
                user.ClosedNoti = COUNTS[0].C_NOTI != 0;
                user.RecievedFrds = COUNTS[0].R_COUNT;
                user.ReceivedNoti = COUNTS[0].R_NOTI != 0;
                user.PendingFRDs = COUNTS[0].P_COUNT;
                user.ManagerReceivedFRDs = COUNTS[0].M_R_COUNT;
                user.IsManager = COUNTS[0].MANAGER_COUNT > 0;
                user.MRNoti = COUNTS[0].M_R_COUNT > 0;
            }
            return user;
        }




        public static List<ManagerDashBoardData.MyEmployee> GetManagerData()
        {
            List<ManagerDashBoardData.MyEmployee> list = new List<ManagerDashBoardData.MyEmployee>();
            MyCommand = new OracleCommand("SELECT U.ID,U.E_MAIL,U.NAME,U.SURNAME FROM USERS U  WHERE U.STATUS='E' AND U.MANAGER_ID=:id", MyConnection);
            MyCommand.Parameters.Add("id", Session.Functions.GetID());
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.LogIn_InfoDataTable table = new Main_DB.LogIn_InfoDataTable();
            MyAdapter.Fill(table);//get users list

            foreach(var user in table)
            {
                var counts = GetUserCounts(user.ID);
                list.Add(new ManagerDashBoardData.MyEmployee {
                    ActiveFrd = counts.ActiveFrd,
                    ClosedFrd= counts.ClosedFrd,
                    PendingFRDs= counts.PendingFRDs,
                    RecievedFrds= counts.RecievedFrds,

                    UserInfo = new Employee {
                        Id = user.ID,
                        E_Mail = user.E_MAIL,
                        Name = user.NAME,
                        Surname = user.SURNAME }
                });
            }

            return list;
        }


        public static ManagerDashBoardData.MyEmployee GetUserCounts(string id)
        {
            ManagerDashBoardData.MyEmployee user = new ManagerDashBoardData.MyEmployee();

            MyCommand = new OracleCommand(
                "SELECT ACOUNT.ACO A_COUNT,CCOUNT.CCO C_COUNT,RCOUNT.RCO R_COUNT,PCOUNT.PCO  P_COUNT " +
                "FROM (SELECT COUNT(ID) AS ACO FROM FRDOCUMENT WHERE STATUS='A' AND USER_ID=:id) ACOUNT," +
                " (SELECT COUNT(FRD.ID) AS CCO FROM FRDOCUMENT FRD LEFT OUTER JOIN FRD_GROUP_MEMBERS FGM ON FRD.ID=FGM.FRD_ID AND FGM.MEMBER_ID=:id  WHERE FRD.STATUS='C' AND (FRD.USER_ID=:id OR FGM.MEMBER_ID=:id)) CCOUNT," +
                "(SELECT COUNT(R.FRD_ID) AS RCO FROM FRD_GROUP_MEMBERS R,FRDOCUMENT FRD WHERE FRD.ID=R.FRD_ID AND FRD.STATUS<>'C'  AND R.MEMBER_ID=:id AND R.GROUP_ID!='0000') RCOUNT," +
                "    (SELECT COUNT(ID) AS PCO FROM FRDOCUMENT WHERE  STATUS='N' AND USER_ID=:id) PCOUNT", MyConnection);
            MyCommand.Parameters.Add("id", id);
            MyCommand.BindByName = true;
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.FRD_COUNTSDataTable COUNTS = new Main_DB.FRD_COUNTSDataTable();
            try
            {
                MyAdapter.Fill(COUNTS);
            }
            catch (Exception)
            {
                user.ActiveFrd = 0;
                //user.ActiveNoti = false;
                user.ClosedFrd = 0;
                user.RecievedFrds = 0;
                user.PendingFRDs = 0;
                return user;
            }
            if (COUNTS.Count == 1)
            {
                user.ActiveFrd = COUNTS[0].A_COUNT;
                //user.ActiveNoti = COUNTS[0].A_NOTI != 0;
                user.ClosedFrd = COUNTS[0].C_COUNT;
                //user.ClosedNoti = COUNTS[0].C_NOTI != 0;
                user.RecievedFrds = COUNTS[0].R_COUNT;
                //user.ReceivedNoti = COUNTS[0].R_NOTI != 0;
                user.PendingFRDs = COUNTS[0].P_COUNT;
                user.ManagerReceivedFRDs = COUNTS[0].M_R_COUNT;
                //user.IsManager = COUNTS[0].MANAGER_COUNT > 0;
            }
            return user;
        }

        //=======================================================================================================
        //===========================================================================
        //===================================================




        // Get Defaults
        public static List<TargetAudience> DefaultTargets()
        {
            List<TargetAudience> list = new List<TargetAudience>();
            MyCommand = new OracleCommand("SELECT NAME,CODE,ID,DATE_ADDED FROM TARGET_AUDIENCE ORDER BY NAME", MyConnection);
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.TARGET_AUDIENCEDataTable COUNTS = new Main_DB.TARGET_AUDIENCEDataTable();
            try
            {
                MyAdapter.Fill(COUNTS);
            }
            catch (Exception)
            {
                return list;
            }
            foreach (Main_DB.TARGET_AUDIENCERow x in COUNTS)
            {
                list.Add(new TargetAudience { Name = x.NAME, Code = x.CODE });
            }
            return list;
        }





        public static List<Channel> DefaultChannels()
        {
            List<Channel> list = new List<Channel>();

            MyCommand = new OracleCommand("SELECT NAME,CODE,ID,DATE_ADDED FROM CHANNELS ORDER BY NAME", MyConnection);
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.CHANNELSDataTable COUNTS = new Main_DB.CHANNELSDataTable();
            try
            {
                MyAdapter.Fill(COUNTS);
            }
            catch (Exception)
            {
                return list;
            }
            foreach (Main_DB.CHANNELSRow x in COUNTS)
            {
                list.Add(new Channel { Name = x.NAME, Code = x.CODE });
            }
            return list;
        }




        public static List<Distribution_Groups> DefaultGroups()
        {
            List<Distribution_Groups> list = new List<Distribution_Groups>();
            MyAdapter = new OracleDataAdapter("SELECT DG.ID AS GROUP_ID,DG.GROUP_NAME,GM.USER_ID,U.NAME AS USER_NAME,U.SURNAME AS USER_SURNAME FROM DISTRIBUTION_GROUPS DG JOIN GROUP_MEMBERS GM ON DG.ID=GM.GROUP_ID JOIN USERS U ON U.ID = GM.USER_ID ORDER BY DG.GROUP_NAME,DG.ID", MyConnection);
            Main_DB.GROUPSDataTable data = new Main_DB.GROUPSDataTable();
            MyAdapter.Fill(data);

            string groupID = null;

            foreach (var x in data)
            {
                if (groupID == null || !groupID.Equals(x.GROUP_ID))
                {
                    groupID = x.GROUP_ID;
                    list.Add(new Distribution_Groups
                    {
                        Id = x.GROUP_ID,
                        Name = x.GROUP_NAME,
                        Employess = new List<Confirmation> { new Confirmation { Id = x.USER_ID, Name = x.USER_NAME, Surname = x.USER_SURNAME } }
                    });

                }
                else
                {
                    list[list.Count - 1].Employess.Add(new Confirmation { Id = x.USER_ID, Name = x.USER_NAME, Surname = x.USER_SURNAME });
                }


            }

            return list;
        }



        public static List<Sender> DefaultSmsSenders()
        {
            List<Sender> list = new List<Sender>();
            MyCommand = new OracleCommand(
                            "SELECT UNIQUE(SENDER) FROM SMS ORDER BY SENDER ASC", MyConnection);
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.SENDERSDataTable table = new Main_DB.SENDERSDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception)
            {
                return list;
            }
            int I = 0;
            foreach (var row in table)
            {
                list.Add(
                    new Sender
                    {
                        Title = row.SENDER,
                        Id = I++
                    }
                );
            }
            return list;
        }





        public static List<SmsCode> DefaultSmsCodes(string Sender)
        {
            List<SmsCode> list = new List<SmsCode>();
            MyCommand = new OracleCommand(
                            "SELECT CODE,EXPLANATION FROM SMS WHERE SENDER=:sender ORDER BY CODE ASC", MyConnection);
            MyCommand.Parameters.Add("sender", Sender);
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.CodesDataTable table = new Main_DB.CodesDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception)
            {
                return list;
            }
            int I = 0;
            foreach (var row in table)
            {
                list.Add(
                    new SmsCode
                    {
                        Code = row.CODE,
                        Explaination = row.EXPLANATION
                    }
                    );
            }
            return list;
        }



        public static Content DefaultSmsContent(string Sender, string Code)
        {
            Content d = new Content { English = " ", Turkish = " " };
            MyCommand = new OracleCommand(
                            "SELECT CONTENT_EN,CONTENT_TR FROM SMS WHERE SENDER=:sender AND CODE=:code", MyConnection);
            MyCommand.Parameters.Add("sender", Sender);
            MyCommand.Parameters.Add("code", Code);
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.SMS_ContentDataTable table = new Main_DB.SMS_ContentDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception)
            {
                return d;
            }
            if (table.Count == 1)
            {
                d.English = table[0].CONTENT_EN;
                d.Turkish = table[0].CONTENT_TR;
            }
            return d;
        }



        //=======================================================================================================
        //===========================================================================
        //===================================================


        //Get Lists

        public static List<ActiveClosedFRDItem> ReceivedListAsUser(string UserID=null)
        {

            List<ActiveClosedFRDItem> listAsUser = new List<ActiveClosedFRDItem>();
            MyCommand = new OracleCommand(
                "SELECT FRD.ID,FRD.TITLE,FRD.LATEST_V AS VERSION,U.NAME,U.SURNAME,V1.DATE_CREATED AS DATE_CREATED,V2.DATE_CREATED AS LAST_UPDATE,COUNT(N.ID)AS N_COUNT FROM FRD_GROUP_MEMBERS R,FRDOCUMENT FRD INNER JOIN VERSIONS V1 ON V1.FRD_ID = FRD.ID AND V1.VERSION=1 LEFT OUTER JOIN NOTIFICATION N ON N.FRD_ID=FRD.ID AND N.USER_ID = :id AND N.SEEN!='S',VERSIONS V2 ,USERS U  WHERE R.MEMBER_ID=:id AND V2.FRD_ID=FRD.ID AND V2.VERSION= FRD.LATEST_V AND FRD.ID=R.FRD_ID AND FRD.STATUS<>'C' AND U.ID=FRD.USER_ID  AND R.ENABLED=1 AND NOT (R.GROUP_ID='0000') group by FRD.ID, FRD.TITLE, FRD.LATEST_V, U.NAME, U.SURNAME, V1.DATE_CREATED, V2.DATE_CREATED ORDER BY V2.DATE_CREATED  DESC",
                MyConnection);
            MyCommand.Parameters.Add("id", UserID??Models.Session.Functions.GetID());
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.ReceivedFRDsDataTable table = new Main_DB.ReceivedFRDsDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception)
            {
                return listAsUser;
            }
            foreach (var row in table)
            {
                ActiveClosedFRDItem item = new ActiveClosedFRDItem
                {
                    ID = row.ID,
                    Title = row.TITLE,
                    Created = row.DATE_CREATED,
                    LastUpdate = row.LAST_UPDATE,
                    Notification = row.N_COUNT,
                    Version = row.VERSION,
                    Owner = new Employee { Name = row.NAME, Surname = row.SURNAME },
                    Hashed_ID = G_Functions.GetHash(row.ID)
                };
                listAsUser.Add(item);
            }

            return listAsUser;

        }

        public static List<ActiveClosedFRDItem> ReceivedListAsManager()
        {
            List<ActiveClosedFRDItem> listAsManager = new List<ActiveClosedFRDItem>();
            MyCommand = new OracleCommand(
                "SELECT FRD.ID,FRD.TITLE,FRD.LATEST_V AS VERSION,U.NAME,U.SURNAME,V1.DATE_CREATED AS DATE_CREATED,V2.DATE_CREATED AS LAST_UPDATE,COUNT(N.ID)AS N_COUNT " +
                "FROM FRD_GROUP_MEMBERS R," +
                "FRDOCUMENT FRD  INNER JOIN VERSIONS V1 ON V1.FRD_ID = FRD.ID AND V1.VERSION=1 " +
                "LEFT OUTER JOIN NOTIFICATION N ON N.FRD_ID=FRD.ID AND N.USER_ID = :id AND N.SEEN!='S',VERSIONS V2 ,USERS U  " +
                "WHERE R.MEMBER_ID=:id AND V2.FRD_ID=FRD.ID AND V2.VERSION= FRD.LATEST_V AND FRD.ID=R.FRD_ID " +
                "AND FRD.STATUS<>'C' AND U.ID=FRD.USER_ID  AND  R.GROUP_ID='0000'" +

               //REMOVES ANY RECEIVED AS A USER FRDS FROM RECEIVED AS A MANAGER
               //" AND FRD.ID  NOT IN" +
               //         "(SELECT FRD.ID " +
               //         "FROM FRDOCUMENT FRD JOIN FRD_GROUP_MEMBERS FGM ON FRD.ID=FGM.FRD_ID AND FGM.MEMBER_ID=:id AND FGM.GROUP_ID!='0000' " +
               //         "WHERE FRD.STATUS='A')" +

               " group by FRD.ID, FRD.TITLE, FRD.LATEST_V, U.NAME, U.SURNAME, V1.DATE_CREATED, V2.DATE_CREATED " +
               "ORDER BY V2.DATE_CREATED  DESC",
                MyConnection);
            MyCommand.Parameters.Add("id", Models.Session.Functions.GetID());
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.ReceivedFRDsDataTable table = new Main_DB.ReceivedFRDsDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception)
            {
                return listAsManager;
            }
            foreach (var row in table)
            {
                ActiveClosedFRDItem item = new ActiveClosedFRDItem
                {
                    ID = row.ID,
                    Title = row.TITLE,
                    Created = row.DATE_CREATED,
                    LastUpdate = row.LAST_UPDATE,
                    Notification = row.N_COUNT,
                    Version = row.VERSION,
                    Owner = new Employee { Name = row.NAME, Surname = row.SURNAME },
                    Hashed_ID = G_Functions.GetHash(row.ID)
                };
                listAsManager.Add(item);
            }

            return listAsManager;

        }


        public static List<ActiveClosedFRDItem> ActiveList(string UserID=null)
        {
            List<ActiveClosedFRDItem> list = new List<ActiveClosedFRDItem>();
            MyCommand = new OracleCommand(
                "SELECT FRD.ID,FRD.TITLE,FRD.LATEST_V,FRD.DATE_CREATED,COUNT(N.ID) AS N_COUNT,V.DATE_CREATED AS LAST_UPDATE,FRD.STATUS" +
                "   FROM FRDOCUMENT FRD LEFT OUTER JOIN NOTIFICATION N ON N.FRD_ID=FRD.ID AND N.USER_ID=:id AND N.SEEN='N'" +
                ", VERSIONS V " +
                "WHERE FRD.STATUS='A' AND FRD.USER_ID=:id AND V.FRD_ID=FRD.ID AND V.VERSION=FRD.LATEST_V " +
                "group by FRD.ID, FRD.TITLE, FRD.LATEST_V, FRD.DATE_CREATED, V.DATE_CREATED, FRD.STATUS ORDER BY FRD.DATE_CREATED DESC", MyConnection);
            MyCommand.Parameters.Add("id", UserID??Models.Session.Functions.GetID());
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.Active_FRDsDataTable table = new Main_DB.Active_FRDsDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return list;
            }

            foreach (var row in table)
            {
                ActiveClosedFRDItem item = new ActiveClosedFRDItem
                {
                    ID = row.ID,
                    Title = row.TITLE,
                    Created = row.DATE_CREATED,
                    LastUpdate = row.LAST_UPDATE,
                    Notification = row.N_COUNT,
                    Version = Decimal.ToInt32(row.LATEST_V),
                    Hashed_ID = G_Functions.GetHash(row.ID),
                    Type = (row.STATUS == "A" ? 1 : 0)
                };
                list.Add(item);
            }
            return list;
        }


        public static List<ActiveClosedFRDItem> PendingList()
        {
            List<ActiveClosedFRDItem> list = new List<ActiveClosedFRDItem>();
            MyCommand = new OracleCommand(
                "SELECT FRD.ID,FRD.TITLE,FRD.LATEST_V,FRD.DATE_CREATED,COUNT(N.ID) AS N_COUNT,V.DATE_CREATED AS LAST_UPDATE,FRD.STATUS" +
                "   FROM FRDOCUMENT FRD LEFT OUTER JOIN NOTIFICATION N ON N.FRD_ID=FRD.ID AND N.USER_ID=:id AND N.SEEN='N'" +
                ", VERSIONS V " +
                "WHERE FRD.STATUS='N' AND FRD.USER_ID=:id AND V.FRD_ID=FRD.ID AND V.VERSION=FRD.LATEST_V " +
                "group by FRD.ID, FRD.TITLE, FRD.LATEST_V, FRD.DATE_CREATED, V.DATE_CREATED, FRD.STATUS ORDER BY FRD.DATE_CREATED DESC", MyConnection);
            MyCommand.Parameters.Add("id", Models.Session.Functions.GetID());
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.Active_FRDsDataTable table = new Main_DB.Active_FRDsDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return list;
            }

            foreach (var row in table)
            {
                ActiveClosedFRDItem item = new ActiveClosedFRDItem
                {
                    ID = row.ID,
                    Title = row.TITLE,
                    Created = row.DATE_CREATED,
                    LastUpdate = row.LAST_UPDATE,
                    Notification = row.N_COUNT,
                    Version = Decimal.ToInt32(row.LATEST_V),
                    Hashed_ID = G_Functions.GetHash(row.ID),
                    Type = (row.STATUS == "A" ? 1 : 0)
                };
                list.Add(item);
            }
            return list;
        }


        public static List<ActiveClosedFRDItem> ClosedList(int option,string UserID=null)
        {
            List<ActiveClosedFRDItem> list = new List<ActiveClosedFRDItem>();
            switch (option)
            {
                case 0://all
                    MyCommand = new OracleCommand(
                            "SELECT FRD.ID,FRD.TITLE,FRD.LATEST_V,FRD.DATE_CREATED,C.DATE_CREATED AS DATE_CLOSED " +
                            "FROM FRDOCUMENT FRD LEFT OUTER JOIN FRD_GROUP_MEMBERS FGM ON FRD.ID=FGM.FRD_ID AND FGM.MEMBER_ID=:id " +
                            "INNER JOIN VERSIONS C ON C.FRD_ID=FRD.ID AND C.VERSION=FRD.LATEST_V " +
                            "WHERE (FRD.USER_ID=:id OR  FGM.MEMBER_ID=:id) AND FRD.STATUS='C'  ORDER BY C.DATE_CREATED DESC ", MyConnection);
                    break;
                case 1://closed by me
                    MyCommand = new OracleCommand(
                            "SELECT FRD.ID,FRD.TITLE,FRD.LATEST_V,FRD.DATE_CREATED,C.DATE_CREATED AS DATE_CLOSED " +
                            "FROM FRDOCUMENT FRD INNER JOIN VERSIONS C ON C.FRD_ID=FRD.ID AND C.VERSION=FRD.LATEST_V " +
                            "WHERE FRD.STATUS='C' AND FRD.USER_ID=:id " +
                            "ORDER BY C.DATE_CREATED DESC ", MyConnection);
                    break;
                case 2://by others
                    MyCommand = new OracleCommand(
                            "SELECT FRD.ID,FRD.TITLE,FRD.LATEST_V,FRD.DATE_CREATED,C.DATE_CREATED AS DATE_CLOSED " +
                            " FROM FRDOCUMENT FRD JOIN FRD_GROUP_MEMBERS FGM " +
                            "ON FRD.ID=FGM.FRD_ID AND FGM.MEMBER_ID=:id " +
                            " INNER JOIN VERSIONS C " +
                            "ON C.FRD_ID=FRD.ID AND C.VERSION=FRD.LATEST_V " +
                            " WHERE FRD.STATUS='C'  ORDER BY C.DATE_CREATED DESC ", MyConnection);
                    break;

            }

            MyCommand.Parameters.Add("id", UserID??Models.Session.Functions.GetID());
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.Closed_FRDsDataTable table = new Main_DB.Closed_FRDsDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception)
            {
                return list;
            }
            foreach (var row in table)
            {
                ActiveClosedFRDItem item = new ActiveClosedFRDItem
                {
                    ID = row.ID,
                    Title = row.TITLE,
                    Created = row.DATE_CREATED,
                    LastUpdate = row.DATE_CLOSED,
                    Notification = 0,
                    Version = Decimal.ToInt32(row.LATEST_V),
                    Hashed_ID = G_Functions.GetHash(row.ID)
                };
                list.Add(item);
            }
            return list;
        }

        public static List<ActiveClosedFRDItem> ClosedByOthers()
        {
            List<ActiveClosedFRDItem> list = new List<ActiveClosedFRDItem>();
            MyCommand = new OracleCommand(
                            "SELECT FRD.ID,FRD.TITLE,FRD.LATEST_V,FRD.DATE_CREATED,C.DATE_CREATED AS DATE_CLOSED " +
                            " FROM FRDOCUMENT FRD JOIN FRD_GROUP_MEMBERS FGM " +
                            "ON FRD.ID=FGM.FRD_ID AND FGM.MEMBER_ID=:id " +
                            " INNER JOIN VERSIONS C " +
                            "ON C.FRD_ID=FRD.ID AND C.VERSION=FRD.LATEST_V " +
                            " WHERE FRD.STATUS='C'  ORDER BY C.DATE_CREATED DESC ", MyConnection);
            MyCommand.Parameters.Add("id", Models.Session.Functions.GetID());
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.Closed_FRDsDataTable table = new Main_DB.Closed_FRDsDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception)
            {
                return list;
            }
            foreach (var row in table)
            {
                ActiveClosedFRDItem item = new ActiveClosedFRDItem
                {
                    ID = row.ID,
                    Title = row.TITLE,
                    Created = row.DATE_CREATED,
                    LastUpdate = row.DATE_CLOSED,
                    Notification = 0,
                    Version = Decimal.ToInt32(row.LATEST_V),
                    Hashed_ID = G_Functions.GetHash(row.ID)
                };
                list.Add(item);
            }
            return list;
        }


        //=======================================================================================================
        //===========================================================================
        //===================================================




        //Checks "Security


        //checks if the user is the owner and frd is active
        public static Frd CheckActive(string FrdId)
        {
            string UserId = Session.Functions.GetID();
            MyCommand = new OracleCommand("SELECT ID,USER_ID,DATE_CREATED,LATEST_V,TITLE,STATUS " +
                "FROM FRDOCUMENT WHERE ID=:frd_id " +
                "AND USER_ID=:user_id AND STATUS ='A'", MyConnection);
            MyCommand.Parameters.Add("frd_id", FrdId);
            MyCommand.Parameters.Add("user_id", UserId);

            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.FRDOCUMENTDataTable table = new Main_DB.FRDOCUMENTDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception)
            {
                return null;
            }
            if (table.Count == 1)
            {
                Frd A_Frd = new Frd
                {
                    Panel0 = new Panels.Panel_0 { Id = FrdId, Owner = UserId, Title = table[0].TITLE },
                    Type = table[0].STATUS == "A" ? Frd.TypesEnum.Active : Frd.TypesEnum.Pending,
                    LatestVersion = Decimal.ToInt32(table[0].LATEST_V)
                };
                A_Frd.Panel0.LatestVersion = Decimal.ToInt32(table[0].LATEST_V);
                return A_Frd;
            }
            //null if no active frd with the frd_id
            return null;
        }


        public static Frd CheckPending(string FrdId)
        {
            string UserId = Session.Functions.GetID();
            MyCommand = new OracleCommand("SELECT ID,USER_ID,DATE_CREATED,LATEST_V,TITLE,STATUS " +
                "FROM FRDOCUMENT WHERE ID=:frd_id " +
                "AND USER_ID=:user_id AND STATUS ='N'", MyConnection);
            MyCommand.Parameters.Add("frd_id", FrdId);
            MyCommand.Parameters.Add("user_id", UserId);

            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.FRDOCUMENTDataTable table = new Main_DB.FRDOCUMENTDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception)
            {
                return null;
            }
            if (table.Count == 1)
            {
                Frd A_Frd = new Frd
                {
                    Panel0 = new Panels.Panel_0 { Id = FrdId, Owner = UserId, Title = table[0].TITLE },
                    Type = table[0].STATUS == "A" ? Frd.TypesEnum.Active : Frd.TypesEnum.Pending,
                    LatestVersion = Decimal.ToInt32(table[0].LATEST_V)
                };
                A_Frd.Panel0.LatestVersion = Decimal.ToInt32(table[0].LATEST_V);
                return A_Frd;
            }
            //null if no active frd with the frd_id
            return null;
        }

        public static Frd CheckReceived(string FrdId)
        {
            string UserId = Session.Functions.GetID();
            MyCommand = new OracleCommand(
                "SELECT F.ID,U.NAME AS OWNER,F.USER_ID AS OWNER_ID,F.DATE_CREATED,F.LATEST_V,F.TITLE,F.STATUS,G.GROUP_ID " +
                "FROM FRDOCUMENT F INNER JOIN FRD_GROUP_MEMBERS G " +
                "ON  G.FRD_ID=F.ID AND G.MEMBER_ID=:user_id AND G.ENABLED=1 AND F.ID=:frd_id  AND F.STATUS<>'C'" +
                " ,USERS U WHERE U.ID=F.USER_ID ORDER BY G.GROUP_ID DESC", MyConnection);
            MyCommand.Parameters.Add("frd_id", FrdId);
            MyCommand.Parameters.Add("user_id", UserId);
            MyCommand.BindByName = true;
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.Received_CheckDataTable table = new Main_DB.Received_CheckDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception)
            {
                return null;
            }
            if (table.Count > 0)
            {
                Frd A_Frd = new Frd
                {
                    Panel0 = new Panels.Panel_0 { Id = FrdId, Owner = table[0].OWNER, Title = table[0].TITLE }
                };
                switch (table[0].STATUS)
                {
                    case "A":
                        if (table[0].GROUP_ID == "0000")
                            A_Frd.Type = Frd.TypesEnum.ReceivedManagerActive;
                        else
                            A_Frd.Type = Frd.TypesEnum.ReceivedUser; //Active
                        break;
                    case "N":
                        A_Frd.Type = Frd.TypesEnum.ReceivedManagerPending;//Manager Approval
                        break;
                }
                A_Frd.LatestVersion = table[0].LATEST_V;
                A_Frd.Panel0.LatestVersion = table[0].LATEST_V;
                A_Frd.OwnerId = table[0].OWNER_ID;
                return A_Frd;
            }
            //null if no active frd with the frd_id
            return null;
        }



        public static Frd CheckClosed(string FrdId)
        {
            string UserId = Session.Functions.GetID();
            MyCommand = new OracleCommand(
                "SELECT ID,USER_ID,DATE_CREATED,LATEST_V,TITLE,STATUS FROM FRDOCUMENT FRD LEFT OUTER JOIN FRD_GROUP_MEMBERS FGM ON FGM.FRD_ID=FRD.ID AND FGM.MEMBER_ID=:user_id WHERE ID=:frd_id AND (USER_ID=:user_id OR FGM.MEMBER_ID=:user_id) AND STATUS ='C'", MyConnection);
            MyCommand.Parameters.Add("frd_id", FrdId);
            MyCommand.Parameters.Add("user_id", UserId);
            MyCommand.BindByName = true;
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.FRDOCUMENTDataTable table = new Main_DB.FRDOCUMENTDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception)
            {
                return null;
            }
            if (table.Count > 0)
            {
                Frd A_Frd = new Frd
                {
                    Panel0 = new Panels.Panel_0 { Id = FrdId, Owner = UserId, Title = table[0].TITLE },
                    Type = Frd.TypesEnum.Closed,//type 2 Closed
                    LatestVersion = Decimal.ToInt32(table[0].LATEST_V)
                };
                A_Frd.Panel0.LatestVersion = Decimal.ToInt32(table[0].LATEST_V);
                return A_Frd;
            }
            //null if no active frd with the frd_id
            return null;
        }



        private static Frd CheckIfManager(string frdId)
        {
            string UserId = Session.Functions.GetID();
            MyCommand = new OracleCommand(
                "SELECT FRD.ID,FRD.USER_ID,FRD.DATE_CREATED,FRD.LATEST_V,FRD.TITLE,FRD.STATUS " +
                "FROM FRD_GROUP_MEMBERS FGM JOIN FRDOCUMENT FRD " +
                "ON FGM.FRD_ID=FRD.ID AND FRD.ID=:frd_id AND FGM.MEMBER_ID=:user_id " +
                "AND FGM.GROUP_ID='0000' AND FRD.STATUS='N'", MyConnection);

            MyCommand.Parameters.Add("frd_id", frdId);
            MyCommand.Parameters.Add("user_id", UserId);

            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.FRDOCUMENTDataTable table = new Main_DB.FRDOCUMENTDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception)
            {
                return null;
            }
            if (table.Count == 1)
            {
                Frd A_Frd = new Frd
                {
                    Panel0 = new Panels.Panel_0 { Id = frdId, Owner = UserId, Title = table[0].TITLE },
                    OwnerId = table[0].USER_ID,
                    Type = Frd.TypesEnum.ReceivedManagerPending,//type 2 Closed
                    LatestVersion = Decimal.ToInt32(table[0].LATEST_V)
                };
                A_Frd.Panel0.LatestVersion = Decimal.ToInt32(table[0].LATEST_V);
                return A_Frd;
            }
            //null if no active frd with the frd_id
            return null;
        }

        public static bool CheckUserManager(string UserID)
        {
            MyCommand = new OracleCommand("SELECT U.ID,U.E_MAIL,U.NAME,U.SURNAME FROM USERS U  WHERE U.STATUS='E' AND U.MANAGER_ID=:id AND U.ID=:user_id", MyConnection);
            MyCommand.Parameters.Add("id", Session.Functions.GetID());
            MyCommand.Parameters.Add("user_id", UserID);
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.LogIn_InfoDataTable table = new Main_DB.LogIn_InfoDataTable();
            MyAdapter.Fill(table);//get users list
            if (table.Count == 1)
                return true;
            return false;
        }

        public static Frd CheckForFrd(string FrdId)
        {
            string UserId = Session.Functions.GetID();
            MyCommand = new OracleCommand(
                "SELECT FR.ID,FR.USER_ID,FR.DATE_CREATED,FR.TITLE,FR.LATEST_V,FR.STATUS,R.GROUP_ID,R.MEMBER_ID" +
                " FROM FRDOCUMENT FR LEFT OUTER JOIN FRD_GROUP_MEMBERS R" +
                " ON(FR.ID = R.FRD_ID AND R.MEMBER_ID =:user_id)" +
                " WHERE(R.MEMBER_ID =:user_id AND R.FRD_ID =:frd_id AND R.GROUP_ID != '0000' AND FR.STATUS != 'N')" +
                " OR(R.MEMBER_ID =:user_id AND R.FRD_ID =:frd_id AND R.GROUP_ID = '0000')" +
                " OR(FR.USER_ID =:user_id AND FR.ID =:frd_id AND R.MEMBER_ID IS NULL)ORDER BY GROUP_ID DESC"
                , MyConnection)
            {
                BindByName = true
            };
            MyCommand.Parameters.Add(":user_id", UserId);
            MyCommand.Parameters.Add(":frd_id", FrdId);




            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.FRDOCUMENTDataTable table = new Main_DB.FRDOCUMENTDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return null;
            }
            if (table.Count > 0)
            {
                Frd A_Frd = new Frd
                {
                    Panel0 = new Panels.Panel_0 { Id = FrdId, Owner = UserId, Title = table[0].TITLE }
                };
                if (table[0].USER_ID.Equals(UserId))
                {
                    switch (table[0].STATUS)
                    {
                        case "A":
                            A_Frd.Type = Frd.TypesEnum.Active;
                            break;
                        case "C":
                            A_Frd.Type = Frd.TypesEnum.Closed;
                            break;
                        case "N":
                            A_Frd.Type = Frd.TypesEnum.Pending;
                            break;
                    }
                }
                else if (table[0].GROUP_ID != null && table[0].GROUP_ID.Equals("0000"))
                {
                    switch (table[0].STATUS)
                    {
                        case "A":
                            A_Frd.Type = Frd.TypesEnum.ReceivedManagerActive;
                            break;
                        case "C":
                            A_Frd.Type = Frd.TypesEnum.Closed;
                            break;
                        case "N":
                            A_Frd.Type = Frd.TypesEnum.ReceivedManagerPending;
                            break;
                    }
                }
                else
                {
                    switch (table[0].STATUS)
                    {
                        case "A":
                            A_Frd.Type = Frd.TypesEnum.ReceivedUser;
                            break;
                        case "C":
                            A_Frd.Type = Frd.TypesEnum.Closed;
                            break;
                    }
                }


                A_Frd.LatestVersion = Decimal.ToInt32(table[0].LATEST_V);
                A_Frd.Panel0.LatestVersion = A_Frd.LatestVersion;
                return A_Frd;
            }
            //null if no active frd with the frd_id
            return null;
        }





        public static bool CheckFrdId(string Id)
        {
            decimal count;
            MyCommand = new OracleCommand("SELECT COUNT(ID) FROM FRDOCUMENT WHERE ID=:id", MyConnection);
            MyCommand.Parameters.Add(":id", Id);
            try
            {
                MyConnection.Open();
                count = (decimal)MyCommand.ExecuteScalar();
                MyConnection.Close();
                if (count == 0)
                    return true;
            }
            catch (Exception e)
            {
                try
                {
                    MyConnection.Close();
                }
                catch (Exception)
                {
                }
                return false;
            }
            return false;
        }

        //=======================================================================================================
        //===========================================================================
        //===================================================


        //Upload Token Functions



        public static string NewUploadToken(UploadTokenType type)
        {
            string userID = Session.Functions.GetID();
            string Token = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
            Token = Token.Replace("-", "");
            CheckOldTokens(userID, type);//checks and delete any temp files for the user for the same type
            InsertUploadToken(Token, userID, type);
            return Token;
        }



        private static void InsertUploadToken(string token, string ID, UploadTokenType type)
        {
            MyCommand = new OracleCommand("INSERT INTO UPLOAD_TOKENS (ID,TYPE,TOKEN) VALUES (:id,:type,:token)", MyConnection);
            MyCommand.Parameters.Add("ID", ID);
            MyCommand.Parameters.Add("type", (int)type);
            MyCommand.Parameters.Add("token", token);
            try
            {
                MyConnection.Open();
                MyCommand.ExecuteNonQuery();
                MyConnection.Close();
            }
            catch (Exception e)
            {

                try
                {
                    MyConnection.Close();
                }
                catch (Exception e2)
                {
                }
            }

        }

        private static void CheckOldTokens(string id, UploadTokenType type)
        {
            MyCommand = new OracleCommand("SELECT ID,TOKEN,TYPE FROM UPLOAD_TOKENS WHERE ID=:id AND TYPE=:type", MyConnection);
            MyCommand.Parameters.Add("id", id);
            MyCommand.Parameters.Add("type", (int)type);
            MyAdapter = new OracleDataAdapter(MyCommand);
            FRD_DataSet.UPLOAD_TOKENSDataTable table = new FRD_DataSet.UPLOAD_TOKENSDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
            }
            if (table.Count == 1)
            {
                DeleteFiles(table[0].TOKEN);
            }
            MyCommand = new OracleCommand("DELETE FROM UPLOAD_TOKENS WHERE ID=:id AND TYPE=:type", MyConnection);
            MyCommand.Parameters.Add("id", id);
            MyCommand.Parameters.Add("type", (int)type);
            try
            {
                MyConnection.Open();
                MyCommand.ExecuteNonQuery();
                MyConnection.Close();
            }
            catch (Exception e)
            {

                try
                {
                    MyConnection.Close();
                }
                catch (Exception e2)
                {
                }
            }
        }




        public static void InsertTempFile(string fileId, string extension, string token, string originalName, long fileSize)
        {
            MyCommand = new OracleCommand("INSERT INTO TEMP_FILES (FILE_NAME,EXTENSION,UPLOAD_TOKEN,OREGENAL_NAME,FILE_SIZE) VALUES (:name,:ext,:token,:o_name,:f_size)", MyConnection);
            MyCommand.Parameters.Add("name", fileId);
            MyCommand.Parameters.Add("ext", extension);
            MyCommand.Parameters.Add("token", token);
            MyCommand.Parameters.Add("o_name", originalName);
            MyCommand.Parameters.Add("f_size", fileSize);
            try
            {
                MyConnection.Open();
                MyCommand.ExecuteNonQuery();
                MyConnection.Close();
            }
            catch (Exception e)
            {
                try
                {
                    MyConnection.Close();
                }
                catch (Exception e2)
                {
                }
            }
        }


        private static void DeleteFiles(string token)
        {
            MyCommand = new OracleCommand("SELECT FILE_NAME,EXTENSION,UPLOAD_DATE,UPLOAD_TOKEN,FILE_SIZE FROM TEMP_FILES WHERE UPLOAD_TOKEN=:token", MyConnection);
            MyCommand.Parameters.Add("token", token);
            MyAdapter = new OracleDataAdapter(MyCommand);
            FRD_DataSet.TEMP_FILESDataTable table = new FRD_DataSet.TEMP_FILESDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {

            }
            DeleteTempFiles(table);
            MyCommand = new OracleCommand("DELETE FROM TEMP_FILES WHERE UPLOAD_TOKEN=:token", MyConnection);
            MyCommand.Parameters.Add("token", token);
            try
            {
                MyConnection.Open();
                MyCommand.ExecuteNonQuery();
                MyConnection.Close();
            }
            catch (Exception e)
            {
                try
                {
                    MyConnection.Close();
                }
                catch (Exception e2)
                {
                }
            }
        }



        private static void DeleteTempFiles(FRD_DataSet.TEMP_FILESDataTable table)
        {
            foreach (FRD_DataSet.TEMP_FILESRow row in table)
            {
                var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Files"), row.FILE_NAME + row.EXTENSION.Replace("application/", "."));
                try
                {
                    File.Delete(path);
                }
                catch
                {
                    continue;
                }
            }
        }




        public static bool CheckToken(string Token)
        {
            MyCommand = new OracleCommand("SELECT ID,TOKEN,TYPE FROM UPLOAD_TOKENS WHERE ID=:id AND TOKEN=:token", MyConnection);
            MyCommand.Parameters.Add("id", Session.Functions.GetID());
            MyCommand.Parameters.Add("token", Token);
            MyAdapter = new OracleDataAdapter(MyCommand);
            FRD_DataSet.UPLOAD_TOKENSDataTable table = new FRD_DataSet.UPLOAD_TOKENSDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return false;
            }
            if (table.Count > 0)
            {
                return true;
            }

            return false;
        }





        //=======================================================================================================
        //===========================================================================
        //===================================================




        //Draft Frd Functions

        public static void SaveToDraft(byte[] file)
        {
            DeleteSavedDraft();
            MyCommand = new OracleCommand("INSERT INTO SAVEDFRD (ID,JSON) VALUES(:id,:json)", MyConnection);
            MyCommand.Parameters.Add("id", Session.Functions.GetID());
            MyCommand.Parameters.Add("json", file);
            MyConnection.Open();
            try
            {
                MyCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                try
                {
                    MyConnection.Close();
                }
                catch (Exception e2)
                {
                }
            }
            MyConnection.Close();
        }




        public static void DeleteSavedDraft()
        {
            MyCommand = new OracleCommand("DELETE FROM SAVEDFRD WHERE ID=:id", MyConnection);
            MyCommand.Parameters.Add("id", Session.Functions.GetID());
            MyConnection.Open();

            try
            {
                MyCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {

                try
                {
                    MyConnection.Close();
                }
                catch (Exception e2)
                {
                }
            }
            MyConnection.Close();
        }


        public static byte[] GetSaved()
        {
            MyCommand = new OracleCommand("SELECT ID,JSON,SAVED_TIME FROM SAVEDFRD WHERE ID=:id", MyConnection);
            MyCommand.Parameters.Add("ID", Session.Functions.GetID());
            MyAdapter = new OracleDataAdapter(MyCommand);
            FRD_DataSet.SAVEDFRDDataTable Saved_Files = new FRD_DataSet.SAVEDFRDDataTable();
            try
            {
                MyAdapter.Fill(Saved_Files);
            }
            catch (Exception e)
            {
                return null;
            }
            if (Saved_Files.Count == 1)
            {
                return Saved_Files[0].JSON;
            }

            return null;
        }




        //=======================================================================================================
        //===========================================================================
        //===================================================



        //New Frd Submit Functions


        public static bool SubmitNewFrd(Frd document)
        {
            LogedInEmployee user = Session.Functions.GetInfo();
            try
            {
                MyCommand = new OracleCommand(
                    "SELECT FILE_NAME,EXTENSION,UPLOAD_DATE,OREGENAL_NAME,UPLOAD_TOKEN,FILE_SIZE " +
                    "from TEMP_FILES " +
                    "where UPLOAD_TOKEN=:token",
                    MyConnection);

                MyCommand.Parameters.Add("token", document.Upload_Token);
                MyAdapter = new OracleDataAdapter(MyCommand);
                FRD_DataSet.TempFilesDataTable table = new FRD_DataSet.TempFilesDataTable();
                MyAdapter.Fill(table);
                MyConnection.Open();
                MyTransaction = MyConnection.BeginTransaction();

                string frdId = document.Panel0.Id;
                int version = 1;
                SavePanel0(document.Panel0, user.UserInfo.ManagerInfo.Id != "0000");
                AddNewVersion(frdId, document.Panel0.Title, version);
                SavePanel1(frdId, document.Panel1);
                SavePanel2(frdId, document.Panel2);
                SavePanel3(frdId, document.Panel3);
                SavePanel4(frdId, document.Panel4);
                SavePanel5(document.Upload_Token, frdId, table);//files
                SavePanel6(frdId, document.Panel6);
                SavePanel7(frdId, document.Panel7);
                MyTransaction.Commit();

                if (!Session.Functions.HasManager())
                    NotifyMembers(frdId, 1, "You Received a New FRD");
                else
                {
                    NotifyManager(frdId, user.UserInfo.ManagerInfo.Id, "A new FRD by " + user.UserInfo.Name + " " + user.UserInfo.Surname + " Is waiting your approval");
                }
                MyConnection.Close();
            }
            catch (Exception e)
            {
                try
                {
                    MyTransaction.Rollback();
                }
                catch (Exception)
                { }
                try
                {
                    MyConnection.Close();
                }
                catch (Exception)
                { }
                return false;
            }

            return true;
        }



        private static void AddNewVersion(string frdId, string title, int version, string explanation = null)
        {
            MyCommand = new OracleCommand("INSERT INTO VERSIONS(FRD_ID,NOTE,CREATOR_ID,VERSION,EXPLANATION) VALUES(:frd_id,:note,:user_id,:version,:explanation)", MyConnection);
            MyCommand.Parameters.Add("frd_id", frdId);
            MyCommand.Parameters.Add("user_id", Session.Functions.GetID());
            MyCommand.Parameters.Add("note", title);//change
            MyCommand.Parameters.Add("version", version);
            MyCommand.Parameters.Add("explanation", explanation);
            MyCommand.BindByName = true;
            try
            {
                MyCommand.ExecuteNonQuery();
            }
            catch (Exception)
            {

                throw;
            }
        }



        private static void SavePanel0(Panel_0 panel0, bool pendingOrActive)
        {
            if (pendingOrActive)
            {
                MyCommand = new OracleCommand("INSERT INTO FRDOCUMENT (ID,USER_ID,TITLE) VALUES(:frd_id,:user_id,:title)", MyConnection);
                MyCommand.Parameters.Add("frd_id", panel0.Id);
                MyCommand.Parameters.Add("user_id", Session.Functions.GetID());
                MyCommand.Parameters.Add("title", panel0.Title);
            }
            else
            {
                MyCommand = new OracleCommand("INSERT INTO FRDOCUMENT (ID,USER_ID,TITLE,STATUS) VALUES(:frd_id,:user_id,:title,'A')", MyConnection);
                MyCommand.Parameters.Add("frd_id", panel0.Id);
                MyCommand.Parameters.Add("user_id", Session.Functions.GetID());
                MyCommand.Parameters.Add("title", panel0.Title);
            }
            MyCommand.BindByName = true;
            try
            {
                MyCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw;
            }
            return;
        }


        //=======================================================================================================
        //===========================================================================
        //===================================================



        //Save Panels 1-7 Functions



        private static void SavePanel1(string id, Panel_1 panel1)
        {
            foreach (var item in panel1.Items)
            {
                MyCommand = new OracleCommand("INSERT INTO FRD_ITEMS (FRD_ID,TEXT) VALUES(:frd_id,:txt)", MyConnection);
                MyCommand.Parameters.Add("frd_id", id);
                MyCommand.Parameters.Add("txt", item.Content);
                MyCommand.BindByName = true;
                try
                {
                    MyCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {

                    throw;
                }
            }
            return;
        }

        public static bool SavePanel1WithComments(ref Frd inViewModel, int version)
        {
            try
            {
                if (inViewModel.Panel1 != null)
                {
                    foreach (var item in inViewModel.Panel1.Items)
                    {
                        if (item.Comments != null)
                        {
                            foreach (var comment in item.Comments)
                            {
                                MyCommand = new OracleCommand("INSERT INTO ITEMS_COMMENTS (FRD_ID,ITEM_ID,USER_ID,CONTENT,VERSION) VALUES(:frd_id,:item_id,:user_id,:content,:version)", MyConnection);
                                MyCommand.Parameters.Add("frd_id", inViewModel.Panel0.Id);
                                MyCommand.Parameters.Add("item_id", item.ItemId);
                                MyCommand.Parameters.Add("user_id", Session.Functions.GetID());
                                MyCommand.Parameters.Add("content", comment.Content);//change
                                MyCommand.Parameters.Add("version", version);
                                MyCommand.BindByName = true;

                                MyCommand.ExecuteNonQuery();

                            }
                        }
                        else
                        {
                            //here insert new Items

                            MyCommand = new OracleCommand("INSERT INTO FRD_ITEMS (FRD_ID,TEXT,VERSION) VALUES(:frd_id,:content,:version)", MyConnection);
                            MyCommand.Parameters.Add("frd_id", inViewModel.Panel0.Id);
                            MyCommand.Parameters.Add("content", item.Content);
                            MyCommand.Parameters.Add("version", version);
                            MyCommand.BindByName = true;
                            MyCommand.ExecuteNonQuery();

                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return true;
        }



        private static void SavePanel2(string id, Panel_2 panel2, int version = 1)
        {
            if (panel2 != null)
            {
                foreach (var target in panel2.Targets)
                {
                    MyCommand = new OracleCommand("INSERT INTO FRD_TARGETS (FRD_ID,VERSION,TARGET_CODE,TARGET) VALUES (:frd_id,:v,:t_code,:t_text)", MyConnection);
                    MyCommand.Parameters.Add("frd_id", id);
                    MyCommand.Parameters.Add("v", version);
                    MyCommand.Parameters.Add("t_code", target.Code);
                    MyCommand.Parameters.Add("t_text", target.Name);
                    MyCommand.BindByName = true;
                    try
                    {
                        MyCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }

            }
            return;
        }



        private static void SavePanel3(string id, Panel_3 panel3, int version = 1)
        {
            if (panel3 != null)
            {
                foreach (var cahnnel in panel3.Channels)
                {
                    MyCommand = new OracleCommand("INSERT INTO FRD_CHANNELS (FRD_ID,VERSION,CHANNEL_CODE,CHANNEL_NAME) VALUES (:frd_id,:v,:c_code,:c_text)", MyConnection);
                    MyCommand.Parameters.Add("frd_id", id);
                    MyCommand.Parameters.Add("v", version);
                    MyCommand.Parameters.Add("c_code", cahnnel.Code);
                    MyCommand.Parameters.Add("c_text", cahnnel.Name);
                    MyCommand.BindByName = true;
                    try
                    {
                        MyCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {

                        throw;
                    }

                }

            }

        }




        private static void SavePanel4(string id, Panel_4 panel4, int version = 1)
        {
            if (panel4 != null)
            {
                if (panel4.SMS != null)
                    foreach (var sms in panel4.SMS)
                    {
                        MyCommand = new OracleCommand("INSERT INTO FRD_SMS (FRD_ID,VERSION,SENDER,CODE,CONTENT_EN,CONTENT_TR) VALUES (:frd_id,:v,:sms_sender,:sms_code,:sms_en_c,:sms_tr_c)", MyConnection);
                        MyCommand.Parameters.Add("frd_id", id);
                        MyCommand.Parameters.Add("v", version);
                        MyCommand.Parameters.Add("sms_sender", sms.Sender ?? " ");
                        MyCommand.Parameters.Add("sms_code", sms.Code ?? " ");
                        MyCommand.Parameters.Add("sms_en_c", sms.EnContent ?? " ");
                        MyCommand.Parameters.Add("sms_tr_c", sms.TrContent ?? " ");
                        MyCommand.BindByName = true;
                        try
                        {
                            MyCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {

                            throw;
                        }


                    }

            }
            return;
        }




        private static void SavePanel5(string token, string frdId, FRD_DataSet.TempFilesDataTable table, int version = 1)
        {
            if (table.Count > 0)
            {
                foreach (var file in table)
                {
                    MyCommand = new OracleCommand("INSERT INTO FRD_FILES (FRD_ID,FILE_NAME,EXTENSION,OREGENAL_NAME,TIME_UPLOADED,FILE_SIZE,VERSION) VALUES (:frd_id,:f_name,:f_ext,:f_o_n,:f_time,:file_size,:version)", MyConnection);
                    MyCommand.Parameters.Add("frd_id", frdId);
                    MyCommand.Parameters.Add("f_name", file.FILE_NAME);
                    MyCommand.Parameters.Add("f_ext", file.EXTENSION);
                    MyCommand.Parameters.Add("f_o_n", file.OREGENAL_NAME);
                    MyCommand.Parameters.Add("f_time", file.UPLOAD_DATE);
                    MyCommand.Parameters.Add("file_size", file.FILE_SIZE);
                    MyCommand.Parameters.Add("version", version);
                    MyCommand.BindByName = true;
                    try
                    {
                        MyCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {

                        throw;
                    }

                }
                MyCommand = new OracleCommand("DELETE FROM TEMP_FILES WHERE UPLOAD_TOKEN=:token", MyConnection);
                MyCommand.Parameters.Add("token", token);
                try
                {
                    MyCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {

                    throw;
                }

            }
            return;
        }



        private static void SavePanel6(string id, Panel_6 panel6, int version = 1)
        {
            if (panel6 != null)
            {
                foreach (var item in panel6.DiscountItems)
                {
                    MyCommand = new OracleCommand("INSERT INTO FRD_DISCOUNTS (FRD_ID,VERSION,CODE, NAME) VALUES (:frd_id,:v,:code,:name)", MyConnection);
                    MyCommand.Parameters.Add("frd_id", id);
                    MyCommand.Parameters.Add("v", version);
                    MyCommand.Parameters.Add("code", item.DiscountCode);
                    MyCommand.Parameters.Add("name", item.DiscountName);
                    MyCommand.BindByName = true;
                    try
                    {
                        MyCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {

                        throw;
                    }
                }


            }
            return;
        }



        private static void SavePanel7(string id, Panel_7 panel7, int Version = 1)
        {


            var x = Session.Functions.GetInfo();

            if (x.UserInfo.ManagerInfo.Id != "0000")
            {
                MyCommand = new OracleCommand("INSERT INTO FRD_GROUP_MEMBERS(FRD_ID,GROUP_ID ,MEMBER_ID,ENABLED) VALUES(:frd_id,:group_id,:member_id,1)", MyConnection);
                MyCommand.Parameters.Add(":frd_id", id);
                MyCommand.Parameters.Add(":group_id", "0000");
                MyCommand.Parameters.Add(":member_id", x.UserInfo.ManagerInfo.Id);
                MyCommand.BindByName = true;
                try
                {
                    MyCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {

                    throw;
                }



                foreach (var group in panel7.DistributionGroups)
                {
                    MyCommand = new OracleCommand("INSERT INTO FRD_GROUPS(FRD_ID,GROUP_ID,GROUP_NAME) VALUES(:frd_id,:group_id,:group_name)", MyConnection);
                    MyCommand.Parameters.Add("group_id", group.Id);
                    MyCommand.Parameters.Add("group_name", group.Name);
                    MyCommand.Parameters.Add("frd_id", id);
                    MyCommand.BindByName = true;
                    try
                    {
                        MyCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {

                        throw;
                    }
                    foreach (var member in group.Employess)
                    {
                        if (member.Id == x.UserInfo.Id)
                            continue;
                        MyCommand = new OracleCommand("INSERT INTO FRD_GROUP_MEMBERS(FRD_ID,GROUP_ID ,MEMBER_ID,ENABLED) VALUES(:frd_id,:group_id,:member_id,0)", MyConnection);
                        MyCommand.Parameters.Add(":frd_id", id);
                        MyCommand.Parameters.Add(":group_id", group.Id);
                        MyCommand.Parameters.Add(":member_id", member.Id);
                        MyCommand.BindByName = true;
                        try
                        {
                            MyCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {

                            throw;
                        }

                    }
                }
            }
            else
            {

                foreach (var Group in panel7.DistributionGroups)
                {
                    MyCommand = new OracleCommand("INSERT INTO FRD_GROUPS(FRD_ID,GROUP_ID ,GROUP_NAME) VALUES(:frd_id,:group_id,:group_name)", MyConnection);
                    MyCommand.Parameters.Add("group_id", Group.Id);
                    MyCommand.Parameters.Add("group_name", Group.Name);
                    MyCommand.Parameters.Add("frd_id", id);
                    MyCommand.BindByName = true;
                    try
                    {
                        MyCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {

                        throw;
                    }
                    foreach (var member in Group.Employess)
                    {
                        if (member.Id == x.UserInfo.ManagerInfo.Id || member.Id == x.UserInfo.Id)
                            continue;
                        MyCommand = new OracleCommand("INSERT INTO FRD_GROUP_MEMBERS(FRD_ID,GROUP_ID ,MEMBER_ID,ENABLED) VALUES(:frd_id,:group_id,:member_id,1) ", MyConnection);
                        MyCommand.Parameters.Add(":frd_id", id);
                        MyCommand.Parameters.Add(":group_id", Group.Id);
                        MyCommand.Parameters.Add(":member_id", member.Id);
                        MyCommand.BindByName = true;
                        try
                        {
                            MyCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {

                            throw;
                        }
                    }
                }

            }
            if (!Session.Functions.HasManager())
            {
                MyCommand = new OracleCommand("NEW_CONFIRMATIONS", MyConnection);
                MyCommand.CommandType = System.Data.CommandType.StoredProcedure;
                MyCommand.Parameters.Add("EFFECTED_FRD_ID", id);
                MyCommand.Parameters.Add("NEW_VERSION", 1);
                try
                {
                    MyCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw;
                }
            }


        }



        //=======================================================================================================
        //===========================================================================
        //===================================================



        //Reading Frd From DB Functions

        public enum NotificationsType { All, Active, Received, Pending, Closed,FRD }
        public static List<Notification> ReadNotifications(string userID, NotificationsType type = NotificationsType.All,string frdID=null)
        {
            List<Notification> list = new List<Notification>();
            if (type == NotificationsType.All)
            {
                MyCommand = new OracleCommand(
                               "SELECT N.ID,N.FRD_ID,FRD.TITLE,N.NOTE,N.SEEN,N.NOTIFICATION_TIME,N.FRD_TYPE,N.VERSION " +
                               "FROM NOTIFICATION N JOIN FRDOCUMENT FRD " +
                               "ON N.FRD_ID=FRD.ID AND N.USER_ID=:user_id " +
                               "ORDER BY N.NOTIFICATION_TIME DESC", MyConnection);
                MyCommand.Parameters.Add("user_id", userID);
            }
            else if (type == NotificationsType.FRD)
            {
                MyCommand = new OracleCommand(
                               "SELECT N.ID,N.FRD_ID,FRD.TITLE,N.NOTE,N.SEEN,N.NOTIFICATION_TIME,N.FRD_TYPE,N.VERSION " +
                               "FROM NOTIFICATION N JOIN FRDOCUMENT FRD " +
                               "ON N.FRD_ID=FRD.ID AND N.USER_ID=:user_id AND N.FRD_ID=:frdid " +
                               "ORDER BY N.NOTIFICATION_TIME DESC", MyConnection);
                MyCommand.Parameters.Add("user_id", userID);
                MyCommand.Parameters.Add("frdid", frdID);
            }
            else
            {
                MyCommand = new OracleCommand(
                               "SELECT N.ID,N.FRD_ID,FRD.TITLE,N.NOTE,N.SEEN,N.NOTIFICATION_TIME,N.FRD_TYPE,N.VERSION " +
                               "FROM NOTIFICATION N JOIN FRDOCUMENT FRD " +
                               "ON N.FRD_ID=FRD.ID AND N.USER_ID=:user_id AND N.FRD_TYPE=:type " +
                               "ORDER BY N.NOTIFICATION_TIME DESC", MyConnection);
                MyCommand.Parameters.Add("user_id", userID);
                switch (type)
                {
                    case NotificationsType.Active:
                        MyCommand.Parameters.Add("type",(int) Notification.FrdType.Active);
                        break;
                    case NotificationsType.Received://Received
                        MyCommand.Parameters.Add("type", (int)Notification.FrdType.Received);
                        break;
                    case NotificationsType.Closed://Received
                        MyCommand.Parameters.Add("type", (int)Notification.FrdType.Closed);
                        break;
                    case NotificationsType.Pending://Received
                        MyCommand.Parameters.Add("type", (int)Notification.FrdType.Pending);
                        break;
                }
            }


            MyCommand.BindByName = true;
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.NOTIFICATIONDataTable table = new Main_DB.NOTIFICATIONDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return list;
            }
            foreach (var x in table)
            {
                list.Add(new Notification
                {
                    ID = x.ID,
                    Content = x.NOTE,
                    NotiTime = x.NOTIFICATION_TIME,
                    FrdTitle = x.TITLE,
                    Seen = x.SEEN == "S" ? true : false,
                    Version = x.VERSION,
                    FrdId=x.FRD_ID
                });
            }

            return list;

        }


        public static int UnseenNotificationsCount()
        {
            MyCommand = new OracleCommand(
                "SELECT COUNT(N.ID) " +
                "FROM NOTIFICATION N JOIN FRDOCUMENT FRD ON N.FRD_ID=FRD.ID AND N.USER_ID=:user_id" +
                " WHERE N.SEEN='N' ORDER BY N.NOTIFICATION_TIME DESC", MyConnection);
            MyCommand.Parameters.Add("user_id", Session.Functions.GetID());
            int result=0;
            try
            {
                MyConnection.Open();
                result= Decimal.ToInt32((decimal)MyCommand.ExecuteScalar());
                MyConnection.Close();
            }
            catch (Exception e)
            {
                try
                {
                    MyConnection.Close();
                }
                catch (Exception)
                {
                }
                result = 0;
            }
            return result;
        }

        public static List<Notification> Read5Notifications(string userID)
        {
            List<Notification> list = new List<Notification>();
            MyCommand = new OracleCommand(
                "SELECT N.ID,FRD.TITLE,N.NOTE,N.SEEN,N.NOTIFICATION_TIME,N.FRD_TYPE,N.VERSION " +
                "FROM NOTIFICATION N JOIN FRDOCUMENT FRD " +
                "ON N.FRD_ID=FRD.ID AND N.USER_ID=:user_id " +
                "ORDER BY N.NOTIFICATION_TIME DESC", MyConnection);
            MyCommand.Parameters.Add("user_id", userID);

            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.NOTIFICATIONDataTable table = new Main_DB.NOTIFICATIONDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return list;
            }
            for (int y = 0; y < (table.Count > 5 ? 5 : table.Count); y++)
            {

                list.Add(new Notification
                {
                    ID = table[y].ID,
                    Content = table[y].NOTE,
                    NotiTime = table[y].NOTIFICATION_TIME,
                    FrdTitle = table[y].TITLE,
                    Seen = table[y].SEEN == "S" ? true : false,
                    Version = table[y].VERSION
                });
            }

            return list;

        }



        public static Notification GetNotiInfo(string id)
        {
            Notification n = null;
            MyCommand = new OracleCommand(
               "SELECT N.ID,N.FRD_ID,FRD.TITLE,N.NOTE,N.SEEN,N.NOTIFICATION_TIME,N.FRD_TYPE,N.VERSION " +
               "FROM NOTIFICATION N JOIN FRDOCUMENT FRD " +
               "ON N.FRD_ID=FRD.ID AND N.USER_ID=:user_id AND N.ID=:n_id " +
               "ORDER BY N.NOTIFICATION_TIME DESC", MyConnection);
            MyCommand.Parameters.Add("user_id", Session.Functions.GetID());
            MyCommand.Parameters.Add("n_id", id);
            MyCommand.BindByName = true;
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.NOTIFICATIONDataTable table = new Main_DB.NOTIFICATIONDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return null;
            }
            if (table.Count == 1)
            {
                n = new Notification
                {

                    FrdId = table[0].FRD_ID,
                    FRDType = (Notification.FrdType)table[0].FRD_TYPE,
                    Version = table[0].VERSION
                };
            }
            return n;
        }

        public static List<FrdVersion> ReadVersionsList(string frdId, int version = 0)
        {
            List<FrdVersion> list = new List<FrdVersion>();

            if (version == 0)
            {
                MyCommand = new OracleCommand("SELECT VERSIONS.VERSION,USERS.NAME,USERS.SURNAME,VERSIONS.NOTE,VERSIONS.EXPLANATION,VERSIONS.DATE_CREATED " +
                    "FROM VERSIONS,USERS " +
                    "WHERE FRD_ID=:frd_id AND VERSIONS.CREATOR_ID=USERS.ID " +
                    "ORDER BY VERSIONS.DATE_CREATED ASC", MyConnection);
                MyCommand.Parameters.Add("frd_id", frdId);
            }
            else
            {
                MyCommand = new OracleCommand(
                    "SELECT VERSIONS.VERSION,USERS.NAME,USERS.SURNAME,VERSIONS.NOTE,VERSIONS.EXPLANATION,VERSIONS.DATE_CREATED" +
                    " FROM VERSIONS,USERS " +
                    "WHERE FRD_ID=:frd_id AND VERSIONS.CREATOR_ID=USERS.ID AND VERSIONS.VERSION<=:version " +
                    "ORDER BY VERSIONS.VERSION ASC", MyConnection);
                MyCommand.Parameters.Add("frd_id", frdId);
                MyCommand.Parameters.Add("version", version);
            }
            MyAdapter = new OracleDataAdapter(MyCommand);
            FRD_DataSet.FRD_VersionsDataTable table = new FRD_DataSet.FRD_VersionsDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return list;
            }
            foreach (var row in table)
            {
                list.Add(
                    new FrdVersion
                    {
                        Number = Decimal.ToInt16(row.VERSION),
                        Date = row.DATE_CREATED,
                        Details = row.NOTE,
                        Explanation = row.EXPLANATION,
                        ReviserInfo = new Reviser { Name = row.NAME, Surname = row.SURNAME }
                    }
                );
            }
            return list;
        }



        public static List<Item> ReadItemsAndComments(string frdId, int version)
        {
            List<Item> list = new List<Item>();
            MyCommand = new OracleCommand(
                 "SELECT  I.ITEM_ID,I.TEXT AS ITEM_CONTENT,I.VERSION AS ITEM_VERSION,I.ADDED_DATE AS ITEM_DATE_ADDED,C.CONTENT AS COMMENT_CONTENT," +
                 "C.DATE_CREATED AS COMMENT_DATE_ADDED,C.VERSION AS COMMENT_VERSION,U.NAME,U.SURNAME  " +
                 "FROM FRD_ITEMS I LEFT OUTER JOIN ITEMS_COMMENTS C ON I.ITEM_ID=C.ITEM_ID AND C.VERSION<=:version " +
                 "LEFT OUTER JOIN USERS U ON U.ID=C.USER_ID WHERE I.FRD_ID=:frd_id AND I.VERSION<=:version " +
                 "ORDER BY  I.ROWID,I.ITEM_ID,I.ADDED_DATE,C.ROWID,C.DATE_CREATED ASC",
                 MyConnection);

            MyCommand.Parameters.Add("frd_id", frdId);
            MyCommand.Parameters.Add("version", version);
            MyCommand.BindByName = true;
            MyAdapter = new OracleDataAdapter(MyCommand);
            FRD_DataSet.FrdItemsCommentsDataTable table = new FRD_DataSet.FrdItemsCommentsDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return list;
            }
            string itemId = null;
            foreach (var row in table)
            {
                if (itemId == null || !itemId.Equals(row.ITEM_ID))
                {
                    if (row.COMMENT_CONTENT == null)
                    {
                        list.Add(new Item
                        {
                            ItemId = row.ITEM_ID,
                            Content = row.ITEM_CONTENT,
                            Date = row.ITEM_DATE_ADDED,
                            VersionNo = row.ITEM_VERSION,
                            Comments = new List<Comment>()
                        }
                        );
                    }
                    else
                    {
                        list.Add(new Item
                        {
                            ItemId = row.ITEM_ID,
                            Content = row.ITEM_CONTENT,
                            Date = row.ITEM_DATE_ADDED,
                            VersionNo = row.ITEM_VERSION,
                            Comments = new List<Comment>
                            {
                                new Comment {
                                    Content = row.COMMENT_CONTENT,
                                    Date = row.COMMENT_DATE_ADDED,
                                    VersionNo = row.COMMENT_VERSION,
                                    Commentor = new Reviser
                                    {
                                        Name = row.NAME,
                                        Surname = row.SURNAME
                                    }
                                }
                            }
                        }
                        );
                    }
                    itemId = row.ITEM_ID;
                }
                else
                {
                    list[list.Count - 1].Comments.Add(new Comment
                    {
                        Content = row.COMMENT_CONTENT,
                        Date = row.COMMENT_DATE_ADDED,
                        VersionNo = row.COMMENT_VERSION,
                        Commentor = new Reviser
                        {
                            Name = row.NAME,
                            Surname = row.SURNAME
                        }
                    }
                    );
                }
            }

            return list;
        }




        public static List<TargetAudience> ReadTargetsWithDefaults(string frdId, int version)
        {
            List<TargetAudience> list = new List<TargetAudience>();
            MyCommand = new OracleCommand("SELECT FRD_ID,TARGET,TARGET_CODE,VERSION FROM FRD_TARGETS WHERE FRD_ID=:frd_id AND VERSION=:version ORDER BY TARGET_CODE", MyConnection);
            MyCommand.Parameters.Add("frd_id", frdId);
            MyCommand.Parameters.Add("version", version);
            MyAdapter = new OracleDataAdapter(MyCommand);
            FRD_DataSet.FRD_TARGETSDataTable table = new FRD_DataSet.FRD_TARGETSDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return list;
            }
            List<TargetAudience> defaultList = DB_Functions.DefaultTargets();
            foreach (var row in table)
            {
                list.Add(new TargetAudience { Code = row.TARGET_CODE, Name = row.TARGET, Selected = true });
            }
            foreach (var x in defaultList)
            {
                bool found = false;
                foreach (var i in list)
                {
                    if (i.Code == x.Code && i.Name == x.Name)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    list.Add(x);
            }
            return list;
        }




        public static List<TargetAudience> ReadTargetsOnly(string frdId, int version)
        {
            List<TargetAudience> list = new List<TargetAudience>();
            MyCommand = new OracleCommand("SELECT FRD_ID,TARGET,TARGET_CODE,VERSION FROM FRD_TARGETS WHERE FRD_ID=:frd_id AND VERSION=:version ORDER BY TARGET_CODE", MyConnection);
            MyCommand.Parameters.Add("frd_id", frdId);
            MyCommand.Parameters.Add("version", version);
            MyAdapter = new OracleDataAdapter(MyCommand);
            FRD_DataSet.FRD_TARGETSDataTable table = new FRD_DataSet.FRD_TARGETSDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return list;
            }

            foreach (var row in table)
            {
                list.Add(new TargetAudience { Code = row.TARGET_CODE, Name = row.TARGET, Selected = true });
            }

            return list;
        }




        public static List<Channel> ReadChannelsWithDefaults(string frdId, int version)
        {
            List<Channel> list = new List<Channel>();
            MyCommand = new OracleCommand("SELECT FRD_ID,CHANNEL_NAME,CHANNEL_CODE,VERSION FROM FRD_CHANNELS WHERE FRD_ID=:frd_id AND VERSION=:version ORDER BY CHANNEL_CODE", MyConnection);
            MyCommand.Parameters.Add("frd_id", frdId);
            MyCommand.Parameters.Add("version", version);
            MyAdapter = new OracleDataAdapter(MyCommand);
            FRD_DataSet.FRD_CHANNELSDataTable table = new FRD_DataSet.FRD_CHANNELSDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return list;
            }
            List<Channel> Dblist = DB_Functions.DefaultChannels();
            foreach (var row in table)
            {
                list.Add(new Channel { Code = row.CHANNEL_CODE, Name = row.CHANNEL_NAME, Selected = true });
            }
            foreach (var channel in Dblist)
            {
                bool found = false;
                foreach (var i in list)
                {
                    if (i.Code == channel.Code && i.Name == channel.Name)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    list.Add(channel);

            }
            return list;
        }




        public static List<Channel> ReadChannelsOnly(string frdId, int version)
        {
            List<Channel> list = new List<Channel>();
            MyCommand = new OracleCommand("SELECT FRD_ID,CHANNEL_NAME,CHANNEL_CODE,VERSION FROM FRD_CHANNELS WHERE FRD_ID=:frd_id AND VERSION=:version ORDER BY CHANNEL_CODE", MyConnection);
            MyCommand.Parameters.Add("frd_id", frdId);
            MyCommand.Parameters.Add("version", version);
            MyAdapter = new OracleDataAdapter(MyCommand);
            FRD_DataSet.FRD_CHANNELSDataTable table = new FRD_DataSet.FRD_CHANNELSDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return list;
            }
            foreach (var row in table)
            {
                list.Add(
                    new Channel
                    {
                        Code = row.CHANNEL_CODE,
                        Name = row.CHANNEL_NAME,
                        Selected = true
                    }
                    );
            }
            return list;
        }



        public static List<SMS> ReadSms(string frdId, int Version, List<FrdVersion> Vlist)
        {
            List<SMS> list = new List<SMS>();
            MyCommand = new OracleCommand(
               "SELECT FSMS.SENDER,FSMS.CODE,FSMS.CONTENT_EN,FSMS.CONTENT_TR,FSMS.DATE_ADDED,U.NAME,U.SURNAME,FSMS.VERSION " +
               "FROM FRD_SMS FSMS,VERSIONS V,USERS U " +
               "WHERE FSMS.FRD_ID=:frd_id AND FSMS.VERSION<= :version AND FSMS.FRD_ID=V.FRD_ID " +
               "AND FSMS.VERSION=V.VERSION AND U.ID=V.CREATOR_ID ORDER BY FSMS.ROWID,FSMS.DATE_ADDED ASC", MyConnection);
            MyCommand.Parameters.Add("frd_id", frdId);
            MyCommand.Parameters.Add("version", Version);
            MyAdapter = new OracleDataAdapter(MyCommand);
            FRD_DataSet.FRD_SMSDataTable table = new FRD_DataSet.FRD_SMSDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return list;
            }
            foreach (var row in table)
            {
                FrdVersion version = Vlist.Find(x => x.Number == row.VERSION);
                list.Add(new SMS
                {
                    Sender = row.SENDER,
                    Code = row.CODE,
                    EnContent = row.CONTENT_EN,
                    TrContent = row.CONTENT_TR,
                    Version = Decimal.ToInt16(row.VERSION),
                    Time = version.Date,
                    AddedBy = version.ReviserInfo
                });
            }

            return list;
        }





        public static List<Attatchment> ReadFiles(string frdId, int Version)
        {
            List<Attatchment> list = new List<Attatchment>();
            MyCommand = new OracleCommand(
               "SELECT FILE_NAME,OREGENAL_NAME,TIME_UPLOADED,VERSION,FILE_SIZE " +
               "FROM FRD_FILES " +
               "WHERE FRD_ID=:frd_id AND VERSION<=:version " +
               "ORDER BY TIME_UPLOADED ASC"
               , MyConnection);
            MyCommand.Parameters.Add("frd_id", frdId);
            MyCommand.Parameters.Add("version", Version);
            MyAdapter = new OracleDataAdapter(MyCommand);
            FRD_DataSet.FRD_FILESDataTable table = new FRD_DataSet.FRD_FILESDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return list;
            }
            foreach (var row in table)
            {
                list.Add(new Attatchment
                {
                    Id = row.FILE_NAME,
                    Name = row.OREGENAL_NAME,
                    Date = row.TIME_UPLOADED,
                    Version = Decimal.ToInt32(row.VERSION),
                    Size = GetFileSize(row.FILE_SIZE)
                });
            }

            return list;
        }


        public static List<Attatchment> ReadTempFiles(string UploadToken)
        {
            List<Attatchment> list = new List<Attatchment>();
            MyCommand = new OracleCommand(
                    "SELECT FILE_NAME,EXTENSION,UPLOAD_DATE,OREGENAL_NAME,UPLOAD_TOKEN,FILE_SIZE " +
                    "from TEMP_FILES " +
                    "where UPLOAD_TOKEN=:token",
                    MyConnection);

            MyCommand.Parameters.Add("token", UploadToken);
            MyAdapter = new OracleDataAdapter(MyCommand);
            FRD_DataSet.TempFilesDataTable table = new FRD_DataSet.TempFilesDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return list;
            }
            foreach (var row in table)
            {
                list.Add(new Attatchment
                {
                    Id = row.FILE_NAME,
                    Name = row.OREGENAL_NAME,
                    Date = row.UPLOAD_DATE,
                    Version = 1,
                    Size = row.FILE_SIZE.ToString()
                });
            }

            return list;
        }

        public static bool DeleteTempFile(string token, string filename)
        {
            MyCommand = new OracleCommand(
                    "SELECT FILE_NAME,EXTENSION,UPLOAD_DATE,OREGENAL_NAME,UPLOAD_TOKEN,FILE_SIZE " +
                    "FROM TEMP_FILES " +
                    "WHERE UPLOAD_TOKEN=:token AND OREGENAL_NAME=:O_NAME",
                    MyConnection);

            MyCommand.Parameters.Add("token", token);
            MyCommand.Parameters.Add("O_NAME", filename);
            MyAdapter = new OracleDataAdapter(MyCommand);
            FRD_DataSet.TempFilesDataTable table = new FRD_DataSet.TempFilesDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return false;
            }
            if (table.Count == 1)
            {
                var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Files"), table[0].FILE_NAME + table[0].EXTENSION.Replace("application/","."));
                if (System.IO.File.Exists(path))
                {
                    try
                    {
                        System.IO.File.Delete(path);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
                MyCommand = new OracleCommand(
                    "DELETE FROM TEMP_FILES " +
                    "WHERE FILE_NAME=:fileid",
                    MyConnection);

                MyCommand.Parameters.Add("fileid", table[0].FILE_NAME);
                try
                {
                    MyConnection.Open();
                    MyCommand.ExecuteNonQuery();
                    MyConnection.Close();
                }
                catch(Exception e)
                {
                    try
                    {
                        MyConnection.Close();
                    }
                    catch (Exception)
                    {
                    }
                    return false;
                }
            }
            return true;
        }

        private static string GetFileSize(decimal fileSize)
        {
            long size = Decimal.ToInt64(fileSize);
            if (size >= (1024 * 1024))
            {
                return (size / (1024 * 1024)) + " MB";
            }
            if (size >= (1024))
            {
                return (size / (1024)) + " KB";
            }
            else
            {
                return size + " Bytes";
            }

        }

        public static List<Discount> ReadDiscounts(string frdId, int Version)
        {
            List<Discount> list = new List<Discount>();
            MyCommand = new OracleCommand(
               "SELECT NAME,CODE,VERSION " +
               "FROM FRD_DISCOUNTS " +
               "WHERE FRD_ID=:frd_id AND VERSION<=:version " +
               "ORDER BY ROWID, NAME ASC"
               , MyConnection);
            MyCommand.Parameters.Add("frd_id", frdId);
            MyCommand.Parameters.Add("version", Version);
            MyAdapter = new OracleDataAdapter(MyCommand);
            FRD_DataSet.FRD_DISCOUNTSDataTable table = new FRD_DataSet.FRD_DISCOUNTSDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
                return list;
            }
            foreach (var row in table)
            {
                list.Add(new Discount { DiscountName = row.NAME, DiscountCode = row.CODE, Version = Decimal.ToInt32(row.VERSION) });
            }

            return list;
        }


        public static List<Distribution_Groups> ReadConfirmations(string frdId, int version)
        {
            List<Distribution_Groups> list = new List<Distribution_Groups>();
            MyCommand = new OracleCommand(
                "SELECT FGM.GROUP_ID,FG.GROUP_NAME,FGM.MEMBER_ID,U.NAME,U.SURNAME,CN.STATUS,CN.LAST_UPDATE" +
                " FROM FRD_GROUPS FG JOIN FRD_GROUP_MEMBERS FGM ON FG.FRD_ID=FGM.FRD_ID AND FG.GROUP_ID=FGM.GROUP_ID " +
                " JOIN USERS U ON U.ID=FGM.MEMBER_ID " +
                "LEFT OUTER JOIN CONFIRMATIONS CN  ON FGM.MEMBER_ID = CN.EMPLOYEE_ID AND CN.VERSION=:version " +
                "AND FG.GROUP_ID=CN.GROUP_ID AND FG.FRD_ID=CN.FRD_ID " +
                "WHERE FG.FRD_ID=:frd_id " +
                " ORDER BY FG.GROUP_NAME,U.NAME,FGM.MEMBER_ID", MyConnection);
            MyCommand.Parameters.Add(":frd_id", frdId);
            MyCommand.Parameters.Add(":version", version);
            MyCommand.BindByName = true;
            MyAdapter = new OracleDataAdapter(MyCommand);
            FRD_DataSet.FrdApprovalsDataTable table = new FRD_DataSet.FrdApprovalsDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {
            }
            string groupId = null;
            foreach (var row in table)
            {
                if (groupId == null || !groupId.Equals(row.GROUP_ID))
                {
                    groupId = row.GROUP_ID;
                    if (row.STATUS != null)
                    {
                        list.Add(new Distribution_Groups
                        {
                            Id = row.GROUP_ID,
                            Name = row.GROUP_NAME,
                            Employess = new List<Confirmation> { new Confirmation { Id = row.MEMBER_ID, Name = row.NAME, Surname = row.SURNAME,
                        Status=row.STATUS=="W"?Confirmation.Status_Enum.Waiting:row.STATUS=="A"?Confirmation.Status_Enum.Approved:Confirmation.Status_Enum.Rejected,
                        LastUpdate=row.LAST_UPDATE} }
                        });
                    }
                    else
                    {
                        list.Add(new Distribution_Groups
                        {
                            Id = row.GROUP_ID,
                            Name = row.GROUP_NAME,
                            Employess = new List<Confirmation> { new Confirmation { Id = row.MEMBER_ID, Name = row.NAME, Surname = row.SURNAME, Selected = true, Status = Confirmation.Status_Enum.Waiting } }
                        });
                    }

                }
                else
                {
                    if (row.STATUS != null)
                    {
                        list[list.Count - 1].Employess.Add(new Confirmation
                        {
                            Id = row.MEMBER_ID,
                            Name = row.NAME,
                            Surname = row.SURNAME,
                            Status = row.STATUS == "W" ? Confirmation.Status_Enum.Waiting : row.STATUS == "A" ? Confirmation.Status_Enum.Approved : Confirmation.Status_Enum.Rejected,
                            LastUpdate = row.LAST_UPDATE
                        });
                    }
                    else
                    {
                        list[list.Count - 1].Employess.Add(new Confirmation
                        {
                            Id = row.MEMBER_ID,
                            Name = row.NAME,
                            Surname = row.SURNAME,
                            Selected = true,
                            Status = Confirmation.Status_Enum.Waiting
                        });
                    }
                }

            }
            return list;
        }




        //=======================================================================================================
        //===========================================================================
        //===================================================


        //Manager Functions


        public static bool ManagerApprove(string frdId)
        {
            Frd Original = CheckIfManager(frdId);
            if (Original == null)
                return false;

            MyCommand = new OracleCommand("UPDATE FRDOCUMENT SET STATUS='A' WHERE ID=:id", MyConnection);
            MyCommand.Parameters.Add("id", frdId);
            int count2;
            try
            {
                MyConnection.Open();
                count2 = MyCommand.ExecuteNonQuery();
                NotifyOwner(frdId, 1, Original.OwnerId, Session.Functions.GetName() + " Approved the FRD");
                NotifyMembers(frdId, 1, "You Received a New FRD");
            }
            catch (Exception e)
            {
                try
                {
                    MyConnection.Close();
                }
                catch (Exception) { }
                return false;
            }

            if (count2 == 1)
            {
                try
                {
                    MyConnection.Close();
                }
                catch (Exception)
                {

                }
                return true;
            }


            try
            {
                MyConnection.Close();
            }
            catch (Exception e)
            {
            }
            return false;
        }




        public static bool ManagerReject(string frdId, string explanation)
        {
            Frd Original = CheckIfManager(frdId);
            if (Original == null)
                return false;
            try
            {
                MyConnection.Open();
                MyTransaction = MyConnection.BeginTransaction();
                if (CloseFrd(frdId))
                {
                    AddNewVersion(frdId, "FRD Was Rejected By The Manager", Original.LatestVersion + 1, explanation ?? "No Explanation Provided");
                }
                MyTransaction.Commit();
                NotifyOwner(frdId, 1, Original.OwnerId, Session.Functions.GetName() + " Rejected the FRD");
                MyConnection.Close();
            }
            catch (Exception e)
            {
                try
                {
                    MyTransaction.Rollback();
                }
                catch (Exception)
                { }
                try
                {
                    MyConnection.Close();
                }
                catch (Exception)
                { }
                return false;
            }


            return true;
        }






        //=======================================================================================================
        //===========================================================================
        //===================================================




        public static bool OwnerClose(string frdID)
        {
            Frd Original = CheckActive(frdID);
            if (Original == null)
                return false;
            try
            {
                MyConnection.Open();
                MyTransaction = MyConnection.BeginTransaction();
                if (CloseFrd(frdID))
                {
                    AddNewVersion(frdID, "The Frd Was Closed By The Owner", Original.LatestVersion + 1);
                }
                MyTransaction.Commit();
                NotifyMembers(frdID, Original.LatestVersion + 1, Original.Panel0.Title + " Was Closed By The Owner");
                MyConnection.Close();
                return true;
            }
            catch (Exception e)
            {
                try
                {
                    MyTransaction.Rollback();
                }
                catch (Exception) { }
                try
                {
                    MyConnection.Close();
                }
                catch (Exception) { }
                return false;
            }

        }




        public static bool CloseFrd(string frdId)
        {
            MyCommand = new OracleCommand("UPDATE FRDOCUMENT SET STATUS='C' WHERE ID=:id", MyConnection);
            MyCommand.Parameters.Add("id", frdId);
            int count2;
            try
            {
                count2 = MyCommand.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }

            if (count2 == 1)
            {
                return true;
            }
            return false;
        }




        //=======================================================================================================
        //===========================================================================
        //===================================================


        //Received User Options



        public static bool SubmitReceiverUpdate(Frd inViewModel)
        {
            try
            {

                //only saves new comments and files
                Frd original = CheckReceived(inViewModel.Panel0.Id);
                if (original == null)
                    return false;
                ActiveFrdProcessor.Process_Panel2_3(ref original);
                int version = original.LatestVersion + 1;
                original.Panel7 = new Panel_7 { DistributionGroups = DB_Functions.ReadConfirmations(original.Panel0.Id, original.LatestVersion) };
                MyCommand = new OracleCommand(
                    "SELECT FILE_NAME,EXTENSION,UPLOAD_DATE,OREGENAL_NAME,UPLOAD_TOKEN,FILE_SIZE" +
                    " FROM TEMP_FILES " +
                    "WHERE UPLOAD_TOKEN=:token", MyConnection);
                MyCommand.Parameters.Add("token", inViewModel.Upload_Token);
                MyAdapter = new OracleDataAdapter(MyCommand);
                FRD_DataSet.TempFilesDataTable table = new FRD_DataSet.TempFilesDataTable();
                MyAdapter.Fill(table);
                if (inViewModel.Panel1 == null && table.Count == 0)
                    return false;
                MyConnection.Open();
                MyTransaction = MyConnection.BeginTransaction();
                if (inViewModel.VersionNotes == null)
                    inViewModel.VersionNotes = "No Note";
                else if (inViewModel.VersionNotes.Trim().Count() == 0)
                    inViewModel.VersionNotes = "No Note";
                AddNewVersion(inViewModel.Panel0.Id, inViewModel.VersionNotes, version);
                SavePanel1WithComments(ref inViewModel, version);
                SavePanel2(original.Panel0.Id, original.Panel2, version);
                SavePanel3(original.Panel0.Id, original.Panel3, version);
                SavePanel5(inViewModel.Upload_Token, inViewModel.Panel0.Id, table, version);



                MyTransaction.Commit();

                //Notifications function  here 
                string Note = "New Version Was Created By " + Session.Functions.GetName() + " on The Frd:" + original.Panel0.Title + " version: " + original.LatestVersion;
                NotifyOwner(inViewModel.Panel0.Id, version, original.OwnerId, Note);

                //Call DB Procedure
                string memberNote = Session.Functions.GetName() + " Added New Update to the FRD";
                NotifyMembers(inViewModel.Panel0.Id, version, memberNote);


                MyConnection.Close();
                //here email functions if any required 
                //Email.send_email();

            }
            catch (Exception e)
            {
                try
                {
                    MyTransaction.Rollback();
                }
                catch (Exception)
                { }
                try
                {
                    MyConnection.Close();
                }
                catch (Exception)
                { }
                return false;
            }
            return true;
        }

        private static void NotifyMembers(string id, int version, string memberNote)
        {
            MyCommand = new OracleCommand("NOTIFIY_FRD", MyConnection);
            MyCommand.CommandType = System.Data.CommandType.StoredProcedure;
            MyCommand.Parameters.Add("IN_FRD_ID", id);
            MyCommand.Parameters.Add("IN_VERSION", version);
            MyCommand.Parameters.Add("IN_NOTE", memberNote);
            MyCommand.Parameters.Add("IN_TYPE", (int)Notification.FrdType.Received);
            MyCommand.Parameters.Add("IN_USER", Session.Functions.GetID());
            MyCommand.ExecuteNonQuery();
        }

        private static void NotifyOwner(string id, int latestVersion, string ownerId, string Note)
        {
            MyCommand = new OracleCommand("INSERT INTO NOTIFICATION (FRD_ID,VERSION, USER_ID,NOTE,FRD_TYPE) VALUES(:a,:b,:c,:d,:e)", MyConnection);
            MyCommand.Parameters.Add("a", id);
            MyCommand.Parameters.Add("b", latestVersion);
            MyCommand.Parameters.Add("c", ownerId);
            MyCommand.Parameters.Add("d", Note);
            MyCommand.Parameters.Add("e", (int)Notification.FrdType.Active);
            MyCommand.BindByName = true;
            MyCommand.ExecuteNonQuery();
        }


        private static void NotifyManager(string id, string managerId, string Note)
        {
            MyCommand = new OracleCommand("INSERT INTO NOTIFICATION (FRD_ID,VERSION, USER_ID,NOTE,FRD_TYPE) VALUES(:a,:b,:c,:d,:e)", MyConnection);
            MyCommand.Parameters.Add("a", id);
            MyCommand.Parameters.Add("b", 1);
            MyCommand.Parameters.Add("c", managerId);
            MyCommand.Parameters.Add("d", Note);
            MyCommand.Parameters.Add("e", (int)Notification.FrdType.Received);
            MyCommand.BindByName = true;
            MyCommand.ExecuteNonQuery();
        }





        //=======================================================================================================
        //===========================================================================
        //===================================================



        //Active FRD owner Options


        public static bool SubmitOwnerUpdate(Frd inViewModel)
        {
            try
            {
                Frd original = CheckActive(inViewModel.Panel0.Id);
                if (original == null)
                    return false;
                if (inViewModel.Panel2 != null || inViewModel.Panel3 != null)
                    ActiveFrdProcessor.Process_Panel2_3(ref original);
                int version = original.LatestVersion + 1;
                FRD_DataSet.TempFilesDataTable table = new FRD_DataSet.TempFilesDataTable();

                GetTempFiles(inViewModel.Upload_Token, ref table);


                if (!CheckChanges(ref inViewModel, ref original, ref table))
                    return false;

                if (inViewModel.VersionNotes == null)
                    inViewModel.VersionNotes = "No Note";
                else if (inViewModel.VersionNotes.Trim().Count() == 0)
                    inViewModel.VersionNotes = "No Note";


                MyConnection.Open();
                MyTransaction = MyConnection.BeginTransaction();
                AddNewVersion(inViewModel.Panel0.Id, inViewModel.VersionNotes, version);


                SavePanel1WithComments(ref inViewModel, version);
                SavePanel2(inViewModel.Panel0.Id, inViewModel.Panel2, version);
                SavePanel3(inViewModel.Panel0.Id, inViewModel.Panel3, version);
                SavePanel4(inViewModel.Panel0.Id, inViewModel.Panel4, version);
                SavePanel5(inViewModel.Upload_Token, inViewModel.Panel0.Id, table, version);
                SavePanel6(inViewModel.Panel0.Id, inViewModel.Panel6, version);

                MyTransaction.Commit();
                //Notifications function  here 

                NotifyMembers(inViewModel.Panel0.Id, version, "Owner Added New Update to:" + original.Panel0.Title + " FRD");

                MyConnection.Close();
            }
            catch (Exception e)
            {
                try
                {
                    MyTransaction.Rollback();
                }
                catch (Exception)
                { }
                try
                {
                    MyConnection.Close();
                }
                catch (Exception)
                { }
                return false;
            }

            return true;
        }

        private static void GetTempFiles(string upload_Token, ref FRD_DataSet.TempFilesDataTable table)
        {
            MyCommand = new OracleCommand("SELECT FILE_NAME,EXTENSION,UPLOAD_DATE,OREGENAL_NAME,UPLOAD_TOKEN,FILE_SIZE from TEMP_FILES where UPLOAD_TOKEN=:token", MyConnection);
            MyCommand.Parameters.Add("token", upload_Token);
            MyAdapter = new OracleDataAdapter(MyCommand);
            MyAdapter.Fill(table);

        }

        private static bool CheckChanges(ref Frd inViewModel, ref Frd original, ref FRD_DataSet.TempFilesDataTable table)
        {
            if (table.Count > 0)
                return true;

            if (inViewModel.Panel1 != null)
                return true;
            if (inViewModel.Panel2 != null)
            {
                List<string> targets = new List<string>();
                foreach (var x in original.Panel2.Targets)
                    targets.Add(x.Code);

                foreach (var x in inViewModel.Panel2.Targets)
                    if (!targets.Contains(x.Code))
                        return true;
            }
            if (inViewModel.Panel3 != null)
            {
                List<string> channels = new List<string>();

                foreach (var x in original.Panel3.Channels)
                    channels.Add(x.Code);

                foreach (var x in inViewModel.Panel3.Channels)
                    if (!channels.Contains(x.Code))
                        return true;
            }
            if (inViewModel.Panel4 != null)
                return true;
            if (inViewModel.Panel6 != null)
                return true;


            return false;
        }



        //=======================================================================================================
        //===========================================================================
        //===================================================


        // File Functions


        public static Attatchment GetFile(string frdId, string id)
        {
            Attatchment file = null;
            MyCommand = new OracleCommand("SELECT F.FILE_NAME,F.OREGENAL_NAME,F.EXTENSION FROM FRD_FILES F WHERE F.FRD_ID=:frd_id AND F.FILE_NAME=:file_name ", MyConnection);
            MyCommand.Parameters.Add("frd_id", frdId);
            MyCommand.Parameters.Add("file_name", id);
            MyCommand.BindByName = true;
            try
            {
                MyConnection.Open();
                OracleDataReader reader = MyCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    file = new Attatchment
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1),
                        Owner = reader.GetString(2)
                    };
                }
                reader.Close();
                MyConnection.Close();

            }
            catch(Exception e)
            {
                try
                {
                    MyConnection.Close();
                }
                catch (Exception)
                {
                }
            }

            return file;
        }



        //=======================================================================================================
        //===========================================================================
        //===================================================


        // File Functions




        public static bool User_Approve(string frdId)
        {
            Frd Original = CheckReceived(frdId);
            if (Original == null)
                return false;
            MyCommand = new OracleCommand("UPDATE CONFIRMATIONS SET STATUS='A',LAST_UPDATE=SYSTIMESTAMP WHERE EMPLOYEE_ID=:user_id AND FRD_ID=:frd_id AND VERSION=:version", MyConnection);
            MyCommand.Parameters.Add("user_id", Session.Functions.GetID());
            MyCommand.Parameters.Add("frd_id", frdId);
            MyCommand.Parameters.Add("version", Original.LatestVersion);
            MyCommand.BindByName = true;
            try
            {
                MyConnection.Open();
                MyTransaction = MyConnection.BeginTransaction();
                MyCommand.ExecuteNonQuery();
                MyTransaction.Commit();
                //notify the owner
                string Note = Session.Functions.GetName() + " Approved The Frd:" + Original.Panel0.Title + " version: " + Original.LatestVersion;
                NotifyOwner(frdId, Original.LatestVersion, Original.OwnerId, Note);
                MyConnection.Close();

            }
            catch (Exception e)
            {
                try
                {
                    MyTransaction.Rollback();
                }
                catch (Exception)
                {
                }
                try
                {
                    MyConnection.Close();
                }
                catch (Exception)
                {
                }
                return false;
            }

            return true;
        }



        public static bool User_Reject(string frdId)
        {
            Frd Original = CheckReceived(frdId);
            if (Original == null)
                return false;
            MyCommand = new OracleCommand("UPDATE CONFIRMATIONS SET STATUS='R',LAST_UPDATE=SYSTIMESTAMP WHERE EMPLOYEE_ID=:user_id AND FRD_ID=:frd_id AND VERSION=:version", MyConnection);
            MyCommand.Parameters.Add("user_id", Session.Functions.GetID());
            MyCommand.Parameters.Add("frd_id", frdId);
            MyCommand.Parameters.Add("version", Original.LatestVersion);
            MyCommand.BindByName = true;
            try
            {
                MyConnection.Open();
                MyTransaction = MyConnection.BeginTransaction();
                MyCommand.ExecuteNonQuery();


                MyTransaction.Commit();
                //notify the owner

                string Note = Session.Functions.GetName() + " Rejected The Frd:" + Original.Panel0.Title + " version: " + Original.LatestVersion;
                NotifyOwner(frdId, Original.LatestVersion, Original.OwnerId, Note);
                MyConnection.Close();

            }
            catch (Exception e)
            {
                try
                {
                    MyTransaction.Rollback();
                }
                catch (Exception)
                {
                }
                try
                {
                    MyConnection.Close();
                }
                catch (Exception)
                {
                }
                return false;
            }

            return true;
        }


        //Search


        public static List<string> SearchAutoComplete(string text)
        {
            List<string> list = new List<string>();
            MyCommand = new OracleCommand("SELECT FRD.ID,FRD.TITLE,FRD.STATUS,FRG.MEMBER_ID " +
                "FROM FRDOCUMENT FRD LEFT OUTER JOIN FRD_GROUP_MEMBERS FRG ON FRD.ID = FRG.FRD_ID AND FRG.MEMBER_ID=:user_id " +
                "WHERE (LOWER(FRD.TITLE) LIKE '%'|| :text ||'%' OR LOWER(FRD.ID) LIKE '%'|| :text ||'%') " +
                "AND (FRD.USER_ID=:user_id OR (FRG.MEMBER_ID=:user_id AND FRD.STATUS<>'C')) AND ROWNUM<6", MyConnection);
            MyCommand.Parameters.Add("text", text.ToLower());
            MyCommand.Parameters.Add("user_id", Session.Functions.GetID());
            MyCommand.BindByName = true;
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.SearchResultDataTable table = new Main_DB.SearchResultDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {

            }
            foreach (var x in table)
            {
                list.Add(x.TITLE);
            }
            return list;
        }


        public static List<SearchItem> Search(string text)
        {
            List<SearchItem> list = new List<SearchItem>();
            MyCommand = new OracleCommand(
                "SELECT FT.ID,FT.TITLE,VT.VERSION,VT.NOTE,IT.TEXT,IT.VERSION AS ITEM_VERSION FROM FRDOCUMENT FT " +
                "LEFT OUTER JOIN VERSIONS VT " +
                "ON VT.FRD_ID=FT.ID AND VT.VERSION!=1 AND (LOWER(VT.NOTE) LIKE '%'|| :text ||'%') " +
                "LEFT OUTER JOIN FRD_ITEMS IT " +
                "ON IT.FRD_ID=FT.ID AND (VT.VERSION IS NULL OR IT.VERSION=VT.VERSION) AND (LOWER(IT.TEXT) LIKE '%'|| :text ||'%')  " +
                "WHERE ((LOWER(FT.ID) LIKE '%'|| :text ||'%') OR (LOWER(FT.TITLE) LIKE '%'|| :text ||'%') OR (LOWER(VT.NOTE) LIKE '%'|| :text ||'%') OR (LOWER(IT.TEXT) LIKE '%'|| :text ||'%')) " +
                "AND FT.ID IN(SELECT F.ID FROM FRDOCUMENT F JOIN FRD_GROUP_MEMBERS M ON M.FRD_ID=F.ID WHERE F.USER_ID=:user_id OR M.MEMBER_ID=:user_id)", MyConnection);
            MyCommand.Parameters.Add("text", text.Trim().ToLower());
            MyCommand.Parameters.Add("user_id", Session.Functions.GetID());
            MyCommand.BindByName = true;
            MyAdapter = new OracleDataAdapter(MyCommand);
            Main_DB.SearchResultDataTable table = new Main_DB.SearchResultDataTable();
            try
            {
                MyAdapter.Fill(table);
            }
            catch (Exception e)
            {

            }
            string frd_id = null;
            int version = 0;
            foreach (var row in table)
            {
                if (frd_id == null || !frd_id.Equals(row.ID))
                {
                    frd_id = row.ID;
                    version = 0;
                    list.Add(new SearchItem
                    {
                        FrdId = row.ID,
                        Title = row.TITLE,
                        I_List = new List<SearchItem.Item_Result>(),
                        V_List = new List<SearchItem.Version_Result>()
                    });


                }
                if (row.NOTE != null && (version == 0 || version != row.VERSION))
                {
                    list[list.Count - 1].V_List.Add(new SearchItem.Version_Result
                    {
                        Version = row.VERSION,
                        Note = row.NOTE
                    });
                    version = row.VERSION;
                }
                if (row.TEXT != null)
                    list[list.Count - 1].I_List.Add(new SearchItem.Item_Result
                    {
                        Version = row.ITEM_VERSION,
                        ItemText = row.TEXT
                    });
            }
            return list;
        }


        public static void NotiSeen(string ID)
        {
            MyCommand = new OracleCommand("UPDATE NOTIFICATION SET SEEN='S' WHERE ID=:id", MyConnection);
            MyCommand.Parameters.Add("ID", ID);

            try
            {
                MyConnection.Open();
                MyCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {

                try
                {
                    MyConnection.Close();
                }
                catch (Exception)
                {
                }
            }

            try
            {
                MyConnection.Close();
            }
            catch (Exception)
            {
            }
        }



        public static void FrdNotisSeen(string FrdID,int Version)
        {
            MyCommand = new OracleCommand("UPDATE NOTIFICATION SET SEEN='S' WHERE FRD_ID=:frd_id AND VERSION=:version AND USER_ID=:user_id", MyConnection);
            MyCommand.Parameters.Add("frd_id", FrdID);
            MyCommand.Parameters.Add("version", Version);
            MyCommand.Parameters.Add("user_id", Session.Functions.GetID());

            try
            {
                MyConnection.Open();
                MyCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {

                try
                {
                    MyConnection.Close();
                }
                catch (Exception)
                {
                }
            }

            try
            {
                MyConnection.Close();
            }
            catch (Exception)
            {
            }
        }


    }
}