using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Logic;
using Assets.CodeBase.StaticData;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeBase.Logic.Collectables
{
    public class MachineModuleCollectable : MonoBehaviour
    {
        public Animator Animator;
        public TriggerObserver Observer;
        [FormerlySerializedAs("Renderer")] public SpriteRenderer IconRenderer;
        public SpriteRenderer RarityRenderer;
        
        
        private MachineModuleStaticData _staticData;
        private IPersistentProgressService _progressService;
        private int _level=0;

        private bool _collected = false;

        public void Construct(IPersistentProgressService progressService,
            MachineModuleStaticData staticData)
        {
            _staticData = staticData;
            _progressService = progressService;
            
            
            _level = _progressService.Progress.GetModuleLevel(_staticData.Id);

            if (_level > 3)
            {
                _level = Random.Range(0, GameConstants.MaxModuleLevel);
            }
            if (_level == 0) RarityRenderer.color = Color.gray;
            else if (_level == 1) RarityRenderer.color = Color.green;
            else if (_level == 2) RarityRenderer.color = Color.magenta;
            
            IconRenderer.sprite = staticData.Icon;

            Observer.TriggerEnter += ObserverOnTriggerEnter;


        }

        private void ObserverOnTriggerEnter(Collider2D obj)
        {

            if (_collected) return;
            _collected = true;
            Animator.SetTrigger("collected");
            IconRenderer.sortingLayerName = GameConstants.EffectsSortingLayerName;
            
            Destroy(this.gameObject, 1.5f);
            _progressService.Progress.ObtainModule(_staticData,
                _level + 1);
        }
    }
}