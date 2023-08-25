using Infrastructure.Services.Input;
using Infrastructure.Services.Ticking;
using MVP.Factory;

namespace MVP.Model
{
    public class ObjectMoverFactory
    {
        private readonly PositionService _positionService;
        private readonly IInputService _inputService;
        private TickService _tickService;

        public ObjectMoverFactory(PositionService positionService, IInputService inputService, TickService tickService)
        {
            _tickService = tickService;
            _positionService = positionService;
            _inputService = inputService;
        }

        public InputObjectMover CreateInputMover(int id)
        {
            var inputObjectMover = new InputObjectMover(_positionService, _inputService, id);
            _tickService.AddTickable(inputObjectMover);
            return inputObjectMover;
        }
    }
}