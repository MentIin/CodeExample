using Assets.CodeBase.Infrastructure.Services;
using Assets.CodeBase.Logic.CameraLogic;
using CodeBase.Infrastructure.States;
using CodeBase.UI;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.CodeBase.Infrastructure
{
    public class Game
    {
        public readonly IGameStateMachine StateMachine;

        public Game(ICoroutineRunner coroutineRunner, LoadingCurtain loadingCurtain, AudioSource audioSource,
            CameraController cameraController, AudioMixerGroup soundsMixerGroup, AudioMixerGroup musicMixerGroup)
        {
            StateMachine = new GameStateMachine(new SceneLoader(coroutineRunner), coroutineRunner,
                AllServices.Container, loadingCurtain, audioSource, cameraController,
                musicMixerGroup, soundsMixerGroup);
        }
    }
}