using Core.Behaviours;
using Infrastructure.AssetManagement;
using UnityEngine;

namespace Infrastructure.Factories
{
    public class ViewFactory
    {
        private const string DefaultSpiderPath = "Characters/Spider";
        private const string DefaultSpiderLegPath = "Characters/SpiderLeg";
        
        private const string WorldPath = "World";
        
        private IAssetProvider _assetProvider;

        public ViewFactory(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }
        
        public SpiderBehaviour DefaultSpider(Vector3 position)
        {
            SpiderBehaviour prefab = _assetProvider.Instantiate<SpiderBehaviour>(DefaultSpiderPath);
            prefab.Transform.position = position;
            
            return prefab;
        }
        
        public SpiderLegBehaviour DefaultSpiderLeg(Transform parent)
        {
            SpiderLegBehaviour prefab = _assetProvider.Instantiate<SpiderLegBehaviour>(DefaultSpiderLegPath);
            prefab.Transform.SetParent(parent);
            prefab.Transform.localPosition = Vector3.zero;
            
            return prefab;
        }

        public WorldBehaviour World()
        {
            return _assetProvider.Instantiate<WorldBehaviour>(WorldPath);
        }
    }
}