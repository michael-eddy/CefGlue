using System;
using System.Reactive.Linq;
using System.Threading;
using Avalonia.Threading;
using Xilium.CefGlue.Common.Handlers;

namespace Xilium.CefGlue.Avalonia
{
    public class AvaloniaBrowserProcessHandler : BrowserProcessHandler
    {
        private IDisposable _current;
        private readonly Lock _schedule = new();

        protected override void OnScheduleMessagePumpWork(long delayMs)
        {
            lock (_schedule)
            {
                _current?.Dispose();

                if (delayMs <= 0)
                {
                    delayMs = 1;
                }

                _current = Observable.Interval(TimeSpan.FromMilliseconds(delayMs)).Subscribe(_ =>
                {
                    Dispatcher.UIThread.Post(CefRuntime.DoMessageLoopWork);
                });
            }
        }
    }
}
