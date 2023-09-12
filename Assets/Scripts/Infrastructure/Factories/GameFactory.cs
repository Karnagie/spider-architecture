using Core.Models.Components;
using Core.Models.Systems;
using Infrastructure.Services.Binding;
using Infrastructure.Services.System;

namespace Infrastructure.Factories
{
    public class GameFactory
    {
        private ViewFactory _viewFactory;
        private SystemService _systemService;
        private ServiceSystemFactory _serviceSystemFactory;

        public GameFactory(ViewFactory viewFactory, SystemService systemService,
            ServiceSystemFactory serviceSystemFactory)
        {
            _serviceSystemFactory = serviceSystemFactory;
            _systemService = systemService;
            _viewFactory = viewFactory;
        }

        public void CreateWorld()
        {
            var behaviour = _viewFactory.World();
            var binder = new Binder();
            var linker = new SystemLinker();
            var model  = new Ground();

            var physicBody = _serviceSystemFactory.DefaultWorld(behaviour.Colliders);
            linker.Add(physicBody);
            linker.Add(model);
            
            binder.LinkHolder(_systemService, linker);
        }
    }
}