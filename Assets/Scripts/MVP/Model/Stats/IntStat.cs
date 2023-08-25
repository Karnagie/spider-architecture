namespace MVP.Model
{
    public sealed class IntStat : Stat<int>
    {
        public IntStat(int value)
        {
            Value = value;
        }

        public override void Decrease(int delta)
        {
            Value -= delta;
        }

        public override void Increase(int delta)
        {
            Value += delta;
        }
    }
}