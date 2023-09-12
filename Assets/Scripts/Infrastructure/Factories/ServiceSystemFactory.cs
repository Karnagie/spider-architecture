using Core.Models;
using Core.Models.Services;
using Core.Models.Systems;
using Infrastructure.Services.Binding;
using Infrastructure.Services.Input;
using UnityEngine;
using Zenject;

namespace Infrastructure.Factories
{
    public class ServiceSystemFactory
    {
        private IInputService _inputService;
        private SpiderService _spiderService;
        private PhysicsService _physicsService;
        private DamageReceiverService _damageReceiverService;

        public ServiceSystemFactory(IInputService inputService, SpiderService spiderService, PhysicsService physicsService, DamageReceiverService damageReceiverService)
        {
            _spiderService = spiderService;
            _physicsService = physicsService;
            _damageReceiverService = damageReceiverService;
            _inputService = inputService;
        }

        public ISystem PlayerMovement(Spider model)
        {
            var movement = new PlayerMovement(_inputService, model);
            return movement;
        }

        public ISystem DamageReceiver(Spider model)
        {
            var damaging = new DamageReceiver(model);
            return damaging;
        }

        public ISystem EnemyMovement(Spider model)
        {
            var movement = new EnemyMovement(model, _spiderService);
            return movement;
        }

        public ISystem EnemyDamager(Spider model, float cooldown)
        {
            var damager = new EnemyDamager(model, cooldown, _damageReceiverService, _physicsService);
            return damager;
        }

        public IDamager PlayerDamager(Spider model, int cooldown)
        {
            var damager = new PlayerDamager(model, cooldown, _damageReceiverService, _physicsService);
            return damager;
        }

        public ISystem DefaultBody(Spider model)
        {
            return new DefaultBody(model.Components.Rigidbody, model.Components.Collider);
        }
        
        public ISystem DefaultWorld(Collider2D[] colliders)
        {
            return new DefaultWorld(colliders);
        }
    }
}