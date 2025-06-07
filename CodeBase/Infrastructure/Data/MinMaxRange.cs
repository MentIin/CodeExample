using System;
using UnityEngine;

namespace CodeBase.Infrastructure.Data
{
    [Serializable]
    public class MinMaxRange
    {
        public int Min;
        public int Max;


        /// <param name="pos"> [0, 1]</param>
        public float GetValue(float pos)
        {
            return (float)Min + (float)(Max - Min) * pos;
        }
    }
    [Serializable]
    public class MinMaxRange<T>
    {
        public T Min;
        public T Max;
    }
}