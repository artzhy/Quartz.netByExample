using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz.Impl;
using Quartz.netByExample.Interfaces;
using Quartz.netByExample.Jobs;

namespace Quartz.netByExample.LMScheduler
{
    public class JobExceptionExample: ILMScheduler
    {
        public async Task Run(ILog log)
        {
            log.Info("------- Initializing ----------------------");

            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();

            log.Info("------- Initialization Complete ------------");

            log.Info("------- Scheduling Jobs -------------------");

            // jobs can be scheduled before start() has been called

            // get a "nice round" time a few seconds in the future...
            DateTimeOffset startTime = DateBuilder.NextGivenSecondDate(null, 15);

            // badJob1 will run every 10 seconds
            // this job will throw an exception and refire
            // immediately
            IJobDetail job = JobBuilder.Create<BadJob>()
                .WithIdentity("badJob1", "group1")
                .UsingJobData("denominator", "0")
                .Build();

            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartAt(startTime)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever())
                .Build();

            DateTimeOffset ft = sched.ScheduleJob(job, trigger);
            log.Info(job.Key + " will run at: " + ft + " and repeat: "
                     + trigger.RepeatCount + " times, every "
                     + trigger.RepeatInterval.TotalSeconds + " seconds");

            // badJob2 will run every five seconds
            // this job will throw an exception and never
            // refire
            job = JobBuilder.Create<BadJob2>()
                .WithIdentity("badJob2", "group1")
                .Build();

            trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger2", "group1")
                .StartAt(startTime)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever())
                .Build();
            ft = sched.ScheduleJob(job, trigger);
            log.Info($"{job.Key} will run at: {ft.ToString("r")} and repeat: {trigger.RepeatCount} times, every {trigger.RepeatInterval.TotalSeconds} seconds");

            log.Info("------- Starting Scheduler ----------------");

            // jobs don't start firing until start() has been called...
            sched.Start();

            log.Info("------- Started Scheduler -----------------");

            // sleep for 30 seconds
            await Task.Delay(TimeSpan.FromSeconds(30));

            log.Info("------- Shutting Down ---------------------");

            sched.Shutdown(false);

            log.Info("------- Shutdown Complete -----------------");

            SchedulerMetaData metaData = sched.GetMetaData();
            log.Info($"Executed {metaData.NumberOfJobsExecuted} jobs.");
        }
    }
}
