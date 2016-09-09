using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz.netByExample.Jobs;

namespace Quartz.netByExample.Listener
{
    public class JobListener : IJobListener
    {
        public string Name
        {
            get { return "job1_to_job2"; }
        }

        public void JobExecutionVetoed(IJobExecutionContext context)
        {
            Console.WriteLine("Job1Listener says: Job Execution was vetoed.");
        }

        public void JobToBeExecuted(IJobExecutionContext context)
        {
            Console.WriteLine("Job1Listener says: Job Is about to be executed.");
        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            Console.WriteLine("Job1Listener says: Job was executed.");

            // Simple job #2
            IJobDetail job2 = JobBuilder.Create<SimpleJob>()
                .WithIdentity("job2")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("job2Trigger")
                .StartNow()
                .Build();

            try
            {
                // schedule the job to run!
                context.Scheduler.ScheduleJob(job2, trigger);
            }
            catch (SchedulerException e)
            {
                Console.WriteLine("Unable to schedule job2!");
                Console.Error.WriteLine(e.StackTrace);
            }
        }
    }
}
