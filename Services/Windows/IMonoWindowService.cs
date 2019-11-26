using MVVM.Core.Services;
using UniRx.Async;
using UnityEngine;

namespace MVVM.Extension.Services.Windows
{
    public interface IMonoWindowService : IWindowService
    {
        UniTask<TWindow> ShowWindowAsync<TWindow>
        (
            string windowName,
            bool dialogue = false
        ) where TWindow : Component;

        UniTask<TWindow> ShowWindowSingletonAsync<TWindow>(string windowName, bool isDialogue)
            where TWindow : Component;
        void CloseWindow(Component window);
    }
}
