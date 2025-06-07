using System;
using System.Collections;
using Assets.CodeBase.Infrastructure;
using Assets.CodeBase.Infrastructure.Services.Input;
using CodeBase.Logic;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.Input
{
    public class StandaloneInputService : IInputService
    {
        private const string Horizontal = "Horizontal";
        private const string Vertical = "Vertical";

        private ICoroutineRunner _coroutineRunner;

        public event Action<int> ModulePressed;

        public StandaloneInputService(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        public void Initialize()
        {
            _coroutineRunner.StartCoroutine(Check());
        }

        public Vector2 GetAxis()
        {
            return new Vector2(SimpleInput.GetAxis(Horizontal), SimpleInput.GetAxis(Vertical)).normalized;
        }
        
        

        public void ClearInput()
        {
            
        }

        private IEnumerator Check()
        {
            while (true)
            {
                KeyCode[] keys = {KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4};
                KeyCode[] keys2 = {KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3,
                    KeyCode.Keypad4};
                for (int i = 0; i < GameConstants.Groups; i++)
                {
                    if (SimpleInput.GetKeyDown(keys[i]) || SimpleInput.GetKeyDown(keys2[i]))
                    {
                        ModulePressed?.Invoke(i);
                    }
                }
                
                yield return null;
            }
        }
    }
}