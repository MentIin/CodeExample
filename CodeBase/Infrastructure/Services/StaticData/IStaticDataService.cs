using System.Collections;
using Assets.CodeBase.StaticData;
using Assets.CodeBase.StaticData.Level;
using CodeBase.UI.Services;

namespace Assets.CodeBase.Infrastructure.Services.StaticData
{
    public interface IStaticDataService : IService
    {
        void Load();
        LevelStaticData ForLevel(int stage);
        MachineStaticData ForStartingMachine(int id);
        MachineModuleStaticData ForMachineModule(string i);
        WindowConfig ForWindow(WindowType type);
        MachineBasementStaticData ForMachineBasement(int id);
        MachineModuleStaticData[] GetAllModules();
        MaterialStaticData ForMaterial(CraftingResourceType type);
        AllStatsDisplayStaticData GetAllStatsDisplayData();
        MachineBasementStaticData[] GetAllMachineBasements();
        MachineStaticData[] GetAllStartingMachines();
        AllLevelsStaticData GetAllLevels();
        MapBlockStaticData[] GetAllMapBlocksData();
        AllEnemiesStaticData GetAllEnemiesStaticData();
    }
}