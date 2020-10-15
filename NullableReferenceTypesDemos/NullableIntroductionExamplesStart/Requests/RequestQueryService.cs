using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NullableIntroductionExamplesStart.Requests
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
            return requests
                .Where(r => r.Target == personId)
                .Select(r => authorizationService.CanView(r))
                .Where(r => r !=null)
                ;
        }
    }
    
    

    public class RequestAuthorizationService
    {
        public Request CanView([MaybeNull] Request request)
        {
            if (!IsAuthorized())
                return null;

            if (!CanViewComments())
                request.Comment = new OptionalComment(null);
            
            return request;
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