using Assets.CodeBase.Infrastructure.Data.PlayerData;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using UnityEngine;

namespace CodeBase.UI.Services
{
    public class WindowBase : MonoBehaviour
    {
        protected IPersistentProgressService progressService;
        protected PlayerProgress Progress => progressService.Progress;


        public void Construct(IPersistentProgressService progressService)
        {
            this.progressService = progressService;
        }

        private void Start()
        {
            Subscribe();
            
        }

        private void OnDestroy() => Unsubscribe();

        public virtual void Initialize(){}
        protected virtual void Subscribe(){}
        protected virtual void Unsubscribe(){}
    }
}