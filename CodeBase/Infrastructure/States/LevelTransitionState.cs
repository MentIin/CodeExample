using Assets.CodeBase.Infrastructure;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Infrastructure.Services.SaveLoad;
using Assets.CodeBase.Logic.CameraLogic;
using CodeBase.UI.Services;
using CodeBase.UI.Services.UIFactory;

namespace CodeBase.Infrastructure.States
{
    class LevelTransitionState : IState
    {
        private readonly IUIFactory _uiFactory;
        private readonly IPersistentProgressService _progressService;
        private readonly GameStateMachine _stateMachine;
        private readonly CameraController _cameraController;
        private readonly SceneLoader _sceneLoader;
        private readonly ISaveLoadService _saveLoadService;

        public LevelTransitionState(IUIFactory uiFactory, IPersistentProgressService progressService,
            GameStateMachine stateMachine, CameraController cameraController, SceneLoader sceneLoader,
            ISaveLoadService saveLoadService)
        {
            _uiFactory = uiFactory;
            _progressService = progressService;
            _stateMachine = stateMachine;
            _cameraController = cameraController;
            _sceneLoader = sceneLoader;
            _saveLoadService = saveLoadService;
        }
        public void Exit()
        {
            
        }

        public void Enter()
        {
            
            _sceneLoader.Load("EmptyScene", ONLoaded);
        }

        private void ONLoaded()
        {
            _uiFactory.CreateWindow(WindowType.LevelTransitionCutScene);
        }
    }
}