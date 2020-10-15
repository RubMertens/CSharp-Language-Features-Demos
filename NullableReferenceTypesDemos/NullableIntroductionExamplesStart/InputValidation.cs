using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NullableIntroductionExamplesStart
{
    public class InputValidation
    {
        public static bool IsValid(string message)
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
        public string GetUserInput()
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