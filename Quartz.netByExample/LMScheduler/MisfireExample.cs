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
    public class MisfireExample: ILMScheduler
    {
        public async Task Run(ILog log)
        {
            log.Info("------- Initializing -------------------");

            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();

            log.Info("------- Initialization Complete -----------");

            log.Info("------- Scheduling Jobs -----------");

            // jobs can be scheduled before start() has been called

            // get a "nice round" time a few seconds in the future...

            DateTimeOffset startTime = DateBuilder.NextGivenSecondDate(null, 15);

            // statefulJob1 will run every three seconds
            // (but it will delay for ten seconds)
            IJobDetail job = JobBuilder.Create<StatefulDumbJob>()
                .WithIdentity("statefulJob1", "group1")
                .UsingJobData(StatefulDumbJob.ExecutionDelay, 10000L)
                .Build();

            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartAt(startTime)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(3).RepeatForever())
                .Build();

            DateTimeOffset ft = sched.ScheduleJob(job, trigger);
            log.Info($"{job.Key} will run at: {ft.ToString("r")} and repeat: {trigger.RepeatCount} times, every {trigger.RepeatInterval.TotalSeconds} seconds");

            // statefulJob2 will run every three seconds
            // (but it will delay for ten seconds - and therefore purposely misfire after a few iterations)
            job = JobBuilder.Create<StatefulDumbJob>()
                .WithIdentity("statefulJob2", "group1")
                .UsingJobData(StatefulDumbJob.ExecutionDelay, 10000L)
                .Build();

            trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger2", "group1")
                .StartAt(startTime)
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(3)
                    .RepeatForever()
                    .WithMisfireHandlingInstructionIgnoreMisfires()) // set misfire instructions
                .Build();
            ft = sched.ScheduleJob(job, trigger);

            log.Info($"{job.Key} will run at: {ft.ToString("r")} and repeat: {trigger.RepeatCount} times, every {trigger.RepeatInterval.TotalSeconds} seconds");

            // MisfireJob will run every three seconds
            // (but it will delay for ten seconds - and therefore purposely misfire after a few iterations)
            job = JobBuilder.Create<MisfireJob>()
                .WithIdentity("MisfireJob", "group1")
                .UsingJobData(MisfireJob.ExecutionDelay, 30L)
                .Build();

            trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger3", "group1")
                .StartAt(startTime)
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(3)
                    .RepeatForever()
                    .WithMisfireHandlingInstructionNowWithExistingCount()) // set misfire instructions
                .Build();
            ft = sched.ScheduleJob(job, trigger);

            log.Info($"{job.Key} will run at: {ft.ToString("r")} and repeat: {trigger.RepeatCount} times, every {trigger.RepeatInterval.TotalSeconds} seconds");


            log.Info("------- Starting Scheduler ----------------");

            // jobs don't start firing until start() has been called...
            sched.Start();

            log.Info("------- Started Scheduler -----------------");

            // sleep for ten minutes for triggers to file....
            await Task.Delay(TimeSpan.FromMinutes(10));

            log.Info("------- Shutting Down ---------------------");

            sched.Shutdown(true);

            log.Info("------- Shutdown Complete -----------------");

            SchedulerMetaData metaData = sched.GetMetaData();
            log.Info($"Executed {metaData.NumberOfJobsExecuted} jobs.");
        }
    }
}
