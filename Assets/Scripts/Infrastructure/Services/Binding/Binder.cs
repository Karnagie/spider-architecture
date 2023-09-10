using System;
using System.Collections.Generic;
using UniRx;
using Observable = Infrastructure.Helpers.Observable;

namespace Infrastructure.Services.Binding
{
    public class Binder : IDisposable
    {
        private List<IDisposable> _disposables = new();
        
        public void Bind<TBinding>(IObservable<TBinding> property, Action<TBinding> onChange)
        {
            _disposables.Add(property.Subscribe(onChange));
        }
        
        public void LinkHolder<TBinding>(IItemHolder<TBinding> itemHolder, TBinding item)
        {
            itemHolder.Add(item);
            _disposables.Add(new DisposeAction(() => itemHolder.Remove(item)));
        }

        public void LinkEvent(Observable observable, Action action)
        {
            observable.Event += action;
            _disposables.Add(observable);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)    
            {
                disposable.Dispose();
            }
            _disposables.Clear();
        }
    }

    public class SystemService : IItemHolder<SystemLinker>
    {
        private List<SystemLinker> _linkedSystems = new();
        
        public TReturn[] TryFindSystems<TReturn>(params IFilter[] filters)
        {
            List<TReturn> targets = new();
            
            foreach (var linker in _linkedSystems)
            {
                if (!Met(linker, filters))
                    continue;
                
                if (linker.TryGetSystem(out TReturn system))
                {
                    targets.Add(system);
                }
            }

            return targets.ToArray();
        }

        private bool Met(SystemLinker linker, IFilter[] filters)
        {
            foreach (var filter in filters)
            {
                if (!filter.Met(linker))
                    return false;
            }

            return true;
        }

        public void Add(SystemLinker item)
        {
            _linkedSystems.Add(item);
        }

        public void Remove(SystemLinker item)
        {
            _linkedSystems.Remove(item);
        }
    }

    public class SystemLinker
    {
        private List<ISystem> _systems = new();

        public void Add(ISystem system)
        {
            _systems.Add(system);
        }
        
        public bool TryGetSystem<T>(out T foundSystem)
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
    }

    public interface ISystem
    {
    }
}