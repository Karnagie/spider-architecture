using Core.Binders;
using Core.Models;
using Infrastructure.Factories;
using Infrastructure.Helpers;
using Infrastructure.Services.Input;
using UI;
using UnityEngine;
using Zenject;

namespace Infrastructure.States
{
    public class LoadLevelState : IPayLoadState<string>
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly SpiderFactory _gameFactory;

        public LoadLevelState(GameStateMachine gameStateMachine, SceneLoader sceneLoader, LoadingCurtain loadingCurtain,
            SpiderFactory gameFactory)
        {
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
            _gameFactory.CreatePlayer(new Vector3(0, 0, 0));
            _gameFactory.CreateEnemy(new Vector3(1, 1, 0));
        }

        public class Factory : PlaceholderFactory<GameStateMachine, SceneLoader, LoadingCurtain, LoadLevelState>
        {
        }
    }
}