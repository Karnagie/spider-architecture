using System;
using Core.Models.Systems;
using Infrastructure.Services;
using Infrastructure.Services.Binding;

namespace Core.Models.Services
{
    public class DamageReceiverService : IDisposable
    {
        public readonly ItemHolder<IDamageReceiver> DamageReceiverHolder = new();
        private BinderService _binderService;

        public DamageReceiverService(BinderService binderService)
        {
            _binderService = binderService;
        }

        public bool TryPerform(int damage, params IFilter[] filters)
        {
            var gotDamage = false;
            var receiver  = _binderService.Find<IDamageReceiver>(filters);
            if(receiver != null)
            {
                receiver.GetDamage(damage);
                gotDamage = true;
            }
            
            return gotDamage;
        }

        public void Dispose()
        {
            DamageReceiverHolder.Dispose();
        }
    }
}