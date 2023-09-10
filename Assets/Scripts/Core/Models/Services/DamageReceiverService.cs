using System;
using Core.Models.Systems;
using Infrastructure.Services;
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

        public bool TryPerform(int damage, params IFilter[] filters)
        {
            var gotDamage = false;
            var receiver = _systemService.TryFindSystems<IDamageReceiver>(filters);
            foreach (var damageReceiver in receiver)
            {
                damageReceiver.GetDamage(damage);
                gotDamage = true;
            }
            
            return gotDamage;
        }
    }
}