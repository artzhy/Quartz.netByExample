using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz.Impl;
using Quartz.Impl.Calendar;
using Quartz.netByExample.Interfaces;
using Quartz.netByExample.Jobs;

namespace Quartz.netByExample.LMScheduler
{
    public class CalendarExample: ILMScheduler
    {
        public async Task Run(ILog log)
        {
            log.Info("------- Initializing ----------------------");

            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();

            log.Info("------- Initialization Complete -----------");

            log.Info("------- Scheduling Jobs -------------------");

            // Add the holiday calendar to the schedule
            AnnualCalendar holidays = new AnnualCalendar();

            // fourth of July (July 4)
            DateTime fourthOfJuly = new DateTime(DateTime.UtcNow.Year, 7, 4);
            holidays.SetDayExcluded(fourthOfJuly, true);

            // halloween (Oct 31)
            DateTime halloween = new DateTime(DateTime.UtcNow.Year, 10, 31);
            holidays.SetDayExcluded(halloween, true);

            // christmas (Dec 25)
            DateTime christmas = new DateTime(DateTime.UtcNow.Year, 12, 25);
            holidays.SetDayExcluded(christmas, true);

            // tell the schedule about our holiday calendar
             sched.AddCalendar("holidays", holidays, false, false);

            // schedule a job to run hourly, starting on halloween
            // at 10 am

            DateTimeOffset runDate = DateBuilder.DateOf(0, 0, 10, 31, 10);

            IJobDetail job = JobBuilder.Create<SimpleJob>()
                .WithIdentity("job1", "group1")
                .Build();

            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartAt(runDate)
                .WithSimpleSchedule(x => x.WithIntervalInHours(1).RepeatForever())
                .ModifiedByCalendar("holidays")
                .Build();

            // schedule the job and print the first run date
            DateTimeOffset firstRunTime = sched.ScheduleJob(job, trigger);

            // print out the first execution date.
            // Note:  Since Halloween (Oct 31) is a holiday, then
            // we will not run until the next day! (Nov 1)
            log.Info($"{job.Key} will run at: {firstRunTime.ToString("r")} and repeat: {trigger.RepeatCount} times, every {trigger.RepeatInterval.TotalSeconds} seconds");

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
