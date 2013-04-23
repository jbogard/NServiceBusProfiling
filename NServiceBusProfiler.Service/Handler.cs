using System;
using System.Data.SqlClient;
using System.Transactions;
using NServiceBus;
using NServiceBusProfiler.Messages;
using NServiceBusProfiling.Messages;

namespace NServiceBusProfiler.Service
{
    public class TimeHandler : IHandleMessages<ICommand>
    {
        private static volatile int Count = 0;

        public void Handle(ICommand message)
        {
            if (Count == 0)
            {
                Console.WriteLine(message.GetType());
                Console.WriteLine(DateTime.Now);
            }
            
            Count++;

            if (Count == 10000)
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
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted}))
            using (var conn = new SqlConnection(EndpointConfig.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Test SET Value = 'Blerg' WHERE Id = 1";
                    cmd.ExecuteNonQuery();
                }

                scope.Complete();
            }
        }
    }

    public class Handler3 : IHandleMessages<DbMsgAction>
    {
        public IBus Bus { get; set; }

        public void Handle(DbMsgAction message)
        {
            // Do nothing
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            using (var conn = new SqlConnection(EndpointConfig.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Test SET Value = 'Blerg' WHERE Id = 1";
                    cmd.ExecuteNonQuery();
                }

                scope.Complete();
            }

            Bus.Send("OtherEndpoint", new SaySomethingElse());
        }
    }
}