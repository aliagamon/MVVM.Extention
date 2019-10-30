using System;
using System.Threading.Tasks;
using DryIocAttributes;
using MVVM.Core.Services;
using MVVM.Extension.Services.Localization;

namespace MVVM.Extension.Services.Windows
{
    [ExportMany, CurrentScopeReuse()]
    public sealed class DialogueService : IDialogueService
    {
        private readonly Lazy<IMonoWindowService> _getWindowService;
        private readonly ILocalization _localization;

        public DialogueService(Lazy<IMonoWindowService> getWindowService, ILocalization localization)
        {
            _localization = localization;
            _getWindowService = getWindowService;
        }

        public async Task<DialogueResult> ShowDialogue(DialogueType type, string tile, string message, string ok = "Ok", string yes = "Yes",
            string no = "No")
        {
            var windowService = _getWindowService.Value;
            var dialogue = await windowService.ShowWindowAsync<DialogueWindowBase>("DialogueWindow", true);
            string LocalizeString(string key, string @default = null) => _localization.TryGet(key) ?? @default ?? key;
            var result = await dialogue.Show(type, LocalizeString(tile), LocalizeString(message), LocalizeString(ok, ok), LocalizeString(yes, yes), LocalizeString(no, no));
            windowService.CloseWindow(dialogue);
            return result;
        }
    }
}
