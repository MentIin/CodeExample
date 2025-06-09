using UnityEngine;

namespace Assets.CodeBase.Logic.Effects
{
    public class Resistance : MonoBehaviour
    {
        [Header("KnockBack")]
        [Range(0f, 1f)] public float Knockback = 0f;

        public bool Flying = false;
    }
}