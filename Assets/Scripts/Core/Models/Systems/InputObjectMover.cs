using Data;
using Infrastructure.Services.Input;
using UniRx;
using Zenject;

namespace Core.Models.Systems
{
    public class InputObjectMover : IFixedTickable
    {
        private readonly IInputService _inputService;
        private readonly ReactiveProperty<Vector3Data> _velocity;

        public IReactiveProperty<Vector3Data> Velocity => _velocity;

        public InputObjectMover(IInputService inputService)
        {
            _inputService = inputService;
            _velocity = new ReactiveProperty<Vector3Data>();
        }

        public void FixedTick()
        {
            _velocity.SetValueAndForceNotify(_inputService.Moving().AsVector2Data());
        }
    }
}
