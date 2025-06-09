using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Infrastructure.Services.SaveLoad;
using Assets.SimpleLocalization.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Elements.Buttons
{
    public class ChangeLanguageButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        
        private IPersistentProgressService _progressService;
        private ISaveLoadService _saveLoadService;

        public void Construct(IPersistentProgressService progressService, ISaveLoadService saveLoadService)
        {
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            
            _button.onClick.AddListener(ChangeLanguage);
        }

        private void ChangeLanguage()
        {
            if (LocalizationManager.Language == "Russian")
            {
                LocalizationManager.Language = "English";
            }
            else
            {
                LocalizationManager.Language = "Russian";
            }

            _progressService.Progress.Language = LocalizationManager.Language;
            _saveLoadService.SaveProgress();
        }
    }
}