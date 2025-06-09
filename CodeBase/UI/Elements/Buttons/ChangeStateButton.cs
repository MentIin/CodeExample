using Assets.CodeBase.Infrastructure.Services;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.Infrastructure.States;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Elements.Buttons
{
    public class ChangeStateButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private State _state = State.MainMenu;
        private IGameStateMachine _stateMachine;
        
        [Header("Ignore if not main menu")]
        [SerializeField]private bool _save = false;


        private bool _pressed = false;
        

        public void Construct(IGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _button.onClick.AddListener(ChangeState);
        }


        private void ChangeState()
        {
            if (_pressed) return;
            _pressed = true;
            if (_state == State.MainMenuWithCleaning) _stateMachine.Enter<GoToMainMenuFromGameState, bool>(_save);
            if (_state == State.MainMenu) _stateMachine.Enter<MainMenuState>();
            if (_state == State.NewGame) _stateMachine.Enter<SetUpNewGameState>();
            if (_state == State.GameStopOver)
            {
                _stateMachine.Enter<GameStopOverState>();

                
            }
            if (_state == State.Play)
            {
                
                _stateMachine.Enter<LoadLevelState>();
            }

            if (_state == State.LevelTransition)
            {
                _stateMachine.Enter<LevelTransitionState>();
            }
        }
    }

    internal enum State
    {
        MainMenuWithCleaning=1, NewGame=2,MainMenu=3,  GameStopOver=5,
        Play=6, LevelTransition=7,
    }
}