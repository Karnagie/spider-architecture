using System.Linq;
using Infrastructure.Helpers;
using Infrastructure.Services;

namespace Core.Models.Services
{
    public class SpiderService
    {
        public ItemHolder<Spider> SpiderHolder = new();

        public bool TryFind(SpiderTag tag, out Spider spider)
        {
            var spiders = SpiderHolder.Get();
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