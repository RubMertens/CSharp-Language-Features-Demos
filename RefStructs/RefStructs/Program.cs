using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Diagnostics.Tracing.Parsers.IIS_Trace;

namespace RefStructs
{
    public class Program
    {
        static void Main(string[] args)
        {
            //SpanOverArray();
             // SpansFromDifferentPlaces();
            // BenchmarkRunner.Run<StringSum>();
            Ranges();
        }
        
        public struct LivesWithItsValues
        {
                        
        }
        
        // public ref struct MySpan<T>
        // {
        //     public int Length;
        //     public ref T Value;
        // }
        
        private static void SpanOverArray()
        {
            var arr = new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};

            var one = arr.AsSpan(0, 3); //0,1,2
            var two = arr.AsSpan(3,3); // 3,4,5
            var three = arr.AsSpan(6,4); // 6,7,8,9

            two[0] = 999;
            
            Console.WriteLine(string.Join(",", one.ToArray()));
            Console.WriteLine(string.Join(",", two.ToArray()));
            Console.WriteLine(string.Join(",", three.ToArray()));
            Console.WriteLine(string.Join("," ,arr));
        }
        
        private static void SpansFromDifferentPlaces()
        {
            //from array 
            Span<int> arraySpan = new[] {1, 2, 3};
            PrintSpan(arraySpan);

            //from unsafe pointer
            var ptr = Marshal.AllocHGlobal(3 * sizeof(int));
            Span<int> unsafeSpan;
            unsafe
            {
                unsafeSpan = new Span<int>(ptr.ToPointer(), 3);
                unsafeSpan[0] = 1;
                unsafeSpan[1] = 2;
                unsafeSpan[2] = 3;
            }
            PrintSpan(unsafeSpan);
            
            //from stackalloc
            Span<int> stackSpan = stackalloc int[] {1, 2, 3};
            PrintSpan(stackSpan);
        }

        private static void PrintSpan(Span<int> sp)
        {
            foreach (var n in sp)
            {
                Console.Write($"{n},");
            }
            Console.WriteLine();
        }

        private static void Ranges()
        {
            string[] words = new string[]
            {
                // index from start    index from end
                "The",      // 0                   ^9
                "quick",    // 1                   ^8
                "brown",    // 2                   ^7
                "fox",      // 3                   ^6
                "jumped",   // 4                   ^5
                "over",     // 5                   ^4
                "the",      // 6                   ^3
                "lazy",     // 7                   ^2
                "dog"       // 8                   ^1
            };              // 9 (or words.Length) ^0
            
            var range= words[new Range(new Index(0), new Index(2))];
            Console.WriteLine(string.Join(" ", range));
            range = words[0..2];
            Console.WriteLine(string.Join(" ", range));
            var r = 0..2;
            range = words[r];
            Console.WriteLine(string.Join(" ", range));

            //open ended range
            range = words[3..];
            Console.WriteLine(string.Join(" ", range));
            range = words[..3];
            Console.WriteLine(string.Join(" ", range));
            
            //starting from the back
            range = words[^3..^0];
            Console.WriteLine(string.Join(" ", range));
            range = words[^1..];
            Console.WriteLine(string.Join(" ", range));
            
            //combining the two
            range = words[2..^2];
            Console.WriteLine(string.Join(" ", range));
            
            
            //separate as index = 
            var last = ^1;
            Console.WriteLine(words[last]);
        }
    }

/*
|             Method |     Mean |     Error |    StdDev |   Median |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------- |---------:|----------:|----------:|---------:|-------:|------:|------:|----------:|
|     StringParseSum | 4.539 us | 0.0644 us | 0.0632 us | 4.544 us | 1.1368 |     - |     - |    4776 B |
|       SpanParseSum | 2.171 us | 0.0182 us | 0.0152 us | 2.172 us |      - |     - |     - |         - |
|   SpanParseUtf8Sum | 1.040 us | 0.0060 us | 0.0057 us | 1.038 us | 0.0839 |     - |     - |     352 B |
| Utf8WithMemoryPool | 1.073 us | 0.0138 us | 0.0122 us | 1.072 us | 0.0057 |     - |     - |      24 B |
|  Utf8WithArrayPool | 1.095 us | 0.0215 us | 0.0593 us | 1.066 us |      - |     - |     - |         - |
*/
    [MemoryDiagnoser]
    public class StringSum
    {
        public static string data = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15";

        [Benchmark]
        /* naive result
         * |     StringParseSum | 4.539 us | 0.0644 us | 0.0632 us | 4.544 us | 1.1368 |     - |     - |    4776 B |
         */
        public int StringParseSum()
        {
            var sum = 0;
            var split = data.Split(',');
            foreach (var part in split)
            {
                sum += int.Parse(part);
            }

            return sum;
        }
        
        [Benchmark]
        /* span result
         * |       SpanParseSum | 2.171 us | 0.0182 us | 0.0152 us | 2.172 us |      - |     - |     - |         - |
         */
        public int SpanParseSum()
        {
            var span = data.AsSpan();
            var sum = 0;

            while (true)
            {
                var i = span.IndexOf(',');

                if (i == -1)
                {
                    sum += int.Parse(span);
                    break;
                }
                sum += int.Parse(span.Slice(0, i));
                span = span.Slice(i + 1);
            }
            
            return sum;
        }
        
        [Benchmark]
        /* utf8 span result
         * |   SpanParseUtf8Sum | 1.040 us | 0.0060 us | 0.0057 us | 1.038 us | 0.0839 |     - |     - |     352 B |
         */
        public int SpanParseUtf8Sum()
        {
            ReadOnlySpan<byte> bytes = Encoding.UTF8.GetBytes(data);
            var sum = 0;
         
            while (true)
            {
                Utf8Parser.TryParse(bytes, out int value, out var bytesConsumed);
                sum += value;
                if(bytes.Length -1 < bytesConsumed)
                    break;
                bytes = bytes.Slice(bytesConsumed + 1);
            }

            return sum;
        }
        
        [Benchmark]
        /* UTF8 Memory Pool result
         * | Utf8WithMemoryPool | 1.073 us | 0.0138 us | 0.0122 us | 1.072 us | 0.0057 |     - |     - |      24 B |
         */
        public int Utf8WithMemoryPool()
        {
            //get minimal length required for 
            var minLength = UTF8Encoding.UTF8.GetMaxByteCount(data.Length); 
            // var arr = ArrayPool<byte>.Shared.Rent(minLength);
            IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent(minLength);
            
            var utf8 = owner.Memory.Span;
            var bytesWritten = UTF8Encoding.UTF8.GetBytes(data, utf8);
            utf8 = utf8.Slice(0, bytesWritten);
            
            int sum = 0;
            while (true)
            {
                Utf8Parser.TryParse(utf8, out int value, out var bytesConsumed);
                sum += value;
                if(utf8.Length -1 < bytesConsumed)
                    break;
                utf8 = utf8.Slice(bytesConsumed + 1);
            }
            
            // ArrayPool<byte>.Shared.Return(arr);
            owner.Dispose();
            return sum;
        }
        
        [Benchmark]
        /* UTF8 ArrayPool result
         * |  Utf8WithArrayPool | 1.095 us | 0.0215 us | 0.0593 us | 1.066 us |      - |     - |     - |         - |
         */
        public int Utf8WithArrayPool()
        {
            //get minimal length required for 
            var minLength = UTF8Encoding.UTF8.GetMaxByteCount(data.Length); 
            var arr = ArrayPool<byte>.Shared.Rent(minLength);
            
            var utf8 = arr.AsSpan();
            var bytesWritten = UTF8Encoding.UTF8.GetBytes(data, utf8);
            utf8 = utf8.Slice(0, bytesWritten);
            
            int sum = 0;
            while (true)
            {
                Utf8Parser.TryParse(utf8, out int value, out var bytesConsumed);
                sum += value;
                if(utf8.Length -1 < bytesConsumed)
                    break;
                utf8 = utf8.Slice(bytesConsumed + 1);
            }
            
            ArrayPool<byte>.Shared.Return(arr);
            return sum;
        }
    }
    
}