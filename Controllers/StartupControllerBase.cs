using System.Linq;
using System.Reflection;
using DryIoc;
using MVVM.Core.Services;
using MVVM.Core.ViewModels;
using MVVM.Extension.Views.Common;
using UnityEngine;

namespace MVVM.Extension.Controllers
{
    public class StartupControllerBase : MonoBehaviour
    {
        protected readonly IContainer Container;

        public StartupControllerBase()
        {
            var assemblies = new []
            {
                typeof(ViewModelBase).Assembly,
                typeof(ViewBase).Assembly,
                typeof(Startup).Assembly,
                Assembly.GetExecutingAssembly(),
                Assembly.GetCallingAssembly()
            }.Distinct().ToArray();
            Container = Startup.SetupContainer(assemblies);
        }

        protected INavigationService Navigation
            => Container.Resolve<INavigationService>();

    }
}