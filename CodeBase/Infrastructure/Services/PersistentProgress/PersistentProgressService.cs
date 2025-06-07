using Assets.CodeBase.Infrastructure.Data.PlayerData;

namespace Assets.CodeBase.Infrastructure.Services.PersistentProgress
{
    public class PersistentProgressService : IPersistentProgressService
    {
        public PlayerProgress Progress { get; set; }

        public PersistentProgressService()
        {
            Progress = new PlayerProgress(0);
        }
    }
}