using UnityEngine;

namespace Core.Models
{
    public class SpiderComponents
    {
        public Transform Transform;
        public Collider2D Collider;

        public SpiderComponents(Transform transform, Collider2D collider)
        {
            Transform = transform;
            Collider = collider;
        }
    }
}