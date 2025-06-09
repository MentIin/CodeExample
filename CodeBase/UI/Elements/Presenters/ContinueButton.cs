using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Infrastructure.Services.StaticData;
using Assets.CodeBase.StaticData.Level;
using Assets.SimpleLocalization.Scripts;
using CodeBase.Infrastructure.States;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Elements.Presenters
{
    public class ContinueButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _button;
        
        private IPersistentProgressService _progressService;
        private IGameStateMachine _gameStateMachine;
        private IStaticDataService _staticDataService;
        
        public void Construct(IPersistentProgressService progressService, IGameStateMachine gameStateMachine,
            IStaticDataService staticDataService)
        {
            _progressService = progressService;
            _gameStateMachine = gameStateMachine;
            _staticDataService = staticDataService;
            
            _button.onClick.AddListener(GoToLevel);
            
            LevelStaticData staticData = _staticDataService.ForLevel(_progressService.Progress.LevelStage);
            
            _text.text = LocalizationManager.Localize("CraftMenu.GoToLevel");
            return;
            _text.text = LocalizationManager.Localize("Level") + " - " +
                         (_progressService.Progress.LevelStage + 1).ToString();
        }
        
        private void GoToLevel()
        {
            _gameStateMachine.Enter<LoadLevelState>();
        }
    }
}