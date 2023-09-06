using Core.Models.Services;
using Infrastructure.Helpers;
using Infrastructure.Services.Binding;
using UnityEngine;

namespace Core.Models.Systems
{
    public class PlayerDamager : IDamager
    {
        private Spider _model;
        private DamageReceiverService _damageReceiverService;
        private float _attackCooldown;
        private float _time;
        private CollisionService _collisionService;

        public PlayerDamager(Spider model, float attackCooldown, 
            DamageReceiverService damageReceiverService, CollisionService collisionService)
        {
            _collisionService = collisionService;
            _attackCooldown = attackCooldown;
            _damageReceiverService = damageReceiverService;
            _model = model;
        }

        public void TryDamage()
        {
            if (_time > Time.time)
                return;
            
            var filterTag = new Filter<Spider>((spider) => spider.Stats.Tag == SpiderTag.Enemy);
            var filterCollision = new Filter<Spider>((spider) =>
                _collisionService.HasCollision(_model.Components.Collider, spider.Components.Collider));
            
            if (_damageReceiverService.TryPerform(_model.Stats.Damage.Value, filterTag, filterCollision))
            {
                _time = Time.time + _attackCooldown;
            }
        }
    }
}