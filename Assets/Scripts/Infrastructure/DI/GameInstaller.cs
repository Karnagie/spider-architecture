using System;
using CompositeDirectorWithGeneratingComposites.CompositeDirector;
using Core.Binders;
using Data;
using Infrastructure.AssetManagement;
using Infrastructure.Factories;
using Infrastructure.Helpers;
using Infrastructure.Services.Input;
using Infrastructure.Services.PersistentProgress;
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

            //Services
            RegisterTickService();
            RegisterInputService();
            RegisterPersistentProgressService();

            //Factories
            RegisterGameFactories();
            RegisterSpiderFactory();
        }

        private void Update()
        {
            CompositeHelper.Perform();
        }

        private void OnDestroy()
        {
            Container.Resolve<CompositeDirector>().Dispose();
        }
        
        private void RegisterDirectorAndComposites()
        {
            var compositeDirector = new CompositeDirector();
            Container.Bind<CompositeDirector>().FromInstance(compositeDirector).AsSingle();
        }

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