using MVP.Model;

namespace MVP.Presenter
{
    public class PauseCommand : ISpiderCommand
    {
        private bool _pause;

        public PauseCommand(bool pause)
        {
            _pause = pause;
        }

        public void Perform(Spider spider)
        {
            // spider.MoveComponent.Working = _pause;
            // spider.AttackComponent.Working = _pause;
        }
    }
}