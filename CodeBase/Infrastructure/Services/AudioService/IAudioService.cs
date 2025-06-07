using System.Threading.Tasks;
using Assets.CodeBase.Infrastructure.Services;
using Assets.CodeBase.Infrastructure.Services.AudioService;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CodeBase.Infrastructure.Services.AudioService
{
    public interface IAudioService : IService
    {
        Task SetAmbient(AssetReference assetReference);
        void ClearAmbient();
        void PlaySound(AudioClip sound, Vector3 pos);
        void PlaySound(AudioClip sound,Vector3 pos, bool randomize);
        LoopingSound CreateLoopingSound(AudioClip sound);
        void PlaySound(AudioClip sound, Vector3 pos, bool randomize, float volume, bool pause, bool ignorePause);
        void UpdateVolume();
        void PauseSounds();
        void ResumeSounds();
        LoopingSound CreateLoopingSound(AudioClip sound, bool ignorePosition);
        void PauseAmbient();
        void ResumeAmbient();
    }
}