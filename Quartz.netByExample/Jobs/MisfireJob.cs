using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartz.netByExample.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class MisfireJob: IJob
    {
        public const string NumExecutions = "NumExecutions";
        public const string ExecutionDelay = "ExecutionDelay";

        public async void Execute(IJobExecutionContext context)
        {
            JobKey jobKey = context.JobDetail.Key;
            Console.WriteLine($"---{jobKey} executing at {DateTime.Now.ToString("r")}");

            // default delay to five seconds
            int delay = 5;

            // use the delay passed in as a job parameter (if it exists)
            JobDataMap map = context.JobDetail.JobDataMap;
            if (map.ContainsKey(ExecutionDelay))
            {
                delay = map.GetInt(ExecutionDelay);
            }

            await Task.Delay(TimeSpan.FromSeconds(delay));

            Console.WriteLine(jobKey);
        }
    }
}
