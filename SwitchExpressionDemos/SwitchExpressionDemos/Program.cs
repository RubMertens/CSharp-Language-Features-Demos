using System;
using System.Threading.Tasks;
using SwitchExpressionDemos._01Basic;

namespace SwitchExpressionDemos
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await TrafficLightRunner.Run();
        }
    } 
}