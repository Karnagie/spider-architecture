using Codice.Client.Commands.TransformerRule;
using Core.Behaviours;
using Core.Models.Services;
using Infrastructure.Services.Binding;
using UnityEngine;
using Zenject;

namespace Core.Models.Systems
{
    public class LegSystem : ITickable, ILegSystem
    {
        private const float LegFollowSpeed = 5;
        
        private IPhysicsService _physicsService;
        private float _length;
        private Transform _startLeg;
        private Vector3? _connectedPosition;
        private Transform _pivot;
        private SpiderLegBehaviour _behaviour;
        private Vector3 _targetPosition;

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
            if(TryConnect())
            {
                MoveLegTowards(_connectedPosition!.Value);
                return;
            }
            
            MoveLegTowards(_behaviour.DefaultPivot.position);
        }

        private void MoveLegTowards(Vector3 target)
        {
            var currentPosition = _pivot.transform.position;
            var nextPosition = Vector3.MoveTowards(currentPosition, target,
                LegFollowSpeed * Time.deltaTime);
            _pivot.transform.position = nextPosition;
        }

        public bool Connecting()
        {
            if (_connectedPosition == null)
                return false;
            
            return Vector2.Distance(_connectedPosition.Value, _startLeg.position) < _length;
        }

        private bool TryConnect()
        {
            var ground = new Filter<Ground>();
            
            IPhysicBody[] collided = _physicsService.All(ground);
            if (collided.Length == 0)
                return false;

            var targetBody = FindClosest(collided);
            var targetPosition = targetBody.ClosestPointTo(_startLeg.position);
            
            if(IsClose(targetPosition) == false)
                return false;

            if (_connectedPosition != null && IsClose(_connectedPosition.Value))
                return true;

            if (IsConnectedToBackground(collided))
            {
                ConnectToBackground();
                return true;
            }

            _connectedPosition = targetPosition;
            return true;
        }

        private void ConnectToBackground()
        {
            if (_physicsService.TryCastVector(_behaviour.DefaultPivot.position, _startLeg.position, out Vector2 point))
            {
                _connectedPosition = point;
            }
            else
            {
                _connectedPosition = _behaviour.DefaultPivot.position;
            }
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

        private bool IsConnectedToBackground(IPhysicBody[] physicBodies)
        {
            var closest = FindClosest(physicBodies);
            var position = _startLeg.position;
            var point = closest.ClosestPointTo(position);
            return Vector2.Distance(point, position) == 0;
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
}