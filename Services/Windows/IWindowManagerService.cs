using MVVM.Core.Services;
using UnityEngine;
using System.Threading.Tasks;
using MVVM.Core.ViewModels;
using MVVM.Extension.Views.Common;

namespace MVVM.Extension.Services.Windows
{
    public interface IWindowManagerService
    {
        Task<T> ShowDialogue<T>(ViewModelBase viewModel) where T : Component;
        Task<T> ShowWindow<T>
        (
             ViewModelBase viewModel,
             bool isSingletonReuse = false
        ) where T : ViewBase;
        bool CloseWindow<T>(T window) where T : Component;
        // This one works like CloseWindow but it wont throw exceptions
        // if the window is not currently open.
        void TryCloseWindow<T>(T window) where T : Component;
    }
}
