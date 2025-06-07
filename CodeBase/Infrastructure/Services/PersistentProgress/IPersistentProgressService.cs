using Assets.CodeBase.Infrastructure.Data.PlayerData;

namespace Assets.CodeBase.Infrastructure.Services.PersistentProgress
{
    public interface IPersistentProgressService : IService
    {
        PlayerProgress Progress { get; set; }
    }
}