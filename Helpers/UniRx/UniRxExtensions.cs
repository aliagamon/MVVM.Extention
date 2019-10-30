using System;
using UniRx;

namespace MVVM.Extension.Helpers.UniRx
{
    public static class UniRxExtensions
    {
        public static IDisposable BindFrom<T>(this IReactiveProperty<T> target, IObservable<T> source)
        {
            return source.SubscribeWithState(target, (v, p) => p.Value = v);
        }
        
        public static IDisposable BindFrom<TProperty, TObservable>(this IReactiveProperty<TProperty> target, IObservable<TObservable> source, Func<TObservable,TProperty> selector)
        {
            return source.SubscribeWithState(target, (v, p) => p.Value = selector(v));
        }
    }
}