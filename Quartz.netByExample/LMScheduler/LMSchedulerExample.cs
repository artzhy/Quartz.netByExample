using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz.netByExample.Interfaces;
using System.Collections.Specialized;
using Quartz.Impl;

namespace Quartz.netByExample.LMScheduler
{
    public class LMSchedulerExample : ILMScheduler
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
                ["quartz.plugin.jobInitializer.scanInterval"] = "10",
                ["quartz.scheduler.instanceName"] = "TestScheduler",
                ["quartz.scheduler.instanceId"] = "instance_one",
                ["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz",
                ["quartz.threadPool.threadCount"] = "5",
                ["quartz.jobStore.misfireThreshold"] = "60000",
                ["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
                ["quartz.jobStore.useProperties"] = "false",
                ["quartz.jobStore.dataSource"] = "default",
                ["quartz.jobStore.tablePrefix"] = "QRTZ_",
                ["quartz.jobStore.clustered"] = "true",
                ["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz",
                ["quartz.dataSource.default.connectionString"] =
                    @"Server=LENOVO-PC\SQLEXPRESS;Database=CountingKs;Trusted_Connection=True;",
                ["quartz.dataSource.default.provider"] = "SqlServer-20"
            };

            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory(properties);
            IScheduler sched = sf.GetScheduler();

            //log.Warn("***** Deleting existing jobs/triggers *****");
            //sched.Clear();

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
