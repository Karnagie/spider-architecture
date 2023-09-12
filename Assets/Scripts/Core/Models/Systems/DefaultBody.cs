using UnityEngine;

namespace Core.Models.Systems
{
    public class DefaultBody : IPhysicBody
    {
        private Rigidbody2D _rigidbody;
        private Collider2D _collider;

        public DefaultBody(Rigidbody2D rigidbody, Collider2D collider)
        {
            _collider = collider;
            _rigidbody = rigidbody;
        }
        
        public void Push(Vector2 force, ForceMode2D forceMode)
        {
            _rigidbody.AddForce(force, forceMode);
        }

        public Vector2 ClosestPointTo(Vector2 position)
        {
            return _collider.ClosestPoint(position);
        }
    }
    
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
    }
}