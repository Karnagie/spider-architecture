namespace Infrastructure.Services.Binding
{
    public interface IFilter
    {
        bool Met(Binder linker);
    }
}