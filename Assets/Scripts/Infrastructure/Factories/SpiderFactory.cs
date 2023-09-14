using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Core.Behaviours;
using Core.Models;
using Core.Models.Components;
using Infrastructure.Helpers;
using Infrastructure.Services;
using Infrastructure.Services.Binding;
using Infrastructure.Services.Input;
using Infrastructure.Services.System;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Infrastructure.Factories
{
    public class SpiderFactory
    {
        private readonly ViewFactory _viewFactory;
        private readonly IInputService _inputService;
        private readonly ServiceSystemFactory _serviceSystemFactory;
        private readonly SystemService _systemService;
        private SpiderLegFactory _spiderLegFactory;

        public SpiderFactory(ViewFactory viewFactory,
            ServiceSystemFactory serviceSystemFactory,
            IInputService inputService,
            SystemService systemService,
            SpiderLegFactory spiderLegFactory)
        {
            _spiderLegFactory = spiderLegFactory;
            _systemService = systemService;
            _inputService = inputService;
            _serviceSystemFactory = serviceSystemFactory;
            _viewFactory = viewFactory;
        }

        public void CreatePlayer(Vector3 position)
        {
            var behaviour = _viewFactory.DefaultSpider(position);
            
            var stats = new SpiderStats(100, 10, SpiderTag.Player);
            var components = new SpiderComponents(behaviour.Transform, behaviour.Collider, behaviour.Rigidbody);
            var model = new Spider(stats, components);
            var binder = new Binder();
            var linker = new SystemLinker();
            
            LinkPlayerSystems(model, binder, linker);
            BindSpider(model, binder, behaviour, linker);
        }
        
        public void CreateEnemy(Vector3 position)
        {
            var behaviour = _viewFactory.DefaultSpider(position);
            
            var stats = new SpiderStats(50, 5, SpiderTag.Enemy);
            var components = new SpiderComponents(behaviour.Transform, behaviour.Collider, behaviour.Rigidbody);
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
            var physicBody = _serviceSystemFactory.DefaultBody(model);
            
            linker.Add(enemyMovement);
            linker.Add(damageReceiver);
            linker.Add(damager);
            linker.Add(model);
            linker.Add(physicBody);
        }

        private void BindSpider(Spider model, Binder binder, SpiderBehaviour behaviour, SystemLinker linker)
        {
            var leg = _spiderLegFactory.Create(model);
            binder.LinkHolder(_systemService, leg);

            binder.Bind(model.Stats.Health, (health => behaviour.HealthText.text = $"hp: {health}"));

            binder.LinkHolder(_systemService, linker);

            binder.LinkEvent(model.Killed, binder.Dispose);
            binder.LinkEvent(model.Killed, (() => Object.Destroy(behaviour.gameObject)));
        }
    }

    public class DeathService : ITickable
    {
        private SystemService _systemService;

        public DeathService(SystemService systemService)
        {
            _systemService = systemService;
        }
        
        public void Tick()
        {
            var models = _systemService.TryFindSystems<Spider>();//change spider to ialive with health
            foreach (var model in models)
            {
                if (model.Stats.Health.Value <= 0)
                {
                    model.Kill();
                }
            }
        }
    }
}