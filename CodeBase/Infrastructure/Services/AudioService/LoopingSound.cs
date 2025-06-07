using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Services.AudioService
{
    public class LoopingSound
    {
        private readonly AudioSource _audioSource;

        public float volume
        {
            set
            {
                _audioSource.volume = value;
            }
        }

        public LoopingSound(AudioSource audioSource, AudioClip audioClip)
        {
            _audioSource = audioSource;
            _audioSource.clip = audioClip;
        }

        public void Play()
        {
            _audioSource.Play();
        }
        public void Stop()
        {
            if (_audioSource) _audioSource.Stop();
        }

        public void SetVolume(float f)
        {
            throw new System.NotImplementedException();
        }

        public void SetPosition(Vector3 transformPosition)
        {
            _audioSource.transform.position = transformPosition;
        }
    }
}