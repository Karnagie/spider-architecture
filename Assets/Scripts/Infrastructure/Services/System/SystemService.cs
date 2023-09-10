using System.Collections.Generic;
using Infrastructure.Services.Binding;

namespace Infrastructure.Services.System
{
    public class SystemService : IItemHolder<SystemLinker>
    {
        private List<SystemLinker> _linkedSystems = new();
        
        public TReturn[] TryFindSystems<TReturn>(params IFilter[] filters)
        {
            List<TReturn> targets = new();
            
            foreach (var linker in _linkedSystems)
            {
                if (!Met(linker, filters))
                    continue;
                
                if (linker.TryGetSystem(out TReturn system))
                {
                    targets.Add(system);
                }
            }

            return targets.ToArray();
        }

        private bool Met(SystemLinker linker, IFilter[] filters)
        {
            foreach (var filter in filters)
            {
                if (!filter.Met(linker))
                    return false;
            }

            return true;
        }

        public void Add(SystemLinker item)
        {
            _linkedSystems.Add(item);
        }

        public void Remove(SystemLinker item)
        {
            _linkedSystems.Remove(item);
        }
    }
}