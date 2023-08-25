using System.Collections.Generic;

namespace CompositeDirectorWithGeneratingComposites.CompositeDirector
{
    public interface IPool<T> : IItemPool where T : IPoolItem
    {
        void Add(T item);
        void Remove(T item);
    
        List<T> Items { get; }
    }

    public interface IItemPool : IPoolItem
    {
        void TryAdd(IPoolItem poolItem);
        void TryRemove(IPoolItem poolItem);
    }
}