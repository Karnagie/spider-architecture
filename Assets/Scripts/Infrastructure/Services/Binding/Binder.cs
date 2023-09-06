using System;
using System.Collections.Generic;
using UniRx;
using Observable = Infrastructure.Helpers.Observable;

namespace Infrastructure.Services.Binding
{
    public class Binder : IDisposable
    {
        private List<IDisposable> _disposables = new();
        private List<object> _components = new();
        
        public void Bind<TBinding>(IObservable<TBinding> property, Action<TBinding> onChange)
        {
            _disposables.Add(property.Subscribe(onChange));
        }
        
        public void LinkHolder<TBinding>(ItemHolder<TBinding> itemHolder, TBinding item)
        {
            itemHolder.Add(item);
            _components.Add(item);
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

        public bool TryGetComponent<THolder>(out THolder component)
        {
            component = default;
            foreach (var holder in _components)
            {
                if (holder is THolder typedHolder)
                {
                    component = typedHolder;
                    return true;
                }
            }

            return false;
        }
    }
}