namespace OurLittleTastyServerBot.Classes.Events
{
    public class EventBrokerProvider
    {
        private static readonly EventBroker Instance = new EventBroker();

        public EventBroker GetInstance()
        {
            return Instance;
        }
    }
}