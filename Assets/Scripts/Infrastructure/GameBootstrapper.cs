using Infrastructure.Helpers;
using Infrastructure.States;
using UI;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class GameBootstrapper : MonoBehaviour, ICoroutineRunner
    {
        public LoadingCurtain Curtain;
    
        private Game _game;
    
        private LoadLevelState.Factory _loadLevelFactory;
        private IInitializable _initializable;

        [Inject]
        void Construct(LoadLevelState.Factory loadLevelFactory, IInitializable initializable)
        {
            _initializable = initializable;
            _loadLevelFactory = loadLevelFactory;
        }

        private void Awake()
        {
            _game = new Game(this, Curtain, _loadLevelFactory, _initializable);
            _game.StateMachine.Enter<BootstrapState>();
        
            DontDestroyOnLoad(this);
        }
    }
}