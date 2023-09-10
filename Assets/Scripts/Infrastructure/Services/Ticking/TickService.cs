using System.Collections.Generic;
using Infrastructure.Services.Binding;
using Zenject;

namespace Infrastructure.Services.Ticking
{
    public class TickService : ITickable, IFixedTickable
    {
        private SystemService _systemService;
        private Filter<ITickable> _tickFilter;
        private Filter<IFixedTickable> _fixedTickFilter;

        public TickService(SystemService systemService)
        {
            _systemService = systemService;
            _tickFilter = new Filter<ITickable>();
            _fixedTickFilter = new Filter<IFixedTickable>();
        }
        
        public void Tick()
        {
            var tickables = _systemService.TryFindSystems<ITickable>();
            
            foreach (var tickable in tickables)
            {
                tickable.Tick();
            }
        }

        public void FixedTick()
        {
            var fixedTickables = _systemService.TryFindSystems<IFixedTickable>();
            
            foreach (var tickable in fixedTickables)
            {
                tickable.FixedTick();
            }
        }
    }
}