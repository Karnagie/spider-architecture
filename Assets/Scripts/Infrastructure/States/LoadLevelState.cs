using Infrastructure.Services.Input;
using MVP.Factory;
using MVP.Model;
using UI;
using Zenject;

namespace Infrastructure.States
{
    public class LoadLevelState : IPayLoadState<string>
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly SpiderFactory _gameFactory;
        private readonly ObjectMoverFactory _objectMoverFactory;

        public LoadLevelState(GameStateMachine gameStateMachine, SceneLoader sceneLoader, LoadingCurtain loadingCurtain,
            SpiderFactory gameFactory, ObjectMoverFactory objectMoverFactory)
        {
            _objectMoverFactory = objectMoverFactory;
            _loadingCurtain = loadingCurtain;
            _sceneLoader = sceneLoader;
            _gameStateMachine = gameStateMachine;
            _gameFactory = gameFactory;
        }

        public void Enter(string sceneName)
        {
            _loadingCurtain.Show();
            _sceneLoader.Load(sceneName, OnLoaded);
        }

        public void Exit()
        {
            _loadingCurtain.Hide();
        }

        private void OnLoaded()
        {
            CreatePlayerSpider();
            
            _gameStateMachine.Enter<GameLoopState>();
        }

        private void CreatePlayerSpider()
        {
            var playerSpider = _gameFactory.Create();
        }

        public class Factory : PlaceholderFactory<GameStateMachine, SceneLoader, LoadingCurtain, LoadLevelState>
        {
        }
    }
}