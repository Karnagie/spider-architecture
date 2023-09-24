using System.Collections.Generic;
using Core.Models.Systems;
using Infrastructure.Services.Binding;
using Infrastructure.Services.System;
using UnityEngine;

namespace Core.Models.Services
{
    public class PhysicsService : IPhysicsService
    {
        private SystemService _systemService;

        public PhysicsService(SystemService systemService)
        {
            _systemService = systemService;
        }
        
        public bool HasCollision(Collider2D first, Collider2D second)
        {
            return first.IsTouching(second);
        }

        public void TryPush(ISystem system, ISpider pusher)
        {
            var linkers = _systemService.LinkersThatHas(system);
            foreach (var linker in linkers)
            {
                if(!linker.TryGetSystems<ISpider>(out var spider))
                    return;
                
                if (linker.TryGetSystems<IPhysicBody>(out var bodies))
                {
                    foreach (var body in bodies)
                    {
                        body.Push(CalculateForce(spider[0], pusher)*5, ForceMode2D.Impulse);   
                    }
                }
            }
        }

        public IPhysicBody[] All(params IFilter[] filters)
        {
            return _systemService.TryFindSystems<IPhysicBody>(filters);
        }

        public bool TryCastVector(Vector3 start, Vector3 end, out Vector2 point)
        {
            point = default;
            var physicsBodies = _systemService.TryFindSystems<IPhysicBody>();

            foreach (var physicsBody in physicsBodies)
            {
                if (physicsBody.OverlapLine(start, end, out var linePoint))
                {
                    point = linePoint;
                    return true;
                }
            }
            
            return false;
        }

        private Vector2 CalculateForce(ISpider target, ISpider pusher)
        {
            return (target.Components.Transform.position - pusher.Components.Transform.position).normalized;
        }
    }

    public interface IPhysicsService
    {
        bool HasCollision(Collider2D first, Collider2D second);
        void TryPush(ISystem system, ISpider pusher);
        IPhysicBody[] All(params IFilter[] filters);
        bool TryCastVector(Vector3 start, Vector3 end, out Vector2 point);
    }
}