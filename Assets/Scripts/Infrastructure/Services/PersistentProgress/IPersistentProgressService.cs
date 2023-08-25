using Data;

namespace Infrastructure.Services.PersistentProgress
{
    public interface IPersistentProgressService
    {
        public IPlayerProgress Progress { get; set; }
    }
}