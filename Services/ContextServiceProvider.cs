using System;
using System.Collections.Generic;
using System.Linq;
using DryIocAttributes;

namespace MVVM.Extension.Services
{
    [ExportMany, TransientReuse]
    public class ContextServiceProvider<TService>
        where TService : class
    {
        private readonly KeyValuePair<object, Lazy<TService>>[] _services;
        private readonly IContextProvider _contextProvider;

        public TService Service => _services.Cast<KeyValuePair<object, Lazy<TService>>?>()
                                       .ToArray().FirstOrDefault(p => p.Value.Key.Equals(_contextProvider.CurrentContext))?.Value.Value
        ?? throw new Exception($"requested service of type: '{typeof(TService).Name}' with key: '{_contextProvider.CurrentContext}' not found.");

        public ContextServiceProvider(KeyValuePair<object, Lazy<TService>>[] services, IContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
            _services = services;
        }
    }

    public interface IContextProvider
        
    {
        object CurrentContext { get; }
    }
}