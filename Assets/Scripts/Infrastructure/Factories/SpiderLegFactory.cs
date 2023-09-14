﻿using Core.Models;
using Core.Models.Services;
using Core.Models.Systems;

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
        
        public LegSystem Create(Spider model)
        {
            var behaviour = _viewFactory.DefaultSpiderLeg(model.Components.Transform.position);
            
            return new LegSystem(model, _physicsService, 1, behaviour);
        }
    }
}