using System;
using CompositeDirectorWithGeneratingComposites.CompositeDirector.CompositeGeneration;
using Data;
using Infrastructure.Disposables;
using MVP.View;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MVP.Binders
{
    public class SpiderView : IClearableWithOptions
    {
        private readonly SpiderBehaviour _behaviour;

        public DisposeTime DisposeTime => DisposeTime.SceneEnd;
        public IReactiveProperty<Transform> Transform { get; }

        public SpiderView(SpiderBehaviour behaviour)
        {
            _behaviour = behaviour;
            Transform = new ReactiveProperty<Transform>(_behaviour.Transform);
        }

        public void Move(Vector3 value)
        {
            _behaviour.Transform.position += value;
        }

        public void ChangeHealth(int value)
        {
            _behaviour.HealthText.text = $"hp: {value}";
        }

        public Result Clear()
        {
            Dispose();
            return Result.Ok;
        }

        public void Dispose()
        {
            if (_behaviour != null)
                Object.Destroy(_behaviour.gameObject);
            
            Disposed?.Invoke();
        }

        public event Action Disposed;
    }
}