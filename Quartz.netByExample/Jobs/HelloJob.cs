using System;

namespace Quartz.netByExample.Jobs
{
    public class HelloJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Hello World! - {0}", DateTime.Now.ToString("r"));
        }
    }
}