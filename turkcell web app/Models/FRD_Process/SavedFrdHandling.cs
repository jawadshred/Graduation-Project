using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.Models.DB_Adapters;
using turkcell_web_app.ViewModels;
using turkcell_web_app.Models.Panels.Classes;

namespace turkcell_web_app.Models.FRD_Process
{
    public class SavedFrdHandling
    {
        public static Frd ProcessInfo(Frd InFrd)
        {
            if (InFrd.Panel1 == null)
            {
                InFrd.Panel1 = new Panels.Panel_1();
                InFrd.Panel1.Items = new List<Item>();
            }



            //Panel2
            if (InFrd.Panel2 == null)
            {
                InFrd.Panel2 = new Panels.Panel_2();
                InFrd.Panel2.Targets = DB_Adapters.DB_Functions.DefaultTargets();
            }
            else
            {
                List<string> mylist = new List<string>();
                foreach (var k in InFrd.Panel2.Targets)
                {
                    mylist.Add(k.Code);
                }
                List<TargetAudience> DBList = DB_Adapters.DB_Functions.DefaultTargets();
                foreach(var x in DBList)
                {
                    if (!mylist.Contains(x.Code))
                    {
                        InFrd.Panel2.Targets.Add(x);
                    }
                }
            }

            
            //Panel3
            if (InFrd.Panel3 == null)
            {
                InFrd.Panel3 = new Panels.Panel_3();
                InFrd.Panel3.Channels = DB_Adapters.DB_Functions.DefaultChannels();
            }
            else
            {
                List<string> mylist = new List<string>();
                foreach(var k in InFrd.Panel3.Channels)
                {
                    mylist.Add(k.Code);
                }
                List<Channel> DBList = DB_Adapters.DB_Functions.DefaultChannels();
                foreach (var x in DBList)
                {
                    if (!mylist.Contains(x.Code))
                    {
                        InFrd.Panel3.Channels.Add(x);
                    }
                }
            }

            //Panel4
            if (InFrd.Panel4 == null)
            {
                InFrd.Panel4 = new Panels.Panel_4();
                InFrd.Panel4.Senders = DB_Adapters.DB_Functions.DefaultSmsSenders();
                InFrd.Panel4.SMS = new List<SMS>();
            }
            else
            {
                InFrd.Panel4.Senders = DB_Adapters.DB_Functions.DefaultSmsSenders();
            }


            //Panel 5
            if (InFrd.Panel5 == null)
            {
                InFrd.Panel5 = new Panels.Panel_5();
            }


            if (InFrd.Upload_Token != null)
            {
                InFrd.Panel5.Attatchments = DB_Functions.ReadTempFiles(InFrd.Upload_Token);
            }

            //Panel 6
            if (InFrd.Panel6 == null)
            {
                InFrd.Panel6 = new Panels.Panel_6();
            }

            //Panel 7
            if (InFrd.Panel7 == null)
            {
                InFrd.Panel7 = new Panels.Panel_7();
                InFrd.Panel7.DistributionGroups = DB_Adapters.DB_Functions.DefaultGroups();
            }
            else
            {
                Dictionary<string, List<String>> groups = new Dictionary<string, List<string>>();
                List<String> IDs = new List<string>();
                foreach(var n in InFrd.Panel7.DistributionGroups)
                {
                    groups.Add(n.Id, new List<string>());
                        foreach(var m in n.Employess)
                    {
                        groups[n.Id].Add(m.Id);
                    }

                }
                List<Distribution_Groups> DB_List = DB_Adapters.DB_Functions.DefaultGroups();
                foreach(var g in DB_List)
                {
                    if (groups.ContainsKey(g.Id))
                    {
                        foreach (var m in g.Employess)
                        {
                            if (groups[g.Id].Contains(m.Id))
                                m.Selected = true;
                        }
                    }
                }
                InFrd.Panel7.DistributionGroups = DB_List;
               
            }

            return InFrd;
        }
    }
}