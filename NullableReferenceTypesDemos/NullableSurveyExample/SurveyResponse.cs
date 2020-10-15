using System;
using System.Collections.Generic;

namespace NullableSurveyExample
{
    public class SurveyResponse
    {
        private Dictionary<int, string?>? surveyResponses;
        public bool AnsweredSurvey => surveyResponses != null;
        public string Answer(int index) => surveyResponses?.GetValueOrDefault(index) ?? "No Answer";
        public bool AnswerSurvey(IEnumerable<SurveyQuestion> questions)
        {
            if (ConsentToSurvey())
            {
                surveyResponses = new Dictionary<int, string?>();
                var index = 0;
                foreach (var question in questions)
                {
                    var answer = GenerateAnswer(question);
                    if(answer != null)
                        surveyResponses.Add(index, answer); 
                    index++;
                }
                
            }
            return surveyResponses != null;
        }
        
        private static bool ConsentToSurvey() => rn.Next(0,2)  == 1;

        private static string? GenerateAnswer(SurveyQuestion question)
        {
            switch (question.TypeOfQuestion)
            {
                case QuestionType.YesNo:
                    return rn.Next(0,1) == 1 ? "yes": "no";
                    
                case QuestionType.Number:
                    var n = rn.Next(-30,101);
                    return n < 0? default: n.ToString();
                default:
                    switch (rn.Next(0, 5))
                    {
                        case 0 : return default;
                        case 1: return "red";
                        case 2: return "blue";
                        case 3: return "green";
                        default: return "idk man, colors are hard";                        
                    }
            }
            
        }
        
        public int Id {get;}
        public SurveyResponse(int id) => Id = id;
        
        private static readonly Random rn = new Random();
        public static SurveyResponse GetRandomId() => new SurveyResponse(rn.Next());
    }
}