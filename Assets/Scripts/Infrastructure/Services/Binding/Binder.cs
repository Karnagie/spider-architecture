using System;
using System.Collections.Generic;
using Core.Models.Stats;
using Infrastructure.Services.System;
using UniRx;
using Observable = Infrastructure.Helpers.Observable;

namespace Infrastructure.Services.Binding
{
    public class Binder : IDisposable
    {
        private List<IDisposable> _disposables = new();
        
        private Binder()
        {
            
        }
        
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
        
        public static class Factory
        {
            public static Binder Create(BinderService binderService)
            {
                var binder = new Binder();
                binderService.LinkerHolder.Add(binder);
                return binder;
            }
        }
    }
}