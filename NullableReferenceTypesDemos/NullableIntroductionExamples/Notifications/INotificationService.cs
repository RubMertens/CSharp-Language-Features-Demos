namespace NullableIntroductionExamples.Notifications
{
    public interface INotificationService
    {
        public void SendMessage(Notification notification);
    }
}