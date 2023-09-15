using Core.Models;
using Core.Models.Services;
using Core.Models.Systems;
using Infrastructure.Services.Binding;
using Infrastructure.Services.Input;
using Infrastructure.Services.System;
using UnityEngine;
using Zenject;

namespace Infrastructure.Factories
{
    public class ServiceSystemFactory
    {
        private IInputService _inputService;
        private SpiderService _spiderService;
        private IPhysicsService _physicsService;
        private DamageReceiverService _damageReceiverService;
        private SpiderLegService _spiderLegService;

        public ServiceSystemFactory(IInputService inputService, SpiderService spiderService, IPhysicsService physicsService, DamageReceiverService damageReceiverService, SystemService systemService, SpiderLegService spiderLegService)
        {
            _spiderService = spiderService;
            _physicsService = physicsService;
            _damageReceiverService = damageReceiverService;
            _spiderLegService = spiderLegService;
            _inputService = inputService;
        }

        public ISystem PlayerMovement(ISpider model)
        {
            var movement = new PlayerMovement(_inputService, model);
            return movement;
        }

        public ISystem DamageReceiver(ISpider model)
        {
            var damaging = new DamageReceiver(model);
            return damaging;
        }

        public ISystem EnemyMovement(ISpider model)
        {
            var movement = new EnemyMovement(model, _spiderService);
            return movement;
        }

        public ISystem EnemyDamager(ISpider model, float cooldown)
        {
            var damager = new EnemyDamager(model, cooldown, _damageReceiverService, _physicsService);
            return damager;
        }

        public IDamager PlayerDamager(ISpider model, int cooldown)
        {
            var damager = new PlayerDamager(model, cooldown, _damageReceiverService, _physicsService);
            return damager;
        }

        public ISystem DefaultBody(ISpider model)
        {
            return new DefaultBody(model.Components.Rigidbody, model.Components.Collider);
        }
        
        public ISystem DefaultWorld(Collider2D[] colliders)
        {
            return new DefaultWorld(colliders);
        }

        public ISystem SpiderWalker(ISpider model)
        {
            return new SpiderWalker(model, _spiderLegService);
        }
    }
}