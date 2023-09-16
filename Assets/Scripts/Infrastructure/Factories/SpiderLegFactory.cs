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
        private IPhysicsService _physicsService;
        private ViewFactory _viewFactory;
        private SystemService _systemService;
        private BinderFactory _binderFactory;

        public SpiderLegFactory(IPhysicsService physicsService, ViewFactory viewFactory, SystemService systemService, BinderFactory binderFactory)
        {
            _systemService = systemService;
            _binderFactory = binderFactory;
            _viewFactory = viewFactory;
            _physicsService = physicsService;
        }
        
        public void CreateAndConnect(ISpider model, Transform parent, bool invert)
        {
            var behaviour = _viewFactory.DefaultSpiderLeg(parent);
            if (invert)
            {
                var polePosition = behaviour.Pole.localPosition;
                polePosition.x = -polePosition.x;
                behaviour.Pole.localPosition = polePosition;
            }

            var binder = _binderFactory.Create();
            var linker = new SystemLinker();

            var legSystem = new LegSystem(_physicsService, behaviour.Length, behaviour);
            
            binder.LinkEvent(model.Killed, () =>
            {
                EnableRagdoll(behaviour);
                binder.Dispose();
            });
            binder.LinkHolder(_systemService, linker);
            
            linker.Add(legSystem);
            linker.Add(model);
        }

        private void EnableRagdoll(SpiderLegBehaviour behaviour)
        {
            foreach (var gameObject in behaviour.Ragdoll)
            {
                gameObject.transform.SetParent(null);
                gameObject.SetActive(true);
            }
            Object.Destroy(behaviour.gameObject);
        }
    }
}