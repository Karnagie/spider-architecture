using System;
using CompositeDirectorWithGeneratingComposites.CompositeDirector.CompositeGeneration;
using Core.Behaviours;
using Data;
using Infrastructure.Disposables;
using Object = UnityEngine.Object;

namespace Core.Binders
{
    public class SpiderView : IClearableWithOptions
    {
        private readonly SpiderBehaviour _behaviour;

        public DisposeTime DisposeTime => DisposeTime.SceneEnd;

        public SpiderView(SpiderBehaviour behaviour)
        {
            _behaviour = behaviour;
        }

        public void Move(Vector3Data value)
        {
            _behaviour.Transform.position += value.AsUnityVector();
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