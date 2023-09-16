using System;
using Core.Models.Services;
using Core.Models.Systems;
using Data;
using Infrastructure.AssetManagement;
using Infrastructure.Factories;
using Infrastructure.Helpers;
using Infrastructure.Services.Binding;
using Infrastructure.Services.Input;
using Infrastructure.Services.PersistentProgress;
using Infrastructure.Services.System;
using Infrastructure.Services.Ticking;
using Infrastructure.Services.Ui;
using Infrastructure.States;
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
            RegisterSceneLoader();
            RegisterLoadingCurtain();

            //Services
            RegisterTickService();
            RegisterInputService();
            RegisterPersistentProgressService();
            Container.Bind<DamageReceiverService>().To<DamageReceiverService>().AsSingle();
            Container.Bind<IPhysicsService>().To<PhysicsService>().AsSingle();
            Container.Bind<SpiderService>().To<SpiderService>().AsSingle();
            Container.Bind<SystemService>().To<SystemService>().AsSingle();
            Container.BindInterfacesAndSelfTo<DeathService>().AsSingle();
            Container.BindInterfacesAndSelfTo<SpiderLegService>().AsSingle();
            Container.Bind<BinderService>().To<BinderService>().AsSingle();
            Container.Bind<WindowService>().To<WindowService>().AsSingle();
            
            //Factories
            Container.Bind<GameFactory>().To<GameFactory>().AsSingle();
            RegisterGameStateFactories();
            RegisterSpiderFactory();
            Container.Bind<ViewFactory>().To<ViewFactory>().AsSingle();
            Container.Bind<ServiceSystemFactory>().To<ServiceSystemFactory>().AsSingle();
            Container.Bind<SpiderLegFactory>().To<SpiderLegFactory>().AsSingle();
            Container.Bind<BinderFactory>().To<BinderFactory>().AsSingle();
        }

        private void RegisterInputService() => Container.BindInterfacesTo<StandaloneInputService>().AsSingle();

        private void RegisterSpiderFactory() => Container.Bind<SpiderFactory>().To<SpiderFactory>().AsSingle();

        private void RegisterTickService() => Container.BindInterfacesAndSelfTo<TickService>().AsSingle();

        private void RegisterPersistentProgressService() => 
            Container.Bind<IPersistentProgressService>().To<PersistentProgressService>().AsSingle();

        private void RegisterGameStateFactories() => 
            Container.BindFactory<GameStateMachine, SceneLoader, LoadingCurtain, LoadLevelState, LoadLevelState.Factory>();

        private void RegisterLoadingCurtain() => Container.Bind<LoadingCurtain>().To<LoadingCurtain>().AsSingle();

        private void RegisterSceneLoader() => Container.Bind<SceneLoader>().To<SceneLoader>().AsSingle();

        private void RegisterAssetProvider() => Container.Bind<IAssetProvider>().To<AssetProvider>().AsSingle();
    }
}