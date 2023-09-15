namespace Core.Models.Commands
{
    public interface ISpiderCommand
    {
        void Perform(ISpider spider);
    }
}