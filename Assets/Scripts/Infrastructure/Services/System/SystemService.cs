using System.Collections;
using System.Collections.Generic;
using Core.Models.Systems;
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
                
                if (linker.TryGetSystems(out TReturn[] systems))
                {
                    targets.AddRange(systems);
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

        public SystemLinker[] LinkersThatHas(ISystem system)
        {
            List<SystemLinker> linkers = new();
            foreach (var linkedSystem in _linkedSystems)
            {
                if (linkedSystem.Has(system))
                {
                    linkers.Add(linkedSystem);
                }
            }

            return linkers.ToArray();
        }
    }
}