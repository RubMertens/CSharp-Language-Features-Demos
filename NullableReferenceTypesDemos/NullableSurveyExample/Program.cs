using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Data.SqlTypes;

namespace NullableSurveyExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var run = new SurveyRun();
            run.AddQuestion(QuestionType.YesNo, "Did you eat the cookies?");
            run.AddQuestion(QuestionType.Number, "How many did you eat?");
            run.AddQuestion(QuestionType.Text, "What color box was it?");
            run.PerformSurvey(50);
            foreach (var participant in run.AllParticipants)
            {
                Console.WriteLine($"Participant: {participant.Id}");
                if (participant.AnsweredSurvey)
                {
                    for (var i = 0; i < run.Questions.Count; i++)
                    {
                        var answer = participant.Answer(i);
                        Console.WriteLine($"\t{run.GetQuestion(i).QuestionText} : {answer}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNo reponses");
                }
            }
        }
    }
}