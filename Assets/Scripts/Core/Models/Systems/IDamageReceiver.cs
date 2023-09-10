using Infrastructure.Services.Binding;

namespace Core.Models.Systems
{
    public interface IDamageReceiver : ISystem
    {
        void GetDamage(int value);
    }
}