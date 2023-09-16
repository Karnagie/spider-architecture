using Infrastructure.Helpers;
using Zenject;

namespace Infrastructure.States
{
    public class BootstrapState : IState
    {
        private const string Bootstrap = "Bootstrap";
    
        private readonly GameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private IInitializable _initializable;

        public BootstrapState(GameStateMachine gameStateMachine, SceneLoader sceneLoader, IInitializable initializable)
        {
            _initializable = initializable;
            _sceneLoader = sceneLoader;
            _gameStateMachine = gameStateMachine;
        }

        public void Enter()
        {
            InitializeServices();
            _sceneLoader.Load(Bootstrap, EnterLoadLevel);
        }

        private void EnterLoadLevel()
        {
            _gameStateMachine.Enter<MenuState, string>("Menu");
        }

        private void InitializeServices()
        {
            _initializable.Initialize();
        }
    
        public void Exit()
        {
        
        }
    }
}