using UnityEngine;

namespace Assets.CodeBase.Logic
{
    public class DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] private float _time;
        [SerializeField] private bool _onAwake = true;
        
        private void Awake()
        {
            if (_onAwake) Destroy(this.gameObject, _time);
        }

        public void Activate()
        {
            Destroy(this.gameObject, _time);
        }

    }
}