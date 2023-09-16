using System;

namespace Infrastructure.Services.Binding
{
    public class BinderService : IDisposable
    {
        public readonly ItemHolder<Binder> LinkerHolder = new();

        public void Dispose()
        {
            var binders = LinkerHolder.Get();
            foreach (var binder in binders)
            {
                binder.Dispose();
            }
            LinkerHolder?.Dispose();
        }
    }
}