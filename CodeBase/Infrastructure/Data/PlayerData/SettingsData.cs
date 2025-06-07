using System;

namespace Assets.CodeBase.Infrastructure.Data.PlayerData
{
    [Serializable]
    public class SettingsData
    {
        public float MusicVolume = 0.7f;
        public float SoundsVolume = 0.7f;
        public event Action VolumeChanged;
    }
}