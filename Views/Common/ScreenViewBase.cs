using System;
using System.Collections.Generic;
using System.Linq;
using MVVM.Core.Utils;
using MVVM.Core.ViewModels;
using MVVM.Extension.Services;
using MVVM.Extension.Services.Windows;
using UniRx.Async;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using Logger = MVVM.Core.Services.Logger;

namespace MVVM.Extension.Views.Common
{
    [Serializable]
    public class BoolUnityEvent : UnityEvent<bool>
    {

    }

    [RequireComponent(typeof(Transform))]
    public abstract class ScreenViewBase : ViewBase, IMonoWindowService
    {
        private readonly Dictionary<string, Component> _openWindowsNameToComponents = new Dictionary<string, Component>();
        private readonly Dictionary<Component, string> _openWindowsComponentsToNames = new Dictionary<Component, string>();

        private Transform _root;
        [SerializeField] private Transform _contentRoot;
        [SerializeField] private Transform _windowRoot;
        [SerializeField] private Transform _dialogueRoot;
        public static ScreenViewBase CurrentScreen { get; private set; }

        [SerializeField] private BoolUnityEvent _anyWindowOpenState;

        public void CloseWindow(string windowName)
        {
            if (string.IsNullOrWhiteSpace(windowName)) return;
            if (_openWindowsNameToComponents.TryGetValue(windowName, out var window))
            {
                CloseWindow(window);
                _openWindowsNameToComponents.Remove(windowName);
                _openWindowsComponentsToNames.Remove(window);
                if (_openWindowsNameToComponents.Count == 0)
                    _anyWindowOpenState.Invoke(false);
            }
            else
            {
                Logger.Instance.LogWarning($"WindowService:: no window with name of {windowName} found to close");
            }
        }

        public async void ShowWindow(string windowName)
        {
            await ShowWindowAsync<Component>(windowName);
        }

        public void CloseWindow(Component window)
        {
            if (window == null) return;
            if (_openWindowsComponentsToNames.TryGetValue(window, out var windowName))
            {
                _openWindowsNameToComponents.Remove(windowName);
                _openWindowsComponentsToNames.Remove(window);
                if (_openWindowsNameToComponents.Count == 0)
                    _anyWindowOpenState.Invoke(false);
            }
            else
            {
                Logger.Instance.Log($"WindowService:: no window with type of {window.GetType().Name} found, but will close");
            }
            if(IsSingleton(window.gameObject) && window is ViewBase vb)
                vb.DiscardView();
            ReleaseWindow(window.gameObject);
        }

        public async UniTask<TWindow> ShowWindowAsync<TWindow>(string windowName, bool dialogue = false)
            where TWindow : Component
        {
            var window = await MakeWindow<TWindow>
                (
                    windowName,
                    dialogue ? _dialogueRoot : _windowRoot
                );
            TrackOpenedWindow(windowName, window);
            return window;
        }

        private const string SingletonMark = "singleton";
        private bool IsSingleton(GameObject window) =>
            !window.gameObject.name.StartsWith(SingletonMark);

        public async UniTask<TWindow> ShowWindowSingletonAsync<TWindow>
            (string windowName, bool isDialogue) where TWindow : Component
        {
            var window = FindObjectOfType<TWindow>()
                ?? await MakeWindow<TWindow>
                (
                    windowName,
                    isDialogue ? _dialogueRoot : _windowRoot
                );
            if (IsSingleton(window.gameObject))
                window.gameObject.name = SingletonMark +
                                            window.gameObject.name;
            return window;
        }

        private async UniTask<TWindow> MakeWindow<TWindow>
        (
            string windowName,
            Transform parent
        )
            where TWindow : Component
        {
            return await MonoFactoryProvider.Instance
                .GetInstanceAsync<TWindow>
                (
                    PrefixWindowName(windowName),
                    parent
                );
        }

        private void ReleaseWindow(GameObject window)
        {
            // If not singleton we dont need it no more so just release it.
            if (!IsSingleton(window))
                MonoFactoryProvider.Instance.Release(window.gameObject);
            else
                window.gameObject.SetActive(false);
        }

        private void TrackOpenedWindow<TWindow>(string windowName, TWindow window) where TWindow : Component
        {
            if (_openWindowsNameToComponents.Count == 0)
                _anyWindowOpenState.Invoke(true);
            _openWindowsNameToComponents[windowName] = window;
            _openWindowsComponentsToNames[window] = windowName;
        }

        public async UniTask ShowViewAsync(string windowName, ViewModelBase viewModel, bool dialogue = false)
        {
            var window = await MakeWindow<Transform>
                (
                    windowName,
                    dialogue ? _dialogueRoot : _windowRoot
                );
            var view = window.GetComponent<IView>();
            TrackOpenedWindow(windowName, (Component)view);
            await view.InitializeAsync(viewModel);
        }


        protected virtual void Awake()
        {
            CurrentScreen = this;
            _root = transform;
            if (_root is null)
                throw new NullReferenceException();
            if ((!(_windowRoot is null) || !(_dialogueRoot is null)) && _contentRoot is null)
                throw new NullReferenceException("if you want to use window and dialogue roots you also need to define your content root");
            // filling default transforms for where nothing is defiend and we dont use this feature
            _contentRoot = _contentRoot ?? _root;
            _windowRoot = _windowRoot ?? _root;
            _dialogueRoot = _dialogueRoot ?? _root;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CurrentScreen = null;
        }

        private static string PrefixWindowName(string windowName)
        {
            return "Window/" + windowName;
        }

        private async UniTask<TComponent> CreateWindow<TComponent>(string windowName)
            where TComponent : Component
        {
            return await MakeWindow<TComponent>(windowName, _windowRoot);
        }
    }

    public abstract class ScreenViewBase<TViewModel> : ScreenViewBase
        where TViewModel : ScreenViewModelBase
    {
        protected new TViewModel ViewModel => (TViewModel)base.ViewModel;

        protected override UniTask OnInitializeAsync()
        {
            base.ViewModel.AssertIsOfType(typeof(TViewModel));
            return base.OnInitializeAsync();
        }
    }

    public abstract class InjectableScreenViewBase<TViewModel> : ScreenViewBase<TViewModel>
        where TViewModel : ScreenViewModelBase
    {
        protected override void Awake()
        {
            base.Awake();
            MonoInitializer.Instance.ResolveDependencies(this);
        }
    }
}
