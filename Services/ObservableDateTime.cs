using System;
using DryIocAttributes;
using UniRx;

namespace MVVM.Extension.Services
{
    public interface IObservableDateTime
    {
        ReadOnlyReactiveProperty<DateTime> Now { get; }
    }
    
    [ExportMany, SingletonReuse]
    public class ObservableDateTime : IObservableDateTime
    {
        public ReadOnlyReactiveProperty<DateTime> Now { get; }

        public ObservableDateTime()
        {
            Now = Observable.EveryUpdate().Select(_ => DateTime.UtcNow).ToReadOnlyReactiveProperty();
        }
    }
}