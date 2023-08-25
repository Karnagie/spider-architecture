﻿using System;
using CompositeDirectorWithGeneratingComposites.CompositeDirector;
using Core.Binders;
using Data;
using Infrastructure.AssetManagement;
using Infrastructure.Disposables;
using Infrastructure.Factories;
using Infrastructure.Helpers;
using Infrastructure.Services.Input;
using Infrastructure.Services.PersistentProgress;
using Infrastructure.Services.Positions;
using Infrastructure.Services.Ticking;
using Infrastructure.States;
using Plugins.CompositeDirectorPlugin;
using UI;
using Zenject;

namespace Infrastructure.DI
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            //Infrastructure
            RegisterAssetProvider();
            // RegisterGameFactory();
            RegisterSceneLoader();
            RegisterLoadingCurtain();
            RegisterDirectorAndComposites();
            Container.Bind<PositionService>().To<PositionService>().AsSingle();

            //Services
            RegisterTickService();
            RegisterInputService();
            RegisterPersistentProgressService();

            //Factories
            RegisterGameFactories();
            RegisterBinderFactory();
            RegisterSpiderFactory();
            RegisterSpiderViewFactory();
            RegisterObjectMoverFactory();
        }

        private void Update()
        {
            CompositeHelper.Perform();
        }

        private void OnDestroy()
        {
            Container.Resolve<IClearable>().Clear().ThatWithTime(DisposeTime.SceneEnd);
            Container.Resolve<CompositeDirector>().Dispose();
        }

        private void RegisterObjectMoverFactory() => 
            Container.Bind<ObjectMoverFactory>().To<ObjectMoverFactory>().AsSingle();

        private void RegisterSpiderViewFactory() => 
            Container.Bind<SpiderViewFactory>().To<SpiderViewFactory>().AsSingle();

        private void RegisterDirectorAndComposites()
        {
            var compositeDirector = new CompositeDirector();
            Container.Bind<CompositeDirector>().FromInstance(compositeDirector).AsSingle();

            var clearables = compositeDirector.SetupComposite<IClearable>();
            Container.Bind<IPool<IClearable>>().FromInstance(clearables).AsSingle();
            Container.Bind<IClearable>().FromInstance(clearables as IClearable).AsSingle();
        }

        private void RegisterBinderFactory() => Container.Bind<BinderFactory>().To<BinderFactory>().AsSingle();

        private void RegisterInputService() => Container.Bind<IInputService>().To<StandaloneInputService>().AsSingle();

        private void RegisterSpiderFactory() => Container.Bind<SpiderFactory>().To<SpiderFactory>().AsSingle();

        private void RegisterTickService() => Container.BindInterfacesAndSelfTo<TickService>().AsSingle();

        private void RegisterPersistentProgressService() => 
            Container.Bind<IPersistentProgressService>().To<PersistentProgressService>().AsSingle();

        private void RegisterGameFactories() => 
            Container.BindFactory<GameStateMachine, SceneLoader, LoadingCurtain, LoadLevelState, LoadLevelState.Factory>();

        private void RegisterLoadingCurtain() => Container.Bind<LoadingCurtain>().To<LoadingCurtain>().AsSingle();

        private void RegisterSceneLoader() => Container.Bind<SceneLoader>().To<SceneLoader>().AsSingle();

        private void RegisterAssetProvider() => Container.Bind<IAssetProvider>().To<AssetProvider>().AsSingle();

        // private void RegisterGameFactory() => Container.Bind<IGameFactory>().To<GameFactory>().AsSingle();
    }
}