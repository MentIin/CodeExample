using Assets.CodeBase.StaticData.Level;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Factory
{
    public class DestructibleTileData
    {
        public int Durability;
        public int CurrentDamage;

        public bool Destroyed => CurrentDamage > Durability;
        

        public MapBlockStaticData StaticData;
        public int LastLevelOfDestruction = 0;
        
        public int LinkedTileChunk;

        public void GetDamage(int damage)
        {
            CurrentDamage += damage;
        }
    }
}