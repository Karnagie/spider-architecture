using System;
using System.Collections.Generic;
using CompositeDirectorWithGeneratingComposites.CompositeDirector.CompositeGeneration;
using Infrastructure.Disposables;
using UniRx;
using UnityEngine;

namespace MVP.Binders
{
    public abstract class Binder : IClearableWithOptions
    {
        private List<IDisposable> _disposables = new();
        
        public void Bind<TBinding>(IReadOnlyReactiveProperty<TBinding> property, Action<TBinding> onChange)
        {
            _disposables.Add(property.Subscribe(onChange));
        }

        public Result Clear()
        {
            Dispose();
            return Result.Ok;
        }

        public abstract void Bind();

        public abstract DisposeTime DisposeTime { get; }

        public void Dispose()
        {
            foreach (var disposable in _disposables)    
            {
                disposable.Dispose();
            }
            Disposed?.Invoke();
            Disposed = null;
        }

        public event Action Disposed;
    }
}