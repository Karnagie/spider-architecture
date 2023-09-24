using UnityEngine;

namespace Core.Models.Systems
{
    public class DefaultWorld : IPhysicBody
    {
        private Collider2D[] _colliders;

        public DefaultWorld(Collider2D[] colliders)
        {
            _colliders = colliders;
        }
        
        public void Push(Vector2 force, ForceMode2D forceMode)
        {
            return;
        }

        public Vector2 ClosestPointTo(Vector2 position)
        {
            var closestPosition = _colliders[0].ClosestPoint(position);
            foreach (var collider in _colliders)
            {
                var point = collider.ClosestPoint(position);
                if (Vector3.Distance(position, point) 
                    < 
                    Vector3.Distance(position, closestPosition))
                {
                    closestPosition = point;
                }
            }

            return closestPosition;
        }

        public bool OverlapLine(Vector3 start, Vector3 end, out Vector3 point)
        {
            point = default;
            RaycastHit2D[] bounds = new RaycastHit2D[10];
            var size = Physics2D.LinecastNonAlloc(start, end, bounds);
            foreach (var collider in _colliders)
            {
                foreach (var bound in bounds)
                {
                    if (bound.collider == collider)
                    {
                        point = bound.point;
                        return true;
                    }
                }
            }
            
            return false;
        }
    }
}