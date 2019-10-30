using UniRx.Async;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MVVM.Extension.Services
{
    public class AddressablesHelper
    {
        public static async UniTask<T> LoadAssetAsync<T>(string id)
        {
            var handle = Addressables.LoadAssetAsync<T>(id);
            await handle.ToUniTask();
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
            else
            {
                Debug.LogException(handle.OperationException);
                return default(T);
            }
        }
        
        public static async UniTask<T> LoadAssetAsync<T>(AssetReference reference)
        {
            var handle = reference.LoadAssetAsync<T>();
            await handle.ToUniTask();
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
            else
            {
                Debug.LogException(handle.OperationException);
                return default(T);
            }
        }
        
        public static async UniTask<T> LoadAssetAsync<T>(AssetReferenceT<T> reference)
        {
            var handle = reference.LoadAssetAsync();
            await handle.ToUniTask();
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
            else
            {
                Debug.LogException(handle.OperationException);
                return default(T);
            }
        }
    }
}