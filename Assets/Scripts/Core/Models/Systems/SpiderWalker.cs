using Core.Models.Services;
using UnityEngine;
using Zenject;

namespace Core.Models.Systems
{
    public class SpiderWalker : ITickable, ISystem
    {
        private ISpider _model;
        private SpiderLegService _spiderLegService;

        public SpiderWalker(ISpider model, SpiderLegService spiderLegService)
        {
            _spiderLegService = spiderLegService;
            _model = model;
        }
        
        public void Tick()
        {
            if (_spiderLegService.IsConnecting(_model))
            {
                _model.Components.Rigidbody.gravityScale = 0;
                CalculateVelocity();
            }
            else
            {
                _model.Components.Rigidbody.gravityScale = 1;
            }
        }

        private void CalculateVelocity()
        {
            var newVelocity = _model.Components.Rigidbody.velocity;
            newVelocity.x -= 10 * newVelocity.x * Time.deltaTime;
            newVelocity.y -= 10 * newVelocity.y * Time.deltaTime;
            _model.Components.Rigidbody.velocity = newVelocity;
        }
    }
}