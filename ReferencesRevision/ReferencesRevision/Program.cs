using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace ReferencesRevision
{
    class Program
    {
        static void Main(string[] args)
        {
            RefReturnExample();
        }
        
        public static void ReferenceDemoQuestion()
        {
            var intArray = new int[] { 10, 20 };
            One(intArray);
            Console.WriteLine(string.Join(", ", intArray));

            intArray = new int[] { 10, 20 };
            Two(intArray);
            Console.WriteLine(string.Join(", ", intArray));

            intArray = new int[] { 10, 20 };
            Three(ref intArray);
            Console.WriteLine(string.Join(", ", intArray));
        }
        
        private static void One(int[] intArray)
        {
            intArray[0] = 30;
        }

        private static void Two(int[] intArray)
        {
            intArray = new int[] { 40, 50 };
        }

        private static void Three(ref int[] intArray)
        {
            intArray = new int[] { 60, 70 };
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
            var original = new SimpleObject() { Number = 100 };
            var copied = original;

            Console.WriteLine($"Original={original},Copied={copied}");
            copied.Number += 500;
            Console.WriteLine($"Original={original},Copied={copied}");
        }
        
        public static void ReplaceReferenceExample()
        {
            var original = new SimpleObject() { Number = 100 };
            SimpleObject copied =  original;

            Console.WriteLine($"Original={original.Number.ToString()}," +
                              $"Copied={copied.Number.ToString()}");
            original = new SimpleObject() { Number = 300 };
            Console.WriteLine($"Original={original.Number.ToString()}," +
                              $"Copied={copied.Number.ToString()}");
        }
        public static void UseNumber(int i)
        {
            i = 2;
        }
        
        
        
        private static void RefValueTypesExample()
        {
            int i = 1;
            ChangeNumb(ref i);
            Console.WriteLine($"i = {i.ToString()}");
        }
        private static void ChangeNumb(ref int i)
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

        //new since c# 7+ --> Ref locals
        private static void RefLocalExample()
        {
            var original = new SimpleObject() { Number = 100 };
            
            ref SimpleObject copied = ref original;
            
            Console.WriteLine($"Original={original.Number.ToString()}," +
                              $"Copied={copied.Number.ToString()}");
            
            copied = new SimpleObject() { Number = 300 };
            
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
                    Console.Write($"{myReallyBigMatrix[i,j]:00}, ");
                }
                Console.WriteLine();
            }
        }

        public static ref int FindInMatrix(int[,] matrix, Func<int, bool> predicate)
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
        
        
        //immutability time

        void DoWork(in BigStruct bigStruct)
        {
            //dowork
            // bigStruct.MutableNumber = 442;
            // bigStruct = new BigStruct();
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