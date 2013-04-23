using System;
using System.Data.SqlClient;
using NServiceBus;
using NServiceBusProfiler.Messages;
using NServiceBusProfiling.Messages;

namespace NServiceBusProfiler.ServiceTx
{
    public class TimeHandler : IHandleMessages<ICommand>
    {
        private static volatile int Count = 0;

        public void Handle(ICommand message)
        {
            int count = ++Count;

            if (count <= 1)
            {
                Console.WriteLine(message.GetType());
                Console.WriteLine(DateTime.Now);
            }

            if (count >= 9999)
                Console.WriteLine(DateTime.Now);
        }
    }

    public class Handler : IHandleMessages<NoOp>
    {
        public IBus Bus { get; set; }

        public void Handle(NoOp message)
        {
            // Do nothing
        }
    }
    public class Handler2 : IHandleMessages<DbAction>
    {
        public IBus Bus { get; set; }

        public void Handle(DbAction message)
        {
            // Do nothing
            using (var conn = new SqlConnection(EndpointConfig.ConnectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = "UPDATE Test SET Value = 'Blerg' WHERE Id = 1";
                    cmd.ExecuteNonQuery();
                    tx.Commit();
                }

            }
        }
    }

    public class Handler3 : IHandleMessages<DbMsgAction>
    {
        public IBus Bus { get; set; }

        public void Handle(DbMsgAction message)
        {
            // Do nothing
            using (var conn = new SqlConnection(EndpointConfig.ConnectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = "UPDATE Test SET Value = 'Blerg' WHERE Id = 1";
                    cmd.ExecuteNonQuery();
                    tx.Commit();
                }

            }

            Bus.Send("OtherEndpoint", new SaySomethingElse());
        }
    }
}