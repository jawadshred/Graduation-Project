using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using turkcell_web_app.ViewModels;
using turkcell_web_app.Models.DB_Adapters;
namespace turkcell_web_app.Models.FRD_Process
{
    public class ActiveFrdProcessor
    {
        public static Frd Process(Frd file,int V=0)
        {
            string FRD_ID = file.Panel0.Id;
            file.Panel0.Versions = DB_Functions.ReadVersionsList(FRD_ID, V);

            int version;
            if (file.Type == Frd.TypesEnum.Closed && V == 0)
            {
                if (file.Panel0.Versions.Count > 1)
                {
                    version = file.Panel0.Versions[file.Panel0.Versions.Count - 2].Number;
                }
                else
                {
                    version = file.Panel0.Versions[file.Panel0.Versions.Count - 1].Number;
                }
            }
            else
            {
                version = file.Panel0.Versions[file.Panel0.Versions.Count - 1].Number;
            }

            file.Panel1 = new Panels.Panel_1() {
                Items = DB_Functions.ReadItemsAndComments(FRD_ID, version)
            };

            file.Panel2 = new Panels.Panel_2 { Targets = DB_Functions.ReadTargetsWithDefaults(FRD_ID, version) };


            file.Panel3 = new Panels.Panel_3 { Channels = DB_Functions.ReadChannelsWithDefaults(FRD_ID, version) };


            file.Panel4 = new Panels.Panel_4 { Senders = DB_Functions.DefaultSmsSenders(), SMS = DB_Functions.ReadSms(FRD_ID, version, file.Panel0.Versions) };


            file.Panel5 = new Panels.Panel_5 { Attatchments = DB_Functions.ReadFiles(FRD_ID, version) };

            foreach(var x in file.Panel5.Attatchments)
            {
                var nn = file.Panel0.Versions.Find(z => z.Number == x.Version);
                x.Owner = nn.ReviserInfo.Name+" "+nn.ReviserInfo.Surname;
            }

            file.Panel6 = new Panels.Panel_6 { DiscountItems = DB_Functions.ReadDiscounts(FRD_ID, version) };
            file.Panel7 = new Panels.Panel_7 { DistributionGroups = DB_Functions.ReadConfirmations(FRD_ID, version) };
            DB_Functions.FrdNotisSeen(file.Panel0.Id, file.LatestVersion);
            return file;
        }


        public static void Process_Panel2_3(ref Frd file)
        {

            string FRD_ID = file.Panel0.Id;

            file.Panel2 = new Panels.Panel_2 { Targets = DB_Functions.ReadTargetsOnly(FRD_ID, file.LatestVersion) };

            file.Panel3 = new Panels.Panel_3 { Channels = DB_Functions.ReadChannelsOnly(FRD_ID, file.LatestVersion) };


        }
    }
}