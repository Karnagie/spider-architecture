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

        public bool OverlapLine(Vector3 start, Vector3 end, out Vector3 point)
        {
            point = default;
            RaycastHit2D[] bounds = new RaycastHit2D[] { };
            var size = Physics2D.LinecastNonAlloc(start, end, bounds);
            
            foreach (var bound in bounds)
            {
                if (bound.collider == _collider)
                {
                    point = bound.point;
                    return true;
                }
            }

            return false;
        }
    }
}