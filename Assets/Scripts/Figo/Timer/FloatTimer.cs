using System;

namespace Figo.Timer
{
    public sealed class FloatTimer : BaseTimer<float>
    {
        public FloatTimer(float max, Action after, bool restart) : base(max, after, restart)
        {
        }

        public override void Update(float time)
        {
            if (!Active) return;
            
            Current -= time;
            
            if (Current <= 0)
            {
                Reset();
                After?.Invoke();

                if (!AutoRestart) Active = false;
            }
        }
        
        public void Restart(float max)
        {
            Active = false;
            Current = Max = max;
            Active = true;
        }
        
        public void RestartWith(float max, float current)
        {
            Active = false;
            Current = Max = max;
            Active = true;
            Current = current;
        }
    }
}