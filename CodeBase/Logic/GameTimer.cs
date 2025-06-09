namespace Assets.CodeBase.Logic
{
    public class GameTimer
    {
        private float _timeRemaining;

        public void Tick(float deltaTime)
        {
            _timeRemaining -= deltaTime;
        }

        public void SetTime(float time)
        {
            _timeRemaining = time;
        }

        public bool Over => _timeRemaining <= 0f;
    }
}