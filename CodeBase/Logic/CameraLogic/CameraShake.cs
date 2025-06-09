using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.CodeBase.Logic.CameraLogic
{
    public class CameraShake : MonoBehaviour
    {
        private CameraController _controller;
        [SerializeField]private float _duration=.5f;
        [FormerlySerializedAs("_strength")]public float Strength = 1f;
        [SerializeField]private int _vibrato= 90;
        [SerializeField]private float _randomness = 90f;

        public void Construct(CameraController controller)
        {
            _controller = controller;
        }

        public void Play()
        {
            _controller.Shake(_duration, Strength, _vibrato, _randomness);
        }
    }
}