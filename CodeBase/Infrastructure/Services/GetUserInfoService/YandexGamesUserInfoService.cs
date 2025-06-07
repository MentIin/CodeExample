using UnityEngine;
using YG;

namespace Assets.CodeBase.Infrastructure.Services.GetUserInfoService
{
    class YandexGamesUserInfoService : IUserInfoService
    {
        private readonly YandexGame _yandexGame;

        public YandexGamesUserInfoService(YandexGame yandexGame)
        {
            _yandexGame = yandexGame;
        }

        public DeviceType GetDeviceType()
        {
            switch (YandexGame.EnvironmentData.deviceType)
            {
                case "desktop": return DeviceType.Desktop;
                case "mobile": return DeviceType.Handheld;
            }

            return DeviceType.Unknown;
        }

        public string GetLanguage()
        {
            return YandexGame.EnvironmentData.language;
        }

        public string GetUserID()
        {
            return YandexGame.playerId;
        }
    }
}