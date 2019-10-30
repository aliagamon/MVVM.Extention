using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DryIocAttributes;
using Newtonsoft.Json;
using UniRx.Async;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MVVM.Extension.Services.Localization
{
    public interface ILocalizationDataProvider
    {
        UniTask<IList<LocalizationEntry>> GetDataAsync(string addressableId);
    }

    //[ExportMany, SingletonReuse]
    public class JsonLocalizationDataProvider : ILocalizationDataProvider
    {
        public async UniTask<IList<LocalizationEntry>>
            GetDataAsync(string addressableId)
        {
            var data = await AddressablesHelper
                .LoadAssetAsync<TextAsset>(addressableId);
            return JsonConvert
                .DeserializeObject<IList<LocalizationEntry>>(data.text);
        }
    }

    [ExportMany, SingletonReuse]
    public class CsvLocalizationDataProvider : ILocalizationDataProvider
    {
        public async UniTask<IList<LocalizationEntry>>
            GetDataAsync(string addressableId)
        {
            var data = await AddressablesHelper
                .LoadAssetAsync<TextAsset>(addressableId);
            return data.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line =>
                {
                    var split = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
                    return new LocalizationEntry() { Key = split[0], Value = split[1].Replace("\\n", "\n") };
                }).ToList();
        }
    }

    public class LocalizationEntry
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
