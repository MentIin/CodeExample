using Assets.CodeBase.Infrastructure.Factory;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Infrastructure.Services.SaveLoad;
using CodeBase.Infrastructure.Services.AudioService;
using CodeBase.UI.Services.UIFactory;

namespace CodeBase.Infrastructure.States
{
    public class GoToMainMenuFromGameState : IPayloadState<bool>
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly IGameFactory _gameFactory;
        private readonly IUIFactory _uiFactory;
        private readonly IAudioService _audioService;
        private readonly IPersistentProgressService _progressService;
        private readonly ISaveLoadService _saveLoadService;

        public GoToMainMenuFromGameState(IGameStateMachine stateMachine, IGameFactory gameFactory,
            IUIFactory uiFactory, IAudioService audioService,
            IPersistentProgressService progressService, ISaveLoadService saveLoadService)
        {
            _stateMachine = stateMachine;
            _gameFactory = gameFactory;
            _uiFactory = uiFactory;
            _audioService = audioService;
            _progressService = progressService;
            _saveLoadService = saveLoadService;
        }
        public void Exit()
        {
            
        }

        public void Enter(bool save)
        {
            _progressService.Progress.HaveSave = save;
            _saveLoadService.SaveProgress();
            _uiFactory.Clear();
            _gameFactory.CleanUp();
            _audioService.ClearAmbient();
            
            _stateMachine.Enter<MainMenuState>();
        }
    }
}