using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;

namespace Quartz.netByExample.Interfaces
{
    public interface ILMScheduler
    {
        Task Run(ILog log);
    }
}
