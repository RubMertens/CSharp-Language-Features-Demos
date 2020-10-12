using System;
using System.Threading.Tasks;
using SwitchExpressionDemos.Ui;

namespace SwitchExpressionDemos._02TupleBased
{
    public static class TrafficLightRunner
    {
        public static async Task Run(){
            var state = LightState.Red;
            var previousState = LightState.Orange;
            var flashCount = 0;
            while (true)
            {
                Console.Clear();
                PrintLight(state);
                (state, previousState, flashCount) = GetNextState(state, previousState, flashCount);
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

        // Austria has following traffic light flow
        // Red -> Orange -> Green -> Flashing Green -> Orange -> Red
        // We now need the state before the current one to determine the next state
        // We could use tuples as input for a switch expression
        // private static (LightState current, LightState previous) GetNextState(LightState current, LightState previous) =>
        //     (current, previous) switch
        //     {
        //         (current: LightState.Red, previous: LightState.Orange) => (LightState.Orange, current),
        //         (current: LightState.Orange, previous: LightState.Red) => (LightState.Green, current),
        //         (current: LightState.Green, previous: LightState.Orange) => (LightState.FlashingGreen, current),
        //         (current: LightState.FlashingGreen, previous: LightState.Green) => (LightState.Orange, current),
        //         (current: LightState.Orange, previous: LightState.FlashingGreen) => (LightState.Red, current),
        //         _ => (LightState.FlashingOrange, current)
        //     };
        
        
        // This is not exactly the behaviour you would want. 
        // You don't want to write out every possible previous value 
        // We can use a discard in our tuple deconstruction
        // private static (LightState current, LightState previous) GetNextState(LightState current, LightState previous) =>
        //     (current, previous) switch
        //     {
        //         (current: LightState.Red, _) => (LightState.Orange, current),
        //         (current: LightState.Orange, previous: LightState.Red) => (LightState.Green, current),
        //         (current: LightState.Green, _) => (LightState.FlashingGreen, current),
        //         (current: LightState.FlashingGreen, _) => (LightState.Orange, current),
        //         (current: LightState.Orange, previous: LightState.FlashingGreen) => (LightState.Red, current),
        //         _ => (LightState.FlashingOrange, current)
        //     };
        
        
        // Next we'll take a look at this flashing light.
        // Say it needs to flash 3 times before going to the next light
        private static (LightState current, LightState previous, int flashCount) GetNextState(LightState current, LightState previous, int flashCount = 0) =>
            (current, previous, flashCount) switch
            {
                (Item1: LightState.Red,_,_) => (LightState.Orange, current,0),
                (Item1: LightState.Orange, Item2: LightState.Red,_) => (LightState.Green, current,0),
                (Item1: LightState.Green, _,_) => (LightState.FlashingGreen, current,0),
                (Item1: LightState.FlashingGreen, _,Item3:2 ) => (LightState.Orange, current,0),
                (Item1: LightState.FlashingGreen, _,_ ) => (LightState.FlashingGreen, current,flashCount+1),
                (Item1: LightState.Orange, Item2: LightState.FlashingGreen,_) => (LightState.Red, current,0),
                _ => (LightState.FlashingOrange, current,0)
            };
    }
}