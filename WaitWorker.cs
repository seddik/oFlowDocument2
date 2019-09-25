using System;
using System.Windows.Threading;

namespace oFlowDocument2
{
    public class WaitWorker
    {
        private DispatcherTimer tt;

        public WaitWorker(Action action, double delay_sec = 0.5)
        {
            if (action != null)
            {
                tt = new DispatcherTimer();
                tt.Interval = TimeSpan.FromSeconds((delay_sec <= 0.0) ? 0.5 : delay_sec);
                tt.Tick += delegate
                {
                    tt.Stop();
                    action();
                };
            }
        }

        public void DoCancel()
        {
            tt.Stop();
        }

        public void DoEvent()
        {
            tt.Stop();
            tt.Start();
        }
    }
}