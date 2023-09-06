using System;
using UniRx;
using UnityEngine;
using Zenject;
using Observable = Core.Binders.Observable;

namespace Infrastructure.Services.Input
{
    public class StandaloneInputService : IInputService, ITickable
    {
        private const int DefaultSpeedMultiplier = 10;

        //It can be made like Moving, but i wanted to show work with events.
        public Observable Attacked { get; } = new();

        public Vector2 Moving()
        {
            var horizontal = UnityEngine.Input.GetAxis("Horizontal");
            var vertical = UnityEngine.Input.GetAxis("Vertical");

            var direction = new Vector2(horizontal, vertical);
            return direction*Time.deltaTime*DefaultSpeedMultiplier;
        }

        public void Tick()
        {
            if (UnityEngine.Input.GetAxis("Fire1") == 1)
            {
                Attacked?.Invoke();
            }
        }
    }
}