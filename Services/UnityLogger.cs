using DryIocAttributes;
using UnityEngine;
using System;

namespace MVVM.Extension.Services
{
    [ExportMany, SingletonReuse]
    public class UnityLogger : MVVM.Core.Services.Logger
    {
        public override void Log(string message) =>
            Debug.Log(message);

        public override void LogWarning(string message) =>
            Debug.LogWarning(message);

        public override void LogError(string message) =>
            Debug.LogError(message);

        public override void Assert(bool condition, string message = null)
        {
            if(message is null)
                Debug.Assert(condition);
            else Debug.Assert(condition, message);
        }

        public override void LogException(Exception exception) =>
            Debug.LogException(exception);
    }
}
