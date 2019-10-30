using MVVM.Core.Services;
using DryIocAttributes;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using MVVM.Extension.Views.Common;
using MVVM.Core.ViewModels;
using Utils.Collections.Generic;
using Logger = MVVM.Core.Services.Logger;

namespace MVVM.Extension.Services.Windows
{
    [ExportMany, CurrentScopeReuse()]
    public class WindowManagerService : IWindowManagerService
    {
        private readonly IScopeManager _scopeManager;
        private readonly Lazy<IMonoWindowService> _windowService;

        public WindowManagerService(IScopeManager scopeManager, Lazy<IMonoWindowService> windowService)
        {
            _scopeManager = scopeManager;
            _windowService = windowService;
        }

        // private readonly Stack<Component> _windowStack = new Stack<Component>();
        private readonly List<Component> _windowStack = new List<Component>();

        private Component _dialogue;

        public async Task<T> ShowDialogue<T>(ViewModelBase viewModel)
            where T : Component
        {
            if (!(_dialogue is null)) throw new Exception("Cant push any more window when there is a dialouge displaying");
            var window = await GetDialogue<T>(true, false);
            PushWindow(window);
            var view = window.GetComponent<IView>();
            await view.InitializeAsync(viewModel);
            Show(window);
            _dialogue = window;
            return window;
        }

        public async Task<T> ShowWindow<T>
        (
             ViewModelBase viewModel,
             bool isSingletonReuse = false
        )
            where T : ViewBase
        {
            if (!(_dialogue is null)) throw new Exception("Cant push any more window when there is a dialouge displaying");
            var window = await GetDialogue<T>(false, isSingletonReuse);
            PushWindow(window);
            var view = window.GetComponent<ViewBase>();
            if(isSingletonReuse)
                await view.UpdateViewAsync(viewModel);
            else
                await view.InitializeAsync(viewModel);
            Show(window);
            return window;
        }

        private void Show(Component window)
        {
            window.gameObject.SetActive(true);
        }

        private void PushWindow<T>(T window) where T : Component
        {
            _windowStack.Push(window);
        }

        private Component PopWindow()
        {
            if (_windowStack.Count >= 0)
                return _windowStack.Pop();
            return null;
        }

        private async Task<T> GetDialogue<T>
        (
            bool isDialogue,
            bool isSingleton
        )
            where T : Component
        {
            var windowService = _windowService.Value;
            var window = await
                (
                    isSingleton ?
                         windowService.ShowWindowSingletonAsync<T>(typeof(T).Name)
                         : windowService.ShowWindowAsync<T>(typeof(T).Name)
                );
            return window;
        }

        public bool CloseWindowAndOverlays<T>(T window) where T : Component
        {
            if (!_windowStack.Contains(window))
                throw new ArgumentOutOfRangeException("");
            Component it;
            while (!((it = PopWindow()) is null))
            {
                if (!CloseWindowItem(it))
                    throw new Exception("failed to close some window");
                if (it == window)
                    return true;
            }
            return false;
        }

        public bool CloseWindow<T>(T window) where T : Component
        {
            if (!_windowStack.Contains(window))
                throw new ArgumentOutOfRangeException("");
            var x = _windowStack.IndexOf(window);
            if (CloseWindowItem(_windowStack[x]))
            {
                _windowStack.RemoveAt(x);
                return true;
            }
            return false;
        }

        public void TryCloseWindow<T>(T window) where T : Component
        {
            if (_windowStack.Contains(window))
                CloseWindow(window);
        }

        private bool CloseWindowItem<T>(T window) where T : Component
        {
            var windowService = _windowService.Value;
            windowService.CloseWindow(window);
            return true;
        }
    }
}
