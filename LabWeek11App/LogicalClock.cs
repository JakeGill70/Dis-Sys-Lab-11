using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace LabWeek11App
{
    public class LogicalClock : IDisposable
    {
        private System.Timers.Timer _timer;
        private int _interval;
        public long Counter { get; private set; } = 0;
        public int Step { get; set; }

        public LogicalClock(int interval, int step)
        {
            _interval = interval;
            this.Step = step;
            _timer = new Timer(interval);
            _timer.AutoReset = true;
            _timer.Elapsed += OnTick;
        }

        public void OnTick(object sender, ElapsedEventArgs e) {
            Counter += Step;
        }

        public void Start() {
            this._timer.Start();
        }

        public void Stop() {
            this._timer.Stop();
        }

        public void Dispose()
        {
            Stop();
            this._timer.Dispose();
        }
    }
}
