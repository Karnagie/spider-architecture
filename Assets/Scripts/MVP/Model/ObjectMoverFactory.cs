using Data;
using Infrastructure.Services.Input;
using Infrastructure.Services.Ticking;
using MVP.Factory;
using UniRx;
using UnityEngine;
using Zenject;

namespace MVP.Model
{
    public class ObjectMoverFactory
    {
        private readonly IInputService _inputService;
        private TickService _tickService;

        public ObjectMoverFactory(IInputService inputService, TickService tickService)
        {
            _tickService = tickService;
            _inputService = inputService;
        }

        public InputObjectMover CreateInputMover()
        {
            var inputObjectMover = new InputObjectMover(_inputService);
            _tickService.AddFixedTickable(inputObjectMover);
            return inputObjectMover;
        }
    }
}