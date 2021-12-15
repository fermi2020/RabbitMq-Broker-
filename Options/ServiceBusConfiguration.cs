namespace BackOffice.Messaging.Send.Options
{
    public class ServiceBusConfiguration
    {
        public string ConnectionString{get;set;}

        public string QueueuName {get;set;}

        public Credentials Credentials { get;set;}
    }
}