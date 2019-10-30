using System.Threading.Tasks;
using UniRx.Async;
using Utils.Unity.Runtime;

namespace MVVM.Extension.Helpers.UniRx
{
    public static class UniRxAsyncHelper
    {
        public static T RunSynchronous<T>(this UniTask<T> action)
        {
            return Task.Run(async () => await action).RunSynchronous();
        }

        public static void RunSynchronous(this UniTask action)
        {
            Task.Run(async () => await action).RunSynchronous();
        }

    }
}