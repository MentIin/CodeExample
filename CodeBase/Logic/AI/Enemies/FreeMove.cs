using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Logic.Follow;
using Assets.CodeBase.Logic.Raycasters;
using CodeBase.Infrastructure.Data;
using CodeBase.Logic.Follow;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Assets.CodeBase.Logic.AI.Enemies
{
    public class FreeMove : MonoBehaviour
    {
        [SerializeField] private ArcRaycasterCircleCollider _raycaster;
        [SerializeField] private BaseFollowTarget _baseFollowTarget;

        [SerializeField] private MinMaxRange<float> _delay;
        [SerializeField] private MinMaxRange<float> _distanse;
        
        private GameTimer _timer;

        private void Awake()
        {
            _timer = new GameTimer();
            _timer.SetTime(Random.Range(_delay.Min, _delay.Max));
        }

        private void Update()
        {
            if (_timer.Over)
            {
                _timer.SetTime(Random.Range(_delay.Min, _delay.Max));
                SetTargetPoint();
            }
            _timer.Tick(Time.deltaTime);
        }

        public void SetTargetPoint()
        {
            float distance = Random.Range(_distanse.Min, _distanse.Max);
            Vector2 offset = Random.insideUnitCircle * distance;

            RaycastHit2D[] hit2Ds = _raycaster.CastAtDirection(Vector2.Angle(Vector2.right, offset));
            foreach (var hit2D in hit2Ds)
            {
                if (hit2D.collider == null) continue;

                if (hit2D.distance < distance)
                {
                    distance = hit2D.distance;
                }
            }

            _baseFollowTarget.SetTargetPoint(transform.position + (Vector3) offset.normalized * distance);
        }
    }
}