// using Infrastructure.AssetManagement;
// using Player;
// using UnityEngine;
//
// namespace Infrastructure
// {
//     public class GameFactory : IGameFactory
//     {
//         private IAssetProvider _assets;
//         private SpiderFactory _spiderFactory;
//
//         public GameFactory(IAssetProvider assets, SpiderFactory spiderFactory)
//         {
//             _spiderFactory = spiderFactory;
//             _assets = assets;
//         }
//
//         public Spider CreateSpider()
//         {
//             var gameObject = _assets.Instantiate(AssetPath.SpiderPath);
//             return _spiderFactory.Get(gameObject);
//         }
//     }
// }