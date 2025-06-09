using System.Collections.Generic;
using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Infrastructure.Data.Stats;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.CodeBase.Logic.DynamicDataLogic.DataHolders
{
    public class BaseDataHolder : MonoBehaviour
    {
        public Dictionary<StatType, Stat> Stats = new SerializableDictionary<StatType, Stat>();
        [FormerlySerializedAs("additionalStats")] public List<Dictionary<StatType, Stat>> AdditionalStats;
    }
    

    public enum StatType
    {
        Health=0, Duration=1, Damage=2, DestructiveCapacity=3, ReloadTime=4, Range=5,
        MoveSpeed=6, AdjustMaxHealth=7, ActivationReload=8,
        DashSpeed=9,
        FocusTime=10,
        Knockback=11, HealthRegeneration=12, EnergyRegeneration=13,
        HealthRegenerationPercent=14,
        SplashRadius=15,


        SpeedUpCCReload=17,
    }
    
}