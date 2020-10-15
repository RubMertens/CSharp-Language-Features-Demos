#nullable enable
using System.Diagnostics.CodeAnalysis;

namespace NullableIntroductionExamples.Notifications
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
        
        [return:NotNullIfNotNull("notification")]
        public static string? AddressFor(Notification? notification)
        {
            if (notification == null)
                return null;
            return $"amqp://notification/{notification.Target}";
        }
    }
}