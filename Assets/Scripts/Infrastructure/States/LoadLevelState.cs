using Core.Models;
using Infrastructure.Factories;
using Infrastructure.Helpers;
using Infrastructure.Services.Binding;
using Infrastructure.Services.Input;
using Infrastructure.Services.System;
using Infrastructure.Services.Ui;
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
        private readonly SpiderFactory _spiderFactory;
        private GameFactory _gameFactory;
        private WindowService _windowService;

        public LoadLevelState(GameStateMachine gameStateMachine, SceneLoader sceneLoader, LoadingCurtain loadingCurtain,
            SpiderFactory spiderFactory, GameFactory gameFactory, WindowService windowService)
        {
            _windowService = windowService;
            _gameFactory = gameFactory;
            _loadingCurtain = loadingCurtain;
            _sceneLoader = sceneLoader;
            _gameStateMachine = gameStateMachine;
            _spiderFactory = spiderFactory;
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
            var window = _windowService.CreateLevelMenu();
            window.Start.onClick.AddListener((() => _gameStateMachine.Enter<MenuState, string>("Menu")));
            
            CreatePlayerSpider();
            
            _gameStateMachine.Enter<GameLoopState>();
        }

        private void CreatePlayerSpider()
        {
            _gameFactory.CreateWorld();
            _spiderFactory.CreatePlayer(new Vector3(0, 0, 0));
            // _spiderFactory.CreateEnemy(new Vector3(1, 1, 0));
        }

        public class Factory : PlaceholderFactory<GameStateMachine, SceneLoader, LoadingCurtain, LoadLevelState>
        {
        }
    }
}