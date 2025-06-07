using Assets.CodeBase.Infrastructure;
using Assets.CodeBase.Logic.CameraLogic;
using CodeBase.DebugLogic;
using CodeBase.Infrastructure.States;
using CodeBase.UI;
using UnityEngine;
using UnityEngine.Audio;
#if YANDEX_GAMES

#endif

namespace CodeBase.Infrastructure
{
    public class GameBootstrapper : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] private LoadingCurtain _loadingCurtain;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private DebugSinglton _debugSinglton;
        [Space(10)]
        [SerializeField] private AudioMixerGroup soundsMixerGroup;
        [SerializeField] private AudioMixerGroup musicMixerGroup;

        private Game _game;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(_debugSinglton.gameObject);
            DontDestroyOnLoad(_cameraController.transform.parent.gameObject);
            DontDestroyOnLoad(_loadingCurtain.gameObject);


            _game = new Game(this, _loadingCurtain, _audioSource, _cameraController, soundsMixerGroup, musicMixerGroup);
            _game.StateMachine.Enter<BootstrapState>();
        }
    }
}