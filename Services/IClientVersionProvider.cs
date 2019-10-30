using System;
using System.Linq;
using DryIocAttributes;
using UnityEngine;

namespace MVVM.Extension.Services
{
    public interface IClientVersionProvider
    {
        Version Version { get; }
    }

    [ExportMany, SingletonReuse]
    public class UnityClientVersionProvider : IClientVersionProvider
    {
        private Version _version;
        public Version Version
        {
            get
            {
                if (_version == null)
                {
                    var str = string.Join(".", Application.version.Split('.')
                        .Select(int.Parse).Concat(Enumerable.Repeat(0, 4)).Take(4));
                    _version = new Version(str);
                }
                
                return _version;
            }
        }
    }
}