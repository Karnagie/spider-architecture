using UnityEngine;

namespace Core.Models.Services
{
    public class CollisionService
    {
        public bool HasCollision(Collider2D first, Collider2D second)
        {
            return first.IsTouching(second);
        }
    }
}