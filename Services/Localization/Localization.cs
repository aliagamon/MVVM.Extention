using System.Collections.Generic;
using System.Text.RegularExpressions;
using DryIocAttributes;
using MVVM.Core.Services;
using UniRx.Async;

namespace MVVM.Extension.Services.Localization
{
    public interface ILocalization
    {
        UniTask InitializeAsync(string addressableId);
        string TryGet(string key);
        string Get(string key);
        string Format(string key, Dictionary<string, object> values);
        string Format(string key, object value);
    }

    [ExportMany]
    [SingletonReuse]
    public class Localization : ILocalization
    {
        private readonly ILocalizationDataProvider _dataProvider;
        private readonly Dictionary<string, string> _table = new Dictionary<string, string>();
        private bool _initialized;

        public Localization(ILocalizationDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async UniTask InitializeAsync(string addressableId)
        {
            var entries = await _dataProvider.GetDataAsync(addressableId);
            foreach (var entry in entries)
            {
                if (_table.ContainsKey(entry.Key))
                    Logger.Instance.LogError(
                        $"Localization: duplicate entry for key: \"{entry.Key}\" found, will use last value.");
                _table[entry.Key] = entry.Value;
            }

            _initialized = true;
        }

        public string TryGet(string key)
        {
            if (IsStateValid(key) == false) return null;

            if (_table.TryGetValue(key, out var value) == false)
                return null;

            return value;
        }

        public string Get(string key)
        {
            if (IsStateValid(key) == false) return null;

            if (_table.TryGetValue(key, out var value) == false)
                Logger.Instance.LogError($"No string with key of \"{key}\" found.");

            return value;
        }

        public string Format(string key, Dictionary<string, object> values)
        {
            if (IsStateValid(key) == false) return null;

            if (values == null || values.Count == 0)
            {
                Logger.Instance.LogWarning($"Localization: requested to format with key: \"{key}\" with null value(s)");
                return Get(key);
            }

            var format = Get(key);
            if (format == null)
                return null;

            var result = format;
            foreach (var pair in values) result = Regex.Replace(result, $"{{{pair.Key}}}", pair.Value.ToString());

            return Regex.Replace(result, @"\s*{.*}", "");
        }

        public string Format(string key, object value)
        {
            if (IsStateValid(key) == false) return null;

            if (value == null)
            {
                Logger.Instance.LogWarning($"Localization: requested to format with key: \"{key}\" with null value(s)");
                return Get(key);
            }

            var format = Get(key);
            if (format == null)
                return null;

            return new Regex("{.*}").Replace(format, value.ToString(), 1);
        }

        private bool IsStateValid(string key)
        {
            if (IsInitialized() == false) return false;
            if (IsKeyValid(key) == false) return false;
            return true;
        }

        private static bool IsKeyValid(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                Logger.Instance.LogError("Localization: invalid key: empty or null.");
                return false;
            }

            return true;
        }

        private bool IsInitialized()
        {
            if (_initialized == false)
            {
                Logger.Instance.LogError($"You must call {nameof(InitializeAsync)}() before using localization");
                return false;
            }

            return true;
        }
    }
}
