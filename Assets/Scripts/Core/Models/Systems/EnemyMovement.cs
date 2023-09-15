using Core.Models.Services;
using Infrastructure.Helpers;
using Infrastructure.Services.Binding;
using UnityEngine;
using Zenject;

namespace Core.Models.Systems
{
    public class EnemyMovement : IFixedTickable, ISystem
    {
        private ISpider _model;
        private SpiderService _spiderHolder;

        public EnemyMovement(ISpider model, SpiderService spiderHolder)
        {
            _spiderHolder = spiderHolder;
            _model = model;
        }

        public void FixedTick()
        {
            if (_spiderHolder.TryFind(SpiderTag.Player, out ISpider player))
            {
                var delta = (player.Components.Transform.position - _model.Components.Transform.position).normalized;
                _model.Components.Transform.Translate(delta*Time.deltaTime);
            }
        }
    }
}