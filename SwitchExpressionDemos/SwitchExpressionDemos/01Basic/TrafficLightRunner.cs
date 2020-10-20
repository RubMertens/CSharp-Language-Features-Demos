using System;
using System.Threading.Tasks;
using SwitchExpressionDemos.Ui;

namespace SwitchExpressionDemos._01Basic
{
    public static class TrafficLightRunner
    {
        public static async Task Run(){
            var state = LightState.Red;
            while (true)
            {
                Console.Clear();
                PrintLight(state);
                state = GetNextState(state);
                await Task.Delay(1000);
            }
        }
        
        private static void PrintLight(LightState state){
            //inline
            switch(state)
            {
                case LightState.Off:
                    Console.WriteLine(ConsoleTrafficLight.Off);
                    break;
                case LightState.Red:
                    Console.WriteLine(ConsoleTrafficLight.Red);
                    break;
                case LightState.Orange:
                    Console.WriteLine(ConsoleTrafficLight.Orange);
                    break;
                case LightState.Green:
                    Console.WriteLine(ConsoleTrafficLight.Green);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private static LightState GetNextState(LightState state)
        {
            // ideal candidate for a switch expression!
            switch (state)
            {
                case LightState.Off:
                    return LightState.Red;
                case LightState.Red:
                    return LightState.Orange;
                case LightState.Orange:
                    return LightState.Green;
                case LightState.Green:
                    return LightState.Red;
                default:
                    return LightState.Off;
            }
        }
    }
}