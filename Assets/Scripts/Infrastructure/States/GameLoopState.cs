using Infrastructure.Services.Binding;

namespace Infrastructure.States
{
    public class GameLoopState : IState
    {
        private BinderService _binderService;

        public GameLoopState(BinderService binderService)
        {
            _binderService = binderService;
        }

        public void Enter()
        {
        
        }

        public void Exit()
        {
            _binderService.Dispose();
        }
    }
}