using Core.Behaviours;
using Core.Models;
using Core.Models.Services;
using Core.Models.Systems;
using Infrastructure.Services.Binding;
using Infrastructure.Services.System;
using UnityEngine;

namespace Infrastructure.Factories
{
    public class SpiderLegFactory
    {
        private PhysicsService _physicsService;
        private ViewFactory _viewFactory;

        public SpiderLegFactory(PhysicsService physicsService, ViewFactory viewFactory)
        {
            _viewFactory = viewFactory;
            _physicsService = physicsService;
        }
        
        public SystemLinker Create(Spider model)
        {
            var behaviour = _viewFactory.DefaultSpiderLeg(model.Components.Transform);
            var binder = new Binder();//add to service
            var linker = new SystemLinker();

            var legSystem = new LegSystem(model, _physicsService, 1, behaviour);
            linker.Add(legSystem);
            binder.LinkEvent(model.Killed, () => Object.Destroy(behaviour.gameObject));
            
            return linker;
        }
    }
}