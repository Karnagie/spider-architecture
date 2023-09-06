using System;
using Infrastructure.Services;
using Zenject;

namespace Core.Models.Services
{
    public class TickingService : IFixedTickable, ITickable, IDisposable
    {
        public ItemHolder<IFixedTickable> FixedTickableHolder = new();
        public ItemHolder<ITickable> TickableHolder = new();

        public void FixedTick()
        {
            var items = FixedTickableHolder.Get();
            foreach (var item in items)
            {
                item.FixedTick();
            }
        }

        public void Tick()
        {
            var items = TickableHolder.Get();
            foreach (var item in items)
            {
                item.Tick();
            }
        }

        public void Dispose()
        {
            FixedTickableHolder.Dispose();
            TickableHolder.Dispose();
        }
    }
}