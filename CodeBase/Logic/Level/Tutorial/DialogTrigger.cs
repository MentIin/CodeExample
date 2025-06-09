using Assets.CodeBase.Logic;
using CodeBase.UI.Services;
using CodeBase.UI.Services.UIFactory;
using UnityEngine;

namespace CodeBase.Logic.Level.Tutorial
{
    public class DialogTrigger : MonoBehaviour
    {
        public WindowType WindowType;
        public TriggerObserver Observer;

        public bool TriggerOnStart = false;
        
        private IUIFactory _uiFactory;


        public void Construct(IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
            Observer.TriggerEnter += ObserverOnTriggerEnter;
            Observer.TriggerStay += ObserverOnTriggerEnter;

            if (TriggerOnStart)
            {
                _uiFactory.CreateWindow(WindowType);
                Destroy(this.gameObject);
            }
        }

        private void ObserverOnTriggerEnter(Collider2D obj)
        {
            _uiFactory.CreateWindow(WindowType);
            Destroy(this.gameObject);
        }
        
    }
}