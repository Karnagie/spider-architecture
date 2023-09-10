using Core.Models.Systems;
using Infrastructure.Services.System;
using UnityEngine;

namespace Core.Models.Services
{
    public class PhysicsService
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

        public void TryPush(ISystem system, Spider pusher)
        {
            var linkers = _systemService.LinkersThatHas(system);
            foreach (var linker in linkers)
            {
                if(!linker.TryGetSystem<Spider>(out var spider))
                    return;
                
                if (linker.TryGetSystem<IPhysicBody>(out var body))
                {
                    body.Push(CalculateForce(spider, pusher)*5, ForceMode2D.Impulse);
                }
            }
        }

        private Vector2 CalculateForce(Spider target, Spider pusher)
        {
            return (target.Components.Transform.position - pusher.Components.Transform.position).normalized;
        }
    }
}