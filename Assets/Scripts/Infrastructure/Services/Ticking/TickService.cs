using System.Collections.Generic;
using Zenject;

namespace Infrastructure.Services.Ticking
{
    public class TickService : ITickable
    {
        private List<ITickable> _tickables = new();

        public void AddTickable(ITickable tickable)
        {
            _tickables.Add(tickable);
        }
        
        public void Tick()
        {
            foreach (var tickable in _tickables)
            {
                tickable.Tick();
            }
        }
    }
}