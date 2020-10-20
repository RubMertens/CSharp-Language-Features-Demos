using System;
using System.Threading.Tasks;
using SwitchExpressionDemos._00_Tuples_And_Expressions;
using SwitchExpressionDemos._01Basic;

namespace SwitchExpressionDemos
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Demos.Tuples();
            Console.WindowWidth = Ui.ConsoleTrafficLight.Width;
            Console.BufferWidth = Ui.ConsoleTrafficLight.Width;
            Console.WindowHeight = Ui.ConsoleTrafficLight.Height;
            Console.BufferHeight = Ui.ConsoleTrafficLight.Height;
            
            
            await TrafficLightRunner.Run();
        }
    } 
}