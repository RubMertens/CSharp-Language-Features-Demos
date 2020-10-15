namespace NullableSurveyExample
{
    public class SurveyQuestion
    {
        public string QuestionText { get; }
        public QuestionType TypeOfQuestion { get; }

        public SurveyQuestion(string questionText, QuestionType typeOfQuestion)
            => (QuestionText, TypeOfQuestion) = (questionText, typeOfQuestion);
    }
}