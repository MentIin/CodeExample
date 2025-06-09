using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.CodeBase.Logic.AttackLogic.Projectiles
{
    public class AddSpreadToProjectile : MonoBehaviour
    {
        [SerializeField] private Transform _firePoint;
        [SerializeField] private float _maxSpread = 20f;
        [SerializeField] private Attack _attack;


        private void Awake()
        {
            _attack.Done += AttackOnDone;
        }

        private void AttackOnDone()
        {
            _firePoint.localEulerAngles = new Vector3(0f, 0f, Random.Range(-_maxSpread, _maxSpread));
        }
    }
}