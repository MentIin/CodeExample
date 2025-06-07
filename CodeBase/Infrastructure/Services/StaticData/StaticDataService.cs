using System.Collections.Generic;
using System.Linq;
using Assets.CodeBase.StaticData;
using Assets.CodeBase.StaticData.Level;
using CodeBase.UI.Services;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Services.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private AllLevelsStaticData _levelsData;
        private Dictionary<int, MachineStaticData> _startingMachineData;
        private Dictionary<string, MachineModuleStaticData> _moduleStaticData;
        private Dictionary<WindowType, WindowConfig> _windowData;
        private Dictionary<int, MachineBasementStaticData> _machineBasementData;

        private static readonly string LevelsStaticData = "StaticData/Levels/AllLevelsStaticData";
        private static readonly string StartingMachinesStaticData = "StaticData/StartingMachines";
        private static readonly string MachineModulesStaticData = "StaticData/MachineModules";
        private static readonly string WindowsStaticData = "StaticData/UI/WindowsStaticData";
        private static readonly string MachineBasementStaticData = "StaticData/MachineBasements";
        private static readonly string MaterialsStaticData = "StaticData/CraftingMaterials";
        private static readonly string AllStatsDisplayStaticData = "StaticData/UI/AllStatsDisplayData";
        private Dictionary<CraftingResourceType, MaterialStaticData> _materialsStaticData;
        private AllStatsDisplayStaticData _allStatsDisplayInfo;
        private MapBlockStaticData[] _mapBlocksStaticData;
        private AllEnemiesStaticData _allEnemiesStaticData;


        public void Load()
        {
            LoadLevelsData();
            //LoadMachineModulesStaticData();
            LoadStartingMachinesData();
            LoadMachineModulesStaticData();
            LoadMachineBasementsStaticData();
            LoadMaterialStaticData();
            _windowData = Resources.Load<WindowsStaticData>(WindowsStaticData).
                WindowConfigs.ToDictionary(x => x.WindowType, x => x);

            _allStatsDisplayInfo = Resources.Load<AllStatsDisplayStaticData>(AllStatsDisplayStaticData);
            _allEnemiesStaticData = Resources.Load<AllEnemiesStaticData>("StaticData/Enemy/AllEnemiesStaticData");
            _mapBlocksStaticData = Resources.LoadAll<MapBlockStaticData>("StaticData/MapBlocks");
        }

        private void LoadLevelsData() => _levelsData = Resources.Load<AllLevelsStaticData>(LevelsStaticData);

        private void LoadStartingMachinesData()
        {
            _startingMachineData = Resources.LoadAll<MachineStaticData>(StartingMachinesStaticData)
                .ToDictionary(x => x.Id, x => x);
            
        }

        private void LoadMachineModulesStaticData()
        {
            _moduleStaticData = Resources.LoadAll<MachineModuleStaticData>(MachineModulesStaticData)
                .ToDictionary(x => x.Id, x => x);
        }

        private void LoadMachineBasementsStaticData()
        {
            _machineBasementData = Resources.LoadAll<MachineBasementStaticData>(MachineBasementStaticData)
                .ToDictionary(x => x.Id, x => x);
        }

        private void LoadMaterialStaticData()
        {
            _materialsStaticData = Resources.LoadAll<MaterialStaticData>(MaterialsStaticData)
                .ToDictionary(x => x.Type, x => x);
        }

        public LevelStaticData ForLevel(int stage) => _levelsData.LevelsStaticData[stage];

        public MachineBasementStaticData ForMachineBasement(int id)
            => _machineBasementData.TryGetValue(id, out MachineBasementStaticData staticData)
            ? staticData
            : null;

        public MachineModuleStaticData[] GetAllModules()
        {
            return _moduleStaticData.Values.ToArray();
        }

        public MachineStaticData ForStartingMachine(int id)
            => _startingMachineData.TryGetValue(id, out MachineStaticData staticData)
                ? staticData
                : null;

        public MachineModuleStaticData ForMachineModule(string id)
            => _moduleStaticData.TryGetValue(id, out MachineModuleStaticData staticData)
                ? staticData
                : null;

        public WindowConfig ForWindow(WindowType type)
            => _windowData.TryGetValue(type, out WindowConfig config)
                ? config
                : null;
        
        public MaterialStaticData ForMaterial(CraftingResourceType type)
            => _materialsStaticData.TryGetValue(type, out MaterialStaticData staticData)
                ? staticData
                : null;

        public AllStatsDisplayStaticData GetAllStatsDisplayData() => _allStatsDisplayInfo;
        public MachineBasementStaticData[] GetAllMachineBasements()
        {
            return _machineBasementData.Values.ToArray();
        }

        public MachineStaticData[] GetAllStartingMachines()
        {
            return _startingMachineData.Values.OrderBy(
                x => x.Id).ToArray();
        }

        public AllLevelsStaticData GetAllLevels()
        {
            return _levelsData;
        }

        public MapBlockStaticData[] GetAllMapBlocksData()
        {
            return _mapBlocksStaticData;
        }

        public AllEnemiesStaticData GetAllEnemiesStaticData()
        {
            return _allEnemiesStaticData;
        }
    }
}