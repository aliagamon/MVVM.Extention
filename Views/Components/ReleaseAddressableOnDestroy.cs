using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MVVM.Extension.Views.Components
{
    public class ReleaseAddressableOnDestroy : MonoBehaviour
    {
        private Object _instance = null;

        public void Init(Object instance)
        {
            _instance = instance;
        }
        
        private void OnDestroy()
        {
            if (_instance == null && gameObject == null) return;
            Addressables.Release(_instance != null ? _instance : this.gameObject);
        }
    }
}