using Infrastructure.Services.System;

namespace Infrastructure.Services.Binding
{
    public interface IFilter
    {
        // bool Met(Binder linker);
        bool Met(SystemLinker linker);
    }
}