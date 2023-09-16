using Infrastructure.Helpers;
using Infrastructure.Services.Binding;
using Infrastructure.Services.Ui;
using Infrastructure.States;
using UI;
using Zenject;

namespace Infrastructure
{
    public class Game
    {
        private GameStateMachine _stateMachine;

        public GameStateMachine StateMachine => _stateMachine;

        public Game(ICoroutineRunner coroutineRunner, LoadingCurtain loadingCurtain, LoadLevelState.Factory loadLevelStateFactor,
            IInitializable initializable, BinderService binderService, WindowService windowService)
        {
            var sceneLoader = new SceneLoader(coroutineRunner);
            var gameStateMachine = new GameStateMachine(sceneLoader, loadingCurtain, loadLevelStateFactor, initializable, 
                binderService, windowService);
            _stateMachine = gameStateMachine;
        }
    }
}