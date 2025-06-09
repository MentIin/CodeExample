using Assets.CodeBase.Infrastructure.Data.PlayerData;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;

namespace CodeBase.UI.Services
{
    public class FinishGameWindow : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _amethistText;
        [SerializeField] private TextMeshProUGUI _levelsText;
        [SerializeField] private TextMeshProUGUI _oresMinedText;
        [SerializeField] private TextMeshProUGUI _difficultyText;
        
        private IPersistentProgressService _progressService;

        public void Construct(IPersistentProgressService progressService)
        {
            _progressService = progressService;
            UpdateVisuals();

            
            if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Easy)
            {
                _amethistText.text = "" + 15.ToString();
            }
            if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Medium)
            {
                _amethistText.text = "" + 25.ToString();
            }
            if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Hard)
            {
                _amethistText.text = "" + 50.ToString();
            }
            if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Extreme)
            {
                _amethistText.text = "" + 100.ToString();
            }
            
        }
        
        
        private void UpdateVisuals()
        {
            //Repeat DeathScreen code but I am lazy
            int levelStage = _progressService.Progress.LevelStage;
            _levelsText.text = LocalizationManager.Localize("DeathScreen.Levels").ToLower()
                               +"   " + levelStage +" " + LocalizationManager.Localize("DeathScreen.From") + " "+ "8";
            _oresMinedText.text = LocalizationManager.Localize("DeathScreen.OresMined").ToLower() + "   " + _progressService.Progress.AmethistsMined;


            if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Easy)
                _difficultyText.text = LocalizationManager.Localize("PreparationWindow.Easy");
            
            if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Medium)
                _difficultyText.text = LocalizationManager.Localize("PreparationWindow.Medium");

            if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Hard)
                _difficultyText.text = LocalizationManager.Localize("PreparationWindow.Hard");

            if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Extreme)
                _difficultyText.text = LocalizationManager.Localize("PreparationWindow.Extreme");

        }
    }
}