using UnityEngine;

namespace MVVM.Extension.Views.Common
{
    public interface IMonoInitializer
    {
        void ResolveDependencies<TComponentType>(TComponentType instance)
            where TComponentType : Component;
    }
}