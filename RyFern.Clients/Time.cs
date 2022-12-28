using System.Diagnostics;

namespace RyFern.Clients
{
    public sealed class Time
    {
        private Stopwatch _timer;
        private double _lastUpdate;

        public float ElapsedTime =>
            _timer.ElapsedMilliseconds * 0.001f;
        public float TotalSeconds =>
            (float)_timer.Elapsed.TotalSeconds;

        public Time()
        {
            _timer = new Stopwatch();
        }

        public void Start()
        {
            _timer.Start();
            _lastUpdate = 0;
        }
        public void Stop() =>
            _timer.Stop();
        public double Update()
        {
            double now = ElapsedTime;
            double updateTime = now - _lastUpdate;
            _lastUpdate = now;
            return updateTime;
        }
    }
}
