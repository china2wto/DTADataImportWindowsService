using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using log4net;
using System.IO;
using System.Configuration;
using DTADataImport;
using System.Threading;

namespace DTADataImportWindowsService
{
    public partial class DTADataImportService : ServiceBase
    {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(DTADataImportService));
        public DTADataImportService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            LOGGER.Info("###############   windows service : DTADataImportWindowsService start!   ###################");
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["windows_service_status"].Value="start";
            config.Save();

           //可以设置一个公共变量（不一定要static的，但必须主线程和子线程都能访问），当主线程结束时设置为true，子线程在循环体中检测变量，检测到true是结束
            Thread workThread = new Thread(new ThreadStart(processThread));
            workThread.IsBackground = true;
            workThread.Start();
            LOGGER.Info("###############  windows service : DTADataImportWindowsService OnStart end!   ###################");
        }

        protected override void OnStop()
        {
            LOGGER.Info("###############   windows service : DTADataImportWindowsService OnStop()!   ###################");
            //解决windows服务产生的子进程，跟着主进程一起结束的问题。
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["windows_service_status"].Value = "stop";
            config.Save();
            LOGGER.Info("###############   windows service : DTADataImportWindowsService OnStop() end!   ###################");
        }

        private static void processThread()
        {

            LOGGER.Info("============ DTADataImportService.processThread() start! ================");

            //if ("stop".Equals(ConfigurationManager.AppSettings.Get("windows_service_status")))
            //{  
            //    LOGGER.Info("processThread ready for breaking!");
            //    break;
            //}
            //if (hasSameProcess())
            //{
            //    return;
            //}
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

            LOGGER.Info("================ DTADataImportService.processThread() end! ================");
           
        }


        private static bool hasSameProcess()
        {
            try
            {
                System.Diagnostics.Process myProcess = System.Diagnostics.Process.GetCurrentProcess();
                string processName = myProcess.MainModule.FileName;
                int processId = myProcess.Id;

                Process[] processes = Process.GetProcesses();
                foreach (Process process in processes)
                {
                    if (process.ProcessName.Equals(myProcess.ProcessName))
                    {
                        if (!process.Id.Equals(processId) && process.MainModule.FileName.Equals(processName))
                        {
                            LOGGER.Info("有相同的进程了！");
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LOGGER.Error(e.ToString());
                return true;
            }
            return false;
        }
    }
}
