using Assets.CodeBase.Infrastructure;
using Assets.CodeBase.Infrastructure.Data.Loot;
using Assets.CodeBase.Infrastructure.Data.PlayerData;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Infrastructure.Services.SaveLoad;
using Assets.CodeBase.Logic.CameraLogic;
using CodeBase.UI.Services;
using CodeBase.UI.Services.UIFactory;

namespace CodeBase.Infrastructure.States
{
    public class FinishGameState : IState
    {
        private readonly IUIFactory _uiFactory;
        private readonly SceneLoader _sceneLoader;
        private ISaveLoadService _saveLoadService;
        private readonly IPersistentProgressService _progressService;
        private readonly CameraController _cameraController;

        public FinishGameState(IUIFactory uiFactory, SceneLoader sceneLoader,
            ISaveLoadService saveLoadService, IPersistentProgressService progressService,
            CameraController cameraController)
        {
            _uiFactory = uiFactory;
            _sceneLoader = sceneLoader;
            _saveLoadService = saveLoadService;
            _progressService = progressService;
            _cameraController = cameraController;
        }
        public void Exit()
        {
            
        }

        public void Enter()
        {
            _cameraController.RemoveShade();
            _sceneLoader.Load("GameStopOverScene");
            //Reward();
            _saveLoadService.SaveProgress();
            
            _uiFactory.CreateWindow(WindowType.FinishGame);
            
        }

        private void Reward()
        {
            LootData lootData = new LootData();
            lootData.Type = LootType.DarkAmethyst;
            if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Easy)
            {
                lootData.Amount = 15;
                _progressService.Progress.CollectResource(lootData);
            }
            if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Medium)
            {
                lootData.Amount = 25;
                _progressService.Progress.CollectResource(lootData);
            }
            if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Hard)
            {
                lootData.Amount = 50;
                _progressService.Progress.CollectResource(lootData);
            }
            if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Extreme)
            {
                lootData.Amount = 120;
                _progressService.Progress.CollectResource(lootData);
            }
        }
    }
}