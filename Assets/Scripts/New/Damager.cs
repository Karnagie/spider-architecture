namespace New
{
    // public class Damager : IDisposable
    // {
    //     private TickingService _tickingService;
    //     private readonly ItemHolder<Spider> _spiderHolder;//SpiderHolder
    //
    //     public Damager(TickingService tickingService, SpiderService spiderService)
    //     {
    //         _tickingService = tickingService;
    //         _spiderHolder = spiderService.SpiderHolder;
    //
    //         _tickingService.Tick += OnTick;
    //     }
    //
    //     private void OnTick()
    //     {
    //         if (!_spiderHolder.TryGetPlayer(out Spider player))
    //             return;
    //
    //         MakeSpiderDamage(player);
    //
    //         RemoveDeadSpiders(player);//Delete spiders from holders
    //     }
    //
    //     private void MakeSpiderDamage(Spider player)
    //     {
    //         IReadOnlyList<Spider> enemies = _spiderHolder.Enemies;
    //         foreach (var enemy in enemies)
    //         {
    //             if (!HasCollision(player, enemy))
    //                 continue;
    //
    //             if (CanAttack(player))
    //                 MakeDamage(player, enemy);
    //
    //             if (CanAttack(enemy))
    //                 MakeDamage(enemy, player);
    //         }
    //     }
    //
    //     private bool CanAttack(Spider player)
    //     {
    //         return player.AttackCooldown.CanAttack();
    //     }
    //
    //     public void Dispose()
    //     {
    //         _tickingService.Tick -= OnTick;
    //     }
    // }
}