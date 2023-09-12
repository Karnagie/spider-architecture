using System;
using System.Collections.Generic;
using Core.Models.Systems;

namespace Infrastructure.Services.System
{
    public class SystemLinker
    {
        private List<ISystem> _systems = new();

        public void Add(ISystem system)
        {
            _systems.Add(system);
        }
        
        public bool TryGetSystem<T>(out T foundSystem)
        {
            foundSystem = default;
            foreach (var system in _systems)
            {
                if (system is T typedSystem)
                {
                    foundSystem = typedSystem;
                    return true;
                }
            }

            return false;
        }
        
        public bool TryGetSystems<T>(out T[] foundSystems)
        {
            var systems = new List<T>();
            foreach (var system in _systems)
            {
                if (system is T typedSystem)
                {
                    systems.Add(typedSystem);
                }
            }

            foundSystems = systems.ToArray();
            
            return systems.Count > 0;
        }

        public bool Has(ISystem findingSystem)
        {
            foreach (var system in _systems)
            {
                if (system == findingSystem)
                {
                    return true;
                }
            }

            return false;
        }
    }
}