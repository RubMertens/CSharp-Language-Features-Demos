namespace SwitchExpressionDemos._04ElapsedTime
{
    public readonly struct LightStatus
    {
        public LightState Current { get; }
        public LightState Previous { get; }
        public int FlashCount { get; }
        
        public int ElapsedMs {get;}

        public LightStatus(LightState current, LightState previous, int flashCount, int elapsedMs)
        {
            Current = current;
            Previous = previous;
            FlashCount = flashCount;
            ElapsedMs = elapsedMs;
        }
        
        public LightStatus(LightState newState, LightStatus previous)
            :this(newState, previous.Current, 0, 0){}
        
        public LightStatus(LightState newState,int flashCount, LightStatus previous)
            :this(newState, previous.Current, flashCount, 0){}
        
        public LightStatus(int flashCount, int elapsedMs, LightStatus previous)
            :this(previous.Current, previous.Previous, flashCount, elapsedMs){}


        public LightStatus(int elapsedSince, LightStatus previous)
            :this(previous.Current, previous.Previous, previous.FlashCount, previous.ElapsedMs + elapsedSince)
        {
            
        }
    }
}