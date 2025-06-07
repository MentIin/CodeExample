using System;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Data.Stats
{
    [Serializable]
    public class SerializableStat
    {
        [HideInInspector]public string name;
        public StatType Type;
        public int Value;
        public void Validate()
        {
            name = Type.ToString();
        }
    }
}