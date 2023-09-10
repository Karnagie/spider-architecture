using Core.Models.Systems;
using Infrastructure.Services.Binding;
using Infrastructure.Services.System;

namespace Core.Models.Services
{
    public class DamageReceiverService
    {
        private SystemService _systemService;

        public DamageReceiverService(SystemService systemService)
        {
            _systemService = systemService;
        }

        public IDamageReceiver[] TryPerform(int damage, params IFilter[] filters)
        {
            var receivers = _systemService.TryFindSystems<IDamageReceiver>(filters);
            foreach (var damageReceiver in receivers)
            {
                damageReceiver.GetDamage(damage);
            }
            
            return receivers;
        }
    }
}