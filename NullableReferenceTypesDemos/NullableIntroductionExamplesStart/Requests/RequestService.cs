using System.Diagnostics.CodeAnalysis;
using NullableIntroductionExamplesStart.Notifications;

namespace NullableIntroductionExamplesStart.Requests
{
    public class RequestService
    {
        private readonly INotificationService notificationService;
        private readonly IRequestsRepository repository;

        public RequestService(
            INotificationService notificationService, 
            IRequestsRepository repository 
            )
        {
            this.notificationService = notificationService;
            this.repository = repository;
        }
        
        public void MakeRequest(Request request)
        {
            EnsureInitialized(request);
            if (request.Comment.HasComment())
            {
                var notification = new Notification(
                    request.Comment.Comment,
                    request.Target
                );
                notificationService.SendMessage(notification);
            }
            repository.Save(request);
        }

        public static void EnsureInitialized(Request request)
        {
            if (request == null)
                request = new Request(new OptionalComment(null), -1);
            if(request.Comment == null)
                request.Comment = new OptionalComment(null);
        }
    }

    public interface IRequestsRepository
    {
        void Save(Request request);
    }
}