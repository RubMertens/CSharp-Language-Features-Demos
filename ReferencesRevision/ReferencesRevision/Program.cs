using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;

namespace ReferencesRevision
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Reference Demos");
            Console.WriteLine("===============");
            Console.WriteLine("1> Reference Demo Question");
            Console.WriteLine("2> Value Example");
            Console.WriteLine("3> Reference Example");
            Console.WriteLine("4> ReplaceReference Example");
            Console.WriteLine("5> MethodByValue Example");
            Console.WriteLine("6> MethodByRef Example");
            Console.WriteLine("7> ReassignObject Answer");
            Console.WriteLine("8> Ref Local Example");
            Console.WriteLine("9> Ref Return Example");
            Console.WriteLine("10> Out Ref Example");
            Console.WriteLine("11> Out Ref Inline");
            Console.WriteLine("12> In Ref Example");
            Console.WriteLine("13> ImmutableArray Example");
            Console.WriteLine("Pick one: ");
            var k = Console.ReadLine();
            if (int.TryParse(k.ToString(), out var n))
            {
                Console.Clear();
                switch (n)
                {
                    case 1 : ReferenceDemoQuestion();
                        break;
                    case 2 : ValueExample();
                        break;
                    case 3 : ReferenceExample();
                        break;
                    case 4 : ReplaceReferenceExample();
                        break;
                    case 5 : MethodByValue();
                        break;
                    case 6 : MethodByRefExample();
                        break;
                    case 7 : ReassignObjectAnswer();
                        break;
                    case 8 : RefLocalExample();
                        break;
                    case 9 : RefReturnExample();
                        break;
                    case 10 : OutRefDemo();
                        break;
                    case 11 : OutRefDemoInline();
                        break;
                    case 12 : InRefExample();
                        break;
                    case 13: ImmutableArrayDemo();
                        break;
                    }
                
                
            }
        }

        public static void ReferenceDemoQuestion()
        {
            var intArray = new int[] {10, 20};
            One(intArray);
            Console.WriteLine(string.Join(", ", intArray));

            intArray = new int[] {10, 20};
            Two(intArray);
            Console.WriteLine(string.Join(", ", intArray));

            intArray = new int[] {10, 20};
            Three(ref intArray);
            Console.WriteLine(string.Join(", ", intArray));
        }

        private static void One(int[] intArray)
        {
            intArray[0] = 30;
        }

        private static void Two(int[] intArray)
        {
            intArray = new int[] {40, 50};
        }

        private static void Three(ref int[] intArray)
        {
            intArray = new int[] {60, 70};
        }


        public static void ValueExample()
        {
            int originalInt = 0;

            int copiedInt = originalInt;

            Console.WriteLine($"original={originalInt},copied={copiedInt}");

            copiedInt += 500; // Add 500 onto the copied int 

            Console.WriteLine($"original={originalInt},copied={copiedInt}");
        }

        public static void ReferenceExample()
        {
            var original = new SimpleObject() {Number = 100};
            var copied = original;

            Console.WriteLine($"Original={original},Copied={copied}");
            copied.Number += 500;
            Console.WriteLine($"Original={original},Copied={copied}");
        }

        public static void ReplaceReferenceExample()
        {
            var original = new SimpleObject() {Number = 100};
            SimpleObject copied = original;

            Console.WriteLine($"Original={original.Number.ToString()}," +
                              $"Copied={copied.Number.ToString()}");
            copied = new SimpleObject() {Number = 300};
            Console.WriteLine($"Original={original.Number.ToString()}," +
                              $"Copied={copied.Number.ToString()}");
        }

        public static void MethodByValue()
        {
            int i = 1;
            ChangeNumberByVal(i);
            Console.WriteLine($"i = {i.ToString()}");
        }

        public static void ChangeNumberByVal(int i)
        {
            i = 2;
        }

        private static void MethodByRefExample()
        {
            int i = 1;
            ChangeNumberByRef(ref i);
            Console.WriteLine($"i = {i.ToString()}");
        }

        private static void ChangeNumberByRef(ref int i)
        {
            i = 2;
        }

        public static void ReassignObject(ref SimpleObject o)
        {
            //will this be visible outside of ReassignObject?
            o = new SimpleObject()
            {
                Number = 14153
            };
        }

        public static void ReassignObjectAnswer()
        {
            var original = new SimpleObject()
            {
                Number = 0
            };

            ReassignObject(ref original);

            Console.WriteLine(original);
        }

        //new since c# 7+ --> Ref locals
        private static void RefLocalExample()
        {
            var original = new SimpleObject() {Number = 100};

            ref SimpleObject copied = ref original;

            Console.WriteLine($"Original={original.Number.ToString()}," +
                              $"Copied={copied.Number.ToString()}");

            copied = new SimpleObject() {Number = 300};

            Console.WriteLine($"Original={original.Number.ToString()}," +
                              $"Copied={copied.Number.ToString()}");
        }

        public static void RefReturnExample()
        {
            var myReallyBigMatrix = new int[,]
            {
                {0, 1, 2, 3, 4, 5},
                {6, 8, 9, 10, 1, 12},
                {13, 14, 15, 16, 17, 18}
            };

            ref int element = ref FindInMatrix(myReallyBigMatrix, i => i == 9);

            element = 999;

            for (var i = 0; i < myReallyBigMatrix.GetLength(0); i++)
            {
                for (var j = 0; j < myReallyBigMatrix.GetLength(1); j++)
                {
                    Console.Write($"{myReallyBigMatrix[i, j]:00}, ");
                }

                Console.WriteLine();
            }
        }

        public static ref T FindInMatrix<T>(T[,] matrix, Func<T, bool> predicate)
        {
            for (var i = 0; i < matrix.GetLength(0); i++)
            for (var j = 0; j < matrix.GetLength(1); j++)
            {
                if (predicate(matrix[i, j]))
                {
                    return ref matrix[i, j];
                }
            }

            throw new InvalidOperationException("can't find element in matrix");
        }

        public static ref int TryFindInMatrix(int[,] matrix, Func<int, bool> predicate, out bool found)
        {
            for (var i = 0; i < matrix.GetLength(0); i++)
            for (var j = 0; j < matrix.GetLength(1); j++)
            {
                if (predicate(matrix[i, j]))
                {
                    found = true;
                    return ref matrix[i, j];
                }
            }

            found = false;
            return ref matrix[0, 0];
        }

        public static void OutRefDemo()
        {
            Console.WriteLine("Enter a number");
            var numberStr = Console.ReadLine();
            var parsed = 0;
            var success = TryParseInt(numberStr, out parsed);
            if (success)
            {
                Console.WriteLine($"You wrote : {parsed}");
            }
            else
            {
                Console.WriteLine("could not parse number :( ");
            }
        }

        public static void OutRefDemoInline()
        {
            Console.WriteLine("Enter a number");
            var numberStr = Console.ReadLine();
            
            if (TryParseInt(numberStr, out var parsed))
            {
                Console.WriteLine($"You wrote : {parsed}");
            }
            else
            {
                Console.WriteLine("could not parse number :( ");
            }
        }

        public static bool TryParseInt(string strInt, out int parsed)
        {
            try
            {
                parsed = int.Parse(strInt);
                return true;
            }
            catch
            {
                parsed = default;
                return false;
            }
        }

        public static void InRefExample()
        {
            var bs = new BigStruct()
            {
                MutableNumber = 42
            };   
            
            BigStructMutator(ref bs);
            BigStructReader(in bs);
        }

        public static void BigStructMutator(ref BigStruct bigStruct)
        {
            bigStruct.MutableNumber = 1531351;
        }

        public static void BigStructReader(in BigStruct bigStruct)
        {
            Console.WriteLine($"reading from bigStruct : {bigStruct.MutableNumber}");
            // bigStruct.MutableNumber = 8428;
        }
        
        public readonly struct ReadonlyReference
        {
            public readonly int Id;
            public ReadonlyReference(int id)
            {
                Id = id;
            }
            
            // public readonly int Squared()
            // {
            //     Id = 123;
            //     return Id * Id;
            // }
        }

        public static void UseReadonlyStruct()
        {
            var rr = new ReadonlyReference(101);
        }

        struct ImmutableArray<T>
        {
            private readonly T[] array;

            public ImmutableArray(T[] original)
            {
                array = original;
            }

            public ref readonly T ItemRef(int i)
            {
                return ref this.array[i];
            }
        }

        public static void ImmutableArrayDemo()
        {
            var arr = new ImmutableArray<int>(new []{1,2,3});

            int copyOfElement = arr.ItemRef(2);
            ref readonly int refOfElement = ref arr.ItemRef(2);
            copyOfElement = 2342;
            // refOfElement = 3423;
        }
    }

    internal class SimpleObject
    {
        public int Number { get; set; }

        public override string ToString()
        {
            return Number.ToString();
        }
    }

    public struct BigStruct
    {
        public int MutableNumber { get; set; }
    }

    // a huge thank you to https://benhall.io/a-reintroduction-to-csharp-references/
    // for the examples and great write up of reference types
}