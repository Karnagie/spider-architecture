using Core.Models.Services;
using Infrastructure.Helpers;
using Infrastructure.Services.Binding;
using UnityEngine;

namespace Core.Models.Systems
{
    public class PlayerDamager : IDamager
    {
        private ISpider _model;
        private DamageReceiverService _damageReceiverService;
        private float _attackCooldown;
        private float _time;
        private IPhysicsService _physicsService;

        public PlayerDamager(ISpider model, float attackCooldown, 
            DamageReceiverService damageReceiverService, IPhysicsService physicsService)
        {
            _physicsService = physicsService;
            _attackCooldown = attackCooldown;
            _damageReceiverService = damageReceiverService;
            _model = model;
        }

        public void TryDamage()
        {
            if (NotReadyToAttack())
                return;

            var filters = SetFiltersForTarget();

            var damaged = _damageReceiverService.TryPerform(_model.Stats.Damage.Value, filters);
            foreach (var reciever in damaged)
            {
                _physicsService.TryPush(reciever, _model);
            }
            
            if (damaged.Length > 0)
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
            var filterTag = new Filter<ISpider>((spider) => spider.Stats.Tag == SpiderTag.Enemy);
            var filterCollision = new Filter<ISpider>((spider) =>
                _physicsService.HasCollision(_model.Components.Collider, spider.Components.Collider));

            return new[] {filterTag, filterCollision};
        }
    }
}