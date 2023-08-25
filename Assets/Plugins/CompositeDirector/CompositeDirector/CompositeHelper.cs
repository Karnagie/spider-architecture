using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CompositeDirectorWithGeneratingComposites.CompositeDirector
{
    public static class CompositeHelper
    {
        public static readonly HashSet<ILayer> Stash = new ();
        public static ILayer Last => Stash.Count > 0 ? Stash.Last() : null;

        public static void Group<T>(Action<T[]> process, IPool<T> composite) where T : IPoolItem
        {
            lock (Stash)
            {
                Stash.Add(new CompositeDecorator<T>(process, composite.Items.ToArray()));
            }
        }

        public static void Perform()
        {
            lock (Stash)
            {
                foreach (var layer in Stash.ToArray())
                {
                    layer.Perform();
                }
                Stash.Clear();   
            }
        }
        
        public static void PerformLast()
        {
            lock (Stash)
            {
                var layer = Last;
                if (layer == null)
                    return;
                layer.Perform();
                Stash.Remove(Last);   
            }
        }

        public static void Reset()
        {
            lock (Stash)
            {
                Stash.Clear();
            }
        }
    }


    public interface ILayer
    {
        void Perform(); 
        void Apply(ICommand command);
    }

    public interface IItemsHandler<out T> : ILayer
    {
        T[] Items { get; }
    }

    public interface ICommand
    {
        T[] Do<T>(T[] items);
    }
}