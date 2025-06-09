using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;

namespace CodeBase.UI
{
    public class LoadingCurtain : MonoBehaviour
    {
        [SerializeField] private SlicedFilledImage _progressBar;
        [SerializeField] private GameObject _containerToHide;
        [SerializeField] private TextMeshProUGUI _loadingText;
        

        public void UpdateLoading(float percent)
        {
            _progressBar.fillAmount = percent / 100;
        }
        
        public void Show(string key)
        {
            _loadingText.text = LocalizationManager.Localize(key);
            UpdateLoading(0);
            _containerToHide.SetActive(true);
        }

        public void Hide()
        {
            _containerToHide.SetActive(false);
        }
    }
}