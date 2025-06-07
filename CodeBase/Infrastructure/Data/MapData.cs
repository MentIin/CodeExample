using System.Collections.Generic;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Data
{
    public class MapData
    {
        public int Width;
        public int Height;

        public MapBlockType[,] BaseMap;
        public int[,] OresMap;
        public List<GeneratedStructureData> Structures;
    }

    public class GeneratedStructureData
    {
        public int Id;
        public Vector2Int Position;
        public Direction Rotation;
    }
}