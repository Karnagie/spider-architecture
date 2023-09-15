using Infrastructure.Services.Binding;
using Infrastructure.Services.Input;
using Zenject;

namespace Core.Models.Systems
{
    public class PlayerMovement : IFixedTickable, ISystem
    {
        private IInputService _inputService;
        private ISpider _model;

        public PlayerMovement(IInputService inputService, ISpider model)
        {   
            _model = model;
            _inputService = inputService;
        }

        public void FixedTick()
        {
            _model.Components.Transform.Translate(_inputService.Moving());
        }
    }
}