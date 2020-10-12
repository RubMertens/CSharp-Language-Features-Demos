namespace SwitchExpressionDemos._03StructBased
{
    public readonly struct LightStatus
    {
        public LightState Current { get; }
        public LightState Previous { get; }
        public int FlashCount { get; }

        public LightStatus(LightState current, LightState previous, int flashCount)
        {
            Current = current;
            Previous = previous;
            FlashCount = flashCount;
        }
        
        public LightStatus(LightState newState, LightStatus previous)
            :this(newState, previous.Current, 0){}
        
        public LightStatus(int flashCount, LightStatus previous)
            :this(previous.Current, previous.Previous, flashCount){}
    }
}