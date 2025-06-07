using Assets.CodeBase.Logic.DynamicDataLogic;
using Assets.CodeBase.Sounds;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Services.AudioService
{
    public class PlaySoundWhenDamaged : MonoBehaviour
    {
        [SerializeField] private PlaySound _playSound;
        [SerializeField] private Health _health;

        private void Awake()
        {
            _health.DamageTaken += HealthOnDamageTaken;
        }

        private void HealthOnDamageTaken(int damage)
        {
            _playSound.PlayOneShot();
        }
    }
}