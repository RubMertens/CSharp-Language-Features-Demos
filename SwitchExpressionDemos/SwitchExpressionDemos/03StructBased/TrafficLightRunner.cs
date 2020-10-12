using System;
using System.Threading.Tasks;
using SwitchExpressionDemos.Ui;

namespace SwitchExpressionDemos._03StructBased
{
    public static class TrafficLightRunner
    {
        public static async Task Run(){
            var state = new LightStatus(LightState.Red, LightState.Off, 0);
            while (true)
            {
                Console.Clear();
                PrintLight(state.Current);
                state = GetNextState(state);
                await Task.Delay(1000);
            }
        }
        
        private static void PrintLight(LightState state){
            var str = state switch
            {
                LightState.Off => ConsoleTrafficLight.Off,
                LightState.Red => ConsoleTrafficLight.Red,
                LightState.Orange => ConsoleTrafficLight.Orange,
                LightState.Green => ConsoleTrafficLight.Green,
                LightState.FlashingGreen => ConsoleTrafficLight.Green,
                LightState.FlashingOrange => ConsoleTrafficLight.Orange,
                _ => ConsoleTrafficLight.Off
            };
            Console.WriteLine(str);
        }

        
        private static LightStatus GetNextState(LightStatus status    ) =>
            status switch
            {
               {Current: LightState.Red} => new LightStatus(LightState.Orange,status),
               {Current: LightState.Orange, Previous: LightState.Red} => new LightStatus(LightState.Green, status),
               {Current: LightState.Green } => new LightStatus(LightState.FlashingGreen,status),
               {Current: LightState.FlashingGreen, FlashCount:2 } => new LightStatus(LightState.Orange, status),
               {Current: LightState.FlashingGreen} => new LightStatus(status.FlashCount+1, status),
               {Current: LightState.Orange, Previous: LightState.FlashingGreen} => new LightStatus(LightState.Red, status),
                _ => new LightStatus(LightState.FlashingOrange, status)
            };
    }
}