using DryIoc;
using DryIocAttributes;
using MVVM.Core.Services;
using UnityEngine;
using ILogger = MVVM.Core.Services.ILogger;

namespace MVVM.Extension.Views.Common
{
    [ExportMany, Reuse(ReuseType.Singleton)]
    public class MonoInitializer : IMonoInitializer
    {
        public static MonoInitializer Instance { get; private set; }

        private readonly IScopeManager _scopeManager;
        private readonly ILogger _logger;

        public MonoInitializer(IScopeManager scopeManager, ILogger logger)
        {
            _logger = logger;
            _scopeManager = scopeManager;
            Instance = this;
        }

        public void ResolveDependencies<TComponentType>(TComponentType instance)
            where TComponentType : Component
        {
            var serviceType = instance.GetType();

            if (_scopeManager.IsRegistered(serviceType))
                _scopeManager.Resolve(serviceType, args: new object[] { instance });
            else
                _logger.LogError($"Request Received for {serviceType} injection, but it's not registered");
        }
    }
}