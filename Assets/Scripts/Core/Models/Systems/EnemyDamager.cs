using System;
using System.Collections.Generic;
using Core.Models.Services;
using Infrastructure.Helpers;
using Infrastructure.Services;
using Infrastructure.Services.Binding;
using UnityEngine;
using Zenject;

namespace Core.Models.Systems
{
    public class EnemyDamager : IDamager, ITickable
    {
        private Spider _model;
        private DamageReceiverService _damageReceiverService;
        private PhysicsService _physicsService;
        private float _attackCooldown;
        private float _time;

        public EnemyDamager(Spider model, float attackCooldown, 
            DamageReceiverService damageReceiverService, PhysicsService physicsService)
        {
            _attackCooldown = attackCooldown;
            _physicsService = physicsService;
            _damageReceiverService = damageReceiverService;
            _model = model;
        }

        public void TryDamage()
        {
            if (_time > Time.time)
                return;
            
            var filterTag = new Filter<Spider>((spider) => spider.Stats.Tag == SpiderTag.Player);
            var filterCollision = new Filter<Spider>((spider) =>
                _physicsService.HasCollision(_model.Components.Collider, spider.Components.Collider));

            var damaged = _damageReceiverService
                .TryPerform(_model.Stats.Damage.Value, filterTag, filterCollision);
            
            if (damaged.Length > 0)
            {
                _time = Time.time + _attackCooldown;
            }
        }

        public void Tick()
        {
            TryDamage();
        }
    }
}