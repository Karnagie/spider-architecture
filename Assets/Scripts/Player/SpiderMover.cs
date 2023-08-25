// using Data;
// using Infrastructure.Services.Input;
// using Infrastructure.Services.PersistentProgress;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using Zenject;
//
// namespace Player
// {
//     public class SpiderMover : ITickable, ISavedProgress
//     {
//         private IInputService _inputService;
//         private Spider _spider;
//
//         public SpiderMover(IInputService inputService, Spider spider)
//         {
//             _spider = spider;
//             _inputService = inputService;
//         }
//
//         public void Tick()
//         {
//             _spider.Movable.Move(_inputService.Moving());
//         }
//
//         public void UpdateProgress(PlayerProgress progress)
//         {
//             progress.WorldData.PositionOnLevel = new PositionOnLevel(
//                 CurrentLevel(),
//                 _spider.Movable.Velocity.AsVector3Data()
//                 );
//         }
//
//         public void LoadProgress(PlayerProgress progress)
//         {
//             if(CurrentLevel() != progress.WorldData.PositionOnLevel.Level)
//                 return;
//             
//             var savedPosition = progress.WorldData.PositionOnLevel.Velocity;
//             _spider.Movable.TeleportTo(savedPosition.AsUnityVector());
//         }
//
//         private string CurrentLevel()
//         {
//             return SceneManager.GetActiveScene().name;
//         }
//     }
// }