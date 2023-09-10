using UnityEngine;

namespace Core.Models.Systems
{
    public interface IPhysicBody : ISystem
    {
        void Push(Vector2 force, ForceMode2D forceMode);
    }
    
    public class DefaultBody : IPhysicBody
    {
        private Rigidbody2D _rigidbody;

        public DefaultBody(Rigidbody2D rigidbody)
        {
            _rigidbody = rigidbody;
        }
        
        public void Push(Vector2 force, ForceMode2D forceMode)
        {
            _rigidbody.AddForce(force, forceMode);
        }
    }
}