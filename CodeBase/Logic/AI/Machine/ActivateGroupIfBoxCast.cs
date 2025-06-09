using System.Collections;
using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Logic.AI.Machine;
using Assets.CodeBase.Logic.DynamicDataLogic;
using Assets.CodeBase.Logic.Raycasters;
using NaughtyAttributes;
using UnityEngine;

namespace CodeBase.Logic.AI.Machine
{
    public class ActivateGroupIfRaycast : MonoBehaviour
    {
        [SerializeField] [Range(0, GameConstants.Groups)] private int _group = 0;
        [Required()] [SerializeField] private MachineBaseReferenceHolder _baseReferenceHolder;
        [Required()] [SerializeField] private Raycaster Raycaster;
        [SerializeField] private bool _haveTargetTeam = false;

        [SerializeField] [ShowIf("_haveTargetTeam")]
        private Team _targetTeam;

        private void Start()
        {
            StartCoroutine(Check());
        }

        private IEnumerator Check()
        {
            while (true)
            {
                yield return new WaitForSeconds(GameConstants.DefaultRaycastCheckDelay);
                bool hitted = false;
                foreach (var hit2D in Raycaster.Cast())
                {
                    if (hit2D.collider)
                    {
                        if (_haveTargetTeam && hit2D.collider.GetComponent<Health>()?.Team != _targetTeam)
                        {
                            continue;
                        }

                        hitted = true;
                        break;
                    }
                }
                _baseReferenceHolder.MachineBase.SetGroupActive(_group, hitted);
            }

        }
    }
}