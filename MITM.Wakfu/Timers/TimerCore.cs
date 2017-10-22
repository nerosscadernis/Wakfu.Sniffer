using System;
using System.Threading;

namespace MITM.Wakfu.Timers
{
    public class TimerCore : IDisposable
    {
        #region Var
        private Timer _tm;
        private AutoResetEvent _autoEvent;
        private Action _callback;
        private TimeSpan _delay;
        private int _LastUpdate;
        private bool _stop;
        #endregion

        public int UntilTime
        { get { return (int)(_delay.Ticks - Math.Floor(_delay.TotalMilliseconds * TimeSpan.TicksPerMillisecond)); } }

        public int PastTimeSecond
        { get { return (int)(_delay.Ticks / TimeSpan.TicksPerMillisecond); } }

        #region Constructor
        public TimerCore(Action callback, int global)
        {
            _callback = callback;
            _autoEvent = new AutoResetEvent(false);
            _delay = new TimeSpan(0, 0, 0, 0, global);
            _tm = new Timer(Execute, _autoEvent, _delay, new TimeSpan(0));
            _LastUpdate = Environment.TickCount;
        }
        public TimerCore(Action callback, int global, int delay, bool stopDirect = false)
        {
            _stop = stopDirect;
            _callback = callback;
            _autoEvent = new AutoResetEvent(false);
            _delay = new TimeSpan(0, 0, 0, 0, delay);
            _tm = new Timer(ExecutePeriod, _autoEvent, new TimeSpan(0, 0, 0, 0, global), _delay);
            _LastUpdate = Environment.TickCount;
        }
        public TimerCore(Action callback, TimeSpan global, TimeSpan delay, bool stopDirect = false)
        {
            _stop = stopDirect;
            _callback = callback;
            _autoEvent = new AutoResetEvent(false);
            _delay = delay;
            _tm = new Timer(ExecutePeriod, _autoEvent, global, delay);
            _LastUpdate = Environment.TickCount;
        }
        public void Stop()
        { _stop = true; }
        public void Start()
        {
            _tm.Change(_delay, _delay);
            _stop = false;
        }
        #endregion

        #region Methods
        private void Execute(object stateInfo)
        {
            if (!_stop)
                _callback?.Invoke();
            Dispose();
        }
        private void ExecutePeriod(object stateInfo)
        {
            if (!_stop)
                _callback?.Invoke();
            _LastUpdate = Environment.TickCount;
        }
        public void Dispose()
        {
            _stop = true;
            _tm?.Dispose();
            _tm = null;
            _autoEvent?.Dispose();
            _autoEvent = null;
        }
        #endregion
    }
}
