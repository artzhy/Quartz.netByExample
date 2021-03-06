﻿using System;
using System.Threading.Tasks;
using Common.Logging;
using Quartz.netByExample.Interfaces;
using Quartz.Impl;
using Quartz.netByExample.Jobs;

namespace Quartz.netByExample.LMScheduler
{
    public class CronTriggerExample : ILMScheduler
    {
        public async Task Run(ILog log)
        {
            log.Info("------- Initializing -------------------");

            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched =  sf.GetScheduler();

            log.Info("------- Initialization Complete --------");

            log.Info("------- Scheduling Jobs ----------------");

            // jobs can be scheduled before sched.start() has been called

            // job 1 will run every 20 seconds

            IJobDetail job = JobBuilder.Create<SimpleJob>()
                .WithIdentity("job1", "group1")
                .Build();

            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithCronSchedule("0/20 * * * * ?")
                .Build();

            DateTimeOffset ft =  sched.ScheduleJob(job, trigger);
            log.Info(job.Key + " has been scheduled to run at: " + ft
                     + " and repeat based on expression: "
                     + trigger.CronExpressionString);

            // job 2 will run every other minute (at 15 seconds past the minute)
            job = JobBuilder.Create<SimpleJob>()
                .WithIdentity("job2", "group1")
                .Build();

            trigger = (ICronTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger2", "group1")
                .WithCronSchedule("15 0/2 * * * ?")
                .Build();

            ft = sched.ScheduleJob(job, trigger);
            log.Info(job.Key + " has been scheduled to run at: " + ft
                     + " and repeat based on expression: "
                     + trigger.CronExpressionString);

            // job 3 will run every other minute but only between 8am and 5pm
            job = JobBuilder.Create<SimpleJob>()
                .WithIdentity("job3", "group1")
                .Build();

            trigger = (ICronTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger3", "group1")
                .WithCronSchedule("0 0/2 8-17 * * ?")
                .Build();

            ft = sched.ScheduleJob(job, trigger);
            log.Info(job.Key + " has been scheduled to run at: " + ft
                     + " and repeat based on expression: "
                     + trigger.CronExpressionString);

            // job 4 will run every three minutes but only between 5pm and 11pm
            job = JobBuilder.Create<SimpleJob>()
                .WithIdentity("job4", "group1")
                .Build();

            trigger = (ICronTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger4", "group1")
                .WithCronSchedule("0 0/3 17-23 * * ?")
                .Build();

            ft = sched.ScheduleJob(job, trigger);
            log.Info(job.Key + " has been scheduled to run at: " + ft
                     + " and repeat based on expression: "
                     + trigger.CronExpressionString);

            // job 5 will run at 10am on the 1st and 15th days of the month
            job = JobBuilder.Create<SimpleJob>()
                .WithIdentity("job5", "group1")
                .Build();

            trigger = (ICronTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger5", "group1")
                .WithCronSchedule("0 0 10am 1,15 * ?")
                .Build();

            ft = sched.ScheduleJob(job, trigger);
            log.Info(job.Key + " has been scheduled to run at: " + ft
                     + " and repeat based on expression: "
                     + trigger.CronExpressionString);

            // job 6 will run every 30 seconds but only on Weekdays (Monday through Friday)
            job = JobBuilder.Create<SimpleJob>()
                .WithIdentity("job6", "group1")
                .Build();

            trigger = (ICronTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger6", "group1")
                .WithCronSchedule("0,30 * * ? * MON-FRI")
                .Build();

            ft = sched.ScheduleJob(job, trigger);
            log.Info(job.Key + " has been scheduled to run at: " + ft
                     + " and repeat based on expression: "
                     + trigger.CronExpressionString);

            // job 7 will run every 30 seconds but only on Weekends (Saturday and Sunday)
            job = JobBuilder.Create<SimpleJob>()
                .WithIdentity("job7", "group1")
                .Build();

            trigger = (ICronTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger7", "group1")
                .WithCronSchedule("0,30 * * ? * SAT,SUN")
                .Build();

            ft = sched.ScheduleJob(job, trigger);
            log.Info(job.Key + " has been scheduled to run at: " + ft
                     + " and repeat based on expression: "
                     + trigger.CronExpressionString);

            job = JobBuilder.Create<SimpleJob>()
                .WithIdentity("job8", "group1")
                .Build();

            trigger = (ICronTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger8", "group1")
                .WithCronSchedule("0 35 5 21 OCT ? *")
                .Build();

            ft = sched.ScheduleJob(job, trigger);
            log.Info(job.Key + " has been scheduled to run at: " + ft
                     + " and repeat based on expression: "
                     + trigger.CronExpressionString);

            log.Info("------- Starting Scheduler ----------------");

            // All of the jobs have been added to the scheduler, but none of the
            // jobs
            // will run until the scheduler has been started
            sched.Start();

            log.Info("------- Started Scheduler -----------------");

            log.Info("------- Waiting five minutes... ------------");

            // wait five minutes to show jobs
            await Task.Delay(TimeSpan.FromMinutes(5));
            // executing...

            log.Info("------- Shutting Down ---------------------");

            sched.Shutdown(true);

            log.Info("------- Shutdown Complete -----------------");

            SchedulerMetaData metaData = sched.GetMetaData();
            log.Info($"Executed {metaData.NumberOfJobsExecuted} jobs.");
        }
    }
}