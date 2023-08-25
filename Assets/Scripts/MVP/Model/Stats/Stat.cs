using UniRx;

namespace MVP.Model
{
    public abstract class Stat<T> : ReactiveProperty<T>
    {
        public abstract void Decrease(T delta);
        public abstract void Increase(T delta);
    }
}