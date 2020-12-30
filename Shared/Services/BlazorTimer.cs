using System;
using System.Timers;

namespace ZoomersClient.Shared.Services
{
    public class BlazorTimer
    {
        private Timer _timer;
        private double _end;
        public double Tick;

        public void SetTimer(double end)
        {
            _end = end * 1000;
            _timer = new Timer(1000);
            _timer.Elapsed += NotifyTimerElapsed;
            _timer.Enabled = true;
        }

        public void DisposeTimer()
        {
            Console.WriteLine("DisposeTimer");
            if (_timer != null)
            {
                Console.WriteLine("Not null, disposing!");
                _timer.Stop();
                _timer.Elapsed -= NotifyTimerElapsed;
                _timer.Dispose();
            }
        }

        public event Action OnElapsed;

        private void NotifyTimerElapsed(Object source, ElapsedEventArgs e)
        {
            if (_timer.Interval * Tick < _end)
            {
                Console.WriteLine("timer tick");
                OnElapsed?.Invoke();
                Tick += 1;
            }
            else 
            {
                Console.WriteLine("ellapsed");
                OnElapsed?.Invoke();
                _timer.Dispose();
            }
        }
    }
}