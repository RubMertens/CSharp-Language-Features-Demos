using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace NullableIntroductionExamples
{
    public class InputValidation
    {
        public static bool IsValid([NotNullWhen(true)]string? message)
        {
            if (message == null)
                return false;
            if (message.Any(c => c != 'a'))
                return false;
            if (string.IsNullOrWhiteSpace(message))
                return false;
            return true;
        }
    }

    public class UsesInputValidation
    {
        public string? GetUserInput()
        {
            return Console.ReadLine();
        }
        
        public void DoStuff()
        {
            var input = GetUserInput();
            if (InputValidation.IsValid(input))
            {
                UseInput(input);
            }
        }

        public void UseInput(string input)
        {
            //expect null safe input
            var l = input.Length;
        }
    }
}