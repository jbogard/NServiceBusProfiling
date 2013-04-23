


namespace NServiceBusProfiler.Service
{
    using NServiceBus;

	/*
		This class configures this endpoint as a Server. More information about how to configure the NServiceBus host
		can be found here: http://nservicebus.com/GenericHost.aspx
	*/
	public class EndpointConfig : IConfigureThisEndpoint, IWantCustomInitialization
    {
        public static readonly string ConnectionString = @"Server=.\;Database=NServiceBusProfiling;Trusted_Connection=true";
        //public static readonly string ConnectionString = @"Data Source=HEADSPRING03;Initial Catalog=NServiceBusConnectivity;User Id=bc;Password=diamonds1;";

        public void Init()
	    {
	        Configure.With()
	                 .DefaultBuilder()
	                 .UnicastBus()
	                 .UseTransport<SqlServer>(() => ConnectionString);  
	        //Configure.Transactions.Disable();
	        Configure.Transactions.Advanced(s =>
	        {
	            s.DisableDistributedTransactions();
	        });
	    }
    }

    public class CustomConfig : IWantToRunBeforeConfigurationIsFinalized
    {
        public void Run()
        {
            Configure.Transactions.Disable();
        }
    }
}