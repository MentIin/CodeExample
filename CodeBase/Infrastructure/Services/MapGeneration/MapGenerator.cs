using System;
using System.Collections.Generic;
using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Infrastructure.Services.MapGeneration.NoiseMap;
using Assets.CodeBase.Infrastructure.Services.Random;
using Assets.CodeBase.StaticData.Level;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Services.MapGeneration
{
    class MapGenerator : IMapGenerator
    {
        private readonly NoiseMapGenerator _noiseMapGenerator;
        private readonly IRandomService _randomService;

        public MapGenerator(IRandomService randomService)
        {
            _noiseMapGenerator = new NoiseMapGenerator();
            _randomService = randomService;
        }

        public MapData GetMap(int seed, LevelStaticData staticData, Vector2Int[] safeZonesPositions,
            Vector2Int[] safeZoneSizes, int chunk)
        {
            MapData data = new MapData
            {
                Width = staticData.Width,
                Height = staticData.Height,
                Structures = new List<GeneratedStructureData>()
            };

            float[] noiseMap = _noiseMapGenerator.GenerateNoiseMap(staticData.Width, staticData.Height, seed,
                staticData.Scale, staticData.Octaves, staticData.Persistence, staticData.Lacunarity, 
                Vector2.up * staticData.Height* chunk);

            if (chunk == 0)
            {
                noiseMap = CreateSafeZone(staticData.PlayerStartingTilePosition, staticData.SafeZone, noiseMap, staticData.Width,
                    staticData.Height, staticData.SafeZoneTransitionLength);
            }
            

            for (int i = 0; i < safeZonesPositions.Length; i++)
            {
                noiseMap = CreateSafeZone(safeZonesPositions[i] + Vector2Int.down * data.Height * chunk, safeZoneSizes[i], noiseMap, staticData.Width,
                    staticData.Height, 5);
            }

            data.BaseMap = GenerateStoneFromNoise(noiseMap, staticData);

            GenerateOres(data, staticData);
            
            GenerateStructures(data, staticData);
            
            return data;
        }

        private void GenerateStructures(MapData mapData, LevelStaticData staticData)
        {
            int index = 0;
            foreach (var structureOnLevelStaticData in staticData.StructuresOnLevelStaticData)
            {
                StructureStaticData data = structureOnLevelStaticData.StructureStaticData;
                for (int i = 0; i < structureOnLevelStaticData.MaxAmount; i++) {
                    if (_randomService.Roulette(structureOnLevelStaticData.Chance)) {
                        int attems = 0;
                        while (attems < 50000)
                        {
                            Direction direction = _randomService.GetDirection();
                            int xPos = _randomService.Next(0, staticData.Width - data.GetSize(direction).x + 1);
                            int yPos = _randomService.Next(0, staticData.Height - data.GetSize(direction).y + 1);
                            if (CheckIfCanPlaceStructure(xPos, yPos, data, mapData, staticData, direction))
                            {
                                GenerateStructure(mapData, index, xPos, yPos, direction, data);
                                break;
                            }
                            attems ++ ;
                        }
                    }
                }
                index++;
            }
            
        }

        private static void GenerateStructure(MapData mapData, int index, int xPos, int yPos,
            Direction direction, StructureStaticData structureStaticData)
        {
            mapData.Structures.Add(new GeneratedStructureData
            {
                Id = index,
                Position = new Vector2Int(xPos, yPos), Rotation = direction
            });
            Vector2Int size = structureStaticData.GetSize(direction);
            
            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    int currentXPos = xPos + x;
                    int currentYPos = yPos + y;
                    int cellId = structureStaticData.GetBlockId(direction, x, y);
                    if (cellId > 0) mapData.BaseMap[currentXPos, currentYPos] = MapBlockType.SafeZone;
                    else if (cellId == 0) mapData.BaseMap[currentXPos, currentYPos] = MapBlockType.Air;
                    
                    
                }
            }
        }

        private bool CheckIfCanPlaceStructure(int xPos, int yPos, StructureStaticData data,
            MapData mapData, LevelStaticData levelStaticData, Direction direction)
        {
            Vector2Int checkedStructureSize = data.GetSize(direction);
            Bounds checkedStructureBounds = new Bounds(new Vector2(xPos, yPos) + checkedStructureSize / 2,
                (Vector2)checkedStructureSize);

            foreach (var structureData in mapData.Structures)
            {
                Vector2Int size = levelStaticData.StructuresOnLevelStaticData[structureData.Id]
                    .StructureStaticData.GetSize(structureData.Rotation);
                Bounds structureBounds = new Bounds((Vector2)structureData.Position + size / 2,
                    (Vector2)size);
                if (structureBounds.Intersects(checkedStructureBounds)) return false;
            }
            
            for (int x = 0; x < checkedStructureSize.x; x++)
            {
                for (int y = 0; y < checkedStructureSize.y; y++)
                {
                    int currentXPos = xPos + x;
                    int currentYPos = yPos + y;
                    int cellId = data.GetBlockId(direction, x, y);
                    if (cellId > 0)
                    {
                        if (mapData.BaseMap[currentXPos, currentYPos] == MapBlockType.SafeZone) return false;
                        
                    }
                    else if (cellId == -1)
                    {
                        if (mapData.BaseMap[currentXPos, currentYPos] != MapBlockType.Air)
                        {
                            return false;
                        }
                    }else if (cellId == -2)
                    {
                        if (mapData.BaseMap[currentXPos, currentYPos] == MapBlockType.Air)
                        {
                            return false;
                        }
                    }
                    
                }
            }

            return true;
        }

        private void GenerateOres(MapData data, LevelStaticData staticData)
        {
            List<Vector2Int> positionsForOres = GetPositionsForOres(data);

            data.OresMap = new int[data.Width, data.Height];
            int oreId = 0;
            foreach (var oreData in staticData.OresGenerationData)
            {
                
                int amount =  _randomService.Next(oreData.VeinAmount.Min, oreData.VeinAmount.Max);
                for (int i = 0; i < amount; i++)
                {
                    GenerateOreVein(data, oreData, positionsForOres, oreId);
                }
                oreId++;
            }
        }

        private void GenerateOreVein(MapData data, OreGenerationData oreData, List<Vector2Int> positions, int oreId)
        {
            int size = _randomService.Next(oreData.VeinSize.Min, oreData.VeinSize.Max);
            if (positions.Count == 0) return;
            Vector2Int currentPosition = positions[_randomService.Next(0, positions.Count)];
            List<Vector2Int> activeTiles = new List<Vector2Int>();

            bool noSpace = false;
            for (int i = 0; i < size; i++)
            {
                data.BaseMap[currentPosition.x, currentPosition.y] = MapBlockType.Ore;
                data.OresMap[currentPosition.x, currentPosition.y] = oreId;
                positions.Remove(currentPosition);
                
                activeTiles.Add(currentPosition);
                
                while (true)
                {
                    if (activeTiles.Count == 0)
                    {
                        noSpace = true;
                        break;
                    }
                    Vector2Int choosen = activeTiles[_randomService.Next(0, activeTiles.Count)];
                    List<Vector2Int> free = GetOreSurroundingFreeTiles(choosen, data);
                    if (free.Count == 0)
                    {
                        activeTiles.Remove(choosen);
                    }
                    else
                    {
                        currentPosition = _randomService.Choose(free.ToArray());
                        break;
                    }
                }
                if (noSpace) break;
            }
        }

        private List<Vector2Int> GetOreSurroundingFreeTiles(Vector2Int pos, MapData data)
        {
            List<Vector2Int> res = new List<Vector2Int>();
            for (int xDif = -1; xDif <= 1; xDif++)
            {
                for (int yDif = -1; yDif <= 1; yDif++)
                {
                    if(xDif + yDif == 0) continue;
                    
                    int x = pos.x + xDif;
                    int y = pos.y + yDif;
                    if (x < 0 || y < 0 || x >= data.Width || y >= data.Height) continue;
                    if (data.BaseMap[x, y] == MapBlockType.Stone)
                    {
                        res.Add(new Vector2Int(x, y));
                    }
                }
            }

            return res;
        }

        private static List<Vector2Int> GetPositionsForOres(MapData data)
        {
            List<Vector2Int> positionsForOres = new List<Vector2Int>();

            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    if (data.BaseMap[x, y] == MapBlockType.Stone)
                    {
                        positionsForOres.Add(new Vector2Int(x, y));
                    }
                }
            }
            return positionsForOres;
        }

        private MapBlockType[,] GenerateStoneFromNoise(float[] noiseMap, LevelStaticData data)
        {
            int width = data.Width;
            int height = data.Height;
            
            MapBlockType[,] map = new MapBlockType[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float noise = noiseMap[y * width + x];
                    if (noise >= data.StoneLevel)
                    {
                        map[x, y] = MapBlockType.Stone;
                    }else if (Math.Abs(noise + 10f) <= 0.1f)
                    {
                        map[x, y] = MapBlockType.SafeZone;
                    }
                }
            }
            return map;
        }

        private float[] CreateSafeZone(Vector2Int position, Vector2Int zone,
            float[] noiseMap, int width, int height, float transitionLenght)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int index = y * width + x;
                    float noise = noiseMap[index];
                    float safeXDist = Mathf.Max(0, Mathf.Abs(position.x - x) - zone.x / 2);
                    float safeYDist = Mathf.Max(0, Mathf.Abs(position.y - y) - zone.y / 2);

                    float safeZoneModifier = MathF.Sqrt(safeXDist * safeXDist + safeYDist * safeYDist) / transitionLenght;
                    if (safeZoneModifier > 1) safeZoneModifier = 1;
                    if (safeZoneModifier <= 0) noiseMap[index] = -10;
                    else noiseMap[index] = noise * safeZoneModifier;
                }
            }

            return noiseMap;
        }
    }
}