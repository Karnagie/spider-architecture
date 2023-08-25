using UnityEngine;

namespace Infrastructure.AssetManagement
{
    public interface IAssetProvider
    {
        GameObject Instantiate(string path);
        T Instantiate<T>(string path) where T : Object;
    }
}