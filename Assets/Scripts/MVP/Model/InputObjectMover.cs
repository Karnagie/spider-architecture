using System;
using Data;
using Infrastructure.Services.Input;
using MVP.Factory;
using UniRx;
using UnityEngine;
using Zenject;

namespace MVP.Model
{
    public class InputObjectMover : IFixedTickable
    {
        private readonly IInputService _inputService;
        private ReactiveProperty<Vector3> _velocity;

        public IReactiveProperty<Vector3> Velocity => _velocity;

        public InputObjectMover(IInputService inputService)
        {
            _inputService = inputService;
            _velocity = new ReactiveProperty<Vector3>();
        }

        public void FixedTick()
        {
            _velocity.SetValueAndForceNotify(_inputService.Moving());
        }
    }
}
