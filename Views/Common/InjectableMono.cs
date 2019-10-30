using UnityEngine;

namespace MVVM.Extension.Views.Common
{
    public abstract class InjectableMono : MonoBehaviour
    {
        protected virtual void Awake()
        {
            MonoInitializer.Instance.ResolveDependencies(this);
        }
    }
}