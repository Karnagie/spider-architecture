using CompositeDirectorWithGeneratingComposites.CompositeDirector;
using Core.Behaviours;
using Core.Binders;
using Infrastructure.AssetManagement;

namespace Infrastructure.Factories
{
    public class ViewFactory
    {
        private const string DefaultSpiderPath = "Characters/Spider";
        
        private IAssetProvider _assetProvider;

        public ViewFactory(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }
        
        public SpiderBehaviour DefaultSpider()
        {
            SpiderBehaviour prefab = _assetProvider.Instantiate<SpiderBehaviour>(DefaultSpiderPath);
            return prefab;
        }
    }
}