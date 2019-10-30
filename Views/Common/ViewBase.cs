using System;
using System.Threading.Tasks;
using MVVM.Core.Utils;
using MVVM.Core.ViewModels;
using MVVM.Extension.Helpers.UniRx;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Logger = MVVM.Core.Services.Logger;

namespace MVVM.Extension.Views.Common
{
    public abstract class ViewBase : MonoBehaviour, IView, IReusableView
    {
        public ReactiveProperty<bool> Initialized { get; } = new
            ReactiveProperty<bool>();
        public ViewModelBase ViewModel { get; private set; }
        protected CompositeDisposable Disposables { get; } = new
            CompositeDisposable();

        public void Show()
        {
            Visibility = true;
        }

        public void Hide()
        {
            Visibility = false;
        }

        public virtual bool Visibility
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        public async UniTask InitializeAsync(ViewModelBase viewModel)
        {
            ViewModel = viewModel;
            ViewModel.AddTo(Disposables);
            Initialized.AddTo(Disposables);
            await OnInitializeAsync();
            Initialized.Value = true;
        }

        public void Initialize(ViewModelBase viewModel)
        {
            InitializeAsync(viewModel)
                .Forget(e => Logger.Instance?.LogError(e.ToString()));
        }

        public void SelfDestroy()
        {
            Destroy(gameObject);
        }

        protected virtual UniTask OnInitializeAsync()
        {
            return UniTask.CompletedTask;
        }

        protected virtual void OnDestroy()
        {
            CleanupView(true);
        }

        public void CleanupView(bool destroy)
        {
            if (Disposables != null && Disposables.Count > 0)
            {
                if (destroy)
                    Disposables.Dispose();
                else
                    Disposables.Clear();
            }
            ViewModel?.Dispose();
            ViewModel = null;
            Initialized.Value = false;
            OnCleanupView();
        }

        protected virtual void OnCleanupView()
        {
        }
        #region IReusableView Implementations
        public void UpdateView(ViewModelBase model) =>
            UpdateViewAsync(model)
            .Forget(e => Logger.Instance?.LogError(e.ToString()));

        public async UniTask UpdateViewAsync(ViewModelBase model)
        {
            if (Initialized.Value)
                DiscardView();
            await InitializeAsync(model);
            UpdateItem();
        }

        protected virtual void UpdateItem()
        {
        }

        public void DiscardView()
        {
            CleanupView(false);
            OnDiscardView();
        }

        protected virtual void OnDiscardView()
        {
        }
        #endregion
    }

    public abstract class ViewBase<TViewModel> : ViewBase
        where TViewModel : ViewModelBase
    {
        protected new TViewModel ViewModel => (TViewModel)base.ViewModel;

        protected override UniTask OnInitializeAsync()
        {
            base.ViewModel.AssertIsOfType(typeof(TViewModel));
            return base.OnInitializeAsync();
        }
    }

    public abstract class InjectableViewBase<TViewModel> : ViewBase<TViewModel>
        where TViewModel : ViewModelBase
    {
        protected virtual void Awake()
        {
            MonoInitializer.Instance.ResolveDependencies(this);
        }
    }
}
