using System.Collections.Generic;
using System.Reflection;
using DryIoc;
using DryIoc.MefAttributedModel;
using MVVM.Core.Services;
using MVVM.Extension.Services;
using MVVM.Extension.Views.Common;

namespace MVVM.Extension.Controllers
{
    public class Startup
    {
        public static IContainer SetupContainer(IEnumerable<Assembly> assemblies = null)
        {
            var asmbs = assemblies ?? new [] {typeof(Startup).Assembly};
            var container = new Container(rules => rules
                    .WithTrackingDisposableTransients()
                    .WithoutFastExpressionCompiler()
                    )
                .WithMef();
            container.RegisterExports(asmbs);
            container.RegisterMany(made:Made.Of(() => ScreenViewBase.CurrentScreen), Reuse.Transient);
            container.Resolve<IMonoInitializer>();
            container.Resolve<IMonoFactory>();
            container.Resolve<IViewModelProvider>();
            return container;
        }
    }
}
