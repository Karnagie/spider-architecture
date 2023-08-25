namespace Core.Models.Commands
{
    public class DamageCommand : ISpiderCommand
    {
        public void Perform(Spider spider)
        {
            spider.Stats.Health.Decrease(10);
        }
    }
}