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
    public class InterruptExample : ILMScheduler
    {
        public async Task Run(ILog log)
        {
            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();

            log.Info("------- Initialization Complete -----------");

            log.Info("------- Scheduling Jobs -------------------");

            // get a "nice round" time a few seconds in the future...

            DateTimeOffset startTime = DateBuilder.NextGivenSecondDate(null, 15);

            IJobDetail job = JobBuilder.Create<SimpleJob>()
                .WithIdentity("interruptableJob1", "group1")
                .Build();

            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartAt(startTime)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever())
                .Build();

            DateTimeOffset ft = sched.ScheduleJob(job, trigger);
            log.Info($"{job.Key} will run at: {ft.ToString("r")} and repeat: {trigger.RepeatCount} times, every {trigger.RepeatInterval.TotalSeconds} seconds");

            // start up the scheduler (jobs do not start to fire until
            // the scheduler has been started)
            sched.Start();
            log.Info("------- Started Scheduler -----------------");

            log.Info("------- Starting loop to interrupt job every 7 seconds ----------");
            for (int i = 0; i < 50; i++)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(7));
                    // tell the scheduler to interrupt our job
                    sched.Interrupt(job.Key);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            log.Info("------- Shutting Down ---------------------");

            sched.Shutdown(true);

            log.Info("------- Shutdown Complete -----------------");
            SchedulerMetaData metaData = sched.GetMetaData();
            log.Info($"Executed {metaData.NumberOfJobsExecuted} jobs.");
        }
    }
}
