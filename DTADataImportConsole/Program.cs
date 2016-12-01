using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using DTADataImport;
using System.Threading;

namespace DTADataImportConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string run_quartz = ConfigurationManager.AppSettings.Get("run_quartz");
            if ("true".Equals(run_quartz))
            {
                IExample example = new XmlConfigurationExample();
                example.Run();
            }
            else
            {
                new ProcessImport().process();
            }
        }
    }
}
