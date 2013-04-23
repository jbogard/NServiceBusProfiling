using NServiceBus;

namespace NServiceBusProfiler.Messages
{
    public class NoOp : ICommand
    {
    }
    public class DbAction : ICommand
    {
        
    }

    public class DbMsgAction : ICommand {}
}
