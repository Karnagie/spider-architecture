using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Core.Behaviours;
using Core.Models;
using Core.Models.Components;
using Core.Models.Services;
using Infrastructure.Helpers;
using Infrastructure.Services.Binding;
using Infrastructure.Services.Input;
using Infrastructure.Services.System;
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
        private IInputService _inputService;
        private ServiceComponentFactory _serviceSystemFactory;
        private SystemService _systemService;

        public SpiderFactory(ViewFactory viewFactory,
            ServiceComponentFactory serviceSystemFactory,
            IInputService inputService,
            SystemService systemService)
        {
            _systemService = systemService;
            _inputService = inputService;
            _serviceSystemFactory = serviceSystemFactory;
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
            var linker = new SystemLinker();
            
            LinkPlayerSystems(model, binder, linker);
            BindSpider(model, binder, behaviour, linker);
        }
        
        public void CreateEnemy(Vector3 position)
        {
            var behaviour = _viewFactory.DefaultSpider();
            behaviour.Transform.position = position;
            
            var stats = new SpiderStats(50, 5, SpiderTag.Enemy);
            var components = new SpiderComponents(behaviour.Transform, behaviour.Collider);
            var model = new Spider(stats, components);
            var binder = new Binder();
            var linker = new SystemLinker();
            
            LinkEnemySystems(model, linker);
            BindSpider(model, binder, behaviour, linker);
        }

        private void LinkPlayerSystems(Spider model, Binder binder, SystemLinker linker)
        {
            var damager = _serviceSystemFactory.PlayerDamager(model, 1);
            var playerMovement = _serviceSystemFactory.PlayerMovement(model);
            var damageReceiver = _serviceSystemFactory.DamageReceiver(model);

            binder.LinkEvent(_inputService.Attacked, damager.TryDamage);
            
            linker.Add(playerMovement);
            linker.Add(damageReceiver);
            linker.Add(model);
        }
        
        private void LinkEnemySystems(Spider model, SystemLinker linker)
        {
            var damager = _serviceSystemFactory.EnemyDamager(model, 1);
            var damageReceiver = _serviceSystemFactory.DamageReceiver(model);
            var enemyMovement = _serviceSystemFactory.EnemyMovement(model);
            
            linker.Add(enemyMovement);
            linker.Add(damageReceiver);
            linker.Add(damager);
            linker.Add(model);
        }

        private void BindSpider(Spider model, Binder binder, SpiderBehaviour behaviour, SystemLinker linker)
        {
            binder.Bind(model.Stats.Health, (health => behaviour.HealthText.text = $"hp: {health}"));
            binder.Bind(model.Stats.Health, (health =>
            {
                if (health > 0) return;
                DisposeModel(binder, behaviour);
            }));
            
            binder.LinkHolder(_systemService, linker);
        }
        
        private static void DisposeModel(Binder binder, SpiderBehaviour behaviour)
        {
            binder.Dispose();
            Object.Destroy(behaviour.Body);
        }
    }
}