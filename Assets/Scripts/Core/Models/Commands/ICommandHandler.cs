namespace Core.Models.Commands
{
    public interface ICommandHandler<TCommand>
    {
        void Perform(TCommand command);
    }
}