using System.Collections.Generic;
using Assets.CodeBase.Infrastructure;
using Assets.CodeBase.Infrastructure.Services.AdsService;
using Assets.CodeBase.Infrastructure.Services.AudioService;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Infrastructure.Services.SaveLoad;
using Assets.CodeBase.Infrastructure.Services.StaticData;
using Assets.CodeBase.Logic.CameraLogic;
using CodeBase.Infrastructure.Data;
using CodeBase.Infrastructure.Services.Analytics;
using CodeBase.Infrastructure.Services.AudioService;
using CodeBase.UI;
using CodeBase.UI.Services;
using CodeBase.UI.Services.UIFactory;
using UnityEngine;

namespace CodeBase.Infrastructure.States
{
    class GameStopOverState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly IUIFactory _uiFactory;
        private readonly IPersistentProgressService _progress;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly IAudioService _audioService;
        private readonly IAdsService _adsService;
        private readonly IStaticDataService _staticDataService;
        private readonly IPersistentProgressService _progressService;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IAnalyticsService _analyticsService;
        private readonly CameraController _cameraController;
        private const string GameStopOver = "GameStopOverScene";
        private const string LOADING_KEY = "Loading.ToGarage";


        public GameStopOverState(IGameStateMachine gameStateMachine, SceneLoader sceneLoader,
             IUIFactory uiFactory, IPersistentProgressService progress,
            LoadingCurtain loadingCurtain, IAudioService audioService, IAdsService adsService,
            IStaticDataService staticDataService, IPersistentProgressService progressService,
             ISaveLoadService saveLoadService, IAnalyticsService analyticsService, CameraController cameraController)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _uiFactory = uiFactory;
            _progress = progress;
            _loadingCurtain = loadingCurtain;
            _audioService = audioService;
            _adsService = adsService;
            _staticDataService = staticDataService;
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            _analyticsService = analyticsService;
            _cameraController = cameraController;
        }
        public void Exit()
        {
            //_progress.Progress.ObtainModule(_staticDataService.ForMachineModule("drill"), 3);
            
            
            _saveLoadService.SaveProgress();
            string text = "";

            foreach (var info in _progressService.Progress.PlayerMachine.MachineModules)
            {
                var moduleInfo = _staticDataService.ForMachineModule(info.Id).Key + " P.-" +
                                 info.Place + " L." + (info.Level+1).ToString() + "--";
                text = text + moduleInfo;
            }

            _analyticsService.SetProperty("CurrentMachine", text + "--- M.-" +
                                                            _progress.Progress.PlayerMachine.
                                                                MachineBasementId.ToString());
            _analyticsService.InvokeEvent("CraftWindowExit", new Dictionary<string, string>
            {
                {"Modules", text},
            });
        }
        public void Enter()
        {
            if (_progressService.Progress.LevelStage >= 6)
            {
                _progressService.Progress.LevelStage = 5;
            }
            _cameraController.RemoveShade();
            if (!_progressService.Progress.TutorialData.FirstTutorialProgress.FullCompleted)
            {
                _progressService.Progress.TutorialData.FirstTutorialProgress.Reset();
                _gameStateMachine.Enter<MainMenuState>();
                return;
            }
            
            
            _audioService.ClearAmbient();
            _loadingCurtain.Show(LOADING_KEY);
            //_progress.Progress.LevelStage += 1;
            
            // remove tutorial machine
            _progress.Progress.RemoveMachineByBasementId(
                _staticDataService.GetAllLevels().Tutorial.StartingMachine.BasementStaticData.Id);



            if (! _progress.Progress.CheckIfHaveMachineByBasementId(
                    _staticDataService.ForLevel(_progress.Progress.LevelStage)
                        .StartingMachine.BasementStaticData.Id))
            {
                MachineModel model = new MachineModel(
                    _staticDataService.ForLevel(_progress.Progress.LevelStage).StartingMachine);
                _progress.Progress.ObtainMachine(model);
            }
            
            _saveLoadService.SaveProgress();
            _sceneLoader.Load(GameStopOver, OnLoaded);
        }

        private async void OnLoaded()
        {
            
            _loadingCurtain.UpdateLoading(20);
            //_gameStateMachine.Enter<LoadLevelState, int>(_progress.Progress.LevelStage);
            await _uiFactory.CreateUIRoot();
            _loadingCurtain.UpdateLoading(50);
            await _uiFactory.CreateWindow(WindowType.CraftWindow);
            _loadingCurtain.UpdateLoading(100);
            _loadingCurtain.Hide();
        }
    }
}