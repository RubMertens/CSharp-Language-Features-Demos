using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncEnumerableDemos
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var asyncIter = new MyAsyncEnumerable().GetAsyncEnumerator();

            while (await asyncIter.MoveNextAsync())
            {
                Console.WriteLine(asyncIter.Current);
            }

            // await foreach (var i in new MyAsyncYieldEnumerable())
            // {
            //     Console.WriteLine(i);    
            // }

            // await using (var asyncDisposable = new UnmanagedResourcesWithAsyncDisposable())
            // {
            // }
        }
    }

    public class MyAsyncEnumerable: IAsyncEnumerable<int>
    {
        public IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            return new MyAsyncEnumerator();
        }
        
        private class MyAsyncEnumerator: IAsyncEnumerator<int>
        {
            private static Random rn = new Random();
            
            public ValueTask DisposeAsync()
            {
                return new ValueTask(Task.CompletedTask);
            }

            public async ValueTask<bool> MoveNextAsync()
            {
                await Task.Delay(1000);
                Current = rn.Next(100);
                return true;
            }

            public int Current { get; private set; }
        }
    }

    public class MyAsyncYieldEnumerable : IAsyncEnumerable<int>
    {
        private static Random rn = new Random();
        public async IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            while (true)
            {
                await Task.Delay(1000, cancellationToken);
                yield return rn.Next(100);    
            }
        }
    }
    
    public class UnmanagedResourcesWithAsyncDisposable: IAsyncDisposable, IDisposable
    {
        private HttpClient client = new HttpClient();
        private MemoryStream image = new MemoryStream();
        
        private IntPtr ptr;

        public UnmanagedResourcesWithAsyncDisposable()
        {
            ptr = Marshal.AllocHGlobal(85_000);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                client?.Dispose();
                image?.Dispose();
            }
            Marshal.FreeHGlobal(ptr);
        }

        private async ValueTask DisposeAsyncCore()
        {
            if (image != null)
            {
                await image.DisposeAsync();
            }
            client?.Dispose();
            image = null;
        }
        
        
    }
}