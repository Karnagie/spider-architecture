using Data;

namespace Infrastructure.Services.PersistentProgress
{
    public class PersistentProgressService : IPersistentProgressService
    {
        public IPlayerProgress Progress { get; set; }
    }
}