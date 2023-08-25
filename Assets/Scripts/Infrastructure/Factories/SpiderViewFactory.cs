using CompositeDirectorWithGeneratingComposites.CompositeDirector;
using Core.Behaviours;
using Core.Binders;
using Infrastructure.AssetManagement;
using Infrastructure.Disposables;

namespace Infrastructure.Factories
{
    public class SpiderViewFactory
    {
        private const string DefaultSpider = "Characters/Spider";
        
        private IAssetProvider _assetProvider;
        private IPool<IClearable> _clearablePool;

        public SpiderViewFactory(IAssetProvider assetProvider, IPool<IClearable> clearablePool)
        {
            _clearablePool = clearablePool;
            _assetProvider = assetProvider;
        }
        
        public SpiderView DefalutSpider()
        {
            SpiderBehaviour prefab = _assetProvider.Instantiate<SpiderBehaviour>(DefaultSpider);
            var spiderView = new SpiderView(prefab);
            _clearablePool.Add(spiderView);
            return spiderView;
        }
    }
}