using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Common.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.netByExample.Interfaces;
using Quartz.netByExample.LMScheduler;
using Serilog;


namespace Quartz.netByExample
{
    class Program
    {
        public static IContainer Container;

        static void Main(string[] args)
        {

            Console.WriteLine("1-SimpleScheduler\n2-SimpleScheduler2\n3-CronTriggerExample\n" +
                              "4-JobStateExample\n5-MisfireExample\n6-JobExceptionExample\n7-InterruptExample\n" +
                              "8-CalendarExample\n9-ListenerExample\n10-PluginExample\n" +
                              "11-LM Example");

            int option = Convert.ToInt32(Console.ReadLine());

            ContainerBuilder builder = new ContainerBuilder();

            var logConf = new LoggerConfiguration().WriteTo.ColoredConsole().CreateLogger();
            // set global instance of Serilog logger which Common.Logger.Serilog requires.
            Log.Logger = logConf;

            builder.RegisterInstance(LogManager.GetLogger("SimpleScheduler")).As<ILog>();

            switch (option)
            {
                case 1:
                    builder.RegisterType<SimpleScheduler>().As<ILMScheduler>();
                    break;
                case 2:
                    builder.RegisterType<SimpleScheduler2>().As<ILMScheduler>();
                    break;
                case 3:
                    builder.RegisterType<CronTriggerExample>().As<ILMScheduler>();
                    break;
                case 4:
                    builder.RegisterType<JobStateExample>().As<ILMScheduler>();
                    break;
                case 5:
                    builder.RegisterType<MisfireExample>().As<ILMScheduler>();
                    break;
                case 6:
                    builder.RegisterType<JobExceptionExample>().As<ILMScheduler>();
                    break;
                case 7:
                    builder.RegisterType<InterruptExample>().As<ILMScheduler>();
                    break;
                case 8:
                    builder.RegisterType<CalendarExample>().As<ILMScheduler>();
                    break;
                case 9:
                    builder.RegisterType<ListenerExample>().As<ILMScheduler>();
                    break;
                case 10:
                    builder.RegisterType<PluginExample>().As<ILMScheduler>();
                    break;
                case 11:
                    builder.RegisterType<LMSchedulerExample>().As<ILMScheduler>();
                    break;
                default:
                    break;
            }


            Container = builder.Build();

            using (var scope = Container.BeginLifetimeScope())
            {
                ILMScheduler scheduler = scope.Resolve<ILMScheduler>();

                ILog log = scope.Resolve<ILog>();

                scheduler.Run(log).Wait();
            }


        }
    }
}
