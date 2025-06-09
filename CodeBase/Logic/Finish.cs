using System.Collections;
using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Logic.CameraLogic;
using Assets.CodeBase.Logic.DynamicDataLogic;
using Assets.CodeBase.Logic.Machine;
using CodeBase.DebugLogic;
using CodeBase.Infrastructure.States;
using CodeBase.Logic.Machine;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.CodeBase.Logic
{
    public class Finish : MonoBehaviour
    {
        public bool Tutorial = false;
        private IGameStateMachine _gameStateMachine;
        private CameraController _cameraController;
        private bool _last;
        private IPersistentProgressService _progressService;

        private bool _finished=false;

        public void Construct(IGameStateMachine stateMachine, CameraController cameraController,
            IPersistentProgressService progressService, bool last)
        {
            _progressService = progressService;
            _gameStateMachine = stateMachine;
            _cameraController = cameraController;
            _last = last;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            
            if (col.GetComponent<Health>()?.Team == Team.Player)
            {
                if (_finished) return;
                _finished = true;
                if (!Tutorial)
                {
                    if (_progressService.Progress.LevelStage < 6) _progressService.Progress.LevelStage += 1;
                    
                }

                StartCoroutine(FinishAnimation(col));
            }
        }

        private IEnumerator FinishAnimation(Collider2D playerCol)
        {
            float time = Random.Range(1f, 3f);
            
            _cameraController.SetScale(5f, time);
            _cameraController.Shade(1f, time);
            
            playerCol.GetComponent<MachineBase>().enabled = false;
            playerCol.GetComponent<Health>().BecomeImmortal();
            Destroy(playerCol.attachedRigidbody);
            
            playerCol.transform.DORotate(playerCol.transform.eulerAngles + new Vector3(0f, 0f, 180f),
                time / 5, RotateMode.Fast).SetLoops(5, LoopType.Incremental).SetEase(Ease.Linear).SetId(875);
            playerCol.transform.DOMove(transform.position, time).SetEase(Ease.OutQuart).SetId(876);
            playerCol.transform.DOScale(Vector3.one*0.1f,time).SetId(877);

            yield return new WaitForSeconds(time);

            if (_last) _gameStateMachine.Enter<FinishGameState>();
            else
            {
                //_gameStateMachine.Enter<GameStopOverState>();
                if (Tutorial)
                {
                    _progressService.Progress.TutorialData.FirstTutorialProgress.FullComplete();
                }
                _gameStateMachine.Enter<LevelTransitionState>();
            }
        }


        private void Awake()
        {
            if (DebugSinglton.Instance.LargeFinish)
            {
                GetComponent<CircleCollider2D>().radius = 3000f;
            }
        }

        private void OnDestroy()
        {
            DOTween.Kill(875);
            DOTween.Kill(876);
            DOTween.Kill(877);
        }
    }
}