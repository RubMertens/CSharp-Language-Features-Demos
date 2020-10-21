using System;
using System.Buffers;
using System.Buffers.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Diagnostics.Tracing;

namespace RefStructs
{
    public class Program
    {
        static void Main(string[] args)
        {
            // SpanOverArray();
             // SpansFromDifferentPlaces();
            BenchmarkRunner.Run<StringSum>();
            // AsyncExamples();
            // Ranges();
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

        private static async Task AsyncExamples()
        {
            Console.WriteLine(await RequestRemotelyAndProcess());
            Console.WriteLine(await RequestRemotelyAndProcessWithSpan());
            Console.WriteLine(await RequestRemotelyAndProcessWithSpanLocalFunction());
        }

        private static async Task<int> RequestRemotelyAndProcess()
        {
            var bytes = await RemoteCall();
            var sum = 0;
            foreach (var b in bytes)
            {
                sum += b;
            }
            return sum;
        }

        private static Task<int> AsyncCompilerExample()
        {
            var stateMachine = new RequestRemotelyAndProcessStateMachine();
            stateMachine.builder = AsyncTaskMethodBuilder<int>.Create();
            stateMachine.state = -1;
            stateMachine.builder.Start(ref stateMachine);
            return stateMachine.builder.Task;
        }
        
        //loosely taken from ILSPY set to lower the language version to c# 4
        private class RequestRemotelyAndProcessStateMachine: IAsyncStateMachine
        {
            public AsyncTaskMethodBuilder<int> builder;
            public int state;


            public byte[] bytes_awaiterResult;
            public byte[] bytes;
            public byte b;
            public int i;
            public byte[] bytesCopy;
            public int sum;

            private TaskAwaiter<byte[]> bytesTaskAwaiter;

            public void MoveNext()
            {
                int num = state;
                int result;
                try
                {
                    TaskAwaiter<byte[]> awaiter;
                    if (num != 0)
                    {
                        awaiter = RemoteCall().GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            num = (state = 0);
                            bytesTaskAwaiter = awaiter;
                            var stateMachine = this;
                            builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
                            return;
                        }
                    }
                    else
                    {
                        awaiter = bytesTaskAwaiter;
                        bytesTaskAwaiter = default(TaskAwaiter<byte[]>);
                        num = (state = -1);
                    }
                    
                    this.bytes_awaiterResult = awaiter.GetResult();
                    bytesCopy = bytes_awaiterResult;
                    bytes_awaiterResult = null;
                    sum = 0;
                    bytes = bytesCopy;
                    
                    for (i = 0; i < bytes.Length; i++)
                    {
                        b = bytes[i];
                        sum += b;
                    }

                    bytes = null;
                    result = sum;
                }
                catch (Exception exception)
                {
                    // <>1__state = -2;
                    //     <bytes>5__1 = null;
                    //     <>t__builder.SetException(exception);
                    // handle exceptions
                    return;
                }

                state = -2;
                bytesCopy = null;
                builder.SetResult(result);            
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
            }
        }

        private static Task<byte[]> RemoteCall()
        {
            return Task.FromResult(new byte[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 128});
        }

        private static async Task<int> RequestRemotelyAndProcessWithSpan()
        {
            var bytes = await RemoteCall();
            return SumBytes(bytes);
        }

        private static int SumBytes(ReadOnlySpan<byte> bytes)
        {
            var sum = 0;
            foreach (var b in bytes)
            {
                sum += b;
            }
            return sum;
        }
        
        private static async Task<int> RequestRemotelyAndProcessWithSpanLocalFunction()
        {
            var bytes = await RemoteCall();
            static int SumBytesLocal(ReadOnlySpan<byte> localFunctionBytes) {
                var sum = 0;
                foreach (var b in localFunctionBytes)
                {
                    sum += b;
                }
                return sum;
            }
            return SumBytesLocal(bytes);
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
|             Method |       Mean |     Error |    StdDev |     Median |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------- |-----------:|----------:|----------:|-----------:|-------:|------:|------:|----------:|
|     StringParseSum | 4,163.3 ns |  81.70 ns |  72.43 ns | 4,153.6 ns | 1.1368 |     - |     - |    4776 B |
|       LinqParseSum | 5,358.4 ns | 105.59 ns | 195.73 ns | 5,268.2 ns | 1.1597 |     - |     - |    4872 B |
|       SpanParseSum | 2,116.7 ns |  19.73 ns |  17.49 ns | 2,116.2 ns |      - |     - |     - |         - |
|   SpanParseUtf8Sum |   980.4 ns |  15.65 ns |  13.88 ns |   980.1 ns | 0.0839 |     - |     - |     352 B |
| Utf8WithMemoryPool | 1,022.4 ns |   9.61 ns |   8.52 ns | 1,020.2 ns | 0.0057 |     - |     - |      24 B |
|  Utf8WithArrayPool | 1,063.9 ns |  28.67 ns |  83.17 ns | 1,013.3 ns |      - |     - |     - |         - |

*/
    [MemoryDiagnoser]
    public class StringSum
    {
        public static string data = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15";

        [Benchmark]
        /* naive result
         * |     StringParseSum | 4,163.3 ns |  81.70 ns |  72.43 ns | 4,153.6 ns | 1.1368 |     - |     - |    4776 B |
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
        /* linq result
         * |       LinqParseSum | 5,358.4 ns | 105.59 ns | 195.73 ns | 5,268.2 ns | 1.1597 |     - |     - |    4872 B |
         */
        public int LinqParseSum()
        {
            return data.Split(',').Sum(int.Parse);
        }
        
        [Benchmark]
        /* span result
         * |       SpanParseSum | 2,116.7 ns |  19.73 ns |  17.49 ns | 2,116.2 ns |      - |     - |     - |         - |
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
         * |   SpanParseUtf8Sum |   980.4 ns |  15.65 ns |  13.88 ns |   980.1 ns | 0.0839 |     - |     - |     352 B |
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
         * | Utf8WithMemoryPool | 1,022.4 ns |   9.61 ns |   8.52 ns | 1,020.2 ns | 0.0057 |     - |     - |      24 B |
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
         * |  Utf8WithArrayPool | 1,063.9 ns |  28.67 ns |  83.17 ns | 1,013.3 ns |      - |     - |     - |         - |
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