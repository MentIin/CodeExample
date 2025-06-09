using System;
using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Logic.DynamicDataLogic;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using Assets.CodeBase.Logic.Raycasters;
using CodeBase.Logic;
using NaughtyAttributes;
using UnityEngine;

namespace Assets.CodeBase.Logic.Enemies
{
    public class CheckIfSeeBounds : MonoBehaviour
    {
        [SerializeField] private MobDataHolder _dataHolder;
        [SerializeField] private TriggerObserver _triggerObserver;
        [SerializeField] private ArcRaycaster _raycaster;

        [SerializeField] private bool _seeOnlyInRadius = false;
        [SerializeField][ShowIf("_seeOnlyInRadius")] private float _radius;

        public bool See
        {
            get;
            private set;
        }

        public float Distance { get => (Point - (Vector2)transform.position).magnitude;}

        public Vector2 Point;

        private bool _inRadius = false;
        private const float CheckDelay = 0.2f;
        private GameTimer _timer;
        private Collider2D _targetCollider;

        public void Awake()
        {
            _timer = new GameTimer();
            _timer.SetTime(CheckDelay);
        }
        private void Start()
        {
            _targetCollider = _dataHolder.PlayerMachineBase.GetComponent<Collider2D>();
            
            _triggerObserver.TriggerEnter += StartChecking;
            _triggerObserver.TriggerExit += StopChecking;
        }

        private void Update()
        {
            _timer.Tick(Time.deltaTime);
            if (_timer.Over && _inRadius)
            {
                _timer.SetTime(CheckDelay);
                Check();
            }
        }

        private void Check()
        {
            if (_targetCollider == null) return;
            
            
            var bounds = _targetCollider.bounds;
            Vector2 targetCenter = bounds.center;
            Vector2 targetExtents = bounds.extents + (Vector3.one * GameConstants.MapBlockSize);

            var direction = Direction(targetCenter, out var angleDegrees);

            // Вычисляем размер дуги, чтобы охватить всю ширину коллайдера игрока
            float arc = 2f * Mathf.Atan(targetExtents.x / direction.magnitude) * Mathf.Rad2Deg;

            _raycaster.Arc = arc;
            _raycaster.Distance = direction.magnitude;
            if (_seeOnlyInRadius)
            {
                if (_raycaster.Distance > _radius) _raycaster.Distance = _radius;
            }

            bool flag = false;
            
            float lastDistance = Single.MaxValue;
            foreach (RaycastHit2D hit in _raycaster.CastAtDirection(angleDegrees))
            {
                if (hit.collider)
                {
                    if (hit.collider.gameObject.GetComponent<Health>()?.Team == Team.Player)
                    {
                        flag = true;
                        if (hit.distance < lastDistance)
                        {
                            lastDistance = hit.distance;
                            Point = hit.point;
                        }
                        
                    }
                }
            }
            
            
            See = flag;
        }

        private Vector2 Direction(Vector2 targetCenter, out float angleDegrees)
        {
            // Вычисляем направление от центра скрипта до центра коллайдера игрока
            Vector2 direction = targetCenter - (Vector2) transform.position;

            // Вычисляем угол направления в радианах
            float angleRadians = Mathf.Atan2(direction.y, direction.x);

            // Вычисляем угол направления в градусах
            angleDegrees = angleRadians * Mathf.Rad2Deg;
            return direction;
        }

        private void StartChecking(Collider2D obj)
        {
            _inRadius = true;
        }

        private void StopChecking(Collider2D obj)
        {
            _inRadius = false;
            See = false;
        }
    }
}