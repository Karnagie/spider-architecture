using Infrastructure.Helpers;

namespace Core.Models.Systems
{
    public class DamageReceiver : IDamageReceiver
    {
        private Spider _model;

        public SpiderTag Tag => _model.Stats.Tag;

        public DamageReceiver(Spider model)
        {
            _model = model;
        }

        public void GetDamage(int value)
        {
            _model.Stats.Health.Decrease(value);
        }
    }
}