namespace NullableIntroductionExamplesStart.Notifications
{
    public class NotificationService: INotificationService
    {
        private readonly IRabbitMqClient client;

        public NotificationService(IRabbitMqClient client)
        {
            this.client = client;
        }

        public void SendMessage(Notification notification)
        {
            var address = Notification.AddressFor(notification);
            if(address == null) return;
            client.Send(address, notification.Message);
        }
    }

    public interface IRabbitMqClient
    {
        void Send(string address, string message);
    }
}