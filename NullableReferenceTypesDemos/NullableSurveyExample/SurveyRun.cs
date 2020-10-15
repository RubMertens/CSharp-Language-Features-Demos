using System.Collections.Generic;
using System.Linq;

namespace NullableSurveyExample
{
    public class SurveyRun
    {
        private readonly List<SurveyQuestion> surveyQuestions = new List<SurveyQuestion>();
        public void AddQuestion(QuestionType type, string question) => AddQuestion(new SurveyQuestion(question, type));
        public void AddQuestion(SurveyQuestion surveyQuestion) => surveyQuestions.Add(surveyQuestion);

        private List<SurveyResponse>? respondents;
        public void PerformSurvey(int numberOfRespondents)
        {
            var respondentsConsenting = 0;
            respondents = new List<SurveyResponse>();
            while (respondentsConsenting < numberOfRespondents)
            {
                var respondent = SurveyResponse.GetRandomId();
                if(respondent.AnswerSurvey(surveyQuestions))
                    respondentsConsenting++;
                respondents.Add(respondent);
            }
        }
        
        public IEnumerable<SurveyResponse> AllParticipants => (respondents ?? Enumerable.Empty<SurveyResponse>());
        public ICollection<SurveyQuestion> Questions => surveyQuestions; 
        public SurveyQuestion GetQuestion(int index) => surveyQuestions[index];
    }
}