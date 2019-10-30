using System.Threading.Tasks;
using DryIocAttributes;
using MVVM.Core.Services;
using MVVM.Core.ViewModels;
using MVVM.Extension.Views.Common;
using UniRx.Async;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MVVM.Extension.Services
{
    [ExportMany, SingletonReuse]
    public class ScreenController : InjectableMono, IScreenController
    {
        [SerializeField] private GameObject _loadingOverlayObject;

        public Task HideScreen()
        {
            _loadingOverlayObject.SetActive(true);
            //await SceneManager.LoadSceneAsync(ScreenNames.Loading);
            return Task.CompletedTask;
        }

        public Task UnHideScreen()
        {
            _loadingOverlayObject.SetActive(false);
            return Task.CompletedTask;
        }

        public async Task PrepareScreen(ScreenViewModelBase viewModel)
        {
            await SceneManager.LoadSceneAsync(viewModel.ScreenName);;
            await ScreenViewBase.CurrentScreen.InitializeAsync(viewModel);
            await UniTask.DelayFrame(1);
            _loadingOverlayObject.SetActive(false);
        }

        [ExportMany]
        [AsDecorator]
        public static ScreenController Init(ScreenController instance)
        {
            return instance;
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}