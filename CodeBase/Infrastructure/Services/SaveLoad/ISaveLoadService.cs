using Assets.CodeBase.Infrastructure.Data.PlayerData;

namespace Assets.CodeBase.Infrastructure.Services.SaveLoad
{
    public interface ISaveLoadService : IService
    {
        public void SaveProgress();
        public PlayerProgress LoadProgress();
    }
}