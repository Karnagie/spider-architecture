using UnityEngine;

namespace Infrastructure.AssetManagement
{
    public class AssetProvider : IAssetProvider
    {
        public GameObject Instantiate(string path)
        {
            var prefab = Resources.Load<GameObject>(path);
            var gameObject = Object.Instantiate(prefab);
            return gameObject;
        }
        
        public T Instantiate<T>(string path) where T : Object
        {
            var prefab = Resources.Load<T>(path);
            if(prefab == null)
                Debug.LogError($"prefab {path} is null");
            var gameObject = Object.Instantiate(prefab);
            return gameObject;
        }
    }
}