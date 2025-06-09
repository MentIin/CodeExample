using System;
using CodeBase.UI.Services;
using CodeBase.UI.Services.UIFactory;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Elements.Buttons
{
    public class OpenWindowButton : MonoBehaviour
    {
        [SerializeField] private WindowType _type;
        [SerializeField] private Button _button;
        private IUIFactory _uiFactory;

        public void Construct(IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
        }

        private void Awake()
        {
            _button.onClick.AddListener(Open);
        }

        private void Update()
        {
            if (_type == WindowType.Pause)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (!_uiFactory.CheckWindowExistence(_type)) Open();
                }
            }
        }

        private void Open()
        {
            
            _uiFactory.CreateWindow(_type);
        }
    }
}