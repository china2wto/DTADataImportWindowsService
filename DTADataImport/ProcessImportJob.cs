using System;

using log4net;
using System.Data;
using DTADataImport;
using Quartz;

namespace DTADataImport
{
    /// <summary>
    /// This is just a simple job that gets fired off many times by example 15.
    /// </summary>
    /// <author>Bill Kratzer</author>
    /// <author>Marko Lahma (.NET)</author>
    public class ProcessImportJob : IJob
    {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof (ProcessImportJob));

        /// <summary>
        /// Called by the <see cref="IScheduler" /> when a
        /// <see cref="ITrigger" /> fires that is associated with the <see cref="IJob" />.
        /// </summary>
        public virtual void Execute(IJobExecutionContext context)
        {
            JobKey jobKey = context.JobDetail.Key;
            //LOGGER.InfoFormat("SimpleJob says: {0} executing at {1}", jobKey, DateTime.Now.ToString("r"));
            new ProcessImport().process();
        }
    }
}