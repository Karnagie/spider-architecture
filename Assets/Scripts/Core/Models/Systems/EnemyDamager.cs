using Core.Models.Services;
using Infrastructure.Helpers;
using Infrastructure.Services.Binding;
using UnityEngine;
using Zenject;

namespace Core.Models.Systems
{
    public class EnemyDamager : IDamager, ITickable
    {
        private Spider _model;
        private DamageReceiverService _damageReceiverService;
        private CollisionService _collisionService;
        private float _attackCooldown;
        private float _time;

        public EnemyDamager(Spider model, float attackCooldown, 
            DamageReceiverService damageReceiverService, CollisionService collisionService)
        {
            _attackCooldown = attackCooldown;
            _collisionService = collisionService;
            _damageReceiverService = damageReceiverService;
            _model = model;
        }

        public void TryDamage()
        {
            if (_time > Time.time)
                return;
            
            var filterTag = new Filter<Spider>((spider) => spider.Stats.Tag == SpiderTag.Player);
            var filterCollision = new Filter<Spider>((spider) =>
                _collisionService.HasCollision(_model.Components.Collider, spider.Components.Collider));
            
            if (_damageReceiverService.TryPerform(_model.Stats.Damage.Value, filterTag, filterCollision))
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