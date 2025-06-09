using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using UnityEngine;

namespace Assets.CodeBase.Logic.MapEditing
{
    public class BlastMapDestroyer : MapBlockDestroyer
    {
        public override void DestroyMap()
        {
            Vector2 position = (Vector2)transform.position;

            tilemapService.DamageWithBlast(position, DataHolder.Stats[StatType.DestructiveCapacity].Value,
                DataHolder.Stats[StatType.SplashRadius].Value);
        }
    }
}