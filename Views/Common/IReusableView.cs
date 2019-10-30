using MVVM.Core.ViewModels;
using UniRx.Async;

namespace MVVM.Extension.Views.Common
{
    public interface IReusableView<T> where T : IViewModel
    {
        void UpdateView(T model);
        UniTask UpdateViewAsync(T model);
        void DiscardView();
        T ViewModel { get; }
    }

    public interface IReusableView : IReusableView<ViewModelBase>
    {
    }
}
