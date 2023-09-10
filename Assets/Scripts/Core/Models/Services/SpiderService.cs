﻿using System.Linq;
using Infrastructure.Helpers;
using Infrastructure.Services;
using Infrastructure.Services.Binding;

namespace Core.Models.Services
{
    public class SpiderService
    {
        private SystemService _systemService;

        public SpiderService(SystemService systemService)
        {
            _systemService = systemService;
        }
        
        public bool TryFind(SpiderTag tag, out Spider spider)
        {
            var spiders = _systemService.TryFindSystems<Spider>();
            
            var found = spiders.FirstOrDefault((spider1 => spider1.Stats.Tag == tag));
            if (found != default)
            {
                spider = found;
                return true;
            }

            spider = null;
            return false;
        }
    }
}