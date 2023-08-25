using UnityEngine;

namespace Infrastructure.Services.Input
{
    public class StandaloneInputService : IInputService
    {
        private const int DefaultSpeedMultiplier = 10;

        public Vector2 Moving()
        {
            var horizontal = UnityEngine.Input.GetAxis("Horizontal");
            var vertical = UnityEngine.Input.GetAxis("Vertical");

            var direction = new Vector2(horizontal, vertical);
            return direction*Time.deltaTime*DefaultSpeedMultiplier;
        }
    }
}