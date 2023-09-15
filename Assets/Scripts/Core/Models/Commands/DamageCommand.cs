namespace Core.Models.Commands
{
    public class DamageCommand : ISpiderCommand
    {
        public void Perform(ISpider spider)
        {
            spider.Stats.Health.Decrease(10);
        }
    }
}