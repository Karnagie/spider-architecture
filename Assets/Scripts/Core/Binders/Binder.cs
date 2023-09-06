using System;
using System.Collections.Generic;
using Infrastructure.Factories;
using UniRx;

namespace Core.Binders
{
    public class Binder : IDisposable
    {
        private List<IDisposable> _disposables = new();
        
        public void Bind<TBinding>(IObservable<TBinding> property, Action<TBinding> onChange)
        {
            _disposables.Add(property.Subscribe(onChange));
        }
        
        public void LinkHolder<TBinding>(ItemHolder<TBinding> itemHolder, TBinding item)
        {
            itemHolder.Add(item);
            _disposables.Add(new DisposeAction(() => itemHolder.Remove(item)));
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)    
            {
                disposable.Dispose();
            }
            _disposables.Clear();
        }

        public void LinkEvent(Observable observable, Action action)
        {
            observable.Event += action;
            _disposables.Add(observable);
        }
    }

    public class DisposeAction : IDisposable
    {
        private Action _action;

        public DisposeAction(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action?.Invoke();
            _action = null;
        }
    }

    public class Observable : IDisposable
    {
        public event Action Event;

        public void Invoke()
        {
            Event?.Invoke();
        }

        public void Dispose()
        {
            Event = null;
        }
    }
}