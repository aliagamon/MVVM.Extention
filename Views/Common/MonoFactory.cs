using DryIocAttributes;
using MVVM.Extension.Views.Components;
using UniRx.Async;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ILogger = MVVM.Core.Services.ILogger;

namespace MVVM.Extension.Views.Common
{
    [ExportMany, SingletonReuse]
    public class MonoFactory : IMonoFactory
    {
        private readonly ILogger _logger;

        public MonoFactory(ILogger logger)
        {
            _logger = logger;
            MonoFactoryProvider.Instance = this;
        }


        public async UniTask<TComponentType> GetInstanceAsync<TComponentType>(string componentId, Transform parent = null)
            where TComponentType : Component
        {
            var instance = await CreateInstanceAsync<TComponentType>(componentId, parent);
            return instance;
        }

        public async UniTask<TComponentType> GetInstanceAsync<TComponentType>(AssetReference reference, Transform parent = null)
            where TComponentType : Component
        {
            var instance = await CreateInstanceAsync<TComponentType>(reference, parent);
            return instance;
        }

        public async UniTask<GameObject> GetInstanceAsync(string componentId, Transform parent = null)
        {
            return await CreateInstanceAsync(componentId, parent);
        }

        public GameObject GetInstance(GameObject prefab, Transform parent = null)
        {
            return Object.Instantiate(prefab, parent, false);
        }

        public async UniTask<GameObject> GetInstanceAsync(AssetReference reference, Transform parent = null)
        {
            var handle = reference.InstantiateAsync(parent, false);
            return await HandleAddressableCreation(handle);
        }

        public void Release(GameObject instance)
        {
            Addressables.ReleaseInstance(instance);
        }

        public TComponentType GetInstance<TComponentType>(TComponentType prefab, Transform parent = null)
            where TComponentType : Component
        {
            return Object.Instantiate(prefab, parent, false);
        }

        private async UniTask<TComponentType> CreateInstanceAsync<TComponentType>(string componentId, Transform parent = null)
            where TComponentType : Component
        {
            var handle = Addressables.InstantiateAsync(componentId, parent, false);
            return await HandleAddressableCreation<TComponentType>(handle);
        }

        private async UniTask<GameObject> CreateInstanceAsync(string componentId, Transform parent = null)
        {
            var handle = Addressables.InstantiateAsync(componentId, parent, false, true);
            return await HandleAddressableCreation(handle);
        }

        private async UniTask<TComponentType> CreateInstanceAsync<TComponentType>(AssetReference reference, Transform parent = null)
            where TComponentType : Component
        {
            var handle = reference.InstantiateAsync(parent, false);
            return await HandleAddressableCreation<TComponentType>(handle);
        }

        private async UniTask<TComponentType> HandleAddressableCreation<TComponentType>(AsyncOperationHandle<GameObject> handle)
            where TComponentType : Component
        {
            var instance = await HandleAddressableCreation(handle);
            return instance != null ? instance.GetComponent<TComponentType>() : null;
        }

        private async UniTask<GameObject> HandleAddressableCreation(AsyncOperationHandle<GameObject> handle)
        {
            await handle.ToUniTask();
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var instance = handle.Result;
//                if (instance.GetComponent<ReleaseAddressableOnDestroy>() == false)
//                    instance.AddComponent<ReleaseAddressableOnDestroy>();
                return instance;
            }
            else
            {
                _logger.LogError(handle.OperationException.ToString());
                return null;
            }
        }
    }
}