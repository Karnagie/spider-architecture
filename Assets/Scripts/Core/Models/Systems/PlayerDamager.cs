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
            if (NotReadyToAttack())
                return;

            var filters = SetFiltersForTarget();
            
            if (_damageReceiverService.TryPerform(_model.Stats.Damage.Value, filters))
            {
                _time = Time.time + _attackCooldown;
            }
        }

        private bool NotReadyToAttack()
        {
            return _time > Time.time;
        }

        private IFilter[] SetFiltersForTarget()
        {
            var filterTag = new Filter<Spider>((spider) => spider.Stats.Tag == SpiderTag.Enemy);
            var filterCollision = new Filter<Spider>((spider) =>
                _collisionService.HasCollision(_model.Components.Collider, spider.Components.Collider));

            return new[] {filterTag, filterCollision};
        }
    }
}