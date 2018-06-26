using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using turkcell_web_app.Models.DB_Adapters;
using turkcell_web_app.ViewModels;

namespace turkcell_web_app.Models.FRD_Process
{
    public class FRD_Submit
    {



        public static bool Save(Frd FRD_File)
        {
            //save frd
            DB_Functions.SaveToDraft(ToByteArray(FRD_File));
            return true;
        }



        private static byte[] ToByteArray(object source)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                return stream.ToArray();
            }
        }
    }
}