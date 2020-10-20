using System;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SwitchExpressionDemos._00_Tuples_And_Expressions
{
    public class Demos
    {
        public static void Tuples()
        {
            (int, string) rank = (3, "Luc");
            (int place, string name) rank2 = (3, "Luc");
            
            Console.WriteLine($"Does rank = rank2? --> {rank == rank2}");
                        
            var numbers = new[] {1, 2, 3, 4, 5};
            var minAndMax = FindMinMax(numbers);
            Console.WriteLine($"Min = {minAndMax.min} - Max = {minAndMax.max}");
            
            //deconstructing value tuples
            var (myMin, myMax) = FindMinMax(numbers);
            Console.WriteLine($"Min = {myMin} - Max = {myMax}");
            
            //tuple out variable
            var success = TryFindMinMax(numbers, out var outMinAndMax);
            var (outMin, outMax) = outMinAndMax;
            Console.WriteLine($"Success = {success} Min = {outMin} - Max = {outMax}");

            var intString = "13";
            var (parsedSuccessfully, parsed) = TryParseInt(intString);
        }

        public static void DeconstructExample()
        {
            var p = new Point(){X = 12, Y = 15};
            // p.Deconstruct(out var x, out var y);
            var (x, y) = p;
        }

        public static bool TryFindMinMax(int[] numbers, out (int min, int max) minAndMax)
        {
            minAndMax = (0, 0);
            if (numbers == null)
                return false;
            minAndMax = (numbers.Min(), numbers.Max());
            return true;
        }

        public static (int min, int max) FindMinMax(int[] numbers)
        {
            return (numbers.Min(), numbers.Max());
        }

        public static (bool success, int parsed) TryParseInt(string input)
        {
            var success = int.TryParse(input, out var parsed);
            return (success, parsed);
        }
    }

    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }
    }

    public class BeforeExpression
    {
        private int privateCounter;
        public int Counter { get; }

        public string Name { get; set; }

        public int Squared()
        {
            return Counter * Counter;
        }

        public BeforeExpression(int counter, string name)
        {
            privateCounter = counter;
            Name = name;
        }
    }

    public class EveryThingAsExpression
    {
        private int _privateCounter;
        public int Counter => _privateCounter;
        public string Name { get; set; }

        public int Squared => Counter * Counter;
        
        public EveryThingAsExpression(int counter, string name) 
            => (_privateCounter, Name) = (counter, name);
    }

    public class JustADto
    {
        public string Name { get; }
        public string MiddleName { get; }
        public string LastName { get; }
        public string CardNumber { get; }
        public string CvcCode { get; }
        public DateTime ExpireDate { get; }

        public JustADto(string name, string middleName, string lastName, string cardNumber, string cvcCode,
            DateTime expireDate)
            => (Name, MiddleName, LastName, CardNumber, CvcCode, ExpireDate) =
                (name, middleName, lastName, cardNumber, cvcCode, expireDate);

        // public JustADto(string name, string middleName, string lastName, string cardNumber, string cvcCode, DateTime expireDate)
        // {
        //     Name = name;
        //     MiddleName = middleName;
        //     LastName = lastName;
        //     CardNumber = cardNumber;
        //     CVCCode = cvcCode;
        //     ExpireDate = expireDate;
        // }
    }
    
    
    }