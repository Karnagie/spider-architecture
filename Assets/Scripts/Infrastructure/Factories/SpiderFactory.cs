﻿using System;
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
using Object = UnityEngine.Object;

namespace Infrastructure.Factories
{
    public class SpiderFactory
    {
        private readonly ViewFactory _viewFactory;
        private readonly IInputService _inputService;
        private readonly ServiceSystemFactory _serviceSystemFactory;
        private readonly SystemService _systemService;
        private readonly SpiderLegFactory _spiderLegFactory;
        private BinderFactory _binderFactory;

        public SpiderFactory(ViewFactory viewFactory,
            ServiceSystemFactory serviceSystemFactory,
            IInputService inputService,
            SystemService systemService,
            SpiderLegFactory spiderLegFactory, BinderFactory binderFactory)
        {
            _spiderLegFactory = spiderLegFactory;
            _binderFactory = binderFactory;
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
            var binder = _binderFactory.Create();
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
            var binder = _binderFactory.Create();
            var linker = new SystemLinker();
            
            LinkEnemySystems(model, linker);
            BindSpider(model, binder, behaviour, linker);
        }

        private void LinkPlayerSystems(ISpider model, Binder binder, SystemLinker linker)
        {
            var damager = _serviceSystemFactory.PlayerDamager(model, 1);
            var playerMovement = _serviceSystemFactory.PlayerMovement(model);
            var damageReceiver = _serviceSystemFactory.DamageReceiver(model);
            var walker = _serviceSystemFactory.SpiderWalker(model);
            
            binder.LinkEvent(_inputService.Attacked, damager.TryDamage);
            
            linker.Add(playerMovement);
            linker.Add(damageReceiver);
            linker.Add(model);
            linker.Add(walker);
        }
        
        private void LinkEnemySystems(ISpider model, SystemLinker linker)
        {
            var damager = _serviceSystemFactory.EnemyDamager(model, 1);
            var damageReceiver = _serviceSystemFactory.DamageReceiver(model);
            var enemyMovement = _serviceSystemFactory.EnemyMovement(model);
            var physicBody = _serviceSystemFactory.DefaultBody(model);
            var walker = _serviceSystemFactory.SpiderWalker(model);
            
            linker.Add(enemyMovement);
            linker.Add(damageReceiver);
            linker.Add(damager);
            linker.Add(model);
            linker.Add(physicBody);
            linker.Add(walker);
        }

        private void BindSpider(ISpider model, Binder binder, SpiderBehaviour behaviour, SystemLinker linker)
        {
            _spiderLegFactory.CreateAndConnect(model, behaviour.LegLeft, true);
            _spiderLegFactory.CreateAndConnect(model, behaviour.LegRight, false);

            binder.Bind(model.Stats.Health, (health => behaviour.HealthText.text = $"hp: {health}"));
            binder.LinkHolder(_systemService, linker);

            BindDisposing(model, binder, behaviour);
        }

        private static void BindDisposing(ISpider model, Binder binder, SpiderBehaviour behaviour)
        {
            binder.LinkEvent(model.Killed, binder.Dispose);
            binder.LinkEvent(model.Killed, (() => Object.Destroy(behaviour.gameObject)));
        }
    }
}