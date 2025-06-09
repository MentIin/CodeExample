using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;

namespace CodeBase.Logic.Level
{
    public class LevelCounter : MonoBehaviour
    {
        public TextMeshProUGUI Text;
        
        private IPersistentProgressService _progressService;

        public void Construct(IPersistentProgressService progressService)
        {
            _progressService = progressService;

            Text.text = LocalizationManager.Localize("HUD.LevelCounter") + " - " + (_progressService.Progress.StatisticData.GamesStarted+1).ToString();

            _progressService.Progress.StatisticData.GamesStarted++;
        }
        
        
    }
}