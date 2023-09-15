using Infrastructure.Services.System;
using Zenject;

namespace Core.Models.Services
{
    public class DeathService : ITickable
    {
        private SystemService _systemService;

        public DeathService(SystemService systemService)
        {
            _systemService = systemService;
        }
        
        public void Tick()
        {
            var models = _systemService.TryFindSystems<ISpider>();//change spider to ialive with health
            foreach (var model in models)
            {
                if (model.Stats.Health.Value <= 0)
                {
                    model.Kill();
                }
            }
        }
    }
}