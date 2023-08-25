using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CompositeDirectorWithGeneratingComposites.CompositeDirector;
using CompositeDirectorWithGeneratingComposites.CompositeDirector.CompositeGeneration;
using UnityEngine;

namespace Plugins.CompositeDirectorPlugin
{
    public class CompositeDirector : IDisposable
    {
#if UNITY_EDITOR
        private readonly CompositeCreator _compositeCreator = new();
#endif
        private readonly List<IPoolItem> _composites = new ();
        private readonly List<IPoolItem> _allItems = new();

        public void Dispose()
        {
            foreach (var composite in _composites)
            {
                composite.Dispose();
            }
        }

        public void Add<T>(T item) where T : class, IPoolItem
        {
            if(_allItems.Contains(item))
                return;
            
            _allItems.Add(item);
            
            foreach (var composite in _composites)
            {
                (composite as IItemPool)?.TryAdd(item);
            }
        }

        public void Remove<T>(T item) where T : class, IPoolItem
        {
            if(_allItems.Contains(item))
                return;
            
            _allItems.Remove(item);
            
            foreach (var composite in _composites)
            {
                (composite as IItemPool)?.TryRemove(item);
            }
        }

        public IPool<T> SetupComposite<T>() where T : class, IPoolItem
        {
 #if UNITY_EDITOR
              var composite = _compositeCreator.Create<T>();
 #else
            //  Assembly assembly = Assembly.Load("CompositeAssembly");
            // var name = $"Composite{typeof(T).Name.Substring(1)}";
            //  var composite = (IPool<T>) Activator.CreateInstance(assembly.GetType(name));  
            
            var composite = new CompositeAffectable() as IPool<T>;
            //  //todo constructor  
            composite!.GetType().GetField("Items")!.SetValue(composite, new List<T>());
            composite!.GetType().GetField("_removeActions", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(composite, new Dictionary<T, Action>()); 
 #endif
            _composites.Add(composite);
            foreach (var item in _allItems)
            {
                composite.Add(item as T);
            }
            
            return composite;
        }

        public T GetComposite<T>() where T : class, IPoolItem
        {
            var composite = _composites.FirstOrDefault((pool => pool is T));
            if (composite != null)
                return composite as T;

            Debug.Log($"There is no composite for {typeof(T)}. Created new one");  
            return SetupComposite<T>() as T;
        }
    }
}