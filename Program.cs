using System;
using NServiceBus;
using NServiceBusProfiler.Messages;

namespace NServiceBusProfiling
{
    public class Program
    {
        private static readonly string ConnectionString =
            @"Server=.\;Database=NServiceBusProfiling;Trusted_Connection=true";
        //private static readonly string ConnectionString =
        //    @"Data Source=HEADSPRING03;Initial Catalog=NServiceBusConnectivity;User Id=bc;Password=diamonds1;";

        static void Main(string[] args)
        {
            var bus = Configure.With()
                               .DefaultBuilder()
                               .XmlSerializer()
                               .UseTransport<SqlServer>(() => ConnectionString)
                               .UnicastBus()
                               .SendOnly();

            while (true)
            {
                Console.WriteLine("Enter a number 1-6:");
                int value = Convert.ToInt32(Console.ReadLine());
                
                Func<ICommand> creator = () => new NoOp();
                string dest = "NServiceBusProfiler.Service";

                if (value%3 == 2)
                {
                    creator = () => new DbAction();
                }
                else if (value%3 == 0)
                {
                    creator = () => new DbMsgAction();
                }
                if (value > 3)
                    dest = "NServiceBusProfiler.ServiceTx";

                for (int i = 0; i < 10000; i++)
                {
                    bus.Send(dest, creator());
                }
            }
        }
    }
}