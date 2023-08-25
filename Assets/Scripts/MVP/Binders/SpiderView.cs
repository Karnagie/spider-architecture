using System;
using CompositeDirectorWithGeneratingComposites.CompositeDirector.CompositeGeneration;
using Data;
using Infrastructure.Disposables;
using MVP.View;
using Object = UnityEngine.Object;

namespace MVP.Binders
{
    public class SpiderView : IClearableWithOptions
    {
        private SpiderBehaviour _behaviour;

        public SpiderView(SpiderBehaviour behaviour)
        {
            _behaviour = behaviour;
        }

        public void ChangePosition(Vector3Data value)
        {
            _behaviour.Transform.position = value.AsUnityVector();
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
            if (_behaviour.gameObject != null)
                Object.Destroy(_behaviour.gameObject);
            
            Disposed?.Invoke();
        }

        public event Action Disposed;
        public DisposeTime DisposeTime => DisposeTime.SceneEnd;
    }
}