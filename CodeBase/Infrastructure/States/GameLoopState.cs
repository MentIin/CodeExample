using Assets.CodeBase.Infrastructure;
using Assets.CodeBase.Infrastructure.Factory;
using Assets.CodeBase.Infrastructure.Services.AdsService;
using Assets.CodeBase.Infrastructure.Services.SaveLoad;
using CodeBase.Infrastructure.Services.AudioService;
using UnityEngine;

namespace CodeBase.Infrastructure.States
{
    class GameLoopState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IGameFactory _gameFactory;
        private readonly IAudioService _audioService;
        private readonly IAdsService _adsService;
        private readonly ISaveLoadService _saveLoadService;

        public GameLoopState(IGameStateMachine gameStateMachine, SceneLoader sceneLoader, ICoroutineRunner coroutineRunner,
            IGameFactory gameFactory, IAudioService audioService, IAdsService adsService, ISaveLoadService saveLoadService)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _coroutineRunner = coroutineRunner;
            _gameFactory = gameFactory;
            _audioService = audioService;
            _adsService = adsService;
            _saveLoadService = saveLoadService;
        }

        public void Exit()
        {
            _saveLoadService.SaveProgress();
        }

        public void Enter()
        {
            
            Debug.Log("Enter GameLoop");
            //_coroutineRunner.StartCoroutine(EndlessGeneration());
        }
        
    }
}