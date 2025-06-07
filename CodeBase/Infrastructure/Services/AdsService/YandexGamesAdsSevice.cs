using System;
using Assets.CodeBase.Infrastructure.Services.Pause;
using UnityEngine;
using YG;

namespace Assets.CodeBase.Infrastructure.Services.AdsService
{
    public class YandexGamesAdsService : IAdsService
    {
        private readonly PauseService _pauseService;

        public Action<int> RewardedAdEvent {
            get => YandexGame.RewardVideoEvent;
            set => YandexGame.RewardVideoEvent = value;
        }

        public bool RewardedAvailable
        {
            get
            {
                Debug.Log("TODO");
                return true;
            }
        }

        public bool InitialAvailable
        {
            get
            {
                Debug.Log("TODO");
                return true;
            }
        }

        public YandexGamesAdsService(PauseService pauseService)
        {
            _pauseService = pauseService;
            YandexGame.CloseFullAdEvent += CloseFullAdEvent;
            YandexGame.ErrorVideoEvent += CloseFullAdEvent;
            YandexGame.OpenFullAdEvent += OpenFullAdEvent;
        }

        private void OpenFullAdEvent()
        {
            _pauseService.Pause();
            _pauseService.PauseAmbient();
        }

        private void CloseFullAdEvent()
        {
            _pauseService.ResumeAmbient();
            _pauseService.Resume();
        }

        public void ShowInitial()
        {
            YandexGame.FullscreenShow();
        }

        public void ShowRewarded(int id)
        {
            YandexGame.RewVideoShow(id);
        }
    }
}