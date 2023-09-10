using Infrastructure.Services.Binding;

namespace Core.Models.Systems
{
    public interface IDamager : ISystem
    {
        public void TryDamage();
    }
}