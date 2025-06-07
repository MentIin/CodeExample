using System.Collections.Generic;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Data.Stats
{
    public class Stat
    {
        private int _baseValue;

        private List<Modifier> _modifiers = new List<Modifier>();
        public int Value
        {
            get
            {
                int res = _baseValue;
                foreach (var modifier in _modifiers)
                {
                    res = Mathf.RoundToInt(res * modifier.Value);
                }
                return res;
            }
        }

        public int BaseValue
        {
            get => _baseValue;
        }

        public void SetBaseValue(int value)
        {
            _baseValue = value;
        }

        public void AddModifier(Modifier modifier)
        {
            _modifiers.Add(modifier);
        }
    }
}