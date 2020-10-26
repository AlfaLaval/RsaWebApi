using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentGenerate
{
    public class AppSettings
    {
        public static string Environment => System.Configuration.ConfigurationManager.AppSettings["Environment"] + "_";

        public static string ImageUploadPath => System.Configuration.ConfigurationManager.AppSettings[Environment + "ImageUploadPath"];
        public static string GoldenTemplate => System.Configuration.ConfigurationManager.AppSettings[Environment + "GoldenTemplate"];
        public static string DocTempPath => System.Configuration.ConfigurationManager.AppSettings[Environment + "DocTempPath"];

        public static string HostEmailAddress => System.Configuration.ConfigurationManager.AppSettings[Environment + "HostEmailAddress"];
        public static string HostEmailPassord => System.Configuration.ConfigurationManager.AppSettings[Environment + "HostEmailPassord"];
        public static string SmtpClientHost => System.Configuration.ConfigurationManager.AppSettings[Environment + "SmtpClientHost"];
        public static int SmtpClientPort => int.Parse(System.Configuration.ConfigurationManager.AppSettings[Environment + "SmtpClientPort"]);
        public static int WaitingTimeForNextCycle => int.Parse(System.Configuration.ConfigurationManager.AppSettings["WaitingTimeForNextCycle"]);
    }
}
