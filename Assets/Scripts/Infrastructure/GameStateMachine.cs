using System;
using System.Collections.Generic;
using Infrastructure.Helpers;
using Infrastructure.Services.Binding;
using Infrastructure.Services.Ui;
using Infrastructure.States;
using UI;
using Zenject;

namespace Infrastructure
{
    public class GameStateMachine
    {
        private readonly Dictionary<Type, IExitableState> _states;
        private IExitableState _activeState;

        public GameStateMachine(
            SceneLoader sceneLoader, 
            LoadingCurtain loadingCurtain,
            LoadLevelState.Factory loadLevelStateFactory, 
            IInitializable initializable, 
            BinderService binderService,
            WindowService windowService)
        {
            _states = new Dictionary<Type, IExitableState>()
            {
                [typeof(BootstrapState)] = new BootstrapState(this, sceneLoader, initializable),
                [typeof(LoadLevelState)] = loadLevelStateFactory.Create(this, sceneLoader, loadingCurtain),
                [typeof(MenuState)]  = new MenuState(this, sceneLoader, loadingCurtain, windowService),
                [typeof(GameLoopState)] = new GameLoopState(binderService),
            };
        }

        public void Enter<TState>() where TState : class, IState
        {
            var state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayLoad>(TPayLoad payLoad) where TState : class, IPayLoadState<TPayLoad>
        {
            var state = ChangeState<TState>();
            state.Enter(payLoad);
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();
        
            var state = GetState<TState>();
            _activeState = state;
        
            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState => 
            _states[typeof(TState)] as TState;
    }
}