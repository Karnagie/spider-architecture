using System.Collections.Generic;
using Zenject;

namespace Infrastructure.Services.Ticking
{
    public class TickService : ITickable, IFixedTickable
    {
        private List<ITickable> _tickables = new();
        private List<IFixedTickable> _fixedTickables = new();

        public void AddTickable(ITickable tickable)
        {
            _tickables.Add(tickable);
        }
        
        public void AddFixedTickable(IFixedTickable fixedTickable)
        {
            _fixedTickables.Add(fixedTickable);
        }

        public void RemoveTickable(ITickable tickable)
        {
            _tickables.Remove(tickable);
        }

        public void RemoveFixedTickable(IFixedTickable fixedTickable)
        {
            _fixedTickables.Remove(fixedTickable);
        }
        
        public void Tick()
        {
            foreach (var tickable in _tickables.ToArray())
            {
                tickable.Tick();
            }
        }

        public void FixedTick()
        {
            foreach (var tickable in _fixedTickables.ToArray())
            {
                tickable.FixedTick();
            }
        }
    }
}