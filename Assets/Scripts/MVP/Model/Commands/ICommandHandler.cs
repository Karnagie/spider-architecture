namespace MVP.Model
{
    public interface ICommandHandler<TCommand>
    {
        void Perform(TCommand command);
    }
}