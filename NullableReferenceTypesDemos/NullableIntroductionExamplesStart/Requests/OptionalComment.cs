namespace NullableIntroductionExamplesStart.Requests
{
    public class OptionalComment
    {
        public string Comment { get; }

        public OptionalComment(string comment)
        {
            Comment = comment;
        }
            
        public bool HasComment()
        {
            return Comment != null;
        }
    }
}