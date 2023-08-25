namespace MVP.Model
{
    public class Spider : ICommandHandler<ISpiderCommand>, IUnique
    {
        public SpiderStats Stats;

        public Spider(int id)
        {
            Stats = new SpiderStats();
            Id = id;
        }
        
        public void Perform(ISpiderCommand command)
        {
            command.Perform(this);
        }

        public int Id { get; }
    }
}