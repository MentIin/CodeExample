using System;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Elements.Buttons
{
    public class CloseWindowButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _window;


        [SerializeField] private bool _activateByButton;
        [SerializeField] private KeyCode _key;

        private void Awake()
        {
            _button.onClick.AddListener(Close);
        }

        private void Close()
        {
            Destroy(_window);
        }

        private void Update()
        {
            if (_activateByButton)
            {
                if (Input.GetKeyDown(_key))
                {
                    Close();
                }
            }
        }
    }
}