using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.CodeBase.Infrastructure;
using Assets.CodeBase.Infrastructure.AssetManagement;
using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Infrastructure.Data.Modules;
using Assets.CodeBase.Infrastructure.Data.PlayerData;
using Assets.CodeBase.Infrastructure.Data.Stats;
using Assets.CodeBase.Infrastructure.Factory;
using Assets.CodeBase.Infrastructure.Services.Input;
using Assets.CodeBase.Infrastructure.Services.MapGeneration;
using Assets.CodeBase.Infrastructure.Services.Pause;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Infrastructure.Services.Random;
using Assets.CodeBase.Infrastructure.Services.StaticData;
using Assets.CodeBase.Logic;
using Assets.CodeBase.Logic.AI.Machine;
using Assets.CodeBase.Logic.AttackLogic;
using Assets.CodeBase.Logic.AttackLogic.Projectiles;
using Assets.CodeBase.Logic.CameraLogic;
using Assets.CodeBase.Logic.DynamicDataLogic;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using Assets.CodeBase.Logic.Machine;
using Assets.CodeBase.Logic.MapEditing;
using Assets.CodeBase.Logic.Raycasters;
using Assets.CodeBase.StaticData;
using Assets.CodeBase.StaticData.Level;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Data;
using CodeBase.Infrastructure.Data.Modules;
using CodeBase.Infrastructure.Services.AudioService;
using CodeBase.Infrastructure.States;
using CodeBase.Logic;
using CodeBase.Logic.Animations;
using CodeBase.Logic.Collectables;
using CodeBase.Logic.DynamicDataLogic;
using CodeBase.Logic.Level;
using CodeBase.Logic.Level.Tutorial;
using CodeBase.Logic.Machine;
using CodeBase.Logic.MachineModules;
using CodeBase.Sounds;
using CodeBase.UI.Elements;
using CodeBase.UI.Elements.Animations;
using CodeBase.UI.Elements.Bars;
using CodeBase.UI.Elements.Buttons;
using CodeBase.UI.Elements.Hud;
using CodeBase.UI.Elements.Presenters;
using CodeBase.UI.Elements.Tutorial;
using CodeBase.UI.Services.GameStopOver;
using CodeBase.UI.Services.UIFactory;
using CodeBase.UI.Tutorial;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace CodeBase.Infrastructure.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly IStaticDataService _staticData;
        private readonly IAssets _assets;
        private readonly IRandomService _randomService;
        private readonly IMapGenerator _mapGenerator;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IPersistentProgressService _progressService;
        private readonly IInputService _inputService;
        private readonly IAudioService _audioService;
        private readonly CameraController _cameraController;
        private readonly ITilemapService _tilemapService;


        private Dictionary<Vector2Int, DestructibleTileData> _mapTiles;

        private List<Vector2Int> _freeTiles;

        private GameObject _tilemapPrefab;
        private GameObject _floorGO;
        private Tilemap _boundsTilemap;
        private SpriteRenderer _ambientOcclusionRenderer;

        private MachineBase _playerBase;
        private MachineBase _bossMachine;
        private IUIFactory _uiFactory;
        private readonly PauseService _pauseService;
        private GameObject _grid;
        public List<ISavedProgressReader> ProgressReaders { get; } = new List<ISavedProgressReader>();
        public List<ISavedProgress> ProgressWriters { get; } = new List<ISavedProgress>();


        public GameFactory(IAssets assets, IStaticDataService staticData, IRandomService randomService,
            IMapGenerator mapGenerator, IGameStateMachine gameStateMachine, IPersistentProgressService progressService,
            IInputService inputService, IAudioService audioService, CameraController cameraController,
            IUIFactory uiFactory, PauseService pauseService)
        {
            _assets = assets;
            _staticData = staticData;
            _randomService = randomService;
            _mapGenerator = mapGenerator;
            _gameStateMachine = gameStateMachine;
            _progressService = progressService;
            _inputService = inputService;
            _audioService = audioService;
            _cameraController = cameraController;
            _uiFactory = uiFactory;
            _pauseService = pauseService;
            _tilemapService = new TilemapService(_progressService, this, audioService, _assets);
        }

        public Task WarmUp()
        {
            _assets.Initialize();
            return Task.CompletedTask;
        }

        public void CleanUp()
        {
            ProgressReaders.Clear();
            ProgressWriters.Clear();

            ClearLevel();

            _assets.CleanUp();
        }

        public async Task GenerateLevel(LevelStaticData staticData, int seed)
        {
            ClearLevel();
            _freeTiles = new List<Vector2Int>();
            _mapTiles = new Dictionary<Vector2Int, DestructibleTileData>();

            if (staticData.HandMade)
            {
                await GenerateHandMadeLevel(staticData, seed);
                return;
            }
            
            // TODO
            var chunks = staticData.chunks;


            Tilemap[] tilemaps = new Tilemap[chunks];
            for (int i = 0; i < chunks; i++)
            {
                tilemaps[i] = await GenerateMap(staticData, 0, seed, i);
                await GenerateMapFloor(staticData, 0, seed, i);
            }



            // generate phantom tiles to 
            TileBase tile = await _assets.Load<TileBase>(_staticData.GetAllLevels().PhantomTile);
            for (int i = 0; i < chunks - 1; i++)
            {
                for (int x = 0; x < staticData.Width; x++)
                {
                    int y = staticData.Height * (i+1);
                
                    Vector2Int key = new Vector2Int(x, y-1);
                    if (_mapTiles.ContainsKey(key))
                    {
                        tilemaps[i+1].SetTile(new Vector3Int(x, y-1, 0), tile);
                        _mapTiles[key].LinkedTileChunk = i+1;
                    }
                    key = new Vector2Int(x, y);
                    if (_mapTiles.ContainsKey(key))
                    {
                        tilemaps[i].SetTile(new Vector3Int(x, y, 0), tile);
                        _mapTiles[key].LinkedTileChunk = i;
                    }
                }
            }



            await GenerateMapBounds(staticData);
            
            _tilemapService.SetData(new Vector2Int(staticData.Width, staticData.Height),
                tilemaps, _mapTiles, _freeTiles, _ambientOcclusionRenderer, chunks);
            
            
            if (!staticData.HaveBoss)
            {
                await SpawnDeathZone(staticData.Width * GameConstants.MapBlockSize,
                    new Vector2(staticData.Width * GameConstants.MapBlockSize / 2, 0));
            }
            else await SpawnBoss(staticData);
            
            await SpawnEnemies(staticData);
            
            
            await SpawnAmbientObject(staticData);

            GameObject counterPrefab = await _assets.Load<GameObject>(AssetAddress.GameCounter);
            Vector3 pos = new Vector3(staticData.Width / 2 * GameConstants.MapBlockSize,
                staticData.PlayerYPos + 20f);
            GameObject counterGO = InstantiateAndRegister(counterPrefab, pos);
            
            counterGO.GetComponent<LevelCounter>().Construct(_progressService);
            
            _freeTiles.Clear();
        }


        public async Task SpawnModuleCollectable(MachineModuleStaticData moduleStaticData, Vector2 position)
        {
            GameObject prefab = await _assets.Load<GameObject>(AssetAddress.ModuleCollectable);
            GameObject go = InstantiateAndRegister(prefab, 
                position);

            go.GetComponent<MachineModuleCollectable>().Construct(_progressService,
                moduleStaticData);
        }

        private async Task GenerateHandMadeLevel(LevelStaticData staticData, int seed)
        {

            Tilemap[] tilemaps = new Tilemap[1];


            GameObject mapPrefab = await _assets.Load<GameObject>(staticData.HandMadeMapReference);

            GameObject mapGO = Object.Instantiate(mapPrefab);
            mapGO.transform.position = new Vector3(0f, 0f, 0f);

            Tilemap tilemap = mapGO.GetComponentInChildren<Tilemap>();
            tilemaps[0] = tilemap;

            Dictionary<TileBase, MapBlockStaticData> tileToDataDictionary =
                new Dictionary<TileBase, MapBlockStaticData>();

            
            foreach (var data in _staticData.GetAllMapBlocksData())
            {
                tileToDataDictionary[await _assets.Load<TileBase>(data.BaseTile)] = data;
            }
            Debug.Log(tileToDataDictionary.Keys.Count);
            
            
            for (int x = 0; x < staticData.Width; x++)
            {
                for (int y = 0; y < staticData.Height; y++)
                {

                    TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

                    if (tile == null) continue;
                    if (tileToDataDictionary.ContainsKey(tile))
                    {
                        _mapTiles[new Vector2Int(x, y)] = new DestructibleTileData
                        {
                            Durability = tileToDataDictionary[tile].Durability,
                            StaticData = tileToDataDictionary[tile],
                        };
                    }

                }
            }

            foreach (var enemyDataHolder in Object.FindObjectsByType<EnemyDataHolder>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                await SpawnEnemy(enemyDataHolder.EnemyStaticData, enemyDataHolder.transform.position);
            }
            foreach (var dialogTrigger in Object.FindObjectsByType<DialogTrigger>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                 dialogTrigger.Construct(_uiFactory);
            }
            foreach (var dialogTrigger in Object.FindObjectsByType<Finish>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                dialogTrigger.Construct(_gameStateMachine, _cameraController, _progressService, false);
            }
            
            
            _tilemapService.SetData(new Vector2Int(staticData.Width, staticData.Height),
                tilemaps, _mapTiles, _freeTiles, _ambientOcclusionRenderer, 1);
            
            
            await GenerateMapFloor(staticData, 0, seed, 0);


            _freeTiles.Clear();
        }
        


        public async Task InitMap()
        {
            
            GameObject grid = await _assets.Instantiate(AssetAddress.Grid);
            GameObject tilemap = await _assets.Load<GameObject>(AssetAddress.Tilemap);
            
            GameObject boundsTilemap = await _assets.Instantiate(AssetAddress.MapBoundsTilemap);
            GameObject ambientOcclusion = await _assets.Instantiate(AssetAddress.AmbientOcclusion);

            
            _grid = grid;
            
            boundsTilemap.transform.SetParent(grid.transform);
            ambientOcclusion.transform.SetParent(grid.transform);

            _ambientOcclusionRenderer = ambientOcclusion.GetComponent<SpriteRenderer>();
            _tilemapPrefab = tilemap;
            
            _boundsTilemap = boundsTilemap.GetComponent<Tilemap>();
        }

        public async Task<GameObject> CreateHud(LevelStaticData levelStaticData)
        {
            GameObject hud = await InstantiateAndRegister(AssetAddress.Hud);
            
            
            hud.GetComponentInChildren<HpBar>(true).Construct(
                _playerBase.GetComponent<Health>());
            hud.GetComponentInChildren<PlayerGetDamageFullscreenEffect>().Construct(_playerBase.GetComponent<Health>());

            //hud.GetComponentInChildren<EnergyBar>().Construct(_playerBase.GetComponent<Energy>());
            hud.GetComponentInChildren<FuelBar>(true).Construct(_playerBase.GetComponent<Fuel>());
            
            
            
            hud.GetComponentInChildren<PlayerResourcesPresenter>(true).Construct(_progressService, _staticData);
            hud.GetComponentInChildren<DarkAmethystPresenter>().Construct(_progressService);
            hud.GetComponentInChildren<OpenWindowButton>().Construct(_uiFactory);

            hud.GetComponentInChildren<HudTutorial>(true).Construct(_pauseService, _progressService);


            hud.GetComponentInChildren<ProgressBar>(true).Construct(_playerBase.transform,
                levelStaticData.Height*levelStaticData.chunks * GameConstants.MapBlockSize);

            
            
            
            BossDataPresenter bossDataPresenter = hud.GetComponentInChildren<BossDataPresenter>();
            if (levelStaticData.HaveBoss)
            {
                hud.GetComponentInChildren<ProgressBar>().gameObject.SetActive(false);

                bossDataPresenter.Show();
                bossDataPresenter.Construct(_bossMachine.GetComponent<Health>(), levelStaticData.BossData);
            }
            else bossDataPresenter.Hide();

            foreach (var buttonAnimator in hud.GetComponentsInChildren<SelectGroupButtonAnimator>(true))
            {
                Sprite sprite = null;
                foreach (var moduleInfo in _playerBase.Data.Modules)
                {
                    if (moduleInfo == null) continue;
                    if (moduleInfo.Group == buttonAnimator.Group)
                    {
                        sprite = await _assets.Load<Sprite>(moduleInfo.StaticData.CCIcon);
                    }
                }
                buttonAnimator.Construct(_inputService, _playerBase, sprite);
            }


            hud.GetComponentInChildren<FirstTutorialHUDController>(true).Construct(_progressService);

            RegisterBasic(hud);
            return hud;
        }

        public async Task SpawnEnemies(LevelStaticData staticData)
        {
            int currentEnemiesDifficulty = 0;

            int enemiesCol = staticData.EnemiesOnLevelStaticData.Length;
            int[] chances = new int[enemiesCol];
            int totalChances = 0;

            for (int i = 0; i < enemiesCol; i++)
            {
                chances[i] = staticData.EnemiesOnLevelStaticData[i].Probability;
                totalChances += chances[i];
            }

            EnemyStaticData enemyStaticData;
            int difficultyMinimumSum = staticData.EnemiesDifficultyMinimumSum;

            DifficultyLevel difficultyLevel = _progressService.Progress.DifficultyLevel;
            if (difficultyLevel == DifficultyLevel.Easy)
                difficultyMinimumSum = Mathf.RoundToInt(difficultyMinimumSum * 0.75f);
            if (difficultyLevel == DifficultyLevel.Hard)
                difficultyMinimumSum = Mathf.RoundToInt(difficultyMinimumSum * 1.2f);
            if (difficultyLevel == DifficultyLevel.Extreme)
                difficultyMinimumSum = Mathf.RoundToInt(difficultyMinimumSum * 1.4f);

            int bossesCurrentAmount = 0;
            int miniBossesAmount = 3;
            while (currentEnemiesDifficulty < difficultyMinimumSum)
            {
                int enemyId = _randomService.ChooseElementFromProbabilityArray(chances, totalChances);
                EnemyOnLevelStaticData onLevelStaticData = staticData.EnemiesOnLevelStaticData[enemyId];
                enemyStaticData = onLevelStaticData.EnemyStaticData;
                currentEnemiesDifficulty += onLevelStaticData.Difficulty;
                if (_freeTiles.Count == 0) break;

                int index = _randomService.Next(0, _freeTiles.Count);
                Vector2 position = (_freeTiles[index] + new Vector2(0.5f, 0.5f)) * GameConstants.MapBlockSize;
                _freeTiles.RemoveAt(index);

                if (miniBossesAmount > bossesCurrentAmount && onLevelStaticData.CanBeBoss)
                {
                    GameObject enemy = await SpawnEnemy(enemyStaticData, position, true);
                    
                    Drop drop = enemy.AddComponent<Drop>();
                    drop.Construct(_progressService, this, _staticData,
                        enemy.GetComponent<Death>(), _randomService, staticData);
                    bossesCurrentAmount++;
                }
                else
                {
                    await SpawnEnemy(enemyStaticData, position, false);
                }
            }
        }

        private async Task CreateMachineModule(MachineBase machineBase, MachineModuleStaticData staticData, int place,
            Direction direction, int group, int level, float k)
        {
            GameObject modulePrefab = await _assets.Load<GameObject>(staticData.ModulePrefabReference);
            GameObject moduleGameObject =
                InstantiateAndRegister(modulePrefab, machineBase.GetWorldPositionForModule(place));
            moduleGameObject.transform.localRotation = direction.ToQuaternion();

            RegisterBasic(moduleGameObject);

            MachineModule machineModule = moduleGameObject.GetComponent<MachineModule>();

            GameObject gameObjectWithAttacks;
            gameObjectWithAttacks = machineModule.gameObject;

            RegisterAttacks(gameObjectWithAttacks);
            
            machineModule.Construct(machineBase, group);
            machineModule.SetData(staticData, level, k);
            SetOwnerTeamInComponents(machineBase.Health, machineModule.gameObject);

            machineBase.RegisterModule(machineModule, place, direction, group,
                staticData.CanBeActivated, staticData);
        }

        public async Task<GameObject> SpawnEnemy(EnemyStaticData enemyStaticData, Vector2 at, bool miniBoss)
        {
            GameObject particlesPrefab = await _assets.Load<GameObject>(
                _staticData.GetAllEnemiesStaticData().MiniBossParticles);
            
            
            GameObject prefab = await _assets.Load<GameObject>(enemyStaticData.PrefabReference);

            GameObject enemyGameObject = InstantiateAndRegister(prefab, at);

            MobDataHolder dataHolder = enemyGameObject.GetComponent<MobDataHolder>();

            Dictionary<StatType, Stat> holderStats = enemyStaticData.Stats.ToDictionary();
            List<Dictionary<StatType, Stat>> additionalStats = new List<Dictionary<StatType, Stat>>();

            float k = 1f;
            if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Easy) k = 0.7f;
            if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Hard) k = 1.5f;
            if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Extreme) k = 2f;

            if (miniBoss)
            {
                k *= 2;
                enemyGameObject.transform.localScale = Vector3.one * 1.5f;
                enemyGameObject.name = enemyGameObject.name + "(Mini-Boss)";
                
                GameObject particlesGo = InstantiateAndRegister(particlesPrefab);
                particlesGo.transform.parent = enemyGameObject.transform;
                particlesGo.transform.position = enemyGameObject.transform.position;
            }

            foreach (var stat in holderStats)
            {
                if (stat.Key == StatType.Damage ||
                    stat.Key == StatType.HealthRegeneration)
                {
                    stat.Value.AddModifier(new Modifier {Value = Mathf.Sqrt(k)});
                }

                if (stat.Key == StatType.Health)
                {
                    stat.Value.AddModifier(new Modifier {Value = k*k});
                }
            }

            
            for (int i = 0; i < enemyStaticData.additionalStats.Length; i++)
            {
                Dictionary<StatType,Stat> stats = enemyStaticData.additionalStats[i].Stats.ToDictionary();
                foreach (var stat in stats)
                {
                    if (stat.Key == StatType.Damage || stat.Key == StatType.Health ||
                        stat.Key == StatType.HealthRegeneration)
                    {
                        stat.Value.AddModifier(new Modifier {Value = k});
                    }
                }
                additionalStats.Add(stats);
            }
            

            Health health = enemyGameObject.GetComponentInChildren<Health>();
            health.SetStartHealth(holderStats[StatType.Health].Value);

            health.Team = Team.Enemy;

            SetOwnerTeamInComponents(health, enemyGameObject);

            RegisterAttacks(enemyGameObject);
            RegisterBasic(enemyGameObject);

            dataHolder.PlayerMachineBase = _playerBase;
            dataHolder.Stats = holderStats;
            dataHolder.AdditionalStats = additionalStats;


            GameObject go = new GameObject("Optimizer: " + enemyGameObject.name);
            MobOptimizer optimizer = go.AddComponent<MobOptimizer>();
            
            optimizer.Construct(_cameraController, enemyGameObject);
            optimizer.transform.position = enemyGameObject.transform.position;

            return enemyGameObject;
        }

        public async Task SpawnEnemy(EnemyStaticData enemyStaticData, Vector2 at)
        {
            await SpawnEnemy(enemyStaticData, at, false);
        }

        public async Task<MachineBase> CreateMachine(Vector2 at, MachineBasementStaticData basementStaticData,
            ModulePresentableInfo[] modules, GameObject brainPrefab, Team team, 
            List<UpgradeType> upgrades, bool wrongShip)
        {
            float wrongModifier = 1f; 
            if (wrongShip) // check if ship is last
            {
                wrongModifier = 0.5f;
            }
            
            
            GameObject _basePrefab = await _assets.Load<GameObject>(basementStaticData.BasePrefabReference);
            GameObject instance = InstantiateAndRegister(_basePrefab, at);
            MachineBase _machineBase = instance.GetComponent<MachineBase>();
            
            MachineData machineData = new MachineData
            {
                Modules = new MachineModuleInfo[basementStaticData.ModulesMaxAmount],
                ModulePlacementInfo = basementStaticData.ModulePlacementInfos,
                MovingSpeed = basementStaticData.MoveSpeed.GetValue(
                    upgrades.Count(n => n == UpgradeType.Speed)) * wrongModifier,
                RotationSpeed = (float)basementStaticData.RotationSpeed.GetValue(
                    upgrades.Count(n => n == UpgradeType.RotationSpeed)) * wrongModifier,
                Fuel = (float)basementStaticData.Fuel.GetValue(
                    upgrades.Count(n => n == UpgradeType.Fuel)) * wrongModifier * wrongModifier
                
            };
            
            _machineBase.Construct(machineData, _inputService);

            Health health = _machineBase.GetComponent<Health>();
            health.Team = team;

            
            
            float k = 1f;
            if (team != Team.Player)
            {
                if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Easy) k = 0.80f;
                if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Hard) k = 1.25f;
                if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Extreme) k = 1.5f;
            }

            health.SetStartHealth( Mathf.RoundToInt((float) basementStaticData.Health.GetValue(
                upgrades.Count(n => n == UpgradeType.Health) + 1) * wrongModifier * wrongModifier));

            List<string> keys = new List<string>();
            foreach (var moduleInfo in modules)
            {
                int group=-1;

                if (moduleInfo.ModuleStaticData.CanBeActivated)
                {
                    if (keys.Contains(moduleInfo.ModuleStaticData.Id))
                    {
                        group = keys.IndexOf(moduleInfo.ModuleStaticData.Id);
                    }
                    else
                    {
                        group = keys.Count;
                        keys.Add(moduleInfo.ModuleStaticData.Id);
                    }
                }

                Direction dir = _staticData.ForMachineBasement(
                        _progressService.Progress.PlayerMachine.MachineBasementId)
                    .ModulePlacementInfos[moduleInfo.Place].Direction;
                await CreateMachineModule(_machineBase, moduleInfo.ModuleStaticData, moduleInfo.Place,
                    dir, group,  moduleInfo.Level, k);
            }

            GameObject machineBrain = InstantiateAndRegister(brainPrefab);
            machineBrain.transform.parent = instance.transform;
            machineBrain.transform.position = at;


            foreach (var arcRaycasterCircleCollider in
                machineBrain.GetComponentsInChildren<ArcRaycasterCircleCollider>(true))
            {
                arcRaycasterCircleCollider.Collider = _machineBase.GetComponent<Collider2D>();
            }


            MachineBasePlayerController playerController;
            if (machineBrain.TryGetComponent(out playerController))
                playerController.Construct(_inputService, _machineBase,  _progressService);
            MachineBaseReferenceHolder referenceHolder;
            if (machineBrain.TryGetComponent(out referenceHolder)) referenceHolder.MachineBase = _machineBase;
            MobDataHolder mobDataHolder;
            if (machineBrain.TryGetComponent(out mobDataHolder)) mobDataHolder.PlayerMachineBase = _playerBase;


            foreach (var machineModuleInfo in _machineBase.Data.Modules)
            {
                if (machineModuleInfo == null) continue;
                foreach (var initializable in machineModuleInfo.Module.GetComponentsInChildren<IInitializable>())
                {
                    initializable.Initialize();
                }
            }

            foreach (var raycaster in machineBrain.GetComponentsInChildren<Raycaster>(true))
            {
                raycaster.Health = health;
            }

            
            _machineBase.Initialize();
            return _machineBase;
        }

        public async Task<MachineBase> CreatePlayerMachine(Vector2 at,
            MachineBasementStaticData machineBasementStaticData,
            ModulePresentableInfo[] modules, List<UpgradeType> upgrades)
        {
            GameObject brainPrefab = await _assets.Load<GameObject>(AssetAddress.PlayerMachineBrain);

            bool isWrong = !(_progressService.Progress.PlayerMachine ==
                             _progressService.Progress.PlayerMachines[_progressService.Progress.PlayerMachines.Count - 1]);

            _playerBase = await CreateMachine(at, machineBasementStaticData, modules, brainPrefab, Team.Player, 
                upgrades, isWrong);
            return _playerBase;
        }
        

        public async Task SpawnTileDestructionParticles(Vector2Int tilePosition, DestructibleTileData tileData)
        {
            // TODO
            if (_randomService.Roulette(0.5f))
            {
                return;
            }
            GameObject particlesPrefab = await _assets.Load<GameObject>(AssetAddress.TileDestroyingParticles);

            GameObject particles = Object.Instantiate(particlesPrefab);
            particles.transform.position = new Vector3(tilePosition.x + 0.5f, tilePosition.y + 0.5f, 0f) *
                                           GameConstants.MapBlockSize;

            ParticleSystem particleSystem = particles.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule mainModule = particleSystem.main;

            mainModule.startColor = new ParticleSystem.MinMaxGradient(
                tileData.StaticData.ParticleColors[Random.Range(0, tileData.StaticData.ParticleColors.Count)]);

            Object.Destroy(particles, 1f);
        }


        public async Task<IBullet> CreateProjectile(Transform startPoint, GameObject asset,
            Dictionary<StatType, Stat> stats, Health ownerHealth)
        {
            Vector2 position = startPoint.position;
            Quaternion rotation = startPoint.rotation;
            GameObject prefab = asset;
            GameObject instance = InstantiateAndRegister(prefab);
            Transform instanceTransform = instance.transform;
            instanceTransform.position = position;
            instanceTransform.rotation = rotation;

            RegisterBasic(instance);
            RegisterAttacks(instance);

            ProjectileDataHolder dataHolder;
            if (instance.TryGetComponent(out dataHolder))
            {
                dataHolder.Stats = stats;
            }

            SetOwnerTeamInComponents(ownerHealth, instance);

            Bullet bullet;
            if (instance.TryGetComponent(out bullet)) bullet.Construct(_tilemapService);

            Boomerang boomerang;
            if (instance.TryGetComponent(out boomerang)) boomerang.Construct(_tilemapService, startPoint);

            IBullet ibullet;
            if (instance.TryGetComponent(out ibullet))
            {
                ibullet.OwnerTeam = ownerHealth.Team;
                return ibullet;
            }

            return null;
        }

        private async Task SpawnAmbientObject(LevelStaticData staticData)
        {
            if (staticData.Ambient == null) return;

            GameObject prefab = await _assets.Load<GameObject>(staticData.Ambient);
            if (prefab)
            {
                GameObject gameObject = InstantiateAndRegister(prefab);
                gameObject.transform.position = staticData.LevelCenterWorldPosition;
                AmbientMusic audio;
                if (gameObject.TryGetComponent(out audio))
                {
                    audio.Construct(_audioService);
                }
            }
        }

        private async Task SpawnBoss(LevelStaticData staticData)
        {
            MachineStaticData bossMachineStaticData = staticData.BossData.MachineStaticData;
            Vector2 position = staticData.LevelCenterWorldPosition;
            GameObject bossDataBrainPrefab = await _assets.Load<GameObject>(staticData.BossData.BrainPrefab);
            MachineBase machineBase = await CreateMachine(position, bossMachineStaticData.BasementStaticData,
                bossMachineStaticData.Modules,
                bossDataBrainPrefab, Team.Enemy, new List<UpgradeType>(), false);
            //machineBase.Data.EnergyRegeneration = 99999f;
            _bossMachine = machineBase;

            machineBase.GetComponent<Death>().Happen += () => { OnBossDeath(staticData); };
        }

        private async Task OnBossDeath(LevelStaticData levelStaticData)
        {
            Vector2Int pos = await CreateFinish(levelStaticData);
            _tilemapService.DamageWithBlast((Vector2) pos * GameConstants.MapBlockSize,
                200, 20);
        }

        private void ClearLevel()
        {
            _freeTiles = new List<Vector2Int>();
            _mapTiles = new Dictionary<Vector2Int, DestructibleTileData>();

            
            //if (_tilemap) _tilemap.ClearAllTiles();
        }

        private static void SetOwnerTeamInComponents(Health health, GameObject gameObject)
        {
            if (health)
            {
                foreach (var attack in gameObject.GetComponentsInChildren<Attack>())
                {
                    attack.OwnerHealth = health;
                }

                foreach (var raycaster in gameObject.GetComponentsInChildren<Raycaster>())
                {
                    raycaster.Health = health;
                }

                foreach (var obj in gameObject.GetComponentsInChildren<INeedTeam>())
                {
                    obj.Team = health.Team;
                }
            }
        }

        private async Task SpawnDeathZone(float width, Vector2 position)
        {
            GameObject prefab = await _assets.Load<GameObject>(AssetAddress.DeathZone);
            GameObject instance = InstantiateAndRegister(prefab, position);
            instance.GetComponent<DeathZone>().SetWidth(width);
            instance.GetComponent<DeathZone>().Construct(_playerBase.transform, _progressService);
        }

        private async Task GenerateMapFloor(LevelStaticData levelStaticData, int stage, int seed, int chunk)
        {
            GameObject floorTilemap = await _assets.Instantiate(
                levelStaticData.BGData.Type == BackgroundData.BGType.Tile 
                    ? AssetAddress.FloorTilemap 
                    : AssetAddress.FloorSprite
            );

            //floorTilemap.transform.parent = _tilemap.transform.parent;
            _floorGO = floorTilemap;
            
            if (levelStaticData.BGData.Type == BackgroundData.BGType.Tile)
            {

                var floorTile = await _assets.Load<TileBase>(levelStaticData.BGData.FloorTile);

                List<Vector3Int> positionsArray = new List<Vector3Int>();
                List<TileBase> tileBases = new List<TileBase>();

                for (int x = 0; x < levelStaticData.Width; x++)
                {
                    for (int y = 0; y < levelStaticData.Height; y++)
                    {
                        positionsArray.Add(new Vector3Int(x, y, 0));
                        tileBases.Add(floorTile);
                    }
                }

                _floorGO.GetComponent<Tilemap>().SetTiles(positionsArray.ToArray(), tileBases.ToArray());
            }
            else
            {
                
                var floorSprite = await _assets.Load<Sprite>(levelStaticData.BGData.Sprite);
                var renderer = _floorGO.GetComponent<SpriteRenderer>();
                
                renderer.sprite = floorSprite;
                
                renderer.size = new Vector2(levelStaticData.Width * GameConstants.MapBlockSize,
                    levelStaticData.Height * GameConstants.MapBlockSize);
                renderer.transform.position = new Vector3(levelStaticData.Width / 2f * GameConstants.MapBlockSize,
                    levelStaticData.Height / 2f * GameConstants.MapBlockSize, 0f);

                renderer.transform.position = renderer.transform.position + Vector3.up *GameConstants.MapBlockSize* levelStaticData.Height * chunk;

                var par = _floorGO.GetComponent<Parallax>();
                par.Construct(_cameraController);

            }
        }

        private async Task GenerateMapBounds(LevelStaticData data)
        {
            int tileWidth = GameConstants.BoundsTileWidth;
            TileBase tile = await _assets.Load<TileBase>(data.BoundsTile);

            int tilesNumber = (data.Height * data.chunks + data.Width) * tileWidth * 2 + tileWidth * tileWidth * 4;
            Vector3Int[] positions = new Vector3Int[tilesNumber];

            int index = 0;
            for (int x = -tileWidth; x < data.Width + tileWidth; x++)
            {
                for (int y = -tileWidth; y < 0; y++)
                {
                    positions[index] = new Vector3Int(x, y, 0);
                    index++;
                }

                for (int y = data.Height*data.chunks; y < data.Height * data.chunks + tileWidth; y++)
                {
                    positions[index] = new Vector3Int(x, y, 0);
                    index++;
                }
            }

            for (int y = 0; y < data.Height*data.chunks; y++)
            {
                for (int x = -tileWidth; x < 0; x++)
                {
                    positions[index] = new Vector3Int(x, y, 0);
                    index++;
                }

                for (int x = data.Width; x < data.Width + tileWidth; x++)
                {
                    positions[index] = new Vector3Int(x, y, 0);
                    index++;
                }
            }

            TileBase[] tileArray = new TileBase[tilesNumber];
            for (int i = 0; i < tilesNumber; i++) tileArray[i] = tile;

            var tilemap = InstantiateAndRegister(_tilemapPrefab);
            tilemap.transform.parent = _grid.transform;
            tilemap.GetComponent<Tilemap>().SetTiles(positions, tileArray);
        }

        private async Task<Tilemap> GenerateMap(LevelStaticData levelStaticData, int stage, int seed,
            int chunk)
        {
            Vector2Int[] positions;
            Vector2Int[] sizes;
            if (!levelStaticData.HaveBoss)
            {
                if (chunk == levelStaticData.chunks-1)
                {
                    var finishTilePosition = await CreateFinish(levelStaticData);
                    Vector2Int zone = new Vector2Int(GameConstants.FinishTileSize, GameConstants.FinishTileSize);
                    positions = new Vector2Int[] {finishTilePosition};
                    sizes = new Vector2Int[] {zone};
                    
                }else
                {
                    sizes = new Vector2Int[] {};
                    positions = new Vector2Int[] {};
                }
                
            }
            else
            {
                Vector2Int zone = new Vector2Int(levelStaticData.BossData.SafeZone.x,
                    levelStaticData.BossData.SafeZone.y);
                positions = new Vector2Int[] {levelStaticData.LevelCenterTilePosition};
                sizes = new Vector2Int[] {zone};
            }
            

            MapData mapData = _mapGenerator.GetMap(seed, levelStaticData, positions, sizes, chunk);

            var stoneTile = await _assets.Load<TileBase>(levelStaticData.StoneTile.BaseTile);

            List<Vector3Int> positionsArray = new List<Vector3Int>();
            List<TileBase> tileBases = new List<TileBase>();
            Vector2Int chunkOffset = Vector2Int.up * chunk * levelStaticData.Height;
            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = 0; y < mapData.Height; y++)
                {
                    Vector2Int vector2Int = new Vector2Int(x, y);
                    if (mapData.BaseMap[x, y] == MapBlockType.Stone)
                    {
                        await SetTile(tileBases, positionsArray, vector2Int + chunkOffset, levelStaticData.StoneTile);
                    }
                    else if (mapData.BaseMap[x, y] == MapBlockType.Ore)
                    {
                        MapBlockStaticData oreData = levelStaticData.OresGenerationData[mapData.OresMap[x, y]].Ore;
                        TileBase tileBase = await _assets.Load<TileBase>(oreData.BaseTile);
                        await SetTile(tileBases, positionsArray, vector2Int + chunkOffset, oreData);
                    }
                    else if (mapData.BaseMap[x, y] == MapBlockType.Air)
                    {
                        _freeTiles.Add(vector2Int + chunkOffset);
                    }
                }
            }


            foreach (var structureData in mapData.Structures)
            {
                StructureStaticData data = levelStaticData.StructuresOnLevelStaticData[structureData.Id]
                    .StructureStaticData;
                for (int x = 0; x < data.GetSize(structureData.Rotation).x; x++)
                {
                    for (int y = 0; y < data.GetSize(structureData.Rotation).y; y++)
                    {
                        int blockId = data.GetBlockId(structureData.Rotation, x, y);
                        if (blockId <= 0) continue;
                        Vector2Int position = new Vector2Int(x, y) + structureData.Position;
                        //_freeTiles.Remove(position);
                        
                        await SetTile(tileBases, positionsArray, position + chunkOffset,
                            data.GetMapBlockById(blockId));
                    }
                }
            }

            for (int i = 0; i < positionsArray.Count; i++)
            {
                positionsArray[i] = positionsArray[i];
            }
            
            
            var tilemap = InstantiateAndRegister(_tilemapPrefab).GetComponent<Tilemap>();
            tilemap.transform.parent = _grid.transform;
            tilemap.SetTiles(positionsArray.ToArray(), tileBases.ToArray());

            ChunkController controller = tilemap.gameObject.AddComponent<ChunkController>();
            controller.Construct(_cameraController, tilemap, (levelStaticData.Height / 2 + chunkOffset.y) * GameConstants.MapBlockSize);
            return tilemap;
        }

        private async Task SetTile(List<TileBase> tileBases, List<Vector3Int> positionsArray,
            Vector2Int tilePosition, MapBlockStaticData blockData)
        {
            tileBases.Add(await _assets.Load<TileBase>(blockData.BaseTile));
            positionsArray.Add((Vector3Int) tilePosition);
            
            
            _mapTiles[tilePosition] = new DestructibleTileData
            {
                Durability = blockData.Durability,
                StaticData = blockData,
            };
        }

        private async Task<Vector2Int> CreateFinish(LevelStaticData levelStaticData)
        {
            int finishExtends = Mathf.CeilToInt((float) GameConstants.FinishTileSize / 2);
            int xFinishPosition = _randomService.Next(finishExtends, levelStaticData.Width - finishExtends);
            Vector2Int finishTilePosition = new Vector2Int(xFinishPosition, levelStaticData.Height * levelStaticData.chunks - finishExtends);
            GameObject finishPrefab = await _assets.Load<GameObject>(AssetAddress.Finish);

            GameObject finishGameObject = InstantiateAndRegister(finishPrefab,
                (Vector2) finishTilePosition * GameConstants.MapBlockSize);
            finishGameObject.GetComponent<Finish>().Construct(_gameStateMachine, _cameraController,
                _progressService, levelStaticData.EndLevel);

            return finishTilePosition;
        }

        #region Regestration

        private void RegisterAttacks(GameObject gameObjectWithAttacks)
        {
            foreach (MapBlockDestroyer destroyer in gameObjectWithAttacks.GetComponentsInChildren<MapBlockDestroyer>())
                destroyer.Construct(_tilemapService);
            foreach (AttackWithProjectile projectile in gameObjectWithAttacks
                .GetComponentsInChildren<AttackWithProjectile>())
                projectile.Construct(this);
        }

        private void RegisterBasic(GameObject gameObject)
        {
            foreach (CameraShake component in gameObject.GetComponentsInChildren<CameraShake>())
                component.Construct(_cameraController);
        }

        private void Register(ISavedProgressReader progressReader)
        {
            if (progressReader is ISavedProgress progressWriter)
            {
                ProgressWriters.Add(progressWriter);
            }

            ProgressReaders.Add(progressReader);
        }

        private void RegisterProgressWatchers(GameObject gameObject)
        {
            foreach (ISavedProgressReader progressReader in gameObject.GetComponentsInChildren<ISavedProgressReader>())
            {
                Register(progressReader);
            }
        }

        private async Task<GameObject> InstantiateAndRegister(string address, Vector3 transformPosition)
        {
            var gameObject = await _assets.Instantiate(address, transformPosition);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        private async Task<GameObject> InstantiateAndRegister(string address)
        {
            var gameObject = await _assets.Instantiate(address);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        private GameObject InstantiateAndRegister(GameObject prefab, Vector3 at)
        {
            var gameObject = Object.Instantiate(prefab, at, Quaternion.identity);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        private GameObject InstantiateAndRegister(GameObject prefab)
        {
            var gameObject = Object.Instantiate(prefab);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        #endregion
    }
}