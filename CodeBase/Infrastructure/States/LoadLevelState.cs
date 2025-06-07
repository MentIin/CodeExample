using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.CodeBase.Infrastructure;
using Assets.CodeBase.Infrastructure.Data.Loot;
using Assets.CodeBase.Infrastructure.Data.Modules;
using Assets.CodeBase.Infrastructure.Factory;
using Assets.CodeBase.Infrastructure.Services.AdsService;
using Assets.CodeBase.Infrastructure.Services.Input;
using Assets.CodeBase.Infrastructure.Services.Pause;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Infrastructure.Services.Random;
using Assets.CodeBase.Infrastructure.Services.StaticData;
using Assets.CodeBase.Logic.CameraLogic;
using Assets.CodeBase.Logic.DynamicDataLogic;
using Assets.CodeBase.StaticData;
using Assets.CodeBase.StaticData.Level;
using CodeBase.DebugLogic;
using CodeBase.Infrastructure.Data;
using CodeBase.Infrastructure.Data.Modules;
using CodeBase.Infrastructure.Services.Analytics;
using CodeBase.Logic;
using CodeBase.Logic.Machine;
using CodeBase.UI;
using UnityEngine;

namespace CodeBase.Infrastructure.States
{
    public class LoadLevelState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly IGameFactory _gameFactory;
        private readonly IStaticDataService _staticDataService;
        private readonly IRandomService _randomService;
        private readonly IPersistentProgressService _progressService;
        private readonly LoadingCurtain _loadingCurtain;
        private int _levelId;
        private CameraController _cameraController;
        private readonly IAdsService _adsService;
        private readonly IAnalyticsService _analyticsService;
        private readonly PauseService _pauseService;

        private const string GameSceneName = "GameScene";

        public LoadLevelState(IGameStateMachine gameStateMachine, SceneLoader sceneLoader, IGameFactory gameFactory,
            IStaticDataService staticDataService, IRandomService randomService,
            IPersistentProgressService progressService,
             LoadingCurtain loadingCurtain,
            CameraController cameraController, IAdsService adsService,
            IAnalyticsService analyticsService, PauseService pauseService)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _gameFactory = gameFactory;
            _staticDataService = staticDataService;
            _randomService = randomService;
            _progressService = progressService;
            _loadingCurtain = loadingCurtain;
            _cameraController = cameraController;
            _adsService = adsService;
            _analyticsService = analyticsService;
            _pauseService = pauseService;
        }

        public void Enter()
        {
            if (_progressService.Progress.LevelStage >= 6)
            {
                _progressService.Progress.LevelStage = 5;
            }
            _pauseService.Pause();
            if (_progressService.Progress.TutorialData.FirstTutorialProgress.FullCompleted)
            {
                _progressService.Progress.RemoveMachineByBasementId(
                    _staticDataService.GetAllLevels().Tutorial.StartingMachine.Id);
                // TODO this is copy-paste from GameStopOverState
                if ( ! _progressService.Progress.CheckIfHaveMachineByBasementId(
                        _staticDataService.ForLevel(_progressService.Progress.LevelStage)
                            .StartingMachine.BasementStaticData.Id))
                {
                    MachineModel model = new MachineModel(_staticDataService.ForLevel(
                        _progressService.Progress.LevelStage).StartingMachine);
                    _progressService.Progress.ObtainMachine(model);
                }
            }
            else
            {
                _progressService.Progress.TutorialData.FirstTutorialProgress.Reset();
                MachineModel model = new MachineModel(_staticDataService.GetAllLevels().Tutorial.StartingMachine);
                _progressService.Progress.ObtainMachine(model);
            }
            
            _analyticsService.InvokeEvent("LoadLevel", new Dictionary<string, string>
            {
                {"Stage", _progressService.Progress.LevelStage.ToString()},
            });
            
            
            if (DebugSinglton.Instance.InfinityResources)
            {
                _progressService.Progress.CollectResource(new LootData
                {
                    Type = LootType.DarkAmethyst,
                    Amount = 1000
                });
            }
            _gameFactory.CleanUp();
            _progressService.Progress.AmethistsMined = 0;
            
            _adsService.ShowInitial();
            
            _cameraController.SetScaleToDefault();
            _cameraController.RemoveShade();
            
            _loadingCurtain.Show("Loading.ToFly");
            _levelId = _progressService.Progress.LevelStage;

            System.Random random = new System.Random();
            _randomService.SetSeed(random.Next(0, 100));
            
            _gameFactory.WarmUp();
            _loadingCurtain.UpdateLoading(20);
            _sceneLoader.Load(GameSceneName, OnLoaded);
        }

        public void Exit()
        {
            _pauseService.Resume();
        }

        private async void OnLoaded()
        {
            
            _loadingCurtain.UpdateLoading(25);
            await GenerateMap(_levelId);
            _gameStateMachine.Enter<GameLoopState>();
        }
        

        private async Task GenerateMap(int levelId)
        {
            
            LevelStaticData levelStaticData = _staticDataService.ForLevel(levelId);
            if (!_progressService.Progress.TutorialData.FirstTutorialProgress.FullCompleted)
            {
                levelStaticData = _staticDataService.GetAllLevels().Tutorial;
            }
            
            MachineBasementStaticData basement = _staticDataService.ForMachineBasement(
                _progressService.Progress.PlayerMachine.MachineBasementId);
            
            await _gameFactory.InitMap();
            _loadingCurtain.UpdateLoading(30);
            
            
            var modules = GetModulePresentableInfos();

            MachineBase machineBase = await _gameFactory.CreatePlayerMachine(
                (Vector2)levelStaticData.PlayerStartingTilePosition * GameConstants.MapBlockSize,
                basement, modules, _progressService.Progress.PlayerMachine.Upgrades);

            machineBase.GetComponent<Death>().Happen += OnPlayerDeath;
            
            _loadingCurtain.UpdateLoading(50);
            
            await _gameFactory.GenerateLevel(levelStaticData, _randomService.GetSeed());

            _loadingCurtain.UpdateLoading(99);

            CameraFollow(machineBase.transform);
            
            await _gameFactory.CreateHud(levelStaticData);
            _loadingCurtain.UpdateLoading(100);
            _loadingCurtain.Hide();
        }

        private void OnPlayerDeath()
        {
            _gameStateMachine.Enter<DeathScreenState>();
        }

        private ModulePresentableInfo[] GetModulePresentableInfos()
        {
            List<ModuleSerializableInfo> serializableInfos = _progressService.Progress.PlayerMachine.MachineModules;
            int modulesLength = serializableInfos.Count;
            ModulePresentableInfo[] modules = new ModulePresentableInfo[modulesLength];

            for (int i = 0; i < modulesLength; i++)
            {
                ModulePresentableInfo presentableInfo = new ModulePresentableInfo();
                presentableInfo.Place = serializableInfos[i].Place;
                presentableInfo.Level = serializableInfos[i].Level;
                
                presentableInfo.ModuleStaticData = _staticDataService.ForMachineModule(serializableInfos[i].Id);
                modules[i] = presentableInfo;
            }

            return modules;
        }

        private void CameraFollow(Transform target)
        {
            CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
            cameraFollow.Follow(target);
            cameraFollow.TeleportToTarget();
        }
    }
}