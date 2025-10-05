using System;

namespace Figo.Timer
{
    public class ReversedFloatTimer : BaseTimer<float>
    {
        public ReversedFloatTimer(float max, Action after) : base(max, after)
        {
        }

        public override void Update(float time)
        {
            if (!Active) return;
            
            Current += time;
            
            if (Current >= Max)
            {
                Reset();
                After?.Invoke();
            }
        }

        public override void Reset()
        {
            Current = 0;
        }
    }
}