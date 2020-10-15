
namespace NullableIntroductionExamplesStart.Notifications
{
    public class Notification
    {
        public Notification(string message, int target)
        {
            Message = message;
            Target = target;
        }
        public string Message { get;  }
        public int Target { get;  }
        
        public static string AddressFor(Notification notification)
        {
            if (notification == null)
                return null;
            return $"amqp://notification/{notification.Target}";
        }
    }
}