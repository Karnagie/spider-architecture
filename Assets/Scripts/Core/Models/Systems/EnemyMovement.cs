using Core.Models.Services;
using Infrastructure.Helpers;
using UnityEngine;
using Zenject;

namespace Core.Models.Systems
{
    public class EnemyMovement : IFixedTickable
    {
        private Spider _model;
        private SpiderService _spiderHolder;

        public EnemyMovement(Spider model, SpiderService spiderHolder)
        {
            _spiderHolder = spiderHolder;
            _model = model;
        }

        public void FixedTick()
        {
            if (_spiderHolder.TryFind(SpiderTag.Player, out Spider player))
            {
                var delta = (player.Components.Transform.position - _model.Components.Transform.position).normalized;
                _model.Components.Transform.Translate(delta*Time.deltaTime);
            }
        }
    }
}