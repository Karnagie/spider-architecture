using Core.Behaviours;
using Core.Models.Services;
using Infrastructure.Services.Binding;
using UnityEngine;
using Zenject;

namespace Core.Models.Systems
{
    public class LegSystem : ITickable, ILegSystem
    {
        private IPhysicsService _physicsService;
        private float _length;
        private Transform _startLeg;
        private Vector3? _connectedPosition;
        private Transform _pivot;
        private SpiderLegBehaviour _behaviour;

        public LegSystem(IPhysicsService physicsService, float length, SpiderLegBehaviour behaviour)
        {
            _behaviour = behaviour;
            _pivot = _behaviour.TargetPivot;
            _startLeg = _behaviour.Transform;
            _length = length;
            _physicsService = physicsService;
        }
        
        public void Tick()
        {
            TryConnect();
        }
        
        public bool Connecting()
        {
            if (_connectedPosition == null)
                return false;
            
            return Vector2.Distance(_connectedPosition.Value, _startLeg.position) < _length;
        }

        private void TryConnect()
        {
            var ground = new Filter<Ground>();
            
            IPhysicBody[] collided = _physicsService.All(ground);
            
            if (collided.Length == 0)
                return;

            var targetBody = FindClosest(collided);
            var targetPosition = targetBody.ClosestPointTo(_startLeg.position);
            if(IsClose(targetPosition) == false)
                return;

            _connectedPosition = targetPosition;
            
            _pivot.position = _connectedPosition!.Value;
        }

        private IPhysicBody FindClosest(IPhysicBody[] physicBodies)
        {
            var closest = physicBodies[0];
            foreach (var physicBody in physicBodies)
            {
                
                if (Vector2.Distance(physicBody.ClosestPointTo(_startLeg.position), _startLeg.position)
                    < 
                    Vector2.Distance(closest.ClosestPointTo(_startLeg.position), _startLeg.position))
                {
                    closest = physicBody;
                }
            }

            return closest;
        }

        private bool IsClose(Vector3 position)
        {
            return Vector3.Distance(position, _startLeg.position) <= _length;
        }

        public void Dispose()
        {
            Object.Destroy(_behaviour);
        }
    }

    public interface ILegSystem : ISystem
    {
        bool Connecting();
    }
}