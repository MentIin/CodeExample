using Assets.CodeBase.Infrastructure;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Infrastructure.Services.Random;
using Assets.CodeBase.Infrastructure.Services.SaveLoad;
using CodeBase.UI.Services;
using CodeBase.UI.Services.UIFactory;

namespace CodeBase.Infrastructure.States
{
    public class SetUpNewGameState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IPersistentProgressService _progressService;
        private readonly IUIFactory _uiFactory;
        private readonly IRandomService _randomService;
        private readonly ISaveLoadService _saveLoadService;
        private readonly ICoroutineRunner _coroutineRunner;

        public SetUpNewGameState(IGameStateMachine gameStateMachine,
            IPersistentProgressService progressService, IUIFactory uiFactory,
            IRandomService randomService, ISaveLoadService saveLoadService, ICoroutineRunner coroutineRunner)
        {
            _gameStateMachine = gameStateMachine;
            _progressService = progressService;
            _uiFactory = uiFactory;
            _randomService = randomService;
            _saveLoadService = saveLoadService;
            _coroutineRunner = coroutineRunner;
        }
        public void Exit()
        {
            
        }

        public void Enter()
        {
            
            if (_progressService.Progress.TutorialData.FirstTutorialProgress.FullCompleted)
            {
                _gameStateMachine.Enter<GameStopOverState>();
            }
            else
            {
                _uiFactory.CreateWindow(WindowType.FirstCutscene);
            }

        }
    }
}