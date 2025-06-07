using System;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Services.Input
{
    public interface IInputService : IService
    {
        event Action<int> ModulePressed;
        Vector2 GetAxis();
        void ClearInput();
        void Initialize();
    }
}