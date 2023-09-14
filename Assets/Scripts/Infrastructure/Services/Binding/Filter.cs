using System;
using Core.Models.Systems;
using Infrastructure.Services.System;
using UnityEngine;

namespace Infrastructure.Services.Binding
{
    public class Filter<T> : IFilter where T : class, ISingleSystem
    {
        private Func<T, bool> _condition;

        public Filter(Func<T, bool> condition)
        {
            _condition = condition;
        }
        
        public Filter()
        {
            _condition = _ => true;
        }

        public bool Met(SystemLinker linker)
        {
            if (linker.TryGetSystem<T>(out var component))
            {
                return _condition.Invoke(component);
            }

            return false;
        }
    }
}