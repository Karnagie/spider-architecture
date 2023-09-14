using System;
using System.Collections.Generic;
using Core.Models.Systems;
using UnityEngine;

namespace Infrastructure.Services.System
{
    public class SystemLinker
    {
        private List<ISystem> _systems = new();

        public void Add(ISystem system)
        {
            if (system is ISingleSystem)
            {
                foreach (var system1 in _systems)
                {
                    if (system1.GetType() != system.GetType()) 
                        continue;
                    
                    Debug.LogError($"There is can be only one system '{system1.GetType() }'");
                    return;
                }
            }
            _systems.Add(system);
        }
        
        public bool TryGetSystem<T>(out T foundSystem) where T : ISingleSystem
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