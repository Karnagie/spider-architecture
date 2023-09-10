namespace Core.Models.Stats
{
    public sealed class FloatStat : Stat<float>
    {
        public FloatStat(int value)
        {
            Value = value;
        }

        public override void Decrease(float delta)
        {
            Value -= delta;
        }

        public override void Increase(float delta)
        {
            Value += delta;
        }
    }
}