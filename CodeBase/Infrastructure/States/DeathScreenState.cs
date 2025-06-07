using Assets.CodeBase.Infrastructure.Factory;
using Assets.CodeBase.Infrastructure.Services.SaveLoad;
using CodeBase.Infrastructure.Services.AudioService;
using CodeBase.UI.Services;
using CodeBase.UI.Services.UIFactory;
using UnityEngine;

namespace CodeBase.Infrastructure.States
{
    public class DeathScreenState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IUIFactory _uiFactory;
        private readonly IAudioService _audioService;
        private readonly IGameFactory _gameFactory;
        private readonly ISaveLoadService _saveLoadService;

        public DeathScreenState(IGameStateMachine gameStateMachine, IUIFactory uiFactory,
            IAudioService audioService, IGameFactory gameFactory, ISaveLoadService saveLoadService)
        {
            _gameStateMachine = gameStateMachine;
            _uiFactory = uiFactory;
            _audioService = audioService;
            _gameFactory = gameFactory;
            _saveLoadService = saveLoadService;
        }
        public void Exit()
        {
            _gameFactory.CleanUp();
            _audioService.ClearAmbient();
        }

        public void Enter()
        {
            _saveLoadService.SaveProgress();
            Debug.Log("DeathScreenState");
            
            
            
            _uiFactory.CreateWindow(WindowType.DeathScreen);
        }
    }
}