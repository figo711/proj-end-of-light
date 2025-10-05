using System;

namespace Figo.Timer
{
    public abstract class BaseTimer<T>
    {
        // Use to Enable (set true) / Disable (set false)
        public bool Active { get; set; } // like SetActive(bool)

        public T Current { get; protected set; }
        public bool AutoRestart { get; protected set; }

        public T Max { get; protected set; }

        protected Action After { get; set; }

        protected BaseTimer(T max, Action after, bool restart = true)
        {
            Active = false;
            Current = Max = max;
            After = after;
            AutoRestart = restart;
        }

        public abstract void Update(T time);

        public virtual void Reset()
        {
            Current = Max;
        }
    }
}