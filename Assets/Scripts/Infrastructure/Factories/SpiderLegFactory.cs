﻿using Core.Behaviours;
using Core.Models;
using Core.Models.Services;
using Core.Models.Systems;
using Infrastructure.Services.Binding;
using Infrastructure.Services.System;
using UnityEngine;

namespace Infrastructure.Factories
{
    public class SpiderLegFactory
    {
        private PhysicsService _physicsService;
        private ViewFactory _viewFactory;

        public SpiderLegFactory(PhysicsService physicsService, ViewFactory viewFactory)
        {
            _viewFactory = viewFactory;
            _physicsService = physicsService;
        }
        
        public LegSystem Create(Spider model, Transform parent)
        {
            var behaviour = _viewFactory.DefaultSpiderLeg(parent);
            var binder = new Binder();//add to service

            var legSystem = new LegSystem(_physicsService, behaviour.Length, behaviour);
            binder.LinkEvent(model.Killed, () => Object.Destroy(behaviour.gameObject));
            
            return legSystem;
        }
    }
}