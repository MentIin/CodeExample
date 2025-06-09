using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.Infrastructure.States;

namespace CodeBase.UI.Services.MainMenu
{
    public class MainMenuWindow : WindowBase
    {
        private IGameStateMachine _gameStateMachine;

        public void Construct(IPersistentProgressService progressService, IGameStateMachine stateMachine)
        {
            _gameStateMachine = stateMachine;
        }
        public override void Initialize()
        {
            
        }
        
    }
}