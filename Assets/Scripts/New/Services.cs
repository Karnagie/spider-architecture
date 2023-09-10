namespace New
{
    // public class ProjectileMover : IDisposable
    // {
    //     private TickingService _tickingService;
    //     private ProjectileHolder _holder;
    //     private List<IConcreteProjectileMover> _concreteMovers;
    //
    //     public ProjectileMover(TickingService tickingService, ProjectileHolder holder,
    //         List<IConcreteProjectileMover> concreteMovers)
    //     {
    //         _concreteMovers = concreteMovers;
    //         _holder = holder;
    //         _tickingService = tickingService;
    //         tickingService.Ticked += OnTick;
    //     }
    //
    //     private void OnTick()
    //     {
    //
    //         foreach (var projectile in _holder.Projectiles)
    //         {
    //             foreach (var concreteMover in _concreteMovers)
    //             {
    //                 if (concreteMover.CanMove(projectile))
    //                 {
    //                     concreteMover.Move();
    //                     break;
    //                 }
    //             }
    //         }
    //     }
    //
    //     public void Dispose()
    //     {
    //         _tickingService.Ticked -= OnTick;
    //     }
    // }
    //
    // public interface IConcreteProjectileMover
    // {
    //     bool CanMove(Projectile projectile);
    //     void Move();
    // }
    //
    // public class ProjectileHolder
    // {
    //     private List<Projectile> _projectiles = new();
    //     public IReadOnlyList<Projectile> Projectiles { get; }
    // }
    //
    // public class Projectile
    // {
    //     
    // }
    //
    // public class SpiderProjectileMover
    // {
    //     
    // }
    //
    // public class SnailProjectileMover
    // {
    //     
    // }
}