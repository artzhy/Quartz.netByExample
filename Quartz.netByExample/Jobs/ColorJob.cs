using System;

namespace Quartz.netByExample.Jobs
{
    /// <summary>
    /// This is just a simple job that receives parameters and
    /// maintains state.
    /// </summary>
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ColorJob : IJob
    {

        // parameter names specific to this job
        public const string FavoriteColor = "favorite color";
        public const string ExecutionCount = "count";

        // Since Quartz will re-instantiate a class every time it
        // gets executed, members non-static member variables can
        // not be used to maintain state!
        private int counter = 1;

        /// <summary>
        /// Called by the <see cref="IScheduler" /> when a
        /// <see cref="ITrigger" /> fires that is associated with
        /// the <see cref="IJob" />.
        /// </summary>
        public void Execute(IJobExecutionContext context)
        {
            // This job simply prints out its job name and the
            // date and time that it is running
            JobKey jobKey = context.JobDetail.Key;

            // Grab and print passed parameters
            JobDataMap data = context.JobDetail.JobDataMap;
            string favoriteColor = data.GetString(FavoriteColor);
            int count = data.GetInt(ExecutionCount);
            Console.WriteLine(
                "ColorJob: {0} executing at {1}\n  favorite color is {2}\n  execution count (from job map) is {3}\n  execution count (from job member variable) is {4}",
                jobKey,
                DateTime.Now.ToString("r"),
                favoriteColor,
                count, counter);

            // increment the count and store it back into the 
            // job map so that job state can be properly maintained
            count++;
            data.Put(ExecutionCount, count);

            // Increment the local member variable 
            // This serves no real purpose since job state can not 
            // be maintained via member variables!
            counter++;
        }
    }
}