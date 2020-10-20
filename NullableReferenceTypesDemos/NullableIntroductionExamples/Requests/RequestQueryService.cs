#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NullableIntroductionExamples.Requests
{
    public class RequestQueryService
    {
        private readonly RequestAuthorizationService authorizationService;
        private readonly List<Request> requests = new List<Request>();

        public RequestQueryService(RequestAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        public IEnumerable<Request> RequestForPerson(int personId)
        {
            foreach (var request in requests)
            {
                authorizationService.Authorize(request);
                if(request == null)
                    continue;
                yield return request;
            }
        }
    }
    
    

    public class RequestAuthorizationService
    {
        public void Authorize([MaybeNull] Request request)
        {
            if (!IsAuthorized())
                request = null;

            if (!CanViewComments())
                if (request != null)
                    request.Comment = new OptionalComment(null);
        }

        public bool CanViewComments()
        {
            return false;
        }

        public bool IsAuthorized()
        {
            return false;
        }
    }

    public interface IRequestAuthorizationService
    {
        public bool CanView(Request request);
    }
}