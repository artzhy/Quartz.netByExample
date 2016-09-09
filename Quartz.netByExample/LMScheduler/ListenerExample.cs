using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.netByExample.Interfaces;
using Quartz.netByExample.Jobs;
using Quartz.netByExample.Listener;

namespace Quartz.netByExample.LMScheduler
{
    public class ListenerExample : ILMScheduler
    {
        public async Task Run(ILog log)
        {
            log.Info("------- Initializing ----------------------");

            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();

            log.Info("------- Initialization Complete -----------");

            log.Info("------- Scheduling Jobs -------------------");

            // schedule a job to run immediately
            IJobDetail job = JobBuilder.Create<SimpleJob>()
                .WithIdentity("job1")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1")
                .StartNow()
                .Build();

            // Set up the listener
            IJobListener listener = new JobListener();
            IMatcher<JobKey> matcher = KeyMatcher<JobKey>.KeyEquals(job.Key);
            sched.ListenerManager.AddJobListener(listener, matcher);

            // schedule the job to run
            sched.ScheduleJob(job, trigger);

            // All of the jobs have been added to the scheduler, but none of the jobs
            // will run until the scheduler has been started
            log.Info("------- Starting Scheduler ----------------");
            sched.Start();

            // wait 30 seconds:
            // note:  nothing will run
            log.Info("------- Waiting 30 seconds... --------------");

            // wait 30 seconds to show jobs
            await Task.Delay(TimeSpan.FromSeconds(30));
            // executing...

            // shut down the scheduler
            log.Info("------- Shutting Down ---------------------");
            sched.Shutdown(true);
            log.Info("------- Shutdown Complete -----------------");

            SchedulerMetaData metaData = sched.GetMetaData();
            log.Info($"Executed {metaData.NumberOfJobsExecuted} jobs.");
        }
    }
}
