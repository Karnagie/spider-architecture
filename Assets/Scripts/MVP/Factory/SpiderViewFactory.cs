using CompositeDirectorWithGeneratingComposites.CompositeDirector;
using Infrastructure.AssetManagement;
using Infrastructure.Disposables;
using MVP.Binders;
using MVP.View;
using UnityEngine;

namespace MVP.Factory
{
    public class SpiderViewFactory
    {
        private IAssetProvider _assetProvider;
        private IPool<IClearable> _clearablePool;

        public SpiderViewFactory(IAssetProvider assetProvider, IPool<IClearable> clearablePool)
        {
            _clearablePool = clearablePool;
            _assetProvider = assetProvider;
        }
        
        public SpiderView DefalutSpider()
        {
            SpiderBehaviour prefab = _assetProvider.Instantiate<SpiderBehaviour>("Characters/Spider");
            var spiderView = new SpiderView(prefab);
            _clearablePool.Add(spiderView);
            return spiderView;
        }
    }
}