using System;
using System.Collections.Generic;
using Assets.CodeBase.Infrastructure;
using Assets.CodeBase.Infrastructure.Factory;
using Assets.CodeBase.Infrastructure.Services;
using Assets.CodeBase.Infrastructure.Services.AdsService;
using Assets.CodeBase.Infrastructure.Services.GetUserInfoService;
using Assets.CodeBase.Infrastructure.Services.Pause;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Infrastructure.Services.Random;
using Assets.CodeBase.Infrastructure.Services.SaveLoad;
using Assets.CodeBase.Infrastructure.Services.StaticData;
using Assets.CodeBase.Logic.CameraLogic;
using CodeBase.Infrastructure.Services.Analytics;
using CodeBase.Infrastructure.Services.AudioService;
using CodeBase.UI;
using CodeBase.UI.Services.UIFactory;
using UnityEngine;
using UnityEngine.Audio;

namespace CodeBase.Infrastructure.States
{
    public class GameStateMachine : BaseStateMachine, IGameStateMachine
    {
        private readonly SceneLoader _sceneLoader;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly AllServices _allServices;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly AudioSource _audioSource;
        private readonly CameraController _cameraController;
        private readonly AudioMixerGroup _musicMixerGroup;
        private readonly AudioMixerGroup _soundsMixerGroup;

        public GameStateMachine(SceneLoader sceneLoader, ICoroutineRunner coroutineRunner, AllServices allServices,
            LoadingCurtain loadingCurtain, AudioSource audioSource, CameraController cameraController,
            AudioMixerGroup musicMixerGroup, AudioMixerGroup soundsMixerGroup)
        {
            _sceneLoader = sceneLoader;
            _coroutineRunner = coroutineRunner;
            _allServices = allServices;
            _loadingCurtain = loadingCurtain;
            _audioSource = audioSource;
            _cameraController = cameraController;
            _musicMixerGroup = musicMixerGroup;
            _soundsMixerGroup = soundsMixerGroup;


            new BootstrapState(this, sceneLoader, allServices, loadingCurtain, audioSource,
                coroutineRunner, cameraController, musicMixerGroup, soundsMixerGroup);
            
        }

        public void InitializeStates(BootstrapState bootstrapState)
        {
            _states = new Dictionary<Type, IExitableState>()
            {
                [typeof(BootstrapState)] = bootstrapState
            };
            _states[typeof(BootstrapState)] = bootstrapState;
            _states[typeof(GameLoopState)] = new GameLoopState(this, _sceneLoader, _coroutineRunner,
                _allServices.Single<IGameFactory>(),
                _allServices.Single<IAudioService>(), _allServices.Single<IAdsService>(), _allServices.Single<ISaveLoadService>());

            _states[typeof(LoadLevelState)] = new LoadLevelState(this, _sceneLoader, _allServices.Single<IGameFactory>(),
                _allServices.Single<IStaticDataService>(), _allServices.Single<IRandomService>(),
                _allServices.Single<IPersistentProgressService>(),
                 _loadingCurtain, _cameraController, _allServices.Single<IAdsService>(),
                _allServices.Single<IAnalyticsService>(), _allServices.Single<PauseService>());

            _states[typeof(GameStopOverState)] = new GameStopOverState(this, _sceneLoader, 
                _allServices.Single<IUIFactory>(), _allServices.Single<IPersistentProgressService>(),
                _loadingCurtain, _allServices.Single<IAudioService>(), _allServices.Single<IAdsService>(),
                _allServices.Single<IStaticDataService>(), 
                _allServices.Single<IPersistentProgressService>(),
                _allServices.Single<ISaveLoadService>(), _allServices.Single<IAnalyticsService>(), 
                _cameraController);

            _states[typeof(LoadProgressState)] = new LoadProgressState(this,
                _allServices.Single<IPersistentProgressService>(),
                _allServices.Single<ISaveLoadService>(), _allServices.Single<IStaticDataService>(),
                 _allServices.Single<IUserInfoService>(), _allServices.Single<IAnalyticsService>(),
                _allServices.Single<PauseService>(), _coroutineRunner);
            _states[typeof(MainMenuState)] = new MainMenuState(this, _allServices.Single<IPersistentProgressService>(),
                _allServices.Single<IUIFactory>(), _loadingCurtain, _sceneLoader);

            _states[typeof(SetUpNewGameState)] = new SetUpNewGameState(this,
                _allServices.Single<IPersistentProgressService>(), _allServices.Single<IUIFactory>(),
                _allServices.Single<IRandomService>(), _allServices.Single<ISaveLoadService>(), _coroutineRunner);
            
            _states[typeof(DeathScreenState)] = new DeathScreenState(this, _allServices.Single<IUIFactory>(),
                _allServices.Single<IAudioService>(), _allServices.Single<IGameFactory>(),
                _allServices.Single<ISaveLoadService>());

            _states[typeof(FinishGameState)] = new FinishGameState(_allServices.Single<IUIFactory>(),
                _sceneLoader, _allServices.Single<ISaveLoadService>(), _allServices.Single<IPersistentProgressService>(),
                _cameraController);
            
            _states[typeof(GoToMainMenuFromGameState)] = new GoToMainMenuFromGameState(this,
                _allServices.Single<IGameFactory>(),
                _allServices.Single<IUIFactory>(), _allServices.Single<IAudioService>(),
                _allServices.Single<IPersistentProgressService>(),
                _allServices.Single<ISaveLoadService>());
            
                _states[typeof(LevelTransitionState)] = new LevelTransitionState(_allServices.Single<IUIFactory>(),
                    _allServices.Single<IPersistentProgressService>(), this,
                    _cameraController,_sceneLoader, _allServices.Single<ISaveLoadService>());
                
        }
    }
}