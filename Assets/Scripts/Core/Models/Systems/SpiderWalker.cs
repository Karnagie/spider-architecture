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
            _model.Components.Rigidbody.bodyType = 
                _spiderLegService.IsConnecting(_model) ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;
        }
    }
}