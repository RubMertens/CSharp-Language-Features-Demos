using System;
using System.Threading.Tasks;
using SwitchExpressionDemos.Ui;

namespace SwitchExpressionDemos._04ElapsedTime
{
    public static class TrafficLightRunner
    {
        public static async Task Run(){
            var state = new LightStatus(LightState.FlashingGreen, LightState.Green, 0,0);
            while (true)
            {
                Console.Clear();
                PrintLight(state.Current);
                state = GetNextState(state,100);
                await Task.Delay(100);
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
        
        private static LightStatus GetNextState(LightStatus status, int elapsedMs) =>
            status switch
            {
               {Current: LightState.Red, ElapsedMs: var ms} when elapsedMs + ms >= 5000 => new LightStatus(LightState.Orange,status),
               {Current: LightState.Orange, Previous: LightState.Red, ElapsedMs: var ms} when elapsedMs + ms >= 2000 => new LightStatus(LightState.Green, status),
               {Current: LightState.Green, ElapsedMs: var ms } when elapsedMs + ms >= 3000 => new LightStatus(LightState.FlashingGreen,status),
               
               {Current: LightState.FlashingGreen, FlashCount:2, ElapsedMs: var ms } when ms+elapsedMs >= 500 => new LightStatus(LightState.Orange, status),
               {Current: LightState.FlashingGreen, FlashCount: var fc, ElapsedMs: var ms} when ms + elapsedMs >= 500 && fc < 2 => new LightStatus(LightState.Off, status.FlashCount, status),
               {Current: LightState.Off, Previous: LightState.FlashingGreen, ElapsedMs: var ms} when ms + elapsedMs >= 500 => new LightStatus(LightState.FlashingGreen, status.FlashCount+1, status),
               
               {Current: LightState.Orange, Previous: LightState.FlashingGreen, ElapsedMs: var ms } when ms + elapsedMs >= 3000 => new LightStatus(LightState.Red, status),
               
                _ => new LightStatus(elapsedMs, status)
            };
        
        private static int TimeForLight(LightStatus status) 
            => status switch
            {
                {Current: LightState.Red} => 5_000,
                {Current: LightState.Orange, Previous:LightState.Red} => 2_000,
                {Current: LightState.Orange, Previous:LightState.FlashingGreen} => 3_000,
                {Current: LightState.Green} => 3_000,
                _ => 500
            };
        
        // private static LightStatus GetNextState(LightStatus status, int elapsedMs) =>
        //     status switch
        //     {
        //         {Current: LightState.Red, ElapsedMs: var ms} when elapsedMs + ms >= TimeForLight(status) => new LightStatus(LightState.Orange,status),
        //         {Current: LightState.Orange, Previous: LightState.Red, ElapsedMs: var ms} when elapsedMs + ms >= TimeForLight(status) => new LightStatus(LightState.Green, status),
        //         {Current: LightState.Green, ElapsedMs: var ms } when elapsedMs + ms >= TimeForLight(status) => new LightStatus(LightState.FlashingGreen,status),
        //        
        //         {Current: LightState.FlashingGreen, FlashCount:2, ElapsedMs: var ms } when ms+elapsedMs >= TimeForLight(status) => new LightStatus(LightState.Orange, status),
        //         {Current: LightState.FlashingGreen, FlashCount: var fc, ElapsedMs: var ms} when ms + elapsedMs >= TimeForLight(status) && fc < 2 => new LightStatus(LightState.Off, status.FlashCount, status),
        //         {Current: LightState.Off, Previous: LightState.FlashingGreen, ElapsedMs: var ms} when ms + elapsedMs >= TimeForLight(status) => new LightStatus(LightState.FlashingGreen, status.FlashCount+1, status),
        //        
        //         {Current: LightState.Orange, Previous: LightState.FlashingGreen, ElapsedMs: var ms } when ms + elapsedMs >= TimeForLight(status) => new LightStatus(LightState.Red, status),
        //        
        //         _ => new LightStatus(elapsedMs, status)
        //     };

        private static bool ShouldTransition(LightStatus status, int elapsedMs)
        {
            return status.ElapsedMs + elapsedMs >= TimeForLight(status);
        }

        // private static LightStatus GetNextState(LightStatus status, int elapsedMs) =>
        //     (status, ShouldTransition(status, elapsedMs)) switch
        //     {
        //         (_, false) => new LightStatus(elapsedMs, status),
        //         
        //        ( status: {Current: LightState.Red}, _) => new LightStatus(LightState.Orange,status),
        //        ( {Current: LightState.Orange, Previous: LightState.Red},_)  => new LightStatus(LightState.Green, status),
        //        ( {Current: LightState.Green},_) => new LightStatus(LightState.FlashingGreen,status),
        //        
        //        ( {Current: LightState.FlashingGreen, FlashCount:2},_) => new LightStatus(LightState.Orange, status),
        //        ( {Current: LightState.FlashingGreen, FlashCount: var fc},_) when  fc < 2 => new LightStatus(LightState.Off, status.FlashCount, status),
        //        ( {Current: LightState.Off, Previous: LightState.FlashingGreen},_)  => new LightStatus(LightState.FlashingGreen, status.FlashCount+1, status),
        //        
        //        ( {Current: LightState.Orange, Previous: LightState.FlashingGreen },_) => new LightStatus(LightState.Red, status),
        //        
        //         _ => new LightStatus(elapsedMs, status)
        //     };
        
        private static (LightState newState, int newFlashCount) GetNextLightState(LightState current , LightState previous, int flashCount)
         => (current, previous, flashCount) switch
         {
             (current: LightState.Red, _,_) => (LightState.Orange,0),
             (current: LightState.Orange, previous: LightState.Red, _) => (LightState.Green,0),
             (current: LightState.Green, _,_) => (LightState.FlashingGreen,0),
             (current: LightState.FlashingGreen, _, flashCount: 2) => (LightState.Orange,0),
             (current: LightState.FlashingGreen, _,_) => (LightState.Off,flashCount),
             (current: LightState.Off, previous:LightState.FlashingGreen, _) => (LightState.FlashingGreen,flashCount + 1),
             (current: LightState.Orange, previous: LightState.FlashingGreen, _) => (LightState.Red,0),
         };

        // private static LightStatus GetNextState(LightStatus status, int elapsedMs) =>
        //     (ShouldTransition(status, elapsedMs), GetNextLightState(status.Current, status.Previous, status.FlashCount)) 
        //         switch
        //         {
        //             (false, _) => new LightStatus(elapsedMs, status),
        //             (true, var (newState, newFlashCount)) => new LightStatus(newState, newFlashCount, status)
        //         };
    }
}