using System;

namespace Infrastructure.Helpers
{
    public class Observable : IDisposable
    {
        public event Action Event;

        public void Invoke()
        {
            Event?.Invoke();
        }

        public void Dispose()
        {
            Event = null;
        }
    }
}