using System;
using System.Collections.Generic;

namespace Infrastructure.Services
{
    public class ItemHolder<T> : IDisposable, IItemHolder<T>
    {
        private List<T> _items = new();

        public T[] Get()
        {
            _items.RemoveAll(item => item == null);
            return _items.ToArray();
        }

        public void Add(T item)
        {
            if (_items.Contains(item))
                return;
            _items.Add(item);
        }

        public void Remove(T item)
        {
            if (_items.Contains(item))
                _items.Remove(item);
        }

        public void Dispose()
        {
            _items.Clear();
        }
    }

    public interface IItemHolder<in T>
    {
        void Add(T item);
        void Remove(T item);
    }
}