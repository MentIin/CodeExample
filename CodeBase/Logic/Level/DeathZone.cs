using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Infrastructure.Data.PlayerData;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Logic;
using Assets.CodeBase.Logic.DynamicDataLogic;
using CodeBase.Logic.Markers;
using UnityEngine;

namespace CodeBase.Logic.Level
{
    public class DeathZone : MonoBehaviour
    {
        public int MaxLength=100;
        private float Damage=10f;
        public float Reload = 1f;
        public float ExpandReload=1f;
        public float PreparationsTime=3f;
        public LayerMask LayerMask;
        
        [SerializeField] private BoxCollider2D _boxCollider;
        [Header("Visual")]
        [SerializeField] private SpriteRenderer _renderer;
        public float ParticlesEmmision=40f;
        [SerializeField] private ParticleSystem _particles;

        private GameTimer _damageTimer;
        private GameTimer _expandTimer;
        private Collider2D[] _hitColliders;
        private float _current;

        private Transform _playerTransform;
        private float _startSpeedUpDistance=22f;
        private IPersistentProgressService _progressService;


        private ImmuneToDeathZone _immune;

        public void Construct(Transform playerTransform, IPersistentProgressService progressService)
        {
            _playerTransform = playerTransform;
            _progressService = progressService;

            if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Easy)
            {
                ExpandReload *= 1.2f;
                Reload *= 1f;
            }else if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Hard)
            {
                Reload *= 0.5f;
                ExpandReload *= 0.85f;
            }else if (_progressService.Progress.DifficultyLevel == DifficultyLevel.Extreme)
            {
                Reload *= 0.25f;
                ExpandReload *= 0.7f;
            }
        }
        
        
        private void Awake()
        {
            _damageTimer = new GameTimer();
            _expandTimer = new GameTimer();
            _expandTimer.SetTime(ExpandReload + PreparationsTime);
            _damageTimer.SetTime(Reload);
            _hitColliders = new Collider2D[100];
            SetHeight(0f);
            _renderer.size = new Vector2(_renderer.size.x, 0);
        }
        
        
        

        private void FixedUpdate()
        {
            if (_playerTransform == null) return;;
            int k = 1 + Mathf.FloorToInt((_playerTransform.position.y - _current) / _startSpeedUpDistance);
            if (k < 1) k = 1;

            _damageTimer.Tick(Time.fixedDeltaTime * k);
            _expandTimer.Tick(Time.fixedDeltaTime * k);
            
            
            if (_damageTimer.Over)
            {
                _damageTimer.SetTime(Reload);
                DealDamage();
            }

            if (_expandTimer.Over)
            {
                _expandTimer.SetTime(ExpandReload);
                SetHeight(_current + GameConstants.MapBlockSize);
            }
        }

        private void Update()
        {
            //_renderer.size = new Vector2(_renderer.size.x, Mathf.Lerp(_renderer.size.y, _current, 0.1f));
        }

        private void DealDamage()
        {
            _hitColliders = Physics2D.OverlapBoxAll(_boxCollider.bounds.center,
                _boxCollider.bounds.size, 0, LayerMask);
            if (_hitColliders.Length > 0)
            {
                foreach (var collider in _hitColliders)
                {
                    if (collider)
                    {
                        Health health = collider.GetComponent<Health>();
                        if (health)
                        {
                            if (health.Team == Team.Player || health.Team == Team.Enemy)
                            {
                                if (health.TryGetComponent(out _immune))
                                {
                                    continue;
                                }
                                health.TakeDamagePercent(Damage);
                            }
                        }
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(_boxCollider.bounds.center, _boxCollider.bounds.size);
        }

        private void SetHeight(float height)
        {
            _current = height;
            

            _particles.transform.localScale = new Vector2(_boxCollider.size.x, height);
            _particles.transform.localPosition = new Vector2(0f, height / 2);

            ParticleSystem.EmissionModule emissionModule = _particles.emission;
            emissionModule.rateOverTime = height*ParticlesEmmision;

            if (height < MaxLength)
            {
                _boxCollider.size = new Vector2(_boxCollider.size.x, height);
                _boxCollider.offset = new Vector2(0f, height / 2);
            
                _renderer.size = new Vector2(_boxCollider.size.x, _boxCollider.size.y+2f);
                _renderer.transform.position = new Vector3(_boxCollider.size.x / 2, _renderer.size.y/2 -1f, 0f);

            }
            else
            {
                _boxCollider.size = new Vector2(_boxCollider.size.x, height);
                _boxCollider.offset = new Vector2(0f, height / 2f);
            
                _renderer.size = new Vector2(_boxCollider.size.x, MaxLength+2f);
                
                _renderer.transform.position = new Vector3(_boxCollider.size.x / 2, (height) - MaxLength/2-1f, 0f);

            }



        }

        public void SetWidth(float width)
        {
            _boxCollider.size = new Vector2(width, _boxCollider.size.y);
        }
    }
}