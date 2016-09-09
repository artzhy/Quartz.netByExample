using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz.Impl;
using Quartz.netByExample.Interfaces;
using System.Collections.Specialized;

namespace Quartz.netByExample.LMScheduler
{
    public class PluginExample: ILMScheduler
    {
        public async Task Run(ILog log)
        {
            // our properties that enable XML configuration plugin
            // and makes it watch for changes every two minutes (120 seconds)
            var properties = new NameValueCollection
            {
                ["quartz.plugin.triggHistory.type"] = "Quartz.Plugin.History.LoggingJobHistoryPlugin",
                ["quartz.plugin.jobInitializer.type"] = "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin",
                ["quartz.plugin.jobInitializer.fileNames"] = "JobConfig.xml",
                ["quartz.plugin.jobInitializer.failOnFileNotFound"] = "true",
                ["quartz.plugin.jobInitializer.scanInterval"] = "120"
            };

            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory(properties);
            IScheduler sched = sf.GetScheduler();

            log.Info("------- Initialization Complete -----------");

            log.Info("------- Not Scheduling any Jobs - relying on XML definitions --");

            log.Info("------- Starting Scheduler ----------------");

            // start the schedule
            sched.Start();

            log.Info("------- Started Scheduler -----------------");

            log.Info("------- Waiting five minutes... -----------");

            // wait five minutes to give our jobs a chance to run
            await Task.Delay(TimeSpan.FromMinutes(5));

            // shut down the scheduler
            log.Info("------- Shutting Down ---------------------");
            sched.Shutdown(true);
            log.Info("------- Shutdown Complete -----------------");

            SchedulerMetaData metaData = sched.GetMetaData();
            log.Info("Executed " + metaData.NumberOfJobsExecuted + " jobs.");
        }
    }
}
