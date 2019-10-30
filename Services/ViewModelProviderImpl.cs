using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DryIoc;
using DryIocAttributes;
using MVVM.Core.Services;
using MVVM.Core.ViewModels;
using IfUnresolved = DryIoc.IfUnresolved;

namespace MVVM.Extension.Services
{
    [ExportMany, SingletonReuse]
    public class ViewModelProviderImpl : ViewModelProvider
    {
        private ILogger _logger;
        public ViewModelProviderImpl(IScopeManager scopeManager, ILogger logger)
        {
            _scopeManager = scopeManager;
            _logger = logger;
            RefreshGlobalTypeMap();
        }

        public override List<string> ViewModels
        {
            get
            {
                if (s_ViewModels == null)
                    s_ViewModels = GetViewModels();

                return s_ViewModels;

            }
        }
        private readonly IScopeManager _scopeManager;

        public override List<string> GetViewModels()
        {
            return SGetViewModels();
        }

        public override ViewModelBase GetViewModelBehaviour(string viewModelName)
        {
            return (ViewModelBase) _scopeManager.Resolve(GetViewModelType(viewModelName));
        }

    }
}
