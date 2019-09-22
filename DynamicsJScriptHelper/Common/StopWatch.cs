namespace DynamicsJScriptHelper.Common
{
    using System;
    using static System.DateTime;

    internal struct StopWatch
    {
        private DateTime _start;

        private bool _started;

        internal void Start()
        {
            _start = UtcNow;
            _started = true;
        }

        internal int Stop()
        {
            if (!_started)
            {
                return 0;
            }
            else
            {
                return (UtcNow - _start).Seconds;
            }
        }
    }
}
