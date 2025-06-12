using Assets.CodeBase.Infrastructure;
using Assets.CodeBase.Infrastructure.AssetManagement;
using Assets.CodeBase.Infrastructure.Factory;
using Assets.CodeBase.Infrastructure.Services;
using Assets.CodeBase.Infrastructure.Services.AdsService;
using Assets.CodeBase.Infrastructure.Services.AudioService;
using Assets.CodeBase.Infrastructure.Services.GetUserInfoService;
using Assets.CodeBase.Infrastructure.Services.Input;
using Assets.CodeBase.Infrastructure.Services.MapGeneration;
using Assets.CodeBase.Infrastructure.Services.Pause;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Infrastructure.Services.Random;
using Assets.CodeBase.Infrastructure.Services.SaveLoad;
using Assets.CodeBase.Infrastructure.Services.StaticData;
using Assets.CodeBase.Logic.CameraLogic;
using Assets.SimpleLocalization.Scripts;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services.Analitics;
using CodeBase.Infrastructure.Services.Analytics;
using CodeBase.Infrastructure.Services.AudioService;
using CodeBase.Infrastructure.Services.Input;
using CodeBase.UI;
using CodeBase.UI.Services.UIFactory;
using UnityEngine;
using UnityEngine.Audio;
using YG;

namespace CodeBase.Infrastructure.States
{
    public class BootstrapState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly AllServices _allServices;
        private readonly LoadingCurtain _loadingCurtain;
        private AudioSource _audioSource;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly CameraController _cameraController;
        
        
#if YANDEX_GAMES
        private YandexGame _yandexGame;
#endif
        public BootstrapState(IGameStateMachine gameStateMachine, SceneLoader sceneLoader, AllServices allServices,
            LoadingCurtain loadingCurtain, AudioSource audioSource, ICoroutineRunner coroutineRunner,
            CameraController cameraController, AudioMixerGroup musicMixerGroup, AudioMixerGroup soundsMixerGroup)
        {
            _audioSource = audioSource;
            _coroutineRunner = coroutineRunner;
            _cameraController = cameraController;
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _allServices = allServices;
            _loadingCurtain = loadingCurtain;
            RegisterServices(soundsMixerGroup, musicMixerGroup);
        }

        public void Exit()
        {
            
        }

        public void Enter()
        {
            //_gameStateMachine.Enter<MainMenuState>();
        }

        private void RegisterServices(AudioMixerGroup soundsMixerGroup, AudioMixerGroup musicMixerGroup)
        {
            
            
            _allServices.RegisterSingle<IRandomService>(new RandomService(662));
            
            
            _allServices.RegisterSingle<IAssets>(new Assets.CodeBase.Infrastructure.AssetManagement.Assets());
            _allServices.RegisterSingle<IStaticDataService>(new StaticDataService());
            _allServices.RegisterSingle<IPersistentProgressService>(new PersistentProgressService());
            
            
            _allServices.RegisterSingle<IAudioService>(new AudioService(_audioSource,
                _allServices.Single<IAssets>(), _coroutineRunner,
                _allServices.Single<IPersistentProgressService>(), soundsMixerGroup,
                musicMixerGroup));
            _allServices.RegisterSingle<PauseService>(new PauseService(_allServices.Single<IAudioService>()));
            
#if YANDEX_GAMES
            _yandexGame = Object.Instantiate(Resources.Load<YandexGame>("YandexGame"));
            Object.DontDestroyOnLoad(_yandexGame.gameObject);
            YandexGame.GetDataEvent += () =>
            {
                OnConnected(soundsMixerGroup, musicMixerGroup);
            };
#else
            OnConnected(soundsMixerGroup, musicMixerGroup);
#endif
        }

        private void OnConnected(AudioMixerGroup soundsMixerGroup, AudioMixerGroup musicMixerGroup)
        {
            Debug.Log("OnConnected");


#if YANDEX_GAMES
            _allServices.RegisterSingle<IUserInfoService>(new YandexGamesUserInfoService(_yandexGame));
            _allServices.RegisterSingle<IAdsService>(new YandexGamesAdsService(_allServices.Single<PauseService>()));
            _allServices.RegisterSingle<ISaveLoadService>(
                new YandexGamesSaveLoadService(_allServices.Single<IPersistentProgressService>(), _yandexGame));
            
            
            _allServices.RegisterSingle<IAnalyticsService>(new DTDAnalyticsService(_allServices.Single<IPersistentProgressService>(),
                _allServices.Single<IUserInfoService>()));

#else
            _allServices.RegisterSingle<IUserInfoService>(new MockUserInfoService());
            _allServices.RegisterSingle<IAdsService>(new MockAdsService());
            
            _allServices.RegisterSingle<IAnalyticsService>(new MockAnalyticsService());
            
            _allServices.RegisterSingle<ISaveLoadService>(
                            new SaveLoadService(_allServices.Single<IPersistentProgressService>()));
#endif
            
            
            _allServices.RegisterSingle<IInputService>(new StandaloneInputService(_coroutineRunner));
            

            RegisterUIFactory();
            RegisterFactory();

            _allServices.Single<IInputService>().Initialize();

            
            LocalizationManager.Read();

            _gameStateMachine.InitializeStates(this);
            
            _gameStateMachine.Enter<LoadProgressState>();
        }

        private void RegisterUIFactory()
        {
            _allServices.RegisterSingle<IUIFactory>(new UIFactory(_allServices.Single<IAssets>(),
                _allServices.Single<IStaticDataService>(), _allServices.Single<IPersistentProgressService>(),
                _gameStateMachine, _allServices.Single<IAudioService>(), _allServices.Single<PauseService>(),
                _allServices.Single<ISaveLoadService>(), _allServices.Single<IAdsService>(),
                _cameraController, _allServices.Single<IAnalyticsService>()));
        }

        private void RegisterFactory()
        {
            _allServices.RegisterSingle<IGameFactory>(new GameFactory(
                _allServices.Single<IAssets>(),
                _allServices.Single<IStaticDataService>(),
                _allServices.Single<IRandomService>(),
                new MapGenerator(_allServices.Single<IRandomService>()),
                _gameStateMachine, _allServices.Single<IPersistentProgressService>(),
                _allServices.Single<IInputService>(), _allServices.Single<IAudioService>(), _cameraController,
                _allServices.Single<IUIFactory>(),
                _allServices.Single<PauseService>())
            );
        }
    }
}