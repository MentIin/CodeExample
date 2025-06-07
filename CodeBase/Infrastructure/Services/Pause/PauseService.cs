using Assets.CodeBase.Infrastructure.Services.AudioService;
using CodeBase.Infrastructure.Services.AudioService;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Services.Pause
{
    public class PauseService : IService
    {
        private readonly IAudioService _audioService;

        private int _pausesCalled = 0;

        public PauseService(IAudioService audioService)
        {
            _audioService = audioService;
        }

        public void Pause()
        {
            _pausesCalled++;
            _audioService.PauseSounds();
            Time.timeScale = 0f;
        }

        public void PauseAmbient() => _audioService.PauseAmbient();
        public void ResumeAmbient() => _audioService.ResumeAmbient();

        public void Resume()
        {
            _pausesCalled--;
            if (_pausesCalled == 0)
            {
                _audioService.ResumeSounds();
                Time.timeScale = 1f;
            }

            if (_pausesCalled < 0)
            {
                _pausesCalled = 0;
            }
            
        }
    }
}