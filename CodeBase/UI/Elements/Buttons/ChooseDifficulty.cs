using Assets.CodeBase.Infrastructure.Data.PlayerData;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Elements.Buttons
{
    public class ChooseDifficulty : MonoBehaviour
    {
        [SerializeField] private Sprite _default;
        [SerializeField] private Sprite _choosen;
        [Space(5)]
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [SerializeField]private DifficultyLevel _difficultyLevel;
        
        private IPersistentProgressService _progressService;
        
        private DifficultyLevel _current;

        public void Construct(IPersistentProgressService progressService)
        {
            _progressService = progressService;

            _button.onClick.AddListener(() =>
            {
                SetDifficulty(_difficultyLevel);
            });

            _current = _progressService.Progress.DifficultyChoosen;
            UpdateColor();
        }


        private void SetDifficulty(DifficultyLevel difficultyLevel)
        {
            _progressService.Progress.DifficultyChoosen = difficultyLevel;
        }


        private void LateUpdate()
        {
            if (_progressService == null) return;
            if (_current != _progressService.Progress.DifficultyChoosen)
            {
                Debug.Log(_current != _progressService.Progress.DifficultyChoosen);

                _current = _progressService.Progress.DifficultyChoosen;
                UpdateColor();
            }
            
            
        }

        private void UpdateColor()
        {
            if (_current == _difficultyLevel)
            {
                _image.sprite = _choosen;
            }
            else
            {
                _image.sprite = _default;
            }
        }
    }
}