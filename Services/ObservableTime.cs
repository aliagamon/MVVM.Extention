using DryIocAttributes;
using UniRx;

namespace MVVM.Extension.Services
{
    public interface IObservableTime
    {
        ReadOnlyReactiveProperty<long> Time { get; }
    }

    [ExportMany, SingletonReuse]
    public class ObservableTime : IObservableTime
    {
        public ReadOnlyReactiveProperty<long> Time { get; }

        public ObservableTime()
        {
            Time = Observable.EveryUpdate().Select(_ => (long)(UnityEngine.Time.unscaledTime * 1000)).ToReadOnlyReactiveProperty();
        }
    }
}