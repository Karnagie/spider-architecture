using Core.Models.Systems;
using Infrastructure.Services.System;

namespace Core.Models.Services
{
    public class SpiderLegService
    {
        private SystemService _systemService;

        public SpiderLegService(SystemService systemService)
        {
            _systemService = systemService;
        }
        
        public bool IsConnecting(Spider model)
        {
            var linkers = _systemService.LinkersThatHas(model);
            foreach (var linker in linkers)
            {
                if (linker.TryGetSystems(out LegSystem[] legs))
                {
                    if (IsAnyLegConnecting(legs)) return true;
                }
            }

            return false;
        }
        
        private static bool IsAnyLegConnecting(LegSystem[] legs)
        {
            foreach (var leg in legs)
            {
                if (leg.Connecting())
                    return true;
            }

            return false;
        }
    }
}