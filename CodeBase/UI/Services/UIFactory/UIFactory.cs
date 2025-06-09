using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.CodeBase.Infrastructure.AssetManagement;
using Assets.CodeBase.Infrastructure.Services.AdsService;
using Assets.CodeBase.Infrastructure.Services.Pause;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Infrastructure.Services.SaveLoad;
using Assets.CodeBase.Infrastructure.Services.StaticData;
using Assets.CodeBase.Logic.CameraLogic;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Services.Analytics;
using CodeBase.Infrastructure.Services.AudioService;
using CodeBase.Infrastructure.States;
using CodeBase.UI.CutScenes;
using CodeBase.UI.Elements;
using CodeBase.UI.Elements.Buttons;
using CodeBase.UI.Elements.Presenters;
using CodeBase.UI.Elements.Presenters.Machine;
using CodeBase.UI.Elements.Tutorial;
using CodeBase.UI.Services.DeathScreen;
using CodeBase.UI.Services.GameStopOver;
using CodeBase.UI.Services.MainMenu;
using CodeBase.UI.Tutorial;
using Unity.VisualScripting;
using UnityEngine;

namespace CodeBase.UI.Services.UIFactory
{
    class UIFactory : IUIFactory
    {
        private readonly IStaticDataService _staticData;
        private readonly IPersistentProgressService _persistentProgressService;
        private readonly IGameStateMachine _stateMachine;
        private readonly IAudioService _audioService;
        private readonly IAssets _assets;
        private Transform _uiRoot;
        private PauseService _pauseService;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IAdsService _adsService;
        private readonly CameraController _cameraController;
        private readonly IAnalyticsService _analyticsService;

        private Dictionary<WindowType, WindowBase> Windows = new Dictionary<WindowType, WindowBase>();

        public UIFactory(IAssets assets, IStaticDataService staticData,
            IPersistentProgressService persistentProgressService,
            IGameStateMachine stateMachine, IAudioService audioService,
            PauseService pauseService, ISaveLoadService saveLoadService,
            IAdsService adsService, CameraController cameraController,
            IAnalyticsService analyticsService)
        {
            _pauseService = pauseService;
            _saveLoadService = saveLoadService;
            _adsService = adsService;
            _cameraController = cameraController;
            _analyticsService = analyticsService;
            _assets = assets;
            _staticData = staticData;
            _persistentProgressService = persistentProgressService;
            _stateMachine = stateMachine;
            _audioService = audioService;
        }

        public async Task CreateWindow(WindowType type)
        {
            if (_uiRoot == null) await CreateUIRoot();
            
            WindowConfig data = _staticData.ForWindow(type);
            var prefab = await _assets.Load<GameObject>(data.WindowReference);
            WindowBase windowBase = Object.Instantiate(prefab.GetComponent<WindowBase>(), _uiRoot);
            
            if (type == WindowType.CraftWindow)
            {
                GameStopOverWindow window = (GameStopOverWindow) windowBase;
                CreateCraftWindow(window);
            }
            else if (type == WindowType.DeathScreen)
            {
                DeathScreenWindow window = (DeathScreenWindow) windowBase;
                window.Construct(_persistentProgressService, _stateMachine, _analyticsService);
                foreach (var b in window.GetComponentsInChildren<ChangeStateButton>())
                {
                    b.Construct(_stateMachine);
                }


            }
            else if (type == WindowType.MainMenu)
            {
                MainMenuWindow window = (MainMenuWindow) windowBase;
                window.GetComponentInChildren<ActivateTutorial>()?.Construct(_persistentProgressService);
                
                foreach (OpenWindowButton windowButton in window.GetComponentsInChildren<OpenWindowButton>())
                {
                    windowButton.Construct(this);
                }
                
                foreach (var button in window.GetComponentsInChildren<ChooseDifficulty>())
                {
                    button.Construct(_persistentProgressService);
                }
                foreach (ChangeLanguageButton windowButton in window.GetComponentsInChildren<ChangeLanguageButton>())
                {
                    windowButton.Construct(_persistentProgressService, _saveLoadService);
                }
                
                foreach (HideIfNoSave windowButton in window.GetComponentsInChildren<HideIfNoSave>())
                {
                    windowButton.Construct(_persistentProgressService);
                }
            }
            else if (type == WindowType.GamePreparation)
            {
                WindowBase window = windowBase;
                
                CreatePreparationWindow(window);
                
            }
            else if (type == WindowType.Pause
                      || type == WindowType.SettingsWithoutExit
                      || type == WindowType.SettingsWithSave)
            {
                WindowBase window = windowBase;
                
                CreateSettingsWindow(window);
            }else if (type == WindowType.FinishGame)
            {
                FinishGameWindow window = (FinishGameWindow)windowBase;
                window.Construct(_persistentProgressService);
                
            }else if (type == WindowType.ChooseDifficultyWindow)
            {
                foreach (var button in windowBase.GetComponentsInChildren<ChooseDifficulty>())
                {
                    button.Construct(_persistentProgressService);
                }
                foreach (ChangeLanguageButton windowButton in windowBase.GetComponentsInChildren<ChangeLanguageButton>())
                {
                    windowButton.Construct(_persistentProgressService, _saveLoadService);
                }
            }else if (type == WindowType.LevelTransitionCutScene || type == WindowType.StaticMap)
            {
                LevelTransition window = (LevelTransition)windowBase;
                window.Construct(_persistentProgressService, _stateMachine, _cameraController);
                window.GetComponentInChildren<ChangeStateButton>().Construct(_stateMachine);
            }else if (type == WindowType.TutorialDialog1 || type == WindowType.TutorialDialog2
                                                         || type == WindowType.TutorialDialog3
                                                         || type == WindowType.TutorialDialog4)
            {
                TutorialDialog window = (TutorialDialog)windowBase;
                window.Construct(_pauseService, _persistentProgressService);
            }
            
            foreach (ChangeStateButton windowButton in windowBase.GetComponentsInChildren<ChangeStateButton>(true))
            {
                windowButton.Construct(_stateMachine);
            }

            windowBase.Initialize();
            _analyticsService.InvokeEvent("WindowOpened", new Dictionary<string, string>()
            {
                {"WindowType", type.ToString()}
            });

            WindowCloseAnalytic windowCloseAnalytic = windowBase.AddComponent<WindowCloseAnalytic>();
            windowCloseAnalytic.Construct(_analyticsService, type);

            Windows[type] = windowBase;
        }

        private void CreateSettingsWindow(WindowBase window)
        {
            SettingsWindow settingsWindow = (SettingsWindow) window;
            settingsWindow.Construct(_pauseService);
            foreach (var slider in window.GetComponentsInChildren<ChangeVolumeSlider>())
            {
                slider.Construct(_persistentProgressService, _audioService);
            }
            foreach (var slider in window.GetComponentsInChildren<ChangeStateButton>())
            {
                slider.Construct(_stateMachine);
            }
            
            foreach (var button in window.GetComponentsInChildren<OpenWindowButton>())
            {
                button.Construct(this);
            }
        }

        private void CreatePreparationWindow(WindowBase window)
        {
            Debug.LogWarning("Error");
            return;
        }

        private void CreateCraftWindow(GameStopOverWindow window)
        {
            foreach (var tutorial in window.GetComponentsInChildren<ActivateTutorial>())
            {
                tutorial.Construct(_persistentProgressService);
            }
            foreach (var btn in window.GetComponentsInChildren<ChangeMachineButton>())
            {
                btn.Construct(_persistentProgressService);
            }
            

            
            window.Construct(_persistentProgressService, _stateMachine, _staticData);
            window.GetComponentInChildren<PlayerResourcesPresenter>().Construct(_persistentProgressService, _staticData);
            window.GetComponentInChildren<OldShipLabel>().Construct(_persistentProgressService, _staticData);
            window.GetComponentInChildren<SelectedDialog>().Construct(_persistentProgressService, _staticData);
            window.GetComponentInChildren<DarkAmethystPresenter>().Construct(_persistentProgressService);
            window.GetComponentInChildren<ContinueButton>().Construct(_persistentProgressService, _stateMachine, _staticData);
            window.GetComponentInChildren<OpenWindowButton>().Construct(this);
            //window.GetComponentInChildren<MyModulesButtonAnimation>().Construct(
            //    _persistentProgressService);
        }

        public async Task CreateUIRoot()
        {
            GameObject root = await _assets.Instantiate(AssetAddress.UIRoot);
            _uiRoot = root.transform;
        }

        public void Clear()
        {
            Object.DestroyImmediate(_uiRoot.gameObject);
        }

        public bool CheckWindowExistence(WindowType type)
        {
            if (Windows.ContainsKey(type))
            {
                if (Windows[type] != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}