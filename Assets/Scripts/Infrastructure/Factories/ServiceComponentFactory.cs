﻿using Core.Models;
using Core.Models.Services;
using Core.Models.Systems;
using Infrastructure.Services.Input;
using Zenject;

namespace Infrastructure.Factories
{
    public class ServiceComponentFactory
    {
        private IInputService _inputService;
        private SpiderService _spiderService;
        private CollisionService _collisionService;
        private DamageReceiverService _damageReceiverService;

        public ServiceComponentFactory(IInputService inputService, SpiderService spiderService, CollisionService collisionService, DamageReceiverService damageReceiverService)
        {
            _spiderService = spiderService;
            _collisionService = collisionService;
            _damageReceiverService = damageReceiverService;
            _inputService = inputService;
        }

        public PlayerMovement PlayerMovement(Spider model)
        {
            var movement = new PlayerMovement(_inputService, model);
            return movement;
        }

        public IDamageReceiver DamageReceiver(Spider model)
        {
            var damaging = new DamageReceiver(model);
            return damaging;
        }

        public IFixedTickable EnemyMovement(Spider model)
        {
            var movement = new EnemyMovement(model, _spiderService);
            return movement;
        }

        public EnemyDamager Damager(Spider model, float cooldown)
        {
            var damager = new EnemyDamager(model, cooldown, _damageReceiverService, _collisionService);
            return damager;
        }

        public PlayerDamager PlayerDamager(Spider model, int cooldown)
        {
            var damager = new PlayerDamager(model, cooldown, _damageReceiverService, _collisionService);
            return damager;
        }
    }
}