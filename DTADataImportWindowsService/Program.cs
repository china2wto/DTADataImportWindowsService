using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Diagnostics;
using log4net;

namespace DTADataImportWindowsService
{
    static class Program
    {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(Program));
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            LOGGER.Info("DTADataImportWindowsService.Program.Main()");
            //1. 解决windows服务产生的子进程，跟着主进程一起结束的问题。
            //2. 日志common.log日志打印问题。
            //3. quartz工作的时候1小时停止的问题。做到24天，然后再做个循环。ok
            //4. 部署计划
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new DTADataImportService() 
			};
            ServiceBase.Run(ServicesToRun);
        }

    }
}
