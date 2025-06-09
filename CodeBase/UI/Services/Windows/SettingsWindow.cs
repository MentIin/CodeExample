using Assets.CodeBase.Infrastructure.Services.Pause;
using UnityEngine;

namespace CodeBase.UI.Services
{
    public class SettingsWindow : WindowBase
    {
        private PauseService _pauseService;

        
        public void Construct(PauseService pauseService)
        {
            _pauseService = pauseService;
            _pauseService.Pause();
        }
        private void OnDestroy()
        {
            _pauseService.Resume();
        }
    }
}