using Assets.CodeBase.Infrastructure;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.UI;
using CodeBase.UI.Services;
using CodeBase.UI.Services.UIFactory;
using UnityEngine;

namespace CodeBase.Infrastructure.States
{
    class MainMenuState : IState
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly IPersistentProgressService _progressService;
        private readonly IUIFactory _uiFactory;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly SceneLoader _sceneLoader;
        private const string MainMenuSceneName = "MainMenuScene";
        private const string LoadingToMain = "Loading.ToMain";

        public MainMenuState(IGameStateMachine stateMachine, IPersistentProgressService progressService,
            IUIFactory uiFactory, LoadingCurtain loadingCurtain, SceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _progressService = progressService;
            _uiFactory = uiFactory;
            _loadingCurtain = loadingCurtain;
            _sceneLoader = sceneLoader;
        }
        public void Exit()
        {
            
        }

        public void Enter()
        {
            
            _loadingCurtain.Show(LoadingToMain);
            _sceneLoader.Load(MainMenuSceneName, OnLoaded);

        }

        private void OnLoaded()
        {
            _loadingCurtain.UpdateLoading(100);
            //Object.FindObjectOfType<MainMenuWindow>().Construct(_progressService, _stateMachine);
            _uiFactory.CreateWindow(WindowType.MainMenu);
            _loadingCurtain.Hide();
            Debug.Log("MainMenuState");
        }
    }
}