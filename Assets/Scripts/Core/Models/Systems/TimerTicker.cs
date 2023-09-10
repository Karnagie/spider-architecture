using Core.Models.Stats;
using UnityEngine;
using Zenject;

namespace Infrastructure.Factories
{
    public class TimerTicker : ITickable
    {
        private FloatStat _statsTimer;
        private bool _working;

        public TimerTicker(FloatStat statsTimer)
        {
            _statsTimer = statsTimer;
        }

        public void Tick()
        {
            if(_statsTimer.Value <= 0 || _working)
                return;
            
            _statsTimer.Decrease(Time.deltaTime);
        }

        public void Pause()
        {
            _working = false;
        }

        public void Unpause()
        {
            _working = true;
        }
    }
}