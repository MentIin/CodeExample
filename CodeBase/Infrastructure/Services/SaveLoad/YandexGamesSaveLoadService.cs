using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Infrastructure.Data.PlayerData;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using UnityEngine;
using YG;

namespace Assets.CodeBase.Infrastructure.Services.SaveLoad
{
    class YandexGamesSaveLoadService : ISaveLoadService
    {
        private const string ProgressKey = "Progress";
        private readonly IPersistentProgressService _progressService;
        private readonly YandexGame _yandexGame;

        public YandexGamesSaveLoadService(IPersistentProgressService progressService, YandexGame yandexGame)
        {
            _progressService = progressService;
            _yandexGame = yandexGame;
        }

        public void SaveProgress()
        {
            Debug.Log("Saved");
            YandexGame.savesData.MyJSONSave = _progressService.Progress.AsJson();
            YandexGame.SaveProgress();
        }

        public PlayerProgress LoadProgress()
        {
            return YandexGame.savesData.MyJSONSave?.ToDeserialized<PlayerProgress>();
        }
    }
}