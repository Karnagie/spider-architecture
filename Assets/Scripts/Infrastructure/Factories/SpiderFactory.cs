using System.Runtime.InteropServices;
using Core.Behaviours;
using Core.Models;
using Core.Models.Components;
using Core.Models.Services;
using Infrastructure.Helpers;
using Infrastructure.Services.Binding;
using Infrastructure.Services.Input;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Infrastructure.Factories
{
    public class SpiderFactory
    {
        private readonly ViewFactory _viewFactory;
        private TickingService _tickingService;
        private IInputService _inputService;
        private DamageReceiverService _damageReceiverService;
        private ServiceComponentFactory _serviceComponentFactory;
        private SpiderService _spiderService;
        private BinderService _binderService;

        public SpiderFactory(ViewFactory viewFactory, TickingService tickingService, 
            DamageReceiverService damageReceiverService, ServiceComponentFactory serviceComponentFactory, 
            SpiderService spiderService, DamageReceiverService damagerReceiverService, IInputService inputService,
            BinderService binderService)
        {
            _binderService = binderService;
            _spiderService = spiderService;
            _damageReceiverService = damagerReceiverService;
            _inputService = inputService;
            _serviceComponentFactory = serviceComponentFactory;
            _damageReceiverService = damageReceiverService;
            _tickingService = tickingService;
            _viewFactory = viewFactory;
        }

        public void CreatePlayer(Vector3 position)
        {
            var behaviour = _viewFactory.DefaultSpider();
            behaviour.Transform.position = position;
            
            var stats = new SpiderStats(100, 10, SpiderTag.Player);
            var components = new SpiderComponents(behaviour.Transform, behaviour.Collider);
            var model = new Spider(stats, components);
            var binder = new Binder();
            
            BindPlayer(model, binder);
            BindSpider(model, binder, behaviour);
        }
        
        public void CreateEnemy(Vector3 position)
        {
            var behaviour = _viewFactory.DefaultSpider();
            behaviour.Transform.position = position;
            
            var stats = new SpiderStats(50, 5, SpiderTag.Enemy);
            var components = new SpiderComponents(behaviour.Transform, behaviour.Collider);
            var model = new Spider(stats, components);
            var binder = new Binder();
            
            BindEnemy(model, binder);
            BindSpider(model, binder, behaviour);
        }

        private void BindPlayer(Spider model, Binder binder)
        {
            var damager = _serviceComponentFactory.PlayerDamager(model, 1);
            binder.LinkEvent(_inputService.Attacked, damager.TryDamage);
            
            binder.LinkHolder(_tickingService.FixedTickableHolder, _serviceComponentFactory.PlayerMovement(model));
            binder.LinkHolder(_damageReceiverService.DamageReceiverHolder, _serviceComponentFactory.DamageReceiver(model));
            binder.LinkHolder(_spiderService.SpiderHolder, model);
        }
        
        private void BindEnemy(Spider model, Binder binder)
        {
            var damager = _serviceComponentFactory.Damager(model, 1);
            var damageReceiver = _serviceComponentFactory.DamageReceiver(model);
            var enemyMovement = _serviceComponentFactory.EnemyMovement(model);
            
            binder.LinkHolder(_tickingService.FixedTickableHolder, enemyMovement);
            binder.LinkHolder(_damageReceiverService.DamageReceiverHolder, damageReceiver);
            binder.LinkHolder(_tickingService.TickableHolder, damager);
            binder.LinkHolder(_spiderService.SpiderHolder, model);
        }

        private void BindSpider(Spider model, Binder binder, SpiderBehaviour behaviour)
        {
            binder.Bind(model.Stats.Health, (health => behaviour.HealthText.text = $"hp: {health}"));
            
            binder.Bind(model.Stats.Health, (health =>
            {
                if (health > 0) return;
                Object.Destroy(behaviour.Body);
                binder.Dispose();
            }));
            
            binder.LinkHolder(_binderService.LinkerHolder, binder);
        }
    }
}