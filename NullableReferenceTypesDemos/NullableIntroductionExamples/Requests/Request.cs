namespace NullableIntroductionExamples.Requests
{
    public class Request
    {
        public Request(OptionalComment comment, int target)
        {
            Comment = comment;
            Target = target;
        }

        public OptionalComment Comment { get; set; }
        public int Target { get; set; }
        
    }
}