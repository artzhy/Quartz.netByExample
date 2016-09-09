using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartz.netByExample.Jobs
{
    /// <summary>
    /// A dumb implementation of Job, for unit testing purposes.
    /// </summary>
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class StatefulDumbJob: IJob
    {
        public const string NumExecutions = "NumExecutions";
        public const string ExecutionDelay = "ExecutionDelay";

        public async void Execute(IJobExecutionContext context)
        {
            Console.Error.WriteLine("---{0} executing.[{1}]", context.JobDetail.Key, DateTime.Now.ToString("r"));

            JobDataMap map = context.JobDetail.JobDataMap;

            int executeCount = 0;
            if (map.ContainsKey(NumExecutions))
            {
                executeCount = map.GetInt(NumExecutions);
            }

            executeCount++;

            map.Put(NumExecutions, executeCount);

            int delay = 5;
            if (map.ContainsKey(ExecutionDelay))
            {
                delay = map.GetInt(ExecutionDelay);
            }

            await Task.Delay(delay);

            Console.Error.WriteLine("  -{0} complete ({1}).", context.JobDetail.Key, executeCount);
        }
    }
}
