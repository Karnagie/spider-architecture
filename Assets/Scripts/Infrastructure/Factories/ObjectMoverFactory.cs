using Core.Models.Systems;
using Infrastructure.Services.Input;
using Infrastructure.Services.Ticking;

namespace Infrastructure.Factories
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