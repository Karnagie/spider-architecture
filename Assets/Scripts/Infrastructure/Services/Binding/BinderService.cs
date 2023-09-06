namespace Infrastructure.Services.Binding
{
    public class BinderService
    {
        public readonly ItemHolder<Binder> LinkerHolder = new();

        public TReturn Find<TReturn>(params IFilter[] filters)
        {
            var linkers = LinkerHolder.Get();

            foreach (var linker in linkers)
            {
                var met = true;
                foreach (var filter in filters)
                {
                    if (filter.Met(linker) == false)
                        met = false;
                }
                
                if (met && linker.TryGetComponent<TReturn>(out var component))
                    return component;
            }

            return default;
        }
    }
}