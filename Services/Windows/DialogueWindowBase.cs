using MVVM.Core.Services;
using UniRx.Async;
using UnityEngine;

namespace MVVM.Extension.Services.Windows
{
    public abstract class DialogueWindowBase : MonoBehaviour
    {
        public abstract UniTask<DialogueResult> Show(DialogueType type, string title, string message, string ok, string yes, string no);
    }
}
