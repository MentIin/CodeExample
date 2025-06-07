using System.Collections.Generic;
using Assets.CodeBase.Infrastructure.Data.Modules;
using Assets.CodeBase.Infrastructure.Data.Stats;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using CodeBase.Infrastructure.Data.Modules;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Data
{
    public static class DataExtensions
    {
        public static Vector2Data AsVector2Data(this Vector2 vector2) =>
            new Vector2Data(vector2.x, vector2.y);

        public static Vector2 AsUnityVector2(this Vector2Data vector2Data) =>
            new Vector2(vector2Data.X, vector2Data.Y);

        public static Vector3 Absolute(this Vector3 vector) => new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y),
            Mathf.Abs(vector.z));

        public static ModuleSerializableInfo AsModuleSerializableInfo(this ModulePresentableInfo presentableInfo) =>
            new ModuleSerializableInfo
            {
                Place = presentableInfo.Place,
                Id = presentableInfo.ModuleStaticData.Id
            };
        public static List<ModuleSerializableInfo> AsModuleSerializableInfo(this ModulePresentableInfo[] presentableInfo)
        {
            List<ModuleSerializableInfo> res = new List<ModuleSerializableInfo>();

            for (int i = 0; i < presentableInfo.Length; i++)
            {
                res.Add(presentableInfo[i].AsModuleSerializableInfo());
            }

            return res;
        }

        public static Quaternion ToQuaternion(this Direction direction)
        {
            float z = (float)direction;
            return Quaternion.Euler(0, 0, z);
        }

        public static string AsJson(this object obj) => JsonUtility.ToJson(obj);
        
        public static T ToDeserialized<T>(this string json) => JsonUtility.FromJson<T>(json);
        
        public static Dictionary<StatType, Stat> ToDictionary(this List<SerializableStat> serializableStats)
        {
            Dictionary<StatType, Stat> res = new Dictionary<StatType, Stat>();

            foreach (var serializableStat in serializableStats)
            {
                res[serializableStat.Type] = new Stat();
                res[serializableStat.Type].SetBaseValue(serializableStat.Value);
            }
            return res;
        }

        public static float FromMillisecondsToSeconds(this int mil) => (float) mil / 100;
    }
}