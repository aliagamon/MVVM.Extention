using UniRx.Async;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MVVM.Extension.Views.Common
{
    public interface IMonoFactory
    {
        UniTask<TComponentType> GetInstanceAsync<TComponentType>(string componentId, Transform parent = null)
            where TComponentType : Component;

        TComponentType GetInstance<TComponentType>(TComponentType prefab, Transform parent = null)
            where TComponentType : Component;

        UniTask<TComponentType> GetInstanceAsync<TComponentType>(AssetReference reference, Transform parent = null)
            where TComponentType : Component;

        UniTask<TObject> LoadAsync<TObject>(string componentId)
            where TObject : Object;
        

        UniTask<GameObject> GetInstanceAsync(string componentId, Transform parent = null);

        GameObject GetInstance(GameObject prefab, Transform parent = null);

        UniTask<GameObject> GetInstanceAsync(AssetReference reference, Transform parent = null);

        void Release(GameObject instance);
    }

    /// <summary>
    /// IMonoFactory implementation should set instance variable of this in construction
    /// </summary>
    public static class MonoFactoryProvider
    {
        public static IMonoFactory Instance { get; set; }
    }
}
