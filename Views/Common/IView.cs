using MVVM.Core.ViewModels;
using UniRx.Async;

namespace MVVM.Extension.Views.Common
{
    public interface IView
    {
        void Show();
        void Hide();
        bool Visibility { get; set; }

        UniTask InitializeAsync(ViewModelBase viewModel);
    }
}
