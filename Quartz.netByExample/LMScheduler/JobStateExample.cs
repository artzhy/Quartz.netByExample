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
    public class JobStateExample: ILMScheduler
    {
        public async Task Run(ILog log)
        {
            log.Info("------- Initializing -------------------");

            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();

            log.Info("------- Initialization Complete --------");

            log.Info("------- Scheduling Jobs ----------------");

            // get a "nice round" time a few seconds in the future....
            DateTimeOffset startTime = DateBuilder.NextGivenSecondDate(null, 10);

            // job1 will only run 5 times (at start time, plus 4 repeats), every 10 seconds
            IJobDetail job1 = JobBuilder.Create<ColorJob>()
                .WithIdentity("job1", "group1")
                .Build();

            ISimpleTrigger trigger1 = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartAt(startTime)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).WithRepeatCount(4))
                .Build();

            // pass initialization parameters into the job
            job1.JobDataMap.Put(ColorJob.FavoriteColor, "Green");
            job1.JobDataMap.Put(ColorJob.ExecutionCount, 1);

            // schedule the job to run
            DateTimeOffset scheduleTime1 = sched.ScheduleJob(job1, trigger1);
            log.Info($"{job1.Key} will run at: {scheduleTime1.ToString("r")} and repeat: {trigger1.RepeatCount} times, every {trigger1.RepeatInterval.TotalSeconds} seconds");

            // job2 will also run 5 times, every 10 seconds

            IJobDetail job2 = JobBuilder.Create<ColorJob>()
                .WithIdentity("job2", "group1")
                .Build();

            ISimpleTrigger trigger2 = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger2", "group1")
                .StartAt(startTime)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).WithRepeatCount(4))
                .Build();

            // pass initialization parameters into the job
            // this job has a different favorite color!
            job2.JobDataMap.Put(ColorJob.FavoriteColor, "Red");
            job2.JobDataMap.Put(ColorJob.ExecutionCount, 1);

            // schedule the job to run
            DateTimeOffset scheduleTime2 = sched.ScheduleJob(job2, trigger2);
            log.Info($"{job2.Key} will run at: {scheduleTime2.ToString("r")} and repeat: {trigger2.RepeatCount} times, every {trigger2.RepeatInterval.TotalSeconds} seconds");

            log.Info("------- Starting Scheduler ----------------");

            // All of the jobs have been added to the scheduler, but none of the jobs
            // will run until the scheduler has been started
            sched.Start();

            log.Info("------- Started Scheduler -----------------");

            log.Info("------- Waiting 60 seconds... -------------");

            // wait five minutes to show jobs
            await Task.Delay(TimeSpan.FromMinutes(5));
            // executing...

            log.Info("------- Shutting Down ---------------------");

            sched.Shutdown(true);

            log.Info("------- Shutdown Complete -----------------");

            SchedulerMetaData metaData =  sched.GetMetaData();
            log.Info($"Executed {metaData.NumberOfJobsExecuted} jobs.");
        }
    }
}
