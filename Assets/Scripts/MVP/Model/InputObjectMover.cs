using Data;
using Infrastructure.Services.Input;
using MVP.Factory;
using Zenject;

namespace MVP.Model
{
    public class InputObjectMover : ITickable
    {
        private PositionService _positionService;
        private IInputService _inputService;
        private readonly int _id;

        public InputObjectMover(PositionService positionService, IInputService inputService, int id)
        {
            _inputService = inputService;
            _id = id;
            _positionService = positionService;
        }
        
        public void Tick()
        {
            _positionService.AddOffset(_id, _inputService.Moving().AsVector2Data());
        }
    }
}
