using System;

namespace Figo.Timer
{
    public class IntTimer: BaseTimer<int>
    {
        public IntTimer(int max, Action after) : base(max, after)
        {
        }

        public override void Update(int time)
        {
            if (!Active) return;
            
            Current -= time;
            
            if (Current <= 0)
            {
                Reset();
                After?.Invoke();
            }
        }
    }
}