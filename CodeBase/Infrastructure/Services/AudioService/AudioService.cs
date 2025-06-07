using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.CodeBase.Infrastructure.AssetManagement;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.Infrastructure.Services.AudioService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace Assets.CodeBase.Infrastructure.Services.AudioService
{
    public class AudioService : IAudioService
    {
        private readonly AudioSource _ambientAudioSource;
        private readonly IAssets _assets;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IPersistentProgressService _progressService;
        private readonly AudioMixerGroup _soundsMixerGroup;
        private readonly AudioMixerGroup _musicMixerGroup;

        private List<AudioClip> _playingSounds = new List<AudioClip>();
        private string _volume = "volume";
        private bool _ambientPaused;

        public AudioService(AudioSource ambientAudioSource, IAssets assets, ICoroutineRunner coroutineRunner,
            IPersistentProgressService progressService, AudioMixerGroup soundsMixerGroup,
            AudioMixerGroup musicMixerGroup)
        {
            _ambientAudioSource = ambientAudioSource;
            _assets = assets;
            _coroutineRunner = coroutineRunner;
            _progressService = progressService;
            _soundsMixerGroup = soundsMixerGroup;
            _musicMixerGroup = musicMixerGroup;
        }

        public void UpdateVolume()
        {
            _musicMixerGroup.audioMixer.SetFloat(_volume, Mathf.Max(Mathf.Log10(_progressService.Progress.Settings.MusicVolume) * 20, -80f));
            _soundsMixerGroup.audioMixer.SetFloat(_volume, Mathf.Max(Mathf.Log10(_progressService.Progress.Settings.SoundsVolume) * 20, -80f));
        }

        public void PauseSounds()
        {
            AudioListener.pause = true;
        }

        public void PauseAmbient()
        {
            _ambientPaused = true;
            _ambientAudioSource.Pause();
        }
        public void ResumeAmbient()
        {
            _ambientPaused = false;
            _ambientAudioSource.UnPause();
        }

        public void ResumeSounds()
        {
            AudioListener.pause = false;
        }

        public async Task SetAmbient(AssetReference assetReference)
        {
            _ambientAudioSource.ignoreListenerPause = true;
            _ambientAudioSource.clip = await _assets.Load<AudioClip>(assetReference);
            _ambientAudioSource.outputAudioMixerGroup = _musicMixerGroup;
            _ambientAudioSource.Play();
            if (_ambientPaused)
            {
                _ambientAudioSource.Pause();
            }
        }

        public void ClearAmbient()
        {
            _ambientAudioSource.Stop();
        }

        public void PlaySound(AudioClip sound, Vector3 pos, bool randomize)
        {
            PlaySound(sound,pos, randomize, 1f, false, false);
        }

        
        //Main
        public void PlaySound(AudioClip sound, Vector3 position, bool randomize,
            float volume, bool ignorePause, bool ignorePosition)
        {
            if (_playingSounds.Count > 32)
            {
                if (_playingSounds.Contains(sound)) return;
            }
            
            
            GameObject audioObject = new GameObject("PlaySound");
            
            
            audioObject.transform.position = position;

            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            
            
            audioSource.spatialBlend = ignorePosition ? 0f : 1f ;
            audioSource.minDistance = 20f;
            audioSource.ignoreListenerPause = ignorePause;
            
            audioSource.pitch = 1f;
            if (randomize)
            {
                audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
            }

            audioSource.volume = volume;
            audioSource.outputAudioMixerGroup = _soundsMixerGroup;
            audioSource.PlayOneShot(sound);
            
            _playingSounds.Add(sound);
            _coroutineRunner.StartCoroutine(ClearSound(audioObject, sound));
        }
        

        public LoopingSound CreateLoopingSound(AudioClip sound, bool ignorePosition)
        {
            GameObject audioObject = new GameObject("LoopingSound");
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = 20f;
            audioSource.outputAudioMixerGroup = _soundsMixerGroup;
            
            audioSource.loop = true;
            LoopingSound loopingSound = new LoopingSound(audioSource, sound);
            
            return loopingSound;
        }
        public LoopingSound CreateLoopingSound(AudioClip sound)
        {
            return CreateLoopingSound(sound, false);
        }

        public void PlaySound(AudioClip sound, Vector3 pos)
        {
            PlaySound(sound, pos,false);
        }

        private IEnumerator ClearSound(GameObject audioObject, AudioClip sound)
        {
            yield return new WaitForSeconds(sound.length);
            Object.Destroy(audioObject);

            _playingSounds.Remove(sound);
        }
    }
}