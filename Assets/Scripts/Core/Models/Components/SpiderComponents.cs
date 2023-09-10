using UnityEngine;

namespace Core.Models.Components
{
    public class SpiderComponents
    {
        public readonly Transform Transform;
        public readonly Collider2D Collider;
        public readonly Rigidbody2D Rigidbody;
        
        public SpiderComponents(Transform transform, Collider2D collider, Rigidbody2D rigidbody)
        {
            Transform = transform;
            Collider = collider;
            Rigidbody = rigidbody;
        }
    }
}