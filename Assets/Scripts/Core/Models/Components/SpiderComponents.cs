using UnityEngine;

namespace Core.Models.Components
{
    public class SpiderComponents
    {
        public readonly Transform Transform;
        public readonly Collider2D Collider;
        
        public SpiderComponents(Transform transform, Collider2D collider)
        {
            Transform = transform;
            Collider = collider;
        }
    }
}