using System;

namespace Infrastructure.Services.Binding
{
    public class Filter<T> : IFilter where T : class
    {
        private Func<T, bool> _condition;

        public Filter(Func<T, bool> condition)
        {
            _condition = condition;
        }

        public bool Met(Binder linker)
        {
            if (linker.TryGetComponent<T>(out var component))
            {
                return _condition.Invoke(component);
            }

            return false;
        }
    }
}