using System;

namespace Quartz.netByExample.Jobs
{
    public class SimpleJob: IJob
    {
        public string MyJobKey { get; set; }

        public void Execute(IJobExecutionContext context)
        {
            // This job simply prints out its job name and the
            // date and time that it is running
            JobKey jobKey = context.JobDetail.Key;
            Console.WriteLine("SimpleJob says: {0} executing at {1}", jobKey, DateTime.Now.ToString("r"));
            Console.WriteLine("SimpleJob MyJobKey: {0} executing at {1}", MyJobKey, DateTime.Now.ToString("r"));
        }
    }
}